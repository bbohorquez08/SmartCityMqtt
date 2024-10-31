using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace SmartCity.Infrastructure.Services.MqttServices
{
    public class MqttService : IHostedService
    {
        private readonly ILogger<MqttService> _logger;
        private IMqttClient _mqttClient;
        private MqttClientOptions _mqttOptions;

        // Propiedad para almacenar el último mensaje recibido
        private static string _lastReceivedMessage;

        public MqttService(ILogger<MqttService> logger)
        {
            _logger = logger;
            var mqttFactory = new MqttFactory();

            // Configura el cliente MQTT
            _mqttClient = mqttFactory.CreateMqttClient();

            // Configura las opciones del cliente MQTT
            _mqttOptions = new MqttClientOptionsBuilder()
                .WithClientId("Osiris")
                .WithTcpServer("192.168.211.147", 1883) // Cambia el host y puerto según tu servidor
                .WithCleanSession()
                .Build();

            // Configura el evento para recibir mensajes
            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                // Almacena el mensaje en la propiedad _lastReceivedMessage
                _lastReceivedMessage = $"Tema: {topic}. Contenido: {payload}";

                // Imprime el mensaje en el log
                _logger.LogInformation($"Mensaje recibido. {_lastReceivedMessage}");
                return Task.CompletedTask;
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _mqttClient.ConnectedAsync += async e =>
            {
                _logger.LogInformation("Conectado al servidor MQTT.");

                // Suscribirse a un tema cuando se establece la conexión
                var topicFilter = new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter("smartcity", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                    .Build();

                await _mqttClient.SubscribeAsync(topicFilter, cancellationToken);
                _logger.LogInformation("Suscrito al tema 'smartcity'.");
            };

            _mqttClient.DisconnectedAsync += e =>
            {
                _logger.LogWarning("Desconectado del servidor MQTT.");
                return Task.CompletedTask;
            };

            // Inicia la conexión
            await _mqttClient.ConnectAsync(_mqttOptions, cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_mqttClient.IsConnected)
            {
                var disconnectOptions = new MqttClientDisconnectOptionsBuilder().Build();
                await _mqttClient.DisconnectAsync(disconnectOptions, cancellationToken);
            }
        }

        public bool IsConnected()
        {
            return _mqttClient.IsConnected;
        }

        // Método para obtener el último mensaje recibido
        public string GetLastReceivedMessage()
        {
            return _lastReceivedMessage;
        }
    }
}
