using Carter;
using MediatR;
using Microsoft.AspNetCore.Hosting.Server;
using SmartCity.Infrastructure.Services.MqttServices;

namespace SmartCity.Application.Features.Mqtt.V1.Queries.MqttConnect
{
    public class MqttConnectEndPoint : ICarterModule
    {
        private readonly MqttService _mqttService;

        public MqttConnectEndPoint(MqttService mqttService)
        {
            _mqttService = mqttService;
        }

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/mqtt/last-message", (MqttService mqttService) =>
            {
                var lastMessage = mqttService.GetLastReceivedMessage();
                if (!string.IsNullOrEmpty(lastMessage))
                {
                    return Results.Ok(new { message = lastMessage });
                }
                return Results.NoContent();
            })
            .WithTags("MQTT")
            .AllowAnonymous();
        }
    }

}

