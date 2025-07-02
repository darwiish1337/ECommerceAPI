using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounts.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserVerificationTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "createdbyuseragent",
                table: "refreshtokens",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "revokedbyuseragent",
                table: "refreshtokens",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "userverifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    userid = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    expiresat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    isused = table.Column<bool>(type: "boolean", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_userverifications", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userverifications_userid_type",
                table: "userverifications",
                columns: new[] { "userid", "type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "userverifications");

            migrationBuilder.DropColumn(
                name: "createdbyuseragent",
                table: "refreshtokens");

            migrationBuilder.DropColumn(
                name: "revokedbyuseragent",
                table: "refreshtokens");
        }
    }
}
