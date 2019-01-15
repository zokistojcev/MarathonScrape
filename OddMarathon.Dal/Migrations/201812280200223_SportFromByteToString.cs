namespace OddMarathon.Dal.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SportFromByteToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Odds", "Sport", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Odds", "Sport", c => c.Byte(nullable: false));
        }
    }
}
