namespace Mentoz.AspNet.Api
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class INIT : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Resc",
                c => new
                    {
                        RescId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Identity = c.String(nullable: false, maxLength: 50),
                        Icon = c.String(),
                        Display = c.String(),
                        Available = c.Boolean(nullable: false),
                        ParentId = c.Guid(),
                        CreateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Creator = c.Guid(nullable: false),
                        UpdateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Updator = c.Guid(nullable: false),
                        DeleteTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Deletor = c.Guid(nullable: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.RescId)
                .Index(t => t.Identity, unique: true);
            
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        Display = c.String(nullable: false, maxLength: 50),
                        Available = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Creator = c.Guid(nullable: false),
                        UpdateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Updator = c.Guid(nullable: false),
                        DeleteTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Deletor = c.Guid(nullable: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.RoleId)
                .Index(t => t.Display, unique: true);
            
            CreateTable(
                "dbo.RoleResc",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        RescId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.RescId });
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        Type = c.Int(nullable: false),
                        Account = c.String(nullable: false, maxLength: 50),
                        Password = c.String(),
                        Display = c.String(),
                        Available = c.Boolean(nullable: false),
                        AccessToken = c.String(),
                        AccessTokenExpireTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        RefreshToken = c.String(),
                        RefreshTokenExpireTime = c.DateTime(precision: 7, storeType: "datetime2"),
                        CreateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Creator = c.Guid(nullable: false),
                        UpdateTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Updator = c.Guid(nullable: false),
                        DeleteTime = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Deletor = c.Guid(nullable: false),
                        Version = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Account, unique: true);
            
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId });
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.User", new[] { "Account" });
            DropIndex("dbo.Role", new[] { "Display" });
            DropIndex("dbo.Resc", new[] { "Identity" });
            DropTable("dbo.UserRole");
            DropTable("dbo.User");
            DropTable("dbo.RoleResc");
            DropTable("dbo.Role");
            DropTable("dbo.Resc");
        }
    }
}
