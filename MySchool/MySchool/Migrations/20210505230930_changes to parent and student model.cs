using Microsoft.EntityFrameworkCore.Migrations;

namespace MySchool.Migrations
{
    public partial class changestoparentandstudentmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StudentName",
                table: "Students",
                newName: "StudentLastName");

            migrationBuilder.AddColumn<string>(
                name: "DOB",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentFirstName",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DOB",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StudentFirstName",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Parents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StudentLastName",
                table: "Parents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Parents_StudentId",
                table: "Parents",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Parents_Students_StudentId",
                table: "Parents",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Parents_Students_StudentId",
                table: "Parents");

            migrationBuilder.DropIndex(
                name: "IX_Parents_StudentId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "DOB",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentFirstName",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DOB",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "StudentFirstName",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Parents");

            migrationBuilder.DropColumn(
                name: "StudentLastName",
                table: "Parents");

            migrationBuilder.RenameColumn(
                name: "StudentLastName",
                table: "Students",
                newName: "StudentName");
        }
    }
}
