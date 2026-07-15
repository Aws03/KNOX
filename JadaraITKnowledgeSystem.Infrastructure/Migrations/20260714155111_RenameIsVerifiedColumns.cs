using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JadaraITKnowledgeSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameIsVerifiedColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerficationDate",
                table: "Users",
                newName: "VerificationDate");

            migrationBuilder.RenameColumn(
                name: "IsVerfied",
                table: "Users",
                newName: "IsVerified");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VerificationDate",
                table: "Users",
                newName: "VerficationDate");

            migrationBuilder.RenameColumn(
                name: "IsVerified",
                table: "Users",
                newName: "IsVerfied");
        }
    }
}
