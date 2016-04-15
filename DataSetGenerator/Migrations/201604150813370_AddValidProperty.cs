namespace DataSetGenerator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddValidProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attempts", "Valid", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attempts", "Valid");
        }
    }
}
