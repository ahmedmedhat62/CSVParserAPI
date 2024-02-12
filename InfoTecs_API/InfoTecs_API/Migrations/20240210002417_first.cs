using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoTecs_API.Migrations
{
    /// <inheritdoc />
    public partial class first : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "files",
                columns: table => new
                {
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_files", x => x.file_id);
                });

            migrationBuilder.CreateTable(
                name: "values",
                columns: table => new
                {
                    value_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    DateAndTime = table.Column<string>(type: "text", nullable: false),
                    DateAndTime1 = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IntegerTimeValue = table.Column<int>(type: "integer", nullable: false),
                    FloatingPointIndicator = table.Column<double>(type: "double precision", nullable: false),
                    File_Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_values", x => x.value_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "files");

            migrationBuilder.DropTable(
                name: "values");
        }
    }
}
