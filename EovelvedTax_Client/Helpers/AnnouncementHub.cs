using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
namespace EvolvedTax.Helpers
{
    public class AnnouncementHub : Hub
    {
        public async Task SendAnnouncement(string message)
        {
            await Clients.All.SendAsync("ReceiveAnnouncement", message);
        }

        public async Task TestSendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveAnnouncement", message);
        }
    }
}
