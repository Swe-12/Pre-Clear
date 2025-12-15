using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_audit_entity",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "details",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "entity",
                table: "audit_logs");

            migrationBuilder.RenameColumn(
                name: "performed_at",
                table: "audit_logs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "entity_id",
                table: "audit_logs",
                newName: "shipment_id");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs",
                newName: "idx_audit_user");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "audit_logs",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "audit_logs",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_audit_created_at",
                table: "audit_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "idx_audit_shipment",
                table: "audit_logs",
                column: "shipment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_audit_created_at",
                table: "audit_logs");

            migrationBuilder.DropIndex(
                name: "idx_audit_shipment",
                table: "audit_logs");

            migrationBuilder.DropColumn(
                name: "description",
                table: "audit_logs");

            migrationBuilder.RenameColumn(
                name: "shipment_id",
                table: "audit_logs",
                newName: "entity_id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "audit_logs",
                newName: "performed_at");

            migrationBuilder.RenameIndex(
                name: "idx_audit_user",
                table: "audit_logs",
                newName: "IX_audit_logs_user_id");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "audit_logs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "details",
                table: "audit_logs",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "entity",
                table: "audit_logs",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_audit_entity",
                table: "audit_logs",
                columns: new[] { "entity", "entity_id" });
        }
    }
}
