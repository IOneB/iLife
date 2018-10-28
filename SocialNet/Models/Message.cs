using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNet.Models
{
  
    public class Message 
    {
        //ID сообщеньки
        [Key]
        public int MessageId { get; set; }
        //Ссылка на ChatRoom
        public int? ChatRoomId { get; set; }
        public ChatRoom ChatRoom { get; set; }
        //ID пользователя
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        //Сообщение прочитано
        public bool IsRead { get; set; }
        //Текст сообщения
        public string Text { get; set; }
        //Время отправки
        public DateTime SentTime { get; set; }
    }
}