using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refund_Orders_OrderId",
                table: "Refund");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refund",
                table: "Refund");

            migrationBuilder.RenameTable(
                name: "Refund",
                newName: "Refunds");

            migrationBuilder.RenameIndex(
                name: "IX_Refund_OrderId",
                table: "Refunds",
                newName: "IX_Refunds_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Refunds",
                table: "Refunds");

            migrationBuilder.RenameTable(
                name: "Refunds",
                newName: "Refund");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_OrderId",
                table: "Refund",
                newName: "IX_Refund_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Refund",
                table: "Refund",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Refund_Orders_OrderId",
                table: "Refund",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
