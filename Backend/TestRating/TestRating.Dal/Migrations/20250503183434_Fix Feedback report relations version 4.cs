using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestRating.Dal.Migrations
{
    /// <inheritdoc />
    public partial class FixFeedbackreportrelationsversion4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.AlterColumn<long>(
                name: "ReportedFeedbackId",
                table: "Reports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports");

            migrationBuilder.AlterColumn<long>(
                name: "ReportedFeedbackId",
                table: "Reports",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Feedbacks_ReportedFeedbackId",
                table: "Reports",
                column: "ReportedFeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
