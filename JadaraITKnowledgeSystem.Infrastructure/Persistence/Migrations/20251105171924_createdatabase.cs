using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class createdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserReactionId",
                table: "UserReactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserReactionId",
                table: "UserReactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
