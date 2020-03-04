namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dataDodAlbum : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Album", "DataDodania", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Album", "DataDodania");
        }
    }
}
