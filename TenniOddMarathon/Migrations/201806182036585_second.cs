namespace TenniOddMarathon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Koeficientis",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TennisOddId = c.Int(nullable: false),
                        KoeficientFirst = c.String(),
                        KoeficientSecond = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TennisOdds", t => t.TennisOddId, cascadeDelete: true)
                .Index(t => t.TennisOddId);
            
            CreateTable(
                "dbo.TennisOdds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParOne = c.String(),
                        ParTwo = c.String(),
                        DateAndBeginingTime = c.String(),
                        TurnirDataPocetok = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Koeficientis", "TennisOddId", "dbo.TennisOdds");
            DropIndex("dbo.Koeficientis", new[] { "TennisOddId" });
            DropTable("dbo.TennisOdds");
            DropTable("dbo.Koeficientis");
        }
    }
}
