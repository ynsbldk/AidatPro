using System;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AidatPro.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual DateTime? LastLoginTime { get; set; }
        public virtual DateTime? RegistrationDate { get; set; }       

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
            : base("AidatPro", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<BlokModel> Bloks { get; set; }

        public DbSet<DaireModel> Daires { get; set; }

        public DbSet<DemirbasModel> Demirbas { get; set; }

        public DbSet<DemirbasTeslimModel> DemirbasTeslim { get; set; }

        public DbSet<PersonelModel> Personels { get; set; }

        public DbSet<KasaModel> Kasa { get; set; }

        public DbSet<KasaBakiyeModel> KasaBakiye { get; set; }

        public DbSet<GelirModel> Gelirs { get; set; }

        public DbSet<GiderModel> Giders { get; set; }

        public DbSet<HesapModel> Hesaps { get; set; }

        public DbSet<AidatModel> Aidats { get; set; }

        public DbSet<DaireAccountModel> DaireAccount { get; set; }

        public DbSet<EtkinlikModel> Etkinliks { get; set; }


    }
}