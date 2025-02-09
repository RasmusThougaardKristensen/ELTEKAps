using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELTEKAps.Management.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerToContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_CustomerEntity_CustomerId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CustomerEntity",
                table: "CustomerEntity");

            migrationBuilder.RenameTable(
                name: "CustomerEntity",
                newName: "Customers");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Customers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Customers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "ClusterId",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ClusterId",
                table: "Customers",
                column: "ClusterId",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Customers_CustomerId",
                table: "Tasks",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Customers_CustomerId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_ClusterId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ClusterId",
                table: "Customers");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "CustomerEntity");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "CustomerEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "CustomerEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "CustomerEntity",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CustomerEntity",
                table: "CustomerEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_CustomerEntity_CustomerId",
                table: "Tasks",
                column: "CustomerId",
                principalTable: "CustomerEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
