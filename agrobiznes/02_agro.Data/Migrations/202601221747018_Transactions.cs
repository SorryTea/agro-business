namespace _02_agro.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Transactions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        OccurredAt = c.DateTimeOffset(nullable: false, precision: 7),
                        Amount_Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount_Currency = c.String(),
                        Category = c.Int(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Transactions");
        }
    }
}
