using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestRating.Dal.Migrations
{
    /// <inheritdoc />
    public partial class FixFeedbackreportrelationsversion2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
