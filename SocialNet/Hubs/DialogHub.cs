using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SocialNet.Models;

namespace SocialNet.Hubs
{

    public class ChatHub : Hub
    {
        ApplicationDbContext db = ApplicationDbContext.Create();

        public void SaveMessage(ApplicationUser receiver, string message)
        {
            var sender = db.Users.FirstOrDefault(u => u.UserName == Context.User.Identity.Name);
            var room = db.ChatRooms.FirstOrDefault(r => r.Companions.Any(u => u.Id == sender.Id) && r.Companions.Any(u => u.Id == receiver.Id));
            if (room == null)
            {
                room = new ChatRoom() { Companions = new List<ApplicationUser>() { sender, receiver }, ChatRoomName = "яхзчтотутписать" };
                db.ChatRooms.Add(room);
            }
            var newMessage = new Message { ChatRoom = room, ApplicationUser = sender, SentTime = DateTime.Now, Text = message,  };
            db.Messages.Add(newMessage);
            db.SaveChanges();
        }


        // Отправка сообщений
        public void Send(string receiverUser, string message)
        {
            message = message.Replace("\n", "<br/>");
            var receiver = db.Users.Where(x => x.UserName == receiverUser).First();
            SaveMessage(receiver, message);
            var sender = db.Users.FirstOrDefault(u => u.UserName == Context.User.Identity.Name);
            var userConSender = db.UserConnections.Where(uc => uc.ApplicationUser.UserName == sender.UserName && uc.ToUser == receiver.UserName).ToList();
            var userConReceiver = db.UserConnections.Where(uc => uc.ApplicationUser.UserName == receiver.UserName && uc.ToUser == sender.UserName).ToList();
            if (userConSender != null)
                foreach (var ucSender in userConSender)
                {
                    Clients.Client(ucSender.Connection.ConnectionName).addMessage(Context.User.Identity.Name, message, Context.User.Identity.Name);
                    Clients.Client(ucSender.Connection.ConnectionName).OnSended();
                }
            if (userConReceiver != null)
                foreach (var ucReceiver in userConReceiver)
                {
                    Clients.Client(ucReceiver.Connection.ConnectionName).addMessage(Context.User.Identity.Name, message, receiver.UserName);
                    Clients.Client(ucReceiver.Connection.ConnectionName).OnSended();
                }
        }

        public void Readed(string receiverUser)
        {
            var receiver = db.Users.Where(x => x.UserName == receiverUser).First();
            var sender = db.Users.FirstOrDefault(u => u.UserName == Context.User.Identity.Name);
            var room = db.ChatRooms.FirstOrDefault(r => r.Companions.Any(u => u.Id == sender.Id) && r.Companions.Any(u => u.Id == receiver.Id));
            List<Message> messages = db.Messages.Where(m => m.ChatRoomId == room.ChatRoomId).ToList();
            foreach(var mess in messages)
            {
                mess.IsRead = true;
            }
            db.SaveChanges();
            var userConSender = db.UserConnections.Where(uc=>uc.ApplicationUser.UserName == sender.UserName && uc.ToUser == receiver.UserName).ToList();
            var userConReceiver = db.UserConnections.Where(uc => uc.ApplicationUser.UserName == receiver.UserName && uc.ToUser == sender.UserName).ToList();
            if (userConSender != null)
                foreach(var ucSender in userConSender)
                    Clients.Client(ucSender.Connection.ConnectionName).OnReaded();
            if (userConReceiver != null)
                foreach (var ucReceiver in userConReceiver)
                    Clients.Client(ucReceiver.Connection.ConnectionName).OnReaded();
        }

        // Подключение нового пользователя
        public void Connect(string receiverUser)
        {
            var id = Context.ConnectionId;
            if (!db.UserConnections.Any(x => x.Connection.ConnectionName == id))
            {
                var user = db.Users.Where(x => x.UserName == Context.User.Identity.Name).First();
                db.UserConnections.Add(new ApplicationUserConnection {ApplicationUser = user, Connection = new Connection() { ConnectionName = id}, ToUser = receiverUser });
                db.SaveChanges();
            }
        }

        // Отключение пользователя
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var userConnection = db.UserConnections.Where(uc => uc.Connection.ConnectionName == Context.ConnectionId && uc.ApplicationUser.UserName == Context.User.Identity.Name).FirstOrDefault();
            if (userConnection!=null)
                db.UserConnections.Remove(userConnection);
             return base.OnDisconnected(stopCalled);
        }


        public Task OnSended()
        {
            return null;
        }
        public Task OnReaded()
        {
            return null;
        }
    }
}
