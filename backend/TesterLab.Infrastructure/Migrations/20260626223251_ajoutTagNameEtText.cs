using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesterLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ajoutTagNameEtText : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TagName",
                table: "TestSteps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "TestSteps",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2740));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2750));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2750));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2750));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2750));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2760));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2770));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 20,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2770));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 21,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2770));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 22,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2770));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 23,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2770));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 24,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2780));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 25,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2780));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 26,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2780));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 27,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2780));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 28,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2780));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 30,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2780));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 31,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 32,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 33,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 34,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 35,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 36,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2790));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 37,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2800));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 40,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2800));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 41,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2800));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 42,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2800));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 50,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2800));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 51,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 52,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 53,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 60,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 61,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 62,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 63,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2860));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 64,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 26, 22, 32, 51, 408, DateTimeKind.Utc).AddTicks(2870));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagName",
                table: "TestSteps");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "TestSteps");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(883));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(889));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(890));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(891));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(893));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(897));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(898));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(899));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(900));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(902));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(903));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(904));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(905));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 20,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(906));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 21,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(907));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 22,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(908));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 23,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(909));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 24,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(911));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 25,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(912));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 26,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(913));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 27,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(914));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 28,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(915));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 30,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(916));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 31,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(917));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 32,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(918));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 33,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(919));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 34,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(920));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 35,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(920));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 36,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(921));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 37,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(923));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 40,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(924));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 41,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(925));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 42,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(926));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 50,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(928));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 51,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(929));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 52,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(930));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 53,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(940));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 60,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(941));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 61,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(942));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 62,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(943));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 63,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(944));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 64,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 20, 23, 52, 97, DateTimeKind.Utc).AddTicks(945));
        }
    }
}
