namespace RabbitMQService
{
    public class RabbitMqConfiguration
    {
        public object Shallowcopy()
        {
            return this.MemberwiseClone();
        }
        public string Hostname { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        public string QueueName { get; set; }
        public bool Enabled { get; set; }

    }
}