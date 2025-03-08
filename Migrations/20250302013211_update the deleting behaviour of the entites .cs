using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiscordCloneBackend.Migrations
{
    /// <inheritdoc />
    public partial class updatethedeletingbehaviouroftheentites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Members_MemberOneId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Members_MemberTwoId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Members_MemberId",
                table: "Messages");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Members_MemberOneId",
                table: "Conversations",
                column: "MemberOneId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Members_MemberTwoId",
                table: "Conversations",
                column: "MemberTwoId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Members_MemberId",
                table: "Messages",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Members_MemberOneId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_Members_MemberTwoId",
                table: "Conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Members_MemberId",
                table: "Messages");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Members_MemberOneId",
                table: "Conversations",
                column: "MemberOneId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_Members_MemberTwoId",
                table: "Conversations",
                column: "MemberTwoId",
                principalTable: "Members",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Members_MemberId",
                table: "Messages",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }
    }
}
