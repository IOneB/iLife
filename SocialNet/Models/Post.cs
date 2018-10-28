using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNet.Models
{

    public class Post
    {
        //ID поста
        [Key]
        public int PostId { get; set; }
        //ID пользователя
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        //Тема поста
        public string Theme { get; set; }
        //Текст поста
        public string Text { get; set; }
        //Время отправки
        public DateTime SentTime { get; set; }
    }
}