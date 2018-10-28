using SocialNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.IO;
using System.Reflection;

namespace SocialNet.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext context = ApplicationDbContext.Create();
        List<MessageViewModel> history;
        const int pageSize = 10;

        [Authorize()]
        public ActionResult Index(string CommentAddressee)
        {
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user;
            if (CommentAddressee != null)
            {
                user = listuser.Find(m => m.Email.Equals(CommentAddressee));
            }
            else
            {
                user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            }
            if (user == null)
                return View("About");
            if (CommentAddressee!=User.Identity.Name)
            {
                var currentUser = listuser.Find(m => m.Email.Equals(User.Identity.Name));
                if (currentUser.Contacts.Contains(user))
                {
                    if (currentUser.Contacts2.Contains(user))
                    {
                        ViewBag.InFriends = "Friend";
                    }
                    else
                    {
                        ViewBag.InFriends = "Request";
                    }
                }
            }
            ViewBag.Gorshok = user.Email;
            ViewBag.TitlePage = "Index";
            List<Post> listpost = context.Posts.ToList();
            List<PostView> PostViews = new List<PostView>();
            IEnumerable<Post> userposts = listpost.Where(m => m.ApplicationUserId.Equals(user.Id));
            List<Attachment> listattach = context.Attachments.ToList();
            List<Mark> listmark = context.Marks.ToList();
            int n = 1;
            foreach (Post post in userposts)
            {
                PostView postView = new PostView
                {
                    Number = n,
                    Post = post
                };
                IEnumerable<Attachment> att = listattach.Where(m => m.PostId.Equals(post.PostId));
                foreach (Attachment attach in att)
                {
                    if (attach.AttachmentPath != null)
                    {
                        int i = attach.AttachmentPath.LastIndexOf('.');
                        string exten = attach.AttachmentPath.Substring(i + 1, attach.AttachmentPath.Length - i - 1).ToLower();
                        if ((exten == "jpg") || (exten == "jpeg") || (exten == "bmp") || (exten == "gif") || (exten == "png"))
                        {
                            postView.PostImages.Add(attach);
                        }
                        if ((exten == "mp3") || (exten == "wma") || (exten == "wav"))
                        {
                            postView.PostAudio.Add(attach);
                        }
                        if ((exten == "mp4") || (exten == "avi") || (exten == "mpeg"))
                        {
                            postView.PostVideo.Add(attach);
                        }
                    }
                }
                IEnumerable<Mark> marks = listmark.Where(m => m.PostId.Equals(post.PostId));
                int rating = 0;
                foreach (Mark m in marks)
                {
                    rating = rating + m.Value;
                }
                postView.PostRating = rating;
                n++;
                PostViews.Add(postView);
            }
            for (int i = 0; i < PostViews.Count / 2; i++)
            {
                PostView postView = PostViews[PostViews.Count - i - 1];
                PostViews[PostViews.Count - i - 1] = PostViews[i];
                PostViews[i] = postView;
            }
            ViewBag.PostViews = PostViews;
            TempData["Model"] = null;
            return View();
        }

        [Authorize()]
        public ActionResult Comment(Post model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<Post> listpost = context.Posts.ToList();
            Post post = listpost.Find(m => m.PostId.Equals(model.PostId));
            List<CommentView> CommentViews = new List<CommentView>();
            List<Comment> listcomment = context.Comments.Include(c => c.ApplicationUser).ToList();
            IEnumerable<Comment> comments = listcomment.Where(m => m.PostId.Equals(post.PostId));
            List<Mark> listmark = context.Marks.ToList();
            foreach (Comment com in comments)
            {
                CommentView commentView = new CommentView
                {
                    Comment = com
                };
                IEnumerable<Mark> marks = listmark.Where(m => m.CommentId.Equals(com.CommentId));
                int rating = 0;
                foreach (Mark m in marks)
                {
                    rating = rating + m.Value;
                }
                commentView.CommentRating = rating;
                if (com.CommentAddressee != null)
                {
                    List<ApplicationUser> listuser = context.Users.ToList();
                    ApplicationUser user = listuser.Find(m => m.Email.Equals(com.CommentAddressee));
                    commentView.Href = user.UserSurname + " " + user.UserFullName + ", ";
                }
                CommentViews.Add(commentView);
            }           
            ViewBag.CommentViews = CommentViews;
            ViewBag.PostId = model.PostId;
            return PartialView("CommentView", CommentViews);           
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize()]
        public ActionResult NewPost()
        {
            ViewBag.Theme = "";
            ViewBag.Text = "";
            return View();
        }

        [Authorize()]
        public ActionResult SavePost(string Theme, string Text)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            Post newpost = new Post
            {
                Theme = Theme,
                Text = Text,
                ApplicationUser = user,
                ApplicationUserId = user.Id,
                SentTime = DateTime.Now
            };
            context.Posts.Add(newpost);
            context.SaveChanges();

            List<Post> listpost = context.Posts.ToList();
            Post NewPost = listpost.Last(m => m.ApplicationUserId.Equals(user.Id));
            var path = Server.MapPath("~/Content/users/" + user.Email + "/newfiles");
            var dir = new DirectoryInfo(path);
            var files = dir.EnumerateFiles().Select(f => f.Name);
            foreach (string file in files)
            {
                Attachment attach = new Attachment
                {
                    Post = NewPost,
                    PostId = NewPost.PostId
                };
                string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
                int num = directory.IndexOf(':');
                directory = directory.Substring(num + 2, directory.Length - (num + 2));
                directory = directory.Substring(0, directory.Length - 4);
                string filepathold = directory + "\\Content\\users\\" + user.Email + "\\newfiles\\" + file; ;
                string filepathnew = "";
                int i = file.LastIndexOf('.');
                string exten = file.Substring(i + 1, file.Length - i - 1).ToLower();
                if ((exten == "jpg") || (exten == "jpeg") || (exten == "bmp") || (exten == "gif") || (exten == "png"))
                {
                    filepathnew = "\\Content\\users\\" + user.Email + "\\image\\" + file;
                }
                if ((exten == "mp3") || (exten == "wma") || (exten == "wav"))
                {
                    filepathnew = "\\Content\\users\\" + user.Email + "\\music\\" + file;
                }
                if ((exten == "mp4") || (exten == "avi") || (exten == "mpeg"))
                {
                    filepathnew = "\\Content\\users\\" + user.Email + "\\video\\" + file;
                }
                attach.AttachmentPath = filepathnew;
                filepathnew = directory + filepathnew;
                if (!System.IO.File.Exists(filepathnew))
                    System.IO.File.Move(filepathold, filepathnew);                    
                else
                    System.IO.File.Delete(filepathold);
                context.Attachments.Add(attach);
            }
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize()]
        [HttpPost]
        public ActionResult AddAttach(HttpPostedFileBase file, string Theme1, string Text1)
        {
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            var path = Path.Combine(Server.MapPath("~/Content/users/" + user.Email + "/newfiles"), file.FileName);

            var data = new byte[file.ContentLength];
            file.InputStream.Read(data, 0, file.ContentLength);

            using (var sw = new FileStream(path, FileMode.Create))
            {
                sw.Write(data, 0, data.Length);
            }

            path = Server.MapPath("~/Content/users/" + user.Email + "/newfiles");
            var dir = new DirectoryInfo(path);
            var files = dir.EnumerateFiles().Select(f => f.Name);
            ViewBag.Theme = Theme1;
            ViewBag.Text = Text1;
            return View("NewPost", files);
        }

        [Authorize()]
        public ActionResult DeleteAttach(string file, string Theme1, string Text1)
        {
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
            int num = directory.IndexOf(':');
            directory = directory.Substring(num + 2, directory.Length - (num + 2));
            directory = directory.Substring(0, directory.Length - 4);
            file = file.Replace("%20", " ");
            string filepath = directory + "\\Content\\users\\" + user.Email + "\\newfiles\\" + file; ;
            System.IO.File.Delete(filepath);

            var path = Server.MapPath("~/Content/users/" + user.Email + "/newfiles");
            var dir = new DirectoryInfo(path);
            var files = dir.EnumerateFiles().Select(f => f.Name);
            ViewBag.Theme = Theme1;
            ViewBag.Text = Text1;
            return View("NewPost", files);
        }

        //Удаление поста
        [Authorize()]
        public ActionResult DeletePost(Post model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<Post> listp = context.Posts.ToList();
            Post deletepost = listp.Find(m => m.PostId.Equals(model.PostId));
            List<Attachment> listatt = context.Attachments.ToList();
            List<Comment> listcom = context.Comments.ToList();
            List<Mark> listm = context.Marks.ToList();
            IEnumerable<Attachment> delattach = listatt.Where(m => m.PostId.Equals(deletepost.PostId));
            foreach (Attachment att in delattach)
            {
                context.Attachments.Remove(att);
            }
            IEnumerable<Comment> delcom = listcom.Where(m => m.PostId.Equals(deletepost.PostId));
            foreach (Comment com in delcom)
            {
                context.Comments.Remove(com);
            }
            IEnumerable<Mark> delm = listm.Where(m => m.PostId.Equals(deletepost.PostId));
            foreach (Mark mark in delm)
            {
                context.Marks.Remove(mark);
            }
            context.Posts.Remove(deletepost);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize()]
        public ActionResult ReplyComment(Comment model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            List<Comment> listcomment = context.Comments.ToList();
            Comment comment = listcomment.Find(m => m.CommentId.Equals(model.CommentId));
            Comment newcomment = new Comment();
            newcomment.ApplicationUser = user;
            newcomment.ApplicationUserId = user.Id;
            newcomment.Post = comment.Post;
            newcomment.PostId = comment.PostId;
            newcomment.CommentAddressee = comment.ApplicationUser.Email;
            ViewBag.NewComment = newcomment;
            ViewBag.Href = comment.ApplicationUser.UserSurname + " " + comment.ApplicationUser.UserFullName + ", ";
            ViewBag.HUserId = comment.ApplicationUser.Id;
            return PartialView("AddComment");
        }      
        
        [Authorize()]
        public ActionResult AddComment(Post model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            List<Post> listpost = context.Posts.ToList();
            Post post = listpost.Find(m => m.PostId.Equals(model.PostId));
            Comment newcomment = new Comment();
            newcomment.ApplicationUser = user;
            newcomment.ApplicationUserId = user.Id;
            newcomment.Post = post;
            newcomment.PostId = post.PostId;
            ViewBag.NewComment = newcomment;
            return PartialView("AddComment");
        }

        [Authorize()]
        public ActionResult SaveComment(string Text, string Href, int PostId, string HUserId)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser usercom = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            List<Post> listp = context.Posts.ToList();
            Post p = listp.Find(m => m.PostId.Equals(PostId));
            ApplicationUser replyuser = null;
            if (HUserId != "")
            {
                replyuser = listuser.Find(m => m.Id.Equals(HUserId));
            }
            if (Href != null)
            {
                int num = Text.IndexOf(',');
                Text = Text.Substring(num + 2, Text.Length - (num + 2));
            }
            Comment newcomment = new Comment();
            newcomment.ApplicationUser = usercom;
            newcomment.ApplicationUserId = usercom.Id;
            newcomment.Post = p;
            newcomment.PostId = p.PostId;
            newcomment.SentTime = DateTime.Now;
            newcomment.Text = Text;
            if (replyuser != null)
            {
                newcomment.CommentAddressee = replyuser.Email;
            }
            context.Comments.Add(newcomment);
            context.SaveChanges();

            ApplicationUser user = listuser.Find(m => m.Id.Equals(p.ApplicationUserId));
            List<Post> listpost = context.Posts.ToList();
            List<PostView> PostViews = new List<PostView>();
            IEnumerable<Post> userposts = listpost.Where(m => m.ApplicationUserId.Equals(user.Id));
            List<Attachment> listattach = context.Attachments.ToList();
            List<Mark> listmark = context.Marks.ToList();
            int n = 1;
            foreach (Post post in userposts)
            {
                PostView postView = new PostView();
                postView.Number = n;
                postView.Post = post;
                IEnumerable<Attachment> att = listattach.Where(m => m.PostId.Equals(post.PostId));
                foreach (Attachment attach in att)
                {
                    if (attach.AttachmentPath != null)
                    {
                        int i = attach.AttachmentPath.LastIndexOf('.');
                        string exten = attach.AttachmentPath.Substring(i + 1, attach.AttachmentPath.Length - i - 1);
                        if ((exten == "jpg") || (exten == "jpeg") || (exten == "bmp") || (exten == "gif") || (exten == "png"))
                        {
                            postView.PostImages.Add(attach);
                        }
                        if ((exten == "mp3") || (exten == "wma") || (exten == "wav"))
                        {
                            postView.PostAudio.Add(attach);
                        }
                        if ((exten == "mp4") || (exten == "avi") || (exten == "mpeg"))
                        {
                            postView.PostVideo.Add(attach);
                        }
                    }
                }
                IEnumerable<Mark> marks = listmark.Where(m => m.PostId.Equals(post.PostId));
                int rating = 0;
                foreach (Mark m in marks)
                {
                    rating = rating + m.Value;
                }
                postView.PostRating = rating;
                n++;
                PostViews.Add(postView);
            }
            for (int i = 0; i < PostViews.Count / 2; i++)
            {
                PostView postView = PostViews[PostViews.Count - i - 1];
                PostViews[PostViews.Count - i - 1] = PostViews[i];
                PostViews[i] = postView;
            }
            ViewBag.PostViews = PostViews;
            TempData["Model"] = null;
            return View("Index");
        }

        [Authorize()]
        public ActionResult DeleteComment(Comment model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<Comment> listcom = context.Comments.ToList();
            Comment deletecomment = listcom.Find(m => m.CommentId.Equals(model.CommentId));
            List<Post> listp = context.Posts.ToList();
            Post p = listp.Find(m => m.PostId.Equals(deletecomment.PostId));
            List<Mark> listm = context.Marks.ToList();
            IEnumerable<Mark> delm = listm.Where(m => m.CommentId.Equals(deletecomment.CommentId));
            foreach (Mark mark in delm)
            {
                context.Marks.Remove(mark);
            }
            context.Comments.Remove(deletecomment);
            context.SaveChanges();

            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser userpost = listuser.Find(m => m.Id.Equals(p.ApplicationUserId));
            string CommentAddressee = userpost.Email;
            ApplicationUser user = listuser.Find(m => m.Email.Equals(CommentAddressee));
            List<Post> listpost = context.Posts.ToList();
            List<PostView> PostViews = new List<PostView>();
            IEnumerable<Post> userposts = listpost.Where(m => m.ApplicationUserId.Equals(user.Id));
            List<Attachment> listattach = context.Attachments.ToList();
            List<Mark> listmark = context.Marks.ToList();
            int n = 1;
            foreach (Post post in userposts)
            {
                PostView postView = new PostView
                {
                    Number = n,
                    Post = post
                };
                IEnumerable<Attachment> att = listattach.Where(m => m.PostId.Equals(post.PostId));
                foreach (Attachment attach in att)
                {
                    if (attach.AttachmentPath != null)
                    {
                        int i = attach.AttachmentPath.LastIndexOf('.');
                        string exten = attach.AttachmentPath.Substring(i + 1, attach.AttachmentPath.Length - i - 1).ToLower();
                        if ((exten == "jpg") || (exten == "jpeg") || (exten == "bmp") || (exten == "gif") || (exten == "png"))
                        {
                            postView.PostImages.Add(attach);
                        }
                        if ((exten == "mp3") || (exten == "wma") || (exten == "wav"))
                        {
                            postView.PostAudio.Add(attach);
                        }
                        if ((exten == "mp4") || (exten == "avi") || (exten == "mpeg"))
                        {
                            postView.PostVideo.Add(attach);
                        }
                    }
                }
                IEnumerable<Mark> marks = listmark.Where(m => m.PostId.Equals(post.PostId));
                int rating = 0;
                foreach (Mark m in marks)
                {
                    rating = rating + m.Value;
                }
                postView.PostRating = rating;
                n++;
                PostViews.Add(postView);
            }
            for (int i = 0; i < PostViews.Count / 2; i++)
            {
                PostView postView = PostViews[PostViews.Count - i - 1];
                PostViews[PostViews.Count - i - 1] = PostViews[i];
                PostViews[i] = postView;
            }
            ViewBag.PostViews = PostViews;
            TempData["Model"] = null;
            return View("Index");
        }

        [Authorize()]
        public ActionResult PostLike(Post model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            List<Post> listp = context.Posts.ToList();
            Post evpost = listp.Find(m => m.PostId.Equals(model.PostId));
            List<Mark> listmarks = context.Marks.ToList();
            IEnumerable<Mark> usermarks = listmarks.Where(m => m.ApplicationUserId.Equals(user.Id));
            Mark mark = new Mark
            {
                ApplicationUser = user,
                ApplicationUserId = user.Id,
                Post = evpost,
                PostId = evpost.PostId,
                Value = 1
            };
            bool contain = false;
            if (usermarks.Count() == 0)
            {
                context.Marks.Add(mark);
            }
            else
            {
                foreach (Mark m in usermarks)
                {
                    if (m.PostId == evpost.PostId)
                    {
                        m.Value = 1;
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    context.Marks.Add(mark);
                }
            }
            context.SaveChanges();

            List<Mark> listmark = context.Marks.ToList();
            IEnumerable<Mark> marks = listmark.Where(m => m.PostId.Equals(model.PostId));
            int rating = 0;
            foreach (Mark m in marks)
            {
                rating = rating + m.Value;
            }
            ViewBag.PostRating = rating;
            return PartialView("PartialRating");
        }

        [Authorize()]
        public ActionResult PostDislike(Post model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            List<Post> listp = context.Posts.ToList();
            Post evpost = listp.Find(m => m.PostId.Equals(model.PostId));
            List<Mark> listmarks = context.Marks.ToList();
            IEnumerable<Mark> usermarks = listmarks.Where(m => m.ApplicationUserId.Equals(user.Id));
            Mark mark = new Mark
            {
                ApplicationUser = user,
                ApplicationUserId = user.Id,
                Post = evpost,
                PostId = evpost.PostId,
                Value = -1
            };
            bool contain = false;
            if (usermarks.Count() == 0)
            {
                context.Marks.Add(mark);
            }
            else
            {
                foreach (Mark m in usermarks)
                {
                    if (m.PostId == evpost.PostId)
                    {
                        m.Value = -1;
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    context.Marks.Add(mark);
                }
            }
            context.SaveChanges();

            List<Mark> listmark = context.Marks.ToList();
            IEnumerable<Mark> marks = listmark.Where(m => m.PostId.Equals(model.PostId));
            int rating = 0;
            foreach (Mark m in marks)
            {
                rating = rating + m.Value;
            }
            ViewBag.PostRating = rating;
            return PartialView("PartialRating");
        }

        [Authorize()]
        public ActionResult CommentLike(Comment model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            List<Comment> listcom = context.Comments.ToList();
            Comment evcomment = listcom.Find(m => m.CommentId.Equals(model.CommentId));
            List<Mark> listmarks = context.Marks.ToList();
            IEnumerable<Mark> usermarks = listmarks.Where(m => m.ApplicationUserId.Equals(user.Id));
            Mark mark = new Mark
            {
                ApplicationUser = user,
                ApplicationUserId = user.Id,
                Comment = evcomment,
                CommentId = evcomment.CommentId,
                Value = 1
            };
            bool contain = false;
            if (usermarks.Count() == 0)
            {
                context.Marks.Add(mark);
            }
            else
            {
                foreach (Mark m in usermarks)
                {
                    if (m.CommentId == evcomment.CommentId)
                    {
                        m.Value = 1;
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    context.Marks.Add(mark);
                }
            }
            context.SaveChanges();

            List<Mark> listmark = context.Marks.ToList();
            IEnumerable<Mark> marks = listmark.Where(m => m.CommentId.Equals(model.CommentId));
            int rating = 0;
            foreach (Mark m in marks)
            {
                rating = rating + m.Value;
            }
            ViewBag.PostRating = rating;
            return PartialView("PartialRating");
        }

        [Authorize()]
        public ActionResult CommentDislike(Comment model)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            List<ApplicationUser> listuser = context.Users.ToList();
            ApplicationUser user = listuser.Find(m => m.Email.Equals(User.Identity.Name));
            List<Comment> listcom = context.Comments.ToList();
            Comment evcomment = listcom.Find(m => m.CommentId.Equals(model.CommentId));
            List<Mark> listmarks = context.Marks.ToList();
            IEnumerable<Mark> usermarks = listmarks.Where(m => m.ApplicationUserId.Equals(user.Id));
            Mark mark = new Mark
            {
                ApplicationUser = user,
                ApplicationUserId = user.Id,
                Comment = evcomment,
                CommentId = evcomment.CommentId,
                Value = -1
            };
            bool contain = false;
            if (usermarks.Count() == 0)
            {
                context.Marks.Add(mark);
            }
            else
            {
                foreach (Mark m in usermarks)
                {
                    if (m.CommentId == evcomment.CommentId)
                    {
                        m.Value = -1;
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    context.Marks.Add(mark);
                }
            }
            context.SaveChanges();

            List<Mark> listmark = context.Marks.ToList();
            IEnumerable<Mark> marks = listmark.Where(m => m.CommentId.Equals(model.CommentId));
            int rating = 0;
            foreach (Mark m in marks)
            {
                rating = rating + m.Value;
            }
            ViewBag.PostRating = rating;
            return PartialView("PartialRating");
        }

        [Authorize()]
        public ActionResult Dialog(string receiverUser, int? id = 0)
        {
            if (id < 0)
                id = 0;
            int page = id ?? 0;
            if (receiverUser.ToLower() == User.Identity.Name.ToLower() || !context.Users.Any(u => u.UserName == receiverUser))
            {
                return RedirectToAction("Index");
            }
            var sender = context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (!sender.Contacts.Any(u=>u.UserName==receiverUser) || !sender.Contacts2.Any(u => u.UserName == receiverUser))
                return RedirectToAction("Index");
            ViewBag.interlocutor = receiverUser;
            history = new List<MessageViewModel>();
            var receiver = context.Users.FirstOrDefault(x => x.UserName == receiverUser);
            var room = context.ChatRooms.FirstOrDefault(r => r.Companions.Any(u => u.UserName == sender.UserName) && r.Companions.Any(u => u.UserName == receiver.UserName));
            if (room == null)
                history = null;
            else
            {
                var itemToSkip = page * pageSize;
                var messages = context.Messages.Where(a => a.ChatRoomId == room.ChatRoomId).OrderByDescending(m => m.MessageId).Skip(itemToSkip).Take(pageSize).ToList();
                foreach (var mess in messages)
                {
                    //var attachment = context.Attachments.Where(a => a.AttachmentId == mess.AttachmentId).FirstOrDefault();
                    history.Add(new MessageViewModel()
                    {
                        From = mess.ApplicationUser.UserName,
                        Date = mess.SentTime,
                        Text = mess.Text,
                        IsRead = mess.IsRead
                    });
                    if (!mess.IsRead && mess.ApplicationUser.UserName != User.Identity.Name)
                    {
                        context.Messages.First(m => mess.MessageId == m.MessageId).IsRead = true;
                    }
                }
                context.SaveChanges();
            }
            history?.Reverse();
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Messages", history);
            }
            return PartialView(history);
        }

        public ActionResult New_Layout()
        {
            ViewBag.Message = "Our new layout";

            return View();
        }

        [Authorize()]
        public ActionResult Search(string text)
        {
            var user = context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var temp = user.Contacts.Where(u =>(u.UserSurname+" "+u.UserFullName).ToLower().Contains(text.ToLower())).ToList();
            var listFriends = temp.Where(u => user.Contacts2.Contains(u)).ToList();
            var listContacts = temp.Where(u=>!user.Contacts2.Contains(u)).ToList();
            List<ApplicationUser> other = new List<ApplicationUser>();
            if (text=="")
            {
                other = context.Users.ToList();
                other.RemoveAll(u=> listFriends.Any(us=>us.UserName == u.UserName));
                other.RemoveAll(u => listContacts.Any(us => us.UserName == u.UserName));
            }
            else
            {
                temp = context.Users.Where(u => (u.UserSurname + " " + u.UserFullName).ToLower().Contains(text.ToLower())).ToList();
                other = temp.Where(u => !user.Contacts.Contains(u)).ToList();
            }
            other.RemoveAll(u => u.UserName == User.Identity.Name);
            listFriends.RemoveAll(u => u.UserName == User.Identity.Name);
            listContacts.RemoveAll(u => u.UserName == User.Identity.Name);
            ViewBag.listFriends = listFriends;
            ViewBag.listContacts = listContacts;
            return View(other);
        }

        [Authorize()]
        public ActionResult Contacts()
        {
            var currentUser = context.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefault();
            var tempContacts = currentUser.Contacts;
            var temp = currentUser.Contacts2;
            List<ContactView> listOfUsers = new List<ContactView>();
            List<ContactView> listOfMyRequests = new List<ContactView>();
            List<ContactView> listOfNewRequests = new List<ContactView>();
            var chatrooms = context.ChatRooms.ToList();
            List<ChatRoom> ourChats = new List<ChatRoom>();
            foreach (var chat in chatrooms)
            {
                if (chat.Companions.Contains(currentUser))
                    ourChats.Add(chat);
            }
            foreach (var user in temp)
            {
                if (!tempContacts.Contains(user))
                {
                    listOfNewRequests.Add(new ContactView()
                    {
                        ImagePath = user.ImagePath,
                        UserName = user.UserName,
                        Message = false,
                        Name = user.UserSurname + " " + user.UserFullName
                    });
                    var chatroom = ourChats.Where(c => c.Companions.Any(com => com.UserName == user.UserName)).FirstOrDefault();
                    if (chatroom != null)
                    {
                        var history = context.Messages.Where(m => m.ChatRoomId == chatroom.ChatRoomId).ToList();
                        if (history.Any(m => m.IsRead != true && m.ApplicationUser.UserName != currentUser.UserName))
                        {
                            listOfNewRequests.Last().Message = true;
                        }
                    }
                }
            }
            foreach (var user in tempContacts)
            {
                if (!currentUser.Contacts2.Contains(user))
                {
                    listOfMyRequests.Add(new ContactView()
                    {
                        ImagePath = user.ImagePath,
                        UserName = user.UserName,
                        Message = false,
                        Name = user.UserSurname + " " + user.UserFullName
                    });
                    var chatroom = ourChats.Where(c => c.Companions.Any(com => com.UserName == user.UserName)).FirstOrDefault();
                    if (chatroom != null)
                    {
                        var history = context.Messages.Where(m => m.ChatRoomId == chatroom.ChatRoomId).ToList();
                        if (history.Any(m => m.IsRead != true && m.ApplicationUser.UserName != currentUser.UserName))
                        {
                            listOfMyRequests.Last().Message = true;
                        }
                    }
                }
                else
                {
                    listOfUsers.Add(new ContactView()
                    {
                        ImagePath = user.ImagePath,
                        UserName = user.UserName,
                        Message = false,
                        Name = user.UserSurname+" "+user.UserFullName
                    });
                    var chatroom = ourChats.Where(c => c.Companions.Any(com => com.UserName == user.UserName)).FirstOrDefault();
                    if (chatroom!=null)
                    {
                        var history = context.Messages.Where(m => m.ChatRoomId == chatroom.ChatRoomId).ToList();
                        if (history.Any(m => m.IsRead != true && m.ApplicationUser.UserName != currentUser.UserName))
                        {
                            listOfUsers.Last().Message = true;
                        }
                    }
                }
            }
            ViewBag.listOfRequests = listOfMyRequests.OrderBy(u=>u.Message).ToList();
            ViewBag.listOfNewRequests = listOfNewRequests.OrderBy(u => u.Message).ToList();
            return View(listOfUsers.OrderBy(u => u.Message).ToList());
        }

        [Authorize()]
        public ActionResult AddContact(string contactName)
        {
            var contactUser = context.Users.FirstOrDefault(u=>u.Email==contactName);
            var currentUser = context.Users.FirstOrDefault(u=>u.Email==User.Identity.Name);
            if (currentUser!=null && contactUser!=null && currentUser!=contactUser)
            {
                if (currentUser.Contacts.Contains(contactUser))
                {
                    return null;
                }
                else
                {
                    currentUser.Contacts.Add(contactUser);
                    context.SaveChanges();
                }
            }
            return null;
        }

        [Authorize()]
        public ActionResult NewRequests()
        {
            var currentUser = context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            foreach (var user in currentUser.Contacts2)
            {
                if (!user.Contacts2.Contains(currentUser))
                {
                    ViewBag.New = true;
                    break;
                }
            }
            var chatrooms = context.ChatRooms.ToList();
            foreach (var chat in chatrooms)
            {
                if (chat.Companions.Contains(currentUser))
                {
                    var history = context.Messages.Where(m => m.ChatRoomId == chat.ChatRoomId).ToList();
                    if (history.Any(m=>!m.IsRead && m.ApplicationUserId!=currentUser.Id))
                    {
                        ViewBag.NewMessages = true;
                        break;
                    }
                }
            }
            return PartialView("_NewContacts");
        }

        [Authorize()]
        public ActionResult CancelRequest(string contactName, bool inverse = false)
        {
            var currentUser = context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
            var contactUser = context.Users.FirstOrDefault(u => u.Email == contactName);
            if (contactUser != null && contactUser != currentUser)
            {
                if (inverse)
                {
                    currentUser.Contacts2.Remove(contactUser);
                }
                else
                {
                    currentUser.Contacts.Remove(contactUser);
                }
                context.SaveChanges();
            }
            return PartialView("_NewContacts");
        }

    }
}