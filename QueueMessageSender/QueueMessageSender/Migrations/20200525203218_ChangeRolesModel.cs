using Microsoft.EntityFrameworkCore.Migrations;

namespace QueueMessageSender.Migrations
{
    public partial class ChangeRolesModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_RoleModel_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleModel",
                table: "RoleModel");

            migrationBuilder.RenameTable(
                name: "RoleModel",
                newName: "Roles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_RoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "RoleModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleModel",
                table: "RoleModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_RoleModel_RoleId",
                table: "Users",
                column: "RoleId",
                principalTable: "RoleModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
