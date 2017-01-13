namespace WcfServiceHosting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CurrentUsers",
                c => new
                    {
                        CurrentUserId = c.Int(nullable: false, identity: true),
                        CurrentUserName = c.String(),
                        CurrentUserPassword = c.String(),
                    })
                .PrimaryKey(t => t.CurrentUserId);
            
            CreateTable(
                "dbo.UserFiles",
                c => new
                    {
                        UserFileId = c.Int(nullable: false, identity: true),
                        UserFileName = c.String(),
                        UserFileDescription = c.String(),
                        UserFilePath = c.String(),
                        CurrentUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserFileId)
                .ForeignKey("dbo.CurrentUsers", t => t.CurrentUserId, cascadeDelete: true)
                .Index(t => t.CurrentUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserFiles", "CurrentUserId", "dbo.CurrentUsers");
            DropIndex("dbo.UserFiles", new[] { "CurrentUserId" });
            DropTable("dbo.UserFiles");
            DropTable("dbo.CurrentUsers");
        }
    }
}
