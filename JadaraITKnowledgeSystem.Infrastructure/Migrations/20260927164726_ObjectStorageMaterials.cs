using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JadaraITKnowledgeSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ObjectStorageMaterials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContentUrl",
                table: "CourseMaterials",
                newName: "StorageKey");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "CourseMaterials",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SizeBytes",
                table: "CourseMaterials",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            // Existing rows hold absolute URLs of the old local /uploads storage (or the older Bunny storage
            // zone). Turn them into keys under "legacy/" in the private bucket, where
            // deploy/scripts/migrate-local-uploads.sh copies the old files.
            migrationBuilder.Sql("""
                UPDATE [CourseMaterials]
                SET [StorageKey] = LEFT(N'legacy/' +
                    CASE
                        WHEN CHARINDEX(N'/uploads/', [StorageKey]) > 0
                            THEN SUBSTRING([StorageKey], CHARINDEX(N'/uploads/', [StorageKey]) + 9, 500)
                        ELSE SUBSTRING([StorageKey], CHARINDEX(N'/', [StorageKey], CHARINDEX(N'://', [StorageKey]) + 3) + 1, 500)
                    END, 500)
                WHERE CHARINDEX(N'://', [StorageKey]) > 0;

                UPDATE [CourseMaterials]
                SET [ContentType] = CASE LOWER(RIGHT([StorageKey], CHARINDEX(N'.', REVERSE([StorageKey]))))
                        WHEN N'.pdf' THEN N'application/pdf'
                        WHEN N'.docx' THEN N'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
                        WHEN N'.pptx' THEN N'application/vnd.openxmlformats-officedocument.presentationml.presentation'
                        WHEN N'.png' THEN N'image/png'
                        WHEN N'.jpg' THEN N'image/jpeg'
                        WHEN N'.jpeg' THEN N'image/jpeg'
                        WHEN N'.mp4' THEN N'video/mp4'
                        WHEN N'.webm' THEN N'video/webm'
                        ELSE N'application/octet-stream'
                    END
                WHERE [ContentType] = N'';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "CourseMaterials");

            migrationBuilder.DropColumn(
                name: "SizeBytes",
                table: "CourseMaterials");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "CourseMaterials",
                newName: "ContentUrl");
        }
    }
}
