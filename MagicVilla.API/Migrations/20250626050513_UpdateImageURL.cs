﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagicVilla.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImageURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageURL",
                value: "https://placehold.co/600x401");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageURL",
                value: "https://placehold.co/600x402");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageURL",
                value: "https://placehold.co/600x403");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageURL",
                value: "https://placehold.co/600x404");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageURL",
                value: "https://placehold.co/600x405");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageURL",
                value: "https://dotnetmastery.com/bluevillaimages/villa3.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageURL",
                value: "https://dotnetmastery.com/bluevillaimages/villa1.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageURL",
                value: "https://dotnetmastery.com/bluevillaimages/villa4.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageURL",
                value: "https://dotnetmastery.com/bluevillaimages/villa5.jpg");

            migrationBuilder.UpdateData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageURL",
                value: "https://dotnetmastery.com/bluevillaimages/villa2.jpg");
        }
    }
}
