namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datatypesa : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Album", "DataDodania", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Album", "DataDodania", c => c.DateTime(nullable: false));
        }
    }
}
