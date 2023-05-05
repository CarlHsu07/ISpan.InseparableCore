using ISpan.InseparableCore.Models.DAL;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace ISpan.InseparableCore.Hubs
{
    public class ChatHub : Hub
    {
        private readonly InseparableContext _context;
        private readonly Connections _connections;

        public ChatHub(InseparableContext context, Connections connections)
        {
            _context = context;
            _connections = connections;
        }

        // 連線時
        public override async Task OnConnectedAsync()
        {
            // 取得目前連線的 Connection ID
            string connectionId = Context.ConnectionId;

            // 取得客戶端傳送的會員 ID
            string memberId = Context.GetHttpContext().Request.Query["senderId"];

            // 建立 Connection ID 與會員 ID 的關聯
            if (!string.IsNullOrEmpty(memberId))
            {
                _connections.AddConnection(connectionId, memberId);
            }

            await base.OnConnectedAsync();
        }

        // 斷線時
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // 取得目前連線的 Connection ID
            string connectionId = Context.ConnectionId;
            _connections.RemoveConnection(connectionId);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string receiverId)
        { 
            
        }

            // 傳訊息給指定群組
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



        // 傳訊息給所有連線者
        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
    }

    
}
