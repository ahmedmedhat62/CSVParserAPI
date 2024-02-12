using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoTecs_API.Migrations
{
    /// <inheritdoc />
    public partial class results : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "results",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_id = table.Column<Guid>(type: "uuid", nullable: false),
                    All_Time = table.Column<int>(type: "integer", nullable: false),
                    Minimum_Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Average_Time = table.Column<int>(type: "integer", nullable: false),
                    Average_Indicator = table.Column<double>(type: "double precision", nullable: false),
                    Median_Indicator = table.Column<double>(type: "double precision", nullable: false),
                    Maximum_Indicator = table.Column<double>(type: "double precision", nullable: false),
                    Minimum_Indicator = table.Column<double>(type: "double precision", nullable: false),
                    Row_Numbers = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_results", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "results");
        }
    }
}
