using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SocialNet.Models
{
    public class ChatRoom
    {
        //ID 
        [Key]
        public int ChatRoomId { get; set; }
        //Название
        public string ChatRoomName { get; set; }
        //Для образования связи "многие-ко-многим" с ApplicationUser
        public virtual List<ApplicationUser> Companions { get; set; }
        public ChatRoom() : base()
        {
            Companions = new List<ApplicationUser>();
        }
    }
}