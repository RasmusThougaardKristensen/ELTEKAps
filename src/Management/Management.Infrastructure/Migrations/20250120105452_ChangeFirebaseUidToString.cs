using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELTEKAps.Management.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFirebaseUidToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserEntity_UserId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserEntity",
                table: "UserEntity");

            migrationBuilder.RenameTable(
                name: "UserEntity",
                newName: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "FirebaseId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "ClusterId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ClusterId",
                table: "Users",
                column: "ClusterId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UserId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ClusterId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ClusterId",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "UserEntity");

            migrationBuilder.AlterColumn<Guid>(
                name: "FirebaseId",
                table: "UserEntity",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserEntity",
                table: "UserEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserEntity_UserId",
                table: "Tasks",
                column: "UserId",
                principalTable: "UserEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
