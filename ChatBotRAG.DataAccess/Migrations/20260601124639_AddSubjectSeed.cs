using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBotRAG.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "subjects",
                columns: new[] { "id", "created_at", "description", "name" },
                values: new object[] { 1, new DateTime(2026, 6, 1, 12, 46, 38, 638, DateTimeKind.Utc).AddTicks(8770), "General Documents", "General" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "subjects",
                keyColumn: "id",
                keyValue: 1);
        }
    }
}
