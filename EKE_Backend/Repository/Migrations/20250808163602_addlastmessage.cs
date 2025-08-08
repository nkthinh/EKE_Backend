using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class addlastmessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       

           
          

            migrationBuilder.AddColumn<string>(
                name: "LastMessage",
                table: "Conversations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1425), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1426) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1428), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1428) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1430), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1430) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1432), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1432) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1254), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1255) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1257), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1258) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1260), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1260) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1263), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1263) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1265), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1265) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1267), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1267) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1269), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1269) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1271), new DateTime(2025, 8, 8, 16, 36, 1, 441, DateTimeKind.Utc).AddTicks(1271) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastMessage",
                table: "Conversations");

       
            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2058), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2059) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2060), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2061) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2062), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2063) });

            migrationBuilder.UpdateData(
                table: "AppSettings",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2064), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(2065) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1900), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1900) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1903), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1903) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1905), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1906) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1908), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1908) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1910), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1910) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1912), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1913) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1914), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1915) });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1916), new DateTime(2025, 8, 8, 6, 46, 45, 901, DateTimeKind.Utc).AddTicks(1917) });

 
        }
    }
}
