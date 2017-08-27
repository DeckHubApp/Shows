﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ShtikLive.Shows.Migrate.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Place = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Presenter = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: true),
                    Slug = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Time = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    Title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shows", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Slides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int4", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    HasBeenShown = table.Column<bool>(type: "bool", nullable: false),
                    Html = table.Column<string>(type: "text", nullable: true),
                    Layout = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    Number = table.Column<int>(type: "int4", nullable: false),
                    ShowId = table.Column<int>(type: "int4", nullable: false),
                    Title = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
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
                name: "IX_Shows_Presenter_Slug",
                table: "Shows",
                columns: new[] { "Presenter", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Slides_ShowId",
                table: "Slides",
                column: "ShowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Slides");

            migrationBuilder.DropTable(
                name: "Shows");
        }
    }
}
