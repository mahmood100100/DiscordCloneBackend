using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class updatemessagesanddirectmessagesandconversationsentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessages_Members_MemberId",
                table: "DirectMessages");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "DirectMessages",
                newName: "SenderMemberId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectMessages_MemberId",
                table: "DirectMessages",
                newName: "IX_DirectMessages_SenderMemberId");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverMemberId",
                table: "DirectMessages",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessages_ReceiverMemberId",
                table: "DirectMessages",
                column: "ReceiverMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessages_Members_ReceiverMemberId",
                table: "DirectMessages",
                column: "ReceiverMemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessages_Members_SenderMemberId",
                table: "DirectMessages",
                column: "SenderMemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessages_Members_ReceiverMemberId",
                table: "DirectMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessages_Members_SenderMemberId",
                table: "DirectMessages");

            migrationBuilder.DropIndex(
                name: "IX_DirectMessages_ReceiverMemberId",
                table: "DirectMessages");

            migrationBuilder.DropColumn(
                name: "ReceiverMemberId",
                table: "DirectMessages");

            migrationBuilder.RenameColumn(
                name: "SenderMemberId",
                table: "DirectMessages",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_DirectMessages_SenderMemberId",
                table: "DirectMessages",
                newName: "IX_DirectMessages_MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessages_Members_MemberId",
                table: "DirectMessages",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
