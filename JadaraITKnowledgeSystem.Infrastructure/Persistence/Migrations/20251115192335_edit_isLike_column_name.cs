using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class edit_isLike_column_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLike",
                table: "UserReactions");

            migrationBuilder.AddColumn<int>(
                name: "ReactionType",
                table: "UserReactions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReactionType",
                table: "UserReactions");

            migrationBuilder.AddColumn<bool>(
                name: "IsLike",
                table: "UserReactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
