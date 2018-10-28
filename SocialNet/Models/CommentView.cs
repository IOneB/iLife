using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNet.Models
{
    public class CommentView
    {
        public Comment Comment { get; set; }
        public string Href { get; set; }
        public int CommentRating;
    }
}