namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ZmianaTypuDaty : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Album", "DataDodania", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Album", "DataDodania", c => c.Int(nullable: false));
        }
    }
}
