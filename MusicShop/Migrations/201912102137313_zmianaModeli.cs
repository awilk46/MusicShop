namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zmianaModeli : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Piosenka", "AlbumID", "dbo.Album");
            DropIndex("dbo.Piosenka", new[] { "AlbumID" });
            DropColumn("dbo.Album", "NumerSeryjny");
            DropColumn("dbo.Zamowienie", "Promocja");
            DropTable("dbo.Piosenka");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Piosenka",
                c => new
                    {
                        PiosenkaID = c.Int(nullable: false, identity: true),
                        AlbumID = c.Int(nullable: false),
                        TytulPiosenki = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.PiosenkaID);
            
            AddColumn("dbo.Zamowienie", "Promocja", c => c.Int(nullable: false));
            AddColumn("dbo.Album", "NumerSeryjny", c => c.Int(nullable: false));
            CreateIndex("dbo.Piosenka", "AlbumID");
            AddForeignKey("dbo.Piosenka", "AlbumID", "dbo.Album", "AlbumID", cascadeDelete: true);
        }
    }
}
