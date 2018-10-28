using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SocialNet.Models
{
    public class Mark
    {
        //ID оценки
        [Key]
        public int MarkId { get; set; }
        public int Value { get; set; }
        //ID пользователя
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        //Ссылка на пост
        public int? PostId { get; set; }
        public Post Post { get; set; }
        //Ссылка на комментарий
        public int? CommentId { get; set; }
        public Comment Comment { get; set; }
    }
}