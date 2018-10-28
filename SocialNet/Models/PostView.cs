using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SocialNet.Models
{
    public class PostView
    {
        public int Number { get; set; }
        public Post Post { get; set; }
        public List<Attachment> PostImages;
        public List<Attachment> PostAudio;
        public List<Attachment> PostVideo;
        public List<Comment> PostComments;
        public int PostRating;

        public PostView()
        {
            PostImages = new List<Attachment>();
            PostAudio = new List<Attachment>();
            PostVideo = new List<Attachment>();
            PostComments = new List<Comment>();
        }
    }
}