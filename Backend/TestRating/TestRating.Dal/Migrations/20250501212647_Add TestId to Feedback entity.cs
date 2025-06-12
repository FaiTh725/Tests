using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestRating.Dal.Migrations
{
    /// <inheritdoc />
    public partial class AddTestIdtoFeedbackentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TestId",
                table: "Feedbacks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_TestId",
                table: "Feedbacks",
                column: "TestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_TestId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "TestId",
                table: "Feedbacks");
        }
    }
}
