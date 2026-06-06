using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MediTrack.MedicalAnalysisService.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "adherence_alert",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    patient_id = table.Column<int>(type: "int", nullable: false),
                    severity = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    reason = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    triggered_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    acknowledged_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adherence_alert", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "adherence_metric",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    patient_id = table.Column<int>(type: "int", nullable: false),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    total_scheduled = table.Column<int>(type: "int", nullable: false),
                    total_compliant = table.Column<int>(type: "int", nullable: false),
                    total_missed = table.Column<int>(type: "int", nullable: false),
                    rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    last_updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adherence_metric", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "clinical_record",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    patient_id = table.Column<int>(type: "int", nullable: false),
                    record_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    diagnosis = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    source = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    import_batch_id = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinical_record", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "compliance_statistic",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    window_start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    window_end = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    total_scheduled = table.Column<int>(type: "int", nullable: false),
                    total_compliant = table.Column<int>(type: "int", nullable: false),
                    total_missed = table.Column<int>(type: "int", nullable: false),
                    compliance_rate = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    calculated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compliance_statistic", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_adherence_metric_patient_id_category",
                table: "adherence_metric",
                columns: new[] { "patient_id", "category" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adherence_alert");

            migrationBuilder.DropTable(
                name: "adherence_metric");

            migrationBuilder.DropTable(
                name: "clinical_record");

            migrationBuilder.DropTable(
                name: "compliance_statistic");
        }
    }
}
