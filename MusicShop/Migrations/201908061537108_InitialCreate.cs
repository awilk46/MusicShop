namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Album",
                c => new
                    {
                        AlbumID = c.Int(nullable: false, identity: true),
                        KategoriaID = c.Int(nullable: false),
                        NazwaAlbumu = c.String(nullable: false, maxLength: 50),
                        NazwaZespolu = c.String(nullable: false, maxLength: 50),
                        RokWydania = c.Int(nullable: false),
                        RodzajKrazka = c.Int(nullable: false),
                        NumerSeryjny = c.Int(nullable: false),
                        Okladkalbumu = c.String(),
                        Bestseller = c.Boolean(nullable: false),
                        Ukryty = c.Boolean(nullable: false),
                        CenaAlbumu = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.AlbumID)
                .ForeignKey("dbo.Kategoria", t => t.KategoriaID, cascadeDelete: true)
                .Index(t => t.KategoriaID);
            
            CreateTable(
                "dbo.Kategoria",
                c => new
                    {
                        KategoriaID = c.Int(nullable: false, identity: true),
                        NazwaKategorii = c.String(nullable: false, maxLength: 50),
                        ObrazKategorii = c.String(),
                    })
                .PrimaryKey(t => t.KategoriaID);
            
            CreateTable(
                "dbo.Piosenka",
                c => new
                    {
                        PiosenkaID = c.Int(nullable: false, identity: true),
                        AlbumID = c.Int(nullable: false),
                        TytulPiosenki = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.PiosenkaID)
                .ForeignKey("dbo.Album", t => t.AlbumID, cascadeDelete: true)
                .Index(t => t.AlbumID);
            
            CreateTable(
                "dbo.PozycjaZamowienia",
                c => new
                    {
                        PozycjaZamowieniaID = c.Int(nullable: false, identity: true),
                        ZamowienieID = c.Int(nullable: false),
                        AlbumID = c.Int(nullable: false),
                        Ilosc = c.Int(nullable: false),
                        CenaZakupu = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.PozycjaZamowieniaID)
                .ForeignKey("dbo.Album", t => t.AlbumID, cascadeDelete: true)
                .ForeignKey("dbo.Zamowienie", t => t.ZamowienieID, cascadeDelete: true)
                .Index(t => t.ZamowienieID)
                .Index(t => t.AlbumID);
            
            CreateTable(
                "dbo.Zamowienie",
                c => new
                    {
                        ZamowienieID = c.Int(nullable: false, identity: true),
                        Imie = c.String(nullable: false, maxLength: 50),
                        Nazwisko = c.String(nullable: false, maxLength: 50),
                        Ulica = c.String(nullable: false, maxLength: 50),
                        Miasto = c.String(nullable: false, maxLength: 50),
                        KodPocztowy = c.String(nullable: false, maxLength: 50),
                        Phone = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        DataZamowienia = c.DateTime(nullable: false),
                        CenaZamowienia = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StanZamowienia = c.Int(nullable: false),
                        Promocja = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ZamowienieID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PozycjaZamowienia", "ZamowienieID", "dbo.Zamowienie");
            DropForeignKey("dbo.PozycjaZamowienia", "AlbumID", "dbo.Album");
            DropForeignKey("dbo.Piosenka", "AlbumID", "dbo.Album");
            DropForeignKey("dbo.Album", "KategoriaID", "dbo.Kategoria");
            DropIndex("dbo.PozycjaZamowienia", new[] { "AlbumID" });
            DropIndex("dbo.PozycjaZamowienia", new[] { "ZamowienieID" });
            DropIndex("dbo.Piosenka", new[] { "AlbumID" });
            DropIndex("dbo.Album", new[] { "KategoriaID" });
            DropTable("dbo.Zamowienie");
            DropTable("dbo.PozycjaZamowienia");
            DropTable("dbo.Piosenka");
            DropTable("dbo.Kategoria");
            DropTable("dbo.Album");
        }
    }
}
