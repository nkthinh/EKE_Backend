using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class updatenewdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7144), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7144) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7146), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7146) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7148), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7148) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7150), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7150) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(6999), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(6999) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7002), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7002) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7005), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7005) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7007), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7007) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7009), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7010) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7012), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7012) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7014), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7014) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7016), new DateTime(2025, 8, 10, 5, 30, 16, 525, DateTimeKind.Utc).AddTicks(7016) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9310), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9311) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9313), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9313) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9315), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9315) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9316), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9317) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9102), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9102) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9105), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9105) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9108), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9108) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9110), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9110) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9161), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9162) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9164), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9164) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9166), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9166) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9168), new DateTime(2025, 8, 10, 5, 26, 36, 115, DateTimeKind.Utc).AddTicks(9168) });
        }
    }
}
