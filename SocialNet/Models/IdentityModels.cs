using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace SocialNet.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
            Contacts = new List<ApplicationUser>();
            Contacts2 = new List<ApplicationUser>();
            ChatRooms = new List<ChatRoom>();
        }
        //Фамилия пользователя
        public string UserSurname { get; set; }
        //Имя пользователя
        public string UserFullName { get; set; }
        //Путь к фотографии пользователя
        public string ImagePath { get; set; }
        //Контакты пользователя
        public virtual ICollection<ApplicationUser> Contacts { get; set; }
        public virtual ICollection<ApplicationUser> Contacts2 { get; set; }
    
        //Многие-ко-многим с Connection
        public virtual ICollection<ApplicationUserConnection> ApplicationUserConnection { get; set; }

        //Для образования связи "многие-ко-многим" с ChatRoom
        public virtual ICollection<ChatRoom> ChatRooms { get; set; }

        //public List<ApplicationUser> Contacts; Связь много-ко-многим Metanit
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Обратите внимание, что authenticationType должен совпадать с типом, определенным в CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Здесь добавьте утверждения пользователя
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();

        }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Mark> Marks { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ApplicationUserConnection> UserConnections { get; set; }
    }
}