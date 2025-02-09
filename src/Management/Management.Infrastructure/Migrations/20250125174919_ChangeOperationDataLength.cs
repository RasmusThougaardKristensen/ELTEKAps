using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELTEKAps.Management.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOperationDataLength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "Operations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true)
                .Annotation("SqlServer:Sparse", true)
                .OldAnnotation("SqlServer:Sparse", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "Operations",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true)
                .Annotation("SqlServer:Sparse", true)
                .OldAnnotation("SqlServer:Sparse", true);
        }
    }
}
