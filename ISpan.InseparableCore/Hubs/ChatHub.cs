using Microsoft.AspNetCore.SignalR;

namespace ISpan.InseparableCore.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", senderId, receiverId, message);
        }


        // 範例預設方法，傳訊息給所有連線者
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
    }
}
