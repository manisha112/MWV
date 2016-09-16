namespace MWV.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "name", c => c.String());
            AddColumn("dbo.AspNetUsers", "mobile", c => c.String());
            AddColumn("dbo.AspNetUsers", "landline", c => c.String());
            AddColumn("dbo.AspNetUsers", "address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "address");
            DropColumn("dbo.AspNetUsers", "landline");
            DropColumn("dbo.AspNetUsers", "mobile");
            DropColumn("dbo.AspNetUsers", "name");
        }
    }
}
