using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNet.Models
{
    public class ContactView
    {
        public string UserName { get; set; }
        public string ImagePath { get; set; }
        public bool Message { get; set; }
        public string Name { get; set; }
    }
}