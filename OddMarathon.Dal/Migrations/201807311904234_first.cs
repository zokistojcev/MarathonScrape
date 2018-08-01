namespace OddMarathon.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CoefficientsFootballs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OddId = c.Int(nullable: false),
                        CoefficientHost = c.String(),
                        CoefficientDraw = c.String(),
                        CoefficientVisitor = c.String(),
                        DateAndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Odds", t => t.OddId, cascadeDelete: true)
                .Index(t => t.OddId);
            
            CreateTable(
                "dbo.Odds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PairOne = c.String(),
                        PairTwo = c.String(),
                        BeginingTime = c.DateTime(nullable: false),
                        Tournament = c.String(),
                        Sport = c.Byte(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CoefficientsTennis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OddId = c.Int(nullable: false),
                        CoefficientFirst = c.String(),
                        CoefficientSecond = c.String(),
                        DateAndTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Odds", t => t.OddId, cascadeDelete: true)
                .Index(t => t.OddId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CoefficientsTennis", "OddId", "dbo.Odds");
            DropForeignKey("dbo.CoefficientsFootballs", "OddId", "dbo.Odds");
            DropIndex("dbo.CoefficientsTennis", new[] { "OddId" });
            DropIndex("dbo.CoefficientsFootballs", new[] { "OddId" });
            DropTable("dbo.CoefficientsTennis");
            DropTable("dbo.Odds");
            DropTable("dbo.CoefficientsFootballs");
        }
    }
}
