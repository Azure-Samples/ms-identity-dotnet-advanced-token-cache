using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MsalAccountActivities",
                columns: table => new
                {
                    CacheKey = table.Column<string>(nullable: false),
                    AccountObjectId = table.Column<string>(nullable: true),
                    AccountIdentifier = table.Column<string>(nullable: true),
                    AccountTenantId = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Environment = table.Column<string>(nullable: true),
                    LastActivity = table.Column<DateTime>(nullable: false),
                    FailedToRefresh = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MsalAccountActivities", x => x.CacheKey);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MsalAccountActivities");
        }
    }
}
