using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ISpan.InseparableCore.Hubs
{
    public class ChatHub : Hub
    {
        private readonly InseparableContext _context;

        public ChatHub(InseparableContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            //// 將訊息存進DB中
            //var chatMessage = new TChat
            //{
            //    FSenderId = int.Parse(senderId),
            //    FReceiverId = int.Parse(receiverId),
            //    FMessage = message,
            //    FSendTime = DateTime.Now
            //};

            //_context.TChat.Add(chatMessage);
            //await _context.SaveChangesAsync();

            // 將訊息發送給發送者
            await Clients.User(senderId).SendAsync("ReceiveMessage", senderId, message);

            // 將訊息發送給接收者
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }


        // todo 會員連線時的方法

        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}

        
    }
}
