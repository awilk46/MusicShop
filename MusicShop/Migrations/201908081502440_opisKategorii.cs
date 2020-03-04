namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class opisKategorii : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Kategoria", "OpisKategorii", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Kategoria", "OpisKategorii");
        }
    }
}
