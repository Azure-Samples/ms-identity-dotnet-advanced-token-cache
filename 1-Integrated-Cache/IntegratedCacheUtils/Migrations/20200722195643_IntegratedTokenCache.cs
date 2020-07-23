using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace IntegratedCacheUtils.Migrations
{
    public partial class IntegratedTokenCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MsalAccountActivities",
                columns: table => new
                {
                    AccountIdentifier = table.Column<string>(nullable: false),
                    AccountObjectId = table.Column<string>(nullable: true),
                    AccountTenantId = table.Column<string>(nullable: true),
                    LastActivity = table.Column<DateTime>(nullable: false),
                    FailedToAcquireToken = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsalAccountActivities", x => x.AccountIdentifier);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MsalAccountActivities");
        }
    }
}
