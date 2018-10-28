using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace SocialNet.Models
{
    public class Attachment
    {
        //ID вложения
        [Key]
        public int AttachmentId { get; set; }
        //Путь к вложению
        public string AttachmentPath { get; set; }
        //Ссылка на пост
        public int? PostId { get; set; }
        public Post Post { get; set; }
    }
}