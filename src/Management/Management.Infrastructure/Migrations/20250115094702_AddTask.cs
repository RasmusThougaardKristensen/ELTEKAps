using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELTEKAps.Management.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClusterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    OperationName = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                        .Annotation("SqlServer:Sparse", true),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("SqlServer:Sparse", true),
                    ClusterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhotoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClusterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_Tasks_Comments_CommentEntityId",
                        column: x => x.CommentEntityId,
                        principalTable: "Comments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ClusterId",
                table: "Comments",
                column: "ClusterId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_ClusterId",
                table: "Operations",
                column: "ClusterId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_RequestId",
                table: "Operations",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_TaskId",
                table: "Operations",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ClusterId",
                table: "Tasks",
                column: "ClusterId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CommentEntityId",
                table: "Tasks",
                column: "CommentEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
