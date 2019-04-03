using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GlobalSite.Models
{
    // В профиль пользователя можно добавить дополнительные данные, если указать больше свойств для класса ApplicationUser. Подробности см. на странице https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string LastName { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public string SecurityString { get; set; }
        public string CardNumber { get; set; }
        public virtual ICollection<Album> Albums { get; set; }
        public ApplicationUser()
        {
            Albums = new List<Album>();
        }

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

        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class AppDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            // создаем две роли
            var role1 = new IdentityRole { Name = "admin" };
            var role2 = new IdentityRole { Name = "user" };

            // добавляем роли в бд
            roleManager.Create(role1);
            roleManager.Create(role2);

            // создаем пользователей
            var admin = new ApplicationUser { Email = "admin@gmail.com", UserName = "admin@gmail.com" };
            string passwordAdmin = "8zkXr:Ze8_uC_rM";
            var user = new ApplicationUser { Email = "user@gmail.com", UserName = "user@gmail.com" };
            string passwordUser = "3zkXr:Ze8_uC_rM";

            var resultAdmin = userManager.Create(admin, passwordAdmin);
            var resultUser = userManager.Create(user, passwordUser);

            // если создание пользователя прошло успешно
            if (resultAdmin.Succeeded && resultUser.Succeeded)
            {
                // добавляем для пользователя роль
                userManager.AddToRole(admin.Id, role1.Name);
                userManager.AddToRole(user.Id, role2.Name);
            }

            base.Seed(context);
        }
    }
}