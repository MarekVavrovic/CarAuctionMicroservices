using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotifySvc.Consumers;

public class BidPlacedConsumer(IHubContext<NotificationHub> hubContext) : IConsumer<BidPlaced>
{
    public async Task Consume(ConsumeContext<BidPlaced> context)
    {
        Console.WriteLine("--> BidPlaced message received");

        await hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
    }
}