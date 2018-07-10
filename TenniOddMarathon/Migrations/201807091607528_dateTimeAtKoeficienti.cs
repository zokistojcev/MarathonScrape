namespace TenniOddMarathon.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dateTimeAtKoeficienti : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Koeficientis", "DateAndTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Koeficientis", "DateAndTime");
        }
    }
}
