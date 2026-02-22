using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxTableConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Outbox",
                table: "Outbox");

            migrationBuilder.DropIndex(
                name: "IX_Outbox_CreatedAt",
                table: "Outbox");

            migrationBuilder.DropIndex(
                name: "IX_Outbox_ProcessedAt",
                table: "Outbox");

            migrationBuilder.RenameTable(
                name: "Outbox",
                newName: "OutboxMessage");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "OutboxMessage",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ProcessedAt",
                table: "OutboxMessage",
                column: "ProcessedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OutboxMessage",
                table: "OutboxMessage");

            migrationBuilder.DropIndex(
                name: "IX_OutboxMessage_ProcessedAt",
                table: "OutboxMessage");

            migrationBuilder.RenameTable(
                name: "OutboxMessage",
                newName: "Outbox");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Outbox",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Outbox",
                table: "Outbox",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_CreatedAt",
                table: "Outbox",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Outbox_ProcessedAt",
                table: "Outbox",
                column: "ProcessedAt",
                filter: "ProcessedAt IS NULL");
        }
    }
}
