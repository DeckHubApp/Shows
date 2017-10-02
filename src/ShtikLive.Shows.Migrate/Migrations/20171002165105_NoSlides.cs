using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ShtikLive.Shows.Migrate.Migrations
{
    public partial class NoSlides : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slides");

            migrationBuilder.AddColumn<int>(
                name: "HighestSlideShown",
                table: "Shows",
                type: "int4",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighestSlideShown",
                table: "Shows");

            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HasBeenShown = table.Column<bool>(nullable: false),
                    Html = table.Column<string>(nullable: true),
                    Layout = table.Column<string>(maxLength: 256, nullable: true),
                    Number = table.Column<int>(nullable: false),
                    ShowId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slides_Shows_ShowId",
                        column: x => x.ShowId,
                        principalTable: "Shows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Slides_ShowId",
                table: "Slides",
                column: "ShowId");
        }
    }
}
