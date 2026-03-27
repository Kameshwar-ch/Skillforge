using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skillforge.Migrations
{
    /// <inheritdoc />
    public partial class FirstJashwanth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Competency",
                columns: table => new
                {
                    CompetencyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Level = table.Column<string>(type: "VARCHAR(15)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competency", x => x.CompetencyID);
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Scope = table.Column<int>(type: "int", nullable: false),
                    Metrics = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.ReportID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Phone = table.Column<string>(type: "VARCHAR(10)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Audit",
                columns: table => new
                {
                    AuditID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HRID = table.Column<int>(type: "INT", nullable: false),
                    Scope = table.Column<int>(type: "int", maxLength: 50, nullable: false),
                    Findings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Date = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audit", x => x.AuditID);
                    table.ForeignKey(
                        name: "FK_Audit_User_HRID",
                        column: x => x.HRID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                columns: table => new
                {
                    AuditID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "INT", nullable: false),
                    Action = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Resource = table.Column<string>(type: "VARCHAR(255)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.AuditID);
                    table.ForeignKey(
                        name: "FK_AuditLog_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                columns: table => new
                {
                    CourseID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    Description = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    TrainerID = table.Column<int>(type: "INT", nullable: false),
                    Duration = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.CourseID);
                    table.ForeignKey(
                        name: "FK_Course_User_TrainerID",
                        column: x => x.TrainerID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillGap",
                columns: table => new
                {
                    SkillGapID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "INT", nullable: false),
                    CompetencyID = table.Column<int>(type: "int", nullable: false),
                    GapLevel = table.Column<int>(type: "int", nullable: false),
                    DateIdentified = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillGap", x => x.SkillGapID);
                    table.ForeignKey(
                        name: "FK_SkillGap_Competency_CompetencyID",
                        column: x => x.CompetencyID,
                        principalTable: "Competency",
                        principalColumn: "CompetencyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SkillGap_User_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    AssessmentID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseID = table.Column<int>(type: "INT", nullable: false),
                    Type = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    MaxScore = table.Column<decimal>(type: "DECIMAL(4,1)", precision: 4, scale: 1, nullable: false),
                    Date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.AssessmentID);
                    table.CheckConstraint("CK_Assessment_MaxScore", "[MaxScore] >= 0 AND [MaxScore] <= 100");
                    table.CheckConstraint("CK_Assessment_Type", "[Type] IN ('Quiz', 'Exam', 'Practical')");
                    table.ForeignKey(
                        name: "FK_Assessments_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certifications",
                columns: table => new
                {
                    CertificationID = table.Column<int>(type: "INT", nullable: false),
                    EmployeeID = table.Column<int>(type: "INT", nullable: false),
                    CourseID = table.Column<int>(type: "INT", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Status = table.Column<string>(type: "VARCHAR(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certifications", x => x.CertificationID);
                    table.CheckConstraint("CK_Certificate_Status", "[Status] IN ('Active', 'Revoked', 'Expired')");
                    table.CheckConstraint("CK_Certification_Dates", "[ExpiryDate] IS NULL OR [ExpiryDate] > [IssueDate]");
                    table.ForeignKey(
                        name: "FK_Certifications_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certifications_User_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Enrollment",
                columns: table => new
                {
                    EnrollmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseID = table.Column<int>(type: "INT", nullable: false),
                    EmployeeID = table.Column<int>(type: "INT", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollment", x => x.EnrollmentID);
                    table.ForeignKey(
                        name: "FK_Enrollment_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Enrollment_User_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Module",
                columns: table => new
                {
                    ModuleID = table.Column<int>(type: "INT", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseID = table.Column<int>(type: "INT", nullable: false),
                    Title = table.Column<string>(type: "VARCHAR(20)", nullable: false),
                    ContentURI = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    Duration = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Module", x => x.ModuleID);
                    table.ForeignKey(
                        name: "FK_Module_Course_CourseID",
                        column: x => x.CourseID,
                        principalTable: "Course",
                        principalColumn: "CourseID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    AssessmentID = table.Column<int>(type: "INT", nullable: false),
                    EmployeeID = table.Column<int>(type: "INT", nullable: false),
                    ResultID = table.Column<int>(type: "INT", nullable: false),
                    Score = table.Column<decimal>(type: "DECIMAL(4,1)", precision: 4, scale: 1, nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => new { x.AssessmentID, x.EmployeeID });
                    table.CheckConstraint("CK_Result_Score", "[Score] >= 0 AND [Score] <= 100");
                    table.CheckConstraint("CK_Result_Status", "[Status] IN ('Pass', 'Fail')");
                    table.ForeignKey(
                        name: "FK_Results_Assessments_AssessmentID",
                        column: x => x.AssessmentID,
                        principalTable: "Assessments",
                        principalColumn: "AssessmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Results_User_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ComplianceRecord",
                columns: table => new
                {
                    ComplianceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "INT", nullable: false),
                    CertificationID = table.Column<int>(type: "INT", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "DATETIME", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComplianceRecord", x => x.ComplianceID);
                    table.ForeignKey(
                        name: "FK_ComplianceRecord_Certifications_CertificationID",
                        column: x => x.CertificationID,
                        principalTable: "Certifications",
                        principalColumn: "CertificationID");
                    table.ForeignKey(
                        name: "FK_ComplianceRecord_User_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "User",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Attendance",
                columns: table => new
                {
                    AttendanceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnrollmentID = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendance", x => x.AttendanceID);
                    table.ForeignKey(
                        name: "FK_Attendance_Enrollment_EnrollmentID",
                        column: x => x.EnrollmentID,
                        principalTable: "Enrollment",
                        principalColumn: "EnrollmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_CourseID",
                table: "Assessments",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Attendance_EnrollmentID",
                table: "Attendance",
                column: "EnrollmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Audit_HRID",
                table: "Audit",
                column: "HRID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_UserID",
                table: "AuditLog",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_CourseID",
                table: "Certifications",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Certifications_EmployeeID",
                table: "Certifications",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecord_CertificationID",
                table: "ComplianceRecord",
                column: "CertificationID");

            migrationBuilder.CreateIndex(
                name: "IX_ComplianceRecord_EmployeeID",
                table: "ComplianceRecord",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Course_TrainerID",
                table: "Course",
                column: "TrainerID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_CourseID",
                table: "Enrollment",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_EmployeeID",
                table: "Enrollment",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Module_CourseID",
                table: "Module",
                column: "CourseID");

            migrationBuilder.CreateIndex(
                name: "IX_Results_EmployeeID",
                table: "Results",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillGap_CompetencyID",
                table: "SkillGap",
                column: "CompetencyID");

            migrationBuilder.CreateIndex(
                name: "IX_SkillGap_EmployeeID",
                table: "SkillGap",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendance");

            migrationBuilder.DropTable(
                name: "Audit");

            migrationBuilder.DropTable(
                name: "AuditLog");

            migrationBuilder.DropTable(
                name: "ComplianceRecord");

            migrationBuilder.DropTable(
                name: "Module");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "SkillGap");

            migrationBuilder.DropTable(
                name: "Enrollment");

            migrationBuilder.DropTable(
                name: "Certifications");

            migrationBuilder.DropTable(
                name: "Assessments");

            migrationBuilder.DropTable(
                name: "Competency");

            migrationBuilder.DropTable(
                name: "Course");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
