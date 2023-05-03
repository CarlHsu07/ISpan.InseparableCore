using Microsoft.AspNetCore.SignalR;

namespace ISpan.InseparableCore.Hubs
{
    public class ChatHub : Hub
    {
        // todo 會員連線時的方法

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
