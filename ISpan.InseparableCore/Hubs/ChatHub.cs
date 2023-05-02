using Microsoft.AspNetCore.SignalR;

namespace ISpan.InseparableCore.Hubs
{
    public class ChatHub : Hub
    {
        // todo 連線時
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
