using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELearningSystem.Migrations
{
    /// <inheritdoc />
    public partial class MakeFeedbackNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 12, 4, 22, 48, 5, 41, DateTimeKind.Local).AddTicks(6457), "$2a$11$ah0uqjGDGvrhkPjv/da10uA8s59fL7DvLj7wDGSlpYy3wzO8/nmcC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Feedback",
                table: "Submissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Admins",
                keyColumn: "AdminId",
                keyValue: 1,
                columns: new[] { "CreatedDate", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 19, 18, 4, 29, 385, DateTimeKind.Local).AddTicks(9473), "$2a$11$tV8dHQ1P.QeeD9XcU0zaR.NTw.32sSSwjm.Fo9NvxNoBbVR1YU1eG" });
        }
    }
}
