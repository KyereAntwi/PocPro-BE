using System.Security.Authentication;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace DevSync.PocPro.Shared.Domain.Utils;

public class RabbitMqConnectionFactory
{
    private readonly MessageBroker _messageBroker;
    private readonly ILogger<RabbitMqConnectionFactory> _logger;

    public RabbitMqConnectionFactory(MessageBroker messageBroker, ILogger<RabbitMqConnectionFactory> logger)
    {
        _messageBroker = messageBroker;
        _logger = logger;
    }

    public async Task<IConnection> CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _messageBroker.Host,
                UserName = _messageBroker.Username,
                Password = _messageBroker.Password,
                Port = int.Parse(_messageBroker.Port),
                VirtualHost = _messageBroker.VirtualHost,
                Ssl = new SslOption
                {
                    Enabled = true,
                    ServerName = _messageBroker.Host,
                    Version = SslProtocols.Tls12
                }
            };

            var conn = await factory.CreateConnectionAsync();
            _logger.LogInformation("Rabbit mq connection made");
            return conn;
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error making RabbitMQ connection, Error = {Error}", e.Message);
            throw;
        }
    }
}