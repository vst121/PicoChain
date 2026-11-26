using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PicoChain.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluationTelemetry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentVersion",
                table: "EvaluationRecords",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "EstimatedCost",
                table: "EvaluationRecords",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "InputTokens",
                table: "EvaluationRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ModelIdentifier",
                table: "EvaluationRecords",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "OutputTokens",
                table: "EvaluationRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PromptVersion",
                table: "EvaluationRecords",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentVersion",
                table: "EvaluationRecords");

            migrationBuilder.DropColumn(
                name: "EstimatedCost",
                table: "EvaluationRecords");

            migrationBuilder.DropColumn(
                name: "InputTokens",
                table: "EvaluationRecords");

            migrationBuilder.DropColumn(
                name: "ModelIdentifier",
                table: "EvaluationRecords");

            migrationBuilder.DropColumn(
                name: "OutputTokens",
                table: "EvaluationRecords");

            migrationBuilder.DropColumn(
                name: "PromptVersion",
                table: "EvaluationRecords");
        }
    }
}
