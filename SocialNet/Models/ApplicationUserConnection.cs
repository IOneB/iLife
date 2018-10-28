using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNet.Models
{
    public class ApplicationUserConnection
    {
        [Key, Column(Order = 0), ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        [Key, Column(Order = 1)]
        public int ConnectionID { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Connection Connection { get; set; }

        public string ToUser { get; set; }
    }
}