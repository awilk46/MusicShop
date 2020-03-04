namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dodNowoscAlbum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Album", "Nowosc", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Album", "Nowosc");
        }
    }
}
