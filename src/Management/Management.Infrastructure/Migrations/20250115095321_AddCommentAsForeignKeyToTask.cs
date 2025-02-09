using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELTEKAps.Management.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentAsForeignKeyToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Comments_CommentEntityId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CommentEntityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CommentEntityId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Comments_Id",
                table: "Tasks",
                column: "Id",
                principalTable: "Comments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Comments_Id",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "CommentEntityId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CommentEntityId",
                table: "Tasks",
                column: "CommentEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Comments_CommentEntityId",
                table: "Tasks",
                column: "CommentEntityId",
                principalTable: "Comments",
                principalColumn: "Id");
        }
    }
}
