using Microsoft.AspNet.Identity.EntityFramework;
using MusicShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace MusicShop.DAL
{
    public class AlbumContext : IdentityDbContext<ApplicationUser>
    {
        public AlbumContext() : base("AlbumContext")
        {
        }
        static AlbumContext()
        {
            Database.SetInitializer<AlbumContext>(new AlbumInitializer());
        }
        public static AlbumContext Create()
        {
            return new AlbumContext();
        }

        public DbSet<Album> Albumy { get; set; }
        public DbSet<Kategoria> Kategorie { get; set; }
        public DbSet<Zamowienie> Zamowienia { get; set; }
        public DbSet<PozycjaZamowienia> PozycjeZamowienia { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));
        }

    }
}