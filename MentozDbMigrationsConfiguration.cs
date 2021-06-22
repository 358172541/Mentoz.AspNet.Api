using System;
using System.Data.Entity.Migrations;

namespace Mentoz.AspNet.Api
{
    public class MentozDbMigrationsConfiguration : DbMigrationsConfiguration<MentozDbContext>
    {
        public MentozDbMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }
        protected override void Seed(MentozDbContext context)
        {
            context.Set<Resc>().AddOrUpdate(
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d00"),
                    Type = RescType.MENU,
                    Identity = "System",
                    Icon = "form",
                    Display = "System",
                    Available = true,
                    ParentId = null
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d01"),
                    Type = RescType.MENU,
                    Identity = "Resc",
                    Icon = "form",
                    Display = "Resc",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d00")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d02"),
                    Type = RescType.CTRL,
                    Identity = "Resc.Create",
                    Icon = "form",
                    Display = "Create",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d01")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d03"),
                    Type = RescType.CTRL,
                    Identity = "Resc.Update",
                    Icon = "form",
                    Display = "Update",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d01")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d04"),
                    Type = RescType.CTRL,
                    Identity = "Resc.Delete",
                    Icon = "form",
                    Display = "Delete",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d01")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d05"),
                    Type = RescType.MENU,
                    Identity = "Role",
                    Icon = "form",
                    Display = "Role",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d00")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d06"),
                    Type = RescType.CTRL,
                    Identity = "Role.Create",
                    Icon = "form",
                    Display = "Create",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d05")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d07"),
                    Type = RescType.CTRL,
                    Identity = "Role.Update",
                    Icon = "form",
                    Display = "Update",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d05")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d08"),
                    Type = RescType.CTRL,
                    Identity = "Role.Delete",
                    Icon = "form",
                    Display = "Delete",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d05")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d09"),
                    Type = RescType.MENU,
                    Identity = "User",
                    Icon = "form",
                    Display = "User",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d00")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d10"),
                    Type = RescType.CTRL,
                    Identity = "User.Create",
                    Icon = "form",
                    Display = "Create",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d09")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d11"),
                    Type = RescType.CTRL,
                    Identity = "User.Update",
                    Icon = "form",
                    Display = "Update",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d09")
                },
                new Resc
                {
                    RescId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d12"),
                    Type = RescType.CTRL,
                    Identity = "User.Delete",
                    Icon = "form",
                    Display = "Delete",
                    Available = true,
                    ParentId = Guid.Parse("890d6a3c-6a7f-4277-8e24-e14168f61d09")
                });

            context.Set<User>().AddOrUpdate(
                new User
                {
                    UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Type = UserType.MANAGER,
                    Account = "tester",
                    Password = "tester",
                    Display = "tester",
                    Available = true
                });
        }
    }
}