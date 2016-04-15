namespace DataSetGenerator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attempts",
                c => new
                    {
                        AttemptID = c.Guid(nullable: false, identity: true),
                        ID = c.String(),
                        Time = c.Time(nullable: false, precision: 7),
                        Hit = c.Boolean(nullable: false),
                        Shape = c.Boolean(nullable: false),
                        TargetCell_X = c.Double(nullable: false),
                        TargetCell_Y = c.Double(nullable: false),
                        CurrentCell_X = c.Double(nullable: false),
                        CurrentCell_Y = c.Double(nullable: false),
                        Pointer_X = c.Double(nullable: false),
                        Pointer_Y = c.Double(nullable: false),
                        Size = c.Int(nullable: false),
                        Direction = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        Source = c.Int(nullable: false),
                        AttemptNumber = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AttemptID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Attempts");
        }
    }
}
