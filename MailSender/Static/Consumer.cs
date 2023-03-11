using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace MailSender.Static
{
    public class Consumer
    {
        private readonly object locker = new object();
        private readonly IModel _channel;

        public Consumer(IModel chanel)
        {
            _channel = chanel;
        }

        public void Consume()
        {

            Thread myThread;

            for (int i = 0; i < 1; i++)
            {
                myThread = new(Send);
                myThread.Name = $"Поток {i}";   // устанавливаем имя для каждого потока
                myThread.Start();
            }
        }

        private async void Send()
        {
            _channel.ExchangeDeclare(
                exchange: "MultiThread",
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

            var queue = _channel.QueueDeclare(
                queue: "MultiThread.Queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null).QueueName;

            _channel.QueueBind(queue, "MultiThread", "MultiThread.Queue");

            while (true)
            {
                IMailSenderService send = new MailSenderService();

                var result = _channel.BasicGet(queue, false);
                if (result != null)
                {
                    var message = Encoding.UTF8.GetString(result.Body.ToArray());
                    try
                    {  
                        //await send.SendMessageAsync(message);
                        Console.WriteLine($"{Thread.CurrentThread.Name} | {message}");
                        _channel.BasicAck(result.DeliveryTag, false);
                        Thread.Sleep(1000);
                    }
                    catch
                    {
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Console.WriteLine($"Очередь пуста {Thread.CurrentThread.Name}");
                    Thread.Sleep(1000);
                } 
            }
        }
    }
}
