namespace MusicShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class edycjaZamowien : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Zamowienie", name: "ApplicationUser_Id", newName: "UserID");
            RenameIndex(table: "dbo.Zamowienie", name: "IX_ApplicationUser_Id", newName: "IX_UserID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Zamowienie", name: "IX_UserID", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.Zamowienie", name: "UserID", newName: "ApplicationUser_Id");
        }
    }
}
