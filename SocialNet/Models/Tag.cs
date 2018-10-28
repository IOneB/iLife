using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SocialNet.Models
{
    public class Tag
    {
        //ID тэга 
        [Key]
        public int TagId { get; set; }
        //Наименование тэга
        public string TagName { get; set; }
        //Для образования связи "многие-ко-многим" с Post
        public virtual ICollection<Post> Posts { get; set; }
        public Tag() : base()
        {
            Posts = new List<Post>();
        }
    }
}