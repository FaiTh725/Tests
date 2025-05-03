using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestRating.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Fixrelationsbetweenfeedbackandfeedbackreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ReviewedFeedbackId",
                table: "Reviews",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ReviewedFeedbackId",
                table: "Reviews",
                column: "ReviewedFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Feedbacks_ReviewedFeedbackId",
                table: "Reviews",
                column: "ReviewedFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Feedbacks_ReviewedFeedbackId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_ReviewedFeedbackId",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ReviewedFeedbackId",
                table: "Reviews");
        }
    }
}
