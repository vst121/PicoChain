using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicoChain.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHumanReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HumanReviewRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EvaluationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reviewer = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Decision = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: false),
                    ReviewedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HumanReviewRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HumanReviewRecords_EvaluationId",
                table: "HumanReviewRecords",
                column: "EvaluationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HumanReviewRecords");
        }
    }
}
