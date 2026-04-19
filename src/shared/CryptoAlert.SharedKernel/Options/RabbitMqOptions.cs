namespace CryptoAlert.SharedKernel.Options;

public class RabbitMqOptions
{
    public string HostName { get; set; } = null!;
    public string ExchangeName { get; set; } = null!;
    public string ExchangeType { get; set; } = null!;
    public string QueueName { get; set; } = null!; // 👈 add this
}