namespace MWV.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AssignedMachine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "AssignedMachine", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "AssignedMachine");
        }
    }
}
