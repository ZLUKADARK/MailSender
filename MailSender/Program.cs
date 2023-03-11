using MailSender.Static;
using RabbitMQ.Client;

var factory = new ConnectionFactory() { HostName = "localhost" };
var connection = factory.CreateConnection();
var channel = connection.CreateModel();
var consumer = new Consumer(channel);

consumer.Consume();

Console.ReadLine();