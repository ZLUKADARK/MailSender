public interface IMailSenderService
{
	public Task SendMessageAsync(string jsonMessage);
}
