using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELTEKAps.Management.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedTaskEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Comments_Id",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Photos_Id",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Photos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Photos_TaskId",
                table: "Photos",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TaskId",
                table: "Comments",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Tasks_TaskId",
                table: "Photos",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Tasks_TaskId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Tasks_TaskId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Photos_TaskId",
                table: "Photos");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TaskId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Comments_Id",
                table: "Tasks",
                column: "Id",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Photos_Id",
                table: "Tasks",
                column: "Id",
                principalTable: "Photos",
                principalColumn: "Id");
        }
    }
}
