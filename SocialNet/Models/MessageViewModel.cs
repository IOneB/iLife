using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNet.Models
{
    public class MessageViewModel
    {
        public string Text { get; set; }
        public string From { get; set; }
        public DateTime Date { get; set; }
        public bool IsRead { get; set; }
    }
}