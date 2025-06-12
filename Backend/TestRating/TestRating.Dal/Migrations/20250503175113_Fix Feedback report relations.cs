using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestRating.Dal.Migrations
{
    /// <inheritdoc />
    public partial class FixFeedbackreportrelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
