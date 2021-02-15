namespace RabbitMQService.Sender
{
    public interface IDataSender
    {
        void SendData(string data, string queueName);
    }
}