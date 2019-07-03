namespace SecuroteckWebApplication.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ApiKey = c.String(nullable: false, maxLength: 128),
                        userName = c.String(),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.ApiKey);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}
