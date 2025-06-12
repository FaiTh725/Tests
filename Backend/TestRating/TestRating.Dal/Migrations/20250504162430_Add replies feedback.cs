using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestRating.Dal.Migrations
{
    /// <inheritdoc />
    public partial class Addrepliesfeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentFeedbackId",
                table: "Feedbacks",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_ParentFeedbackId",
                table: "Feedbacks",
                column: "ParentFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Feedbacks_ParentFeedbackId",
                table: "Feedbacks",
                column: "ParentFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Feedbacks_ParentFeedbackId",
                table: "Feedbacks");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_ParentFeedbackId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "ParentFeedbackId",
                table: "Feedbacks");
        }
    }
}
