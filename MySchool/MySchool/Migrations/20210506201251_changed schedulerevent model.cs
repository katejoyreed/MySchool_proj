using Microsoft.EntityFrameworkCore.Migrations;

namespace MySchool.Migrations
{
    public partial class changedschedulereventmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "SchedulerEvents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SchedulerEvents_StudentId",
                table: "SchedulerEvents",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchedulerEvents_Students_StudentId",
                table: "SchedulerEvents",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchedulerEvents_Students_StudentId",
                table: "SchedulerEvents");

            migrationBuilder.DropIndex(
                name: "IX_SchedulerEvents_StudentId",
                table: "SchedulerEvents");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "SchedulerEvents");
        }
    }
}
