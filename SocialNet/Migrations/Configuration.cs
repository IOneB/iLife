namespace SocialNet.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using SocialNet.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SocialNet.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SocialNet.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            //var usermanager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            //context.Users.Add(new ApplicationUser() { Id = new Guid().ToString() });
            //context.Users.Add(new ApplicationUser() { Id = new Guid().ToString() });
            //context.Messages.Find("4").InvestmentPath.Add("123");
            //context.Messages.Add(new Message() { ApplicationUserId = context.Users.First().Id, SentTime = DateTime.Now, ApplicationUserReceiver = context.Users.Find("4c01099d-4e68-40ab-abdb-c55bc83c7b2f").Id });
            //context.Comments.Add(new Comment() { SentTime = DateTime.Now });
            //context.Posts.Add(new Post() { SentTime = DateTime.Now });
            //context.Marks.Add(new Mark() { Value = true, Post = context.Posts.First(), Comment = context.Comments.First() });
        }
    }
}
