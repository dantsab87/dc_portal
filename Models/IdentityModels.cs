using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace dc_portal.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {

        [Display(Name = "First Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name must have a minimum length of 3 and max length of 20.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "First Name must have a minimum length of 3 and max length of 20.")]
        public string LastName { get; set; }

        [Display(Name = "Display Name")]
        [StringLength(25, MinimumLength = 3, ErrorMessage = "First Name must have a minimum length of 3 and max length of 25.")]
        public string DisplayName { get; set; }

        public string AvatarPath { get; set; }


        public virtual Household Household { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }
        public int? HouseholdId { get; set; }

        public ApplicationUser()
        {
            Notifications = new HashSet<Notification>();
            BankAccounts = new HashSet<BankAccount>();
            Transactions = new HashSet<Transaction>();
            Budgets = new HashSet<Budget>();
        }



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
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

        public System.Data.Entity.DbSet<dc_portal.Models.BankAccount> BankAccounts { get; set; }

        public System.Data.Entity.DbSet<dc_portal.Models.Household> Households { get; set; }

        public System.Data.Entity.DbSet<dc_portal.Models.Budget> Budgets { get; set; }

        public System.Data.Entity.DbSet<dc_portal.Models.BudgetItem> BudgetItems { get; set; }

        public System.Data.Entity.DbSet<dc_portal.Models.Invitation> Invitations { get; set; }

        public System.Data.Entity.DbSet<dc_portal.Models.Notification> Notifications { get; set; }

        public System.Data.Entity.DbSet<dc_portal.Models.Transaction> Transactions { get; set; }

        //public System.Data.Entity.DbSet<dc_portal.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<dc_portal.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<dc_portal.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<dc_portal.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}