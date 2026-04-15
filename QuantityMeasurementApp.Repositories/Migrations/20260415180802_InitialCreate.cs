using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace QuantityMeasurementApp.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    GoogleId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Operation = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operand1Value = table.Column<double>(type: "double precision", nullable: false),
                    Operand1Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operand1MeasurementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Operand2Value = table.Column<double>(type: "double precision", nullable: true),
                    Operand2Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Operand2MeasurementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ResultQuantityValue = table.Column<double>(type: "double precision", nullable: true),
                    ResultQuantityUnit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultQuantityMeasurementType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ResultScalar = table.Column<double>(type: "double precision", nullable: true),
                    ResultComparison = table.Column<bool>(type: "boolean", nullable: true),
                    IsError = table.Column<bool>(type: "boolean", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementHistory_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementHistory_UserId",
                table: "MeasurementHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementHistory");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
