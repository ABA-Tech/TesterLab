using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TesterLab.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjoutUserCreatedIdSurJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Jobs",
                type: "TEXT",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Jobs");

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1463));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 2,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1469));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 3,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1470));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1472));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1473));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 6,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1477));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 7,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1478));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 10,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1480));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 11,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1481));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 12,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1483));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 13,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1484));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 14,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1486));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 15,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1487));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 20,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1488));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 21,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1489));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 22,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1491));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 23,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1492));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 24,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1527));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 25,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1528));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 26,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1530));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 27,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1532));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 28,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1533));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 30,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1534));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 31,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1535));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 32,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1536));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 33,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1537));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 34,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1539));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 35,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1540));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 36,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1541));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 37,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1542));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 40,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1544));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 41,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1545));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 42,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1546));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 50,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1551));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 51,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1552));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 52,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1553));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 53,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1554));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 60,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1556));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 61,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1557));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 62,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1558));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 63,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1559));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 64,
                column: "UpdatedAt",
                value: new DateTime(2026, 6, 25, 19, 51, 20, 303, DateTimeKind.Utc).AddTicks(1560));
        }
    }
}
