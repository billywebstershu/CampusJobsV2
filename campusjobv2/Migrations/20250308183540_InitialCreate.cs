using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace campusjobv2.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    User_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    First_Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Last_Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.User_ID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Admin_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Admin_ID);
                    table.ForeignKey(
                        name: "FK_Admins_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Notification_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User_ID = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Time = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Read = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Notification_ID);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Recruiters",
                columns: table => new
                {
                    Recruitment_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    User_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recruiters", x => x.Recruitment_ID);
                    table.ForeignKey(
                        name: "FK_Recruiters_Users_User_ID",
                        column: x => x.User_ID,
                        principalTable: "Users",
                        principalColumn: "User_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Student_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Recruitment_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Student_ID);
                    table.ForeignKey(
                        name: "FK_Employees_Recruiters_Recruitment_ID",
                        column: x => x.Recruitment_ID,
                        principalTable: "Recruiters",
                        principalColumn: "Recruitment_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudentWorkers",
                columns: table => new
                {
                    VisaStatus_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Recruitment_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentWorkers", x => x.VisaStatus_ID);
                    table.ForeignKey(
                        name: "FK_StudentWorkers_Recruiters_Recruitment_ID",
                        column: x => x.Recruitment_ID,
                        principalTable: "Recruiters",
                        principalColumn: "Recruitment_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "OfferedShifts",
                columns: table => new
                {
                    Offer_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Student_ID = table.Column<int>(type: "int", nullable: false),
                    Recruitment_ID = table.Column<int>(type: "int", nullable: false),
                    Date_Offered = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Start_Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    End_Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Total_Hours = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfferedShifts", x => x.Offer_ID);
                    table.ForeignKey(
                        name: "FK_OfferedShifts_Employees_Student_ID",
                        column: x => x.Student_ID,
                        principalTable: "Employees",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OfferedShifts_Recruiters_Recruitment_ID",
                        column: x => x.Recruitment_ID,
                        principalTable: "Recruiters",
                        principalColumn: "Recruitment_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RightToWorkDocuments",
                columns: table => new
                {
                    RTW_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Student_ID = table.Column<int>(type: "int", nullable: false),
                    Document_URL = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Upload_Date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RightToWorkDocuments", x => x.RTW_ID);
                    table.ForeignKey(
                        name: "FK_RightToWorkDocuments_Employees_Student_ID",
                        column: x => x.Student_ID,
                        principalTable: "Employees",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VisaStatuses",
                columns: table => new
                {
                    VisaStatusID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Student_ID = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaStatuses", x => x.VisaStatusID);
                    table.ForeignKey(
                        name: "FK_VisaStatuses_Employees_Student_ID",
                        column: x => x.Student_ID,
                        principalTable: "Employees",
                        principalColumn: "Student_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApprovedShifts",
                columns: table => new
                {
                    Timesheet_ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Offer_ID = table.Column<int>(type: "int", nullable: false),
                    Hours_Worked = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Date_Uploaded = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Date_Reviewed = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovedShifts", x => x.Timesheet_ID);
                    table.ForeignKey(
                        name: "FK_ApprovedShifts_OfferedShifts_Offer_ID",
                        column: x => x.Offer_ID,
                        principalTable: "OfferedShifts",
                        principalColumn: "Offer_ID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_User_ID",
                table: "Admins",
                column: "User_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovedShifts_Offer_ID",
                table: "ApprovedShifts",
                column: "Offer_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Recruitment_ID",
                table: "Employees",
                column: "Recruitment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_User_ID",
                table: "Notifications",
                column: "User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OfferedShifts_Recruitment_ID",
                table: "OfferedShifts",
                column: "Recruitment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_OfferedShifts_Student_ID",
                table: "OfferedShifts",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Recruiters_User_ID",
                table: "Recruiters",
                column: "User_ID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RightToWorkDocuments_Student_ID",
                table: "RightToWorkDocuments",
                column: "Student_ID");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWorkers_Recruitment_ID",
                table: "StudentWorkers",
                column: "Recruitment_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisaStatuses_Student_ID",
                table: "VisaStatuses",
                column: "Student_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "ApprovedShifts");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RightToWorkDocuments");

            migrationBuilder.DropTable(
                name: "StudentWorkers");

            migrationBuilder.DropTable(
                name: "VisaStatuses");

            migrationBuilder.DropTable(
                name: "OfferedShifts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Recruiters");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
