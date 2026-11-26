using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicoChain.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EvaluationRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AgentTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScenarioId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EvaluatorType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ScoresJson = table.Column<string>(type: "text", nullable: false),
                    OverallScore = table.Column<double>(type: "double precision", nullable: false),
                    Passed = table.Column<bool>(type: "boolean", nullable: false),
                    FailureReasonsJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EvaluationRecords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EvaluationRecords_AgentTaskId_CreatedAt",
                table: "EvaluationRecords",
                columns: new[] { "AgentTaskId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EvaluationRecords");
        }
    }
}
