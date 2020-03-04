namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class zmianyda : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Zamowienie", "DataZamowienia", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "LockoutEndDateUtc", c => c.DateTime());
            AlterColumn("dbo.Zamowienie", "DataZamowienia", c => c.DateTime(nullable: false));
        }
    }
}
