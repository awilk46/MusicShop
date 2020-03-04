namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodaniePolaOpis : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Album", "Opis", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Album", "Opis");
        }
    }
}
