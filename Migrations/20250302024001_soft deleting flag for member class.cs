using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class softdeletingflagformemberclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Members");
        }
    }
}
