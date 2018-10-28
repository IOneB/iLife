using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNet.Models
{
    public class Connection
    {
        public int ConnectionID { get; set; }
        public string ConnectionName { get; set; }

        public virtual ICollection<ApplicationUserConnection> ApplicationUserConnection { get; set; }
    }
}