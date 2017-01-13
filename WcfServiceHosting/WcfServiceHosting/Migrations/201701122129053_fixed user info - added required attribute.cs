namespace WcfServiceHosting.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixeduserinfoaddedrequiredattribute : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CurrentUsers", "CurrentUserName", c => c.String(nullable: false));
            AlterColumn("dbo.CurrentUsers", "CurrentUserPassword", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CurrentUsers", "CurrentUserPassword", c => c.String());
            AlterColumn("dbo.CurrentUsers", "CurrentUserName", c => c.String());
        }
    }
}
