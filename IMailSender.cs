using System;
using System.Therding.Task;

public interface IMailSender
{
	public Task SendMessage(string JsonMessage);
}
