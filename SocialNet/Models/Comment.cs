using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialNet.Models
{
    public class Comment
    {
        //ID комментария
        [Key]
        public int CommentId { get; set; }
        //ID пользователя
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        //Ссылка на пост
        public int? PostId { get; set; }
        public Post Post { get; set; }
        //Текст комментария
        public string Text { get; set; }
        //Время отправки
        public DateTime SentTime { get; set; }
        //Адресат комментария
        public string CommentAddressee { get; set; }
    }
}