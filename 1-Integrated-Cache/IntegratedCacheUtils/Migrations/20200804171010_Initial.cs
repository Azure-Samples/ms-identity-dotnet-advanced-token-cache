using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace IntegratedCacheUtils.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MsalAccountActivities",
                columns: table => new
                {
                    AccountCacheKey = table.Column<string>(nullable: false),
                    AccountIdentifier = table.Column<string>(nullable: true),
                    AccountObjectId = table.Column<string>(nullable: true),
                    AccountTenantId = table.Column<string>(nullable: true),
                    UserPrincipalName = table.Column<string>(nullable: true),
                    LastActivity = table.Column<DateTime>(nullable: false),
                    FailedToAcquireToken = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsalAccountActivities", x => x.AccountCacheKey);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MsalAccountActivities");
        }
    }
}