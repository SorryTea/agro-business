namespace _02_agro.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    /// <summary>
    /// Klasa ResetujStrukturę odpowiadajaca za ustawienia początkowe środkowiska symulacji
    /// </summary>
    public partial class StructureReset : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Devices",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        IsOn = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roslinies",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Nazwa = c.String(nullable: false),
                        Typ = c.Int(nullable: false),
                        Cena = c.Single(nullable: false),
                        PoziomWzrostu = c.Single(nullable: false),
                        PoziomNawodnienia = c.Single(nullable: false),
                        PoziomNaslonecznienia = c.Single(nullable: false),
                        IsDead = c.Boolean(nullable: false),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        LogId = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.LogId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SystemLogs");
            DropTable("dbo.Roslinies");
            DropTable("dbo.Devices");
        }
    }
}
