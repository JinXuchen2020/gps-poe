using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POEMgr.Repository.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>("IsDisabled", table: "Sys_User", "nvarchar(100)", nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Poe_CheckPoint");

            migrationBuilder.DropTable(
                name: "Poe_CheckPointStatus");

            migrationBuilder.DropTable(
                name: "Poe_CurrentNumber");

            migrationBuilder.DropTable(
                name: "Poe_Customer");

            migrationBuilder.DropTable(
                name: "Poe_DbLog");

            migrationBuilder.DropTable(
                name: "Poe_Incentive");

            migrationBuilder.DropTable(
                name: "Poe_MailSendRecord");

            migrationBuilder.DropTable(
                name: "Poe_MailTemplate");

            migrationBuilder.DropTable(
                name: "Poe_OperationLog");

            migrationBuilder.DropTable(
                name: "Poe_Partner");

            migrationBuilder.DropTable(
                name: "Poe_PartnerIncentive");

            migrationBuilder.DropTable(
                name: "Poe_POEFile");

            migrationBuilder.DropTable(
                name: "Poe_POERequest");

            migrationBuilder.DropTable(
                name: "Poe_RequestLog");

            migrationBuilder.DropTable(
                name: "Poe_RequestPhase");

            migrationBuilder.DropTable(
                name: "Poe_SchedulerLog");

            migrationBuilder.DropTable(
                name: "Poe_Subscription");

            migrationBuilder.DropTable(
                name: "Poe_SubscriptionStatus");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "Sys_Role");

            migrationBuilder.DropTable(
                name: "Sys_User");

            migrationBuilder.DropTable(
                name: "Sys_UserRole");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
