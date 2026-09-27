using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApexBank.Migrations
{
    /// <inheritdoc />
    public partial class AuditLoanCustomerAlter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditScore",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "DocumentPath",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "RiskRating",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "CreditScore",
                table: "CustomerProfiles");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "AuditLogs");

            migrationBuilder.AddColumn<byte[]>(
                name: "FileData",
                table: "Loans",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileData",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "CreditScore",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DocumentPath",
                table: "Loans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskRating",
                table: "Loans",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CreditScore",
                table: "CustomerProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "AuditLogs",
                type: "nvarchar(45)",
                maxLength: 45,
                nullable: true);
        }
    }
}
