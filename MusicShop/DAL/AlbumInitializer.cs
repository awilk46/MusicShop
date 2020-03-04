using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MusicShop.Migrations;
using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace MusicShop.DAL
{
    public class AlbumInitializer : System.Data.Entity.MigrateDatabaseToLatestVersion<AlbumContext,Configuration>
    {


        public static void SeedAlbumData(AlbumContext context)
        {
            var kategorie = new List<Kategoria>
            {
                new Kategoria(){KategoriaID=1,NazwaKategorii="Rock",ObrazKategorii="rock.png"},
                new Kategoria(){KategoriaID=2,NazwaKategorii="Metal",ObrazKategorii="metal.png"},
                new Kategoria(){KategoriaID=3,NazwaKategorii="Rap",ObrazKategorii="rap.png"},
            };
            kategorie.ForEach(c => context.Kategorie.AddOrUpdate(c));
            context.SaveChanges();



            var albumy = new List<Album>
            {
                new Album(){KategoriaID=1,NazwaAlbumu="AM",NazwaZespolu="Arctic Monkeys",RokWydania=2013,RodzajKrazka=RodzajKrazka.DVD,
                   Okladkalbumu="am.png",Bestseller=true,Ukryty=false,CenaAlbumu=30, DataDodania = DateTime.Now},
                new Album(){KategoriaID=2,NazwaAlbumu="St. Anger",NazwaZespolu="Metallica",RokWydania=2003,RodzajKrazka=RodzajKrazka.DVD,
                    Okladkalbumu="stanger.png",Bestseller=false,Ukryty=false,CenaAlbumu=30, DataDodania = DateTime.Now},
                new Album(){KategoriaID=3,NazwaAlbumu="Relapse",NazwaZespolu="Eminem",RokWydania=2009,RodzajKrazka=RodzajKrazka.CD,
                    Okladkalbumu="relapse.png",Bestseller=true,Ukryty=false,CenaAlbumu=30, DataDodania = DateTime.Now},
                 new Album(){KategoriaID=3,NazwaAlbumu="Niewiem",NazwaZespolu="LOL",RokWydania=2009,RodzajKrazka=RodzajKrazka.CD,
                    Okladkalbumu="relapse.png",Bestseller=true,Ukryty=false,CenaAlbumu=30, DataDodania = DateTime.Now}
            };
            albumy.ForEach(a => context.Albumy.AddOrUpdate(a));
            context.SaveChanges();

            //var loans = new List<Loan>
            //{

            //};
            //loans.ForEach(l => context.Loans.AddOrUpdate(l));
            //context.SaveChanges();

            //var loanPosition = new List<LoanPosition>
            //{
            //};
            //loanPosition.ForEach(c => context.LoanPositions.AddOrUpdate(c));
            //context.SaveChanges();

            //var song = new List<Song>
            //{
            //    new Song(){ AlbumID=1,SongTitle="Do I Wanna Know"}
            //};
            //song.ForEach(s => context.Songs.AddOrUpdate(s));
            //context.SaveChanges();
        }
        public static void SeedUzytkownicy(AlbumContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            const string name = "admin@admin.pl";
            const string password = "P@ssw0rd";
            const string roleName = "Admin";
            const string roleName2 = "Member";

            const bool emailConfirmed = true;

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name, EmailConfirmed=emailConfirmed, DaneUzytkownika = new DaneUzytkownika() };
                var result = userManager.Create(user, password);
            }

            // utworzenie roli Admin jeśli nie istnieje 
            var role = roleManager.FindByName(roleName);
            var role2 = roleManager.FindByName(roleName2);

            if (role == null)
            {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }
            if (role2 == null)
            {
                role2 = new IdentityRole(roleName2);
                var roleresult = roleManager.Create(role2);
            }

            // dodanie uzytkownika do roli Admin jesli juz nie jest w roli
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }
}