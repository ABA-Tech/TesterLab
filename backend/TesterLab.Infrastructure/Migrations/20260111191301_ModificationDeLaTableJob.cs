using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesterLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModificationDeLaTableJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnvironmentId",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TestCaseId",
                table: "Jobs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnvironmentId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "TestCaseId",
                table: "Jobs");
        }
    }
}
