using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClientMSystem.Migrations
{
    public partial class Admin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "adminModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_adminModel", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "clientDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IssuedDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DomainName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Technology = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Assigned = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clientDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "signUps",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConformPassword = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_signUps", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "timeSheets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ExpectedTaskToCompleted = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExpectedHours = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CompletedTasks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    UnPlannedTask = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ActualHours = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CommentsForAnyDealy = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    QuestionsActionsToBeAsked = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeSheets", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "adminModel");

            migrationBuilder.DropTable(
                name: "clientDetails");

            migrationBuilder.DropTable(
                name: "signUps");

            migrationBuilder.DropTable(
                name: "timeSheets");
        }
    }
}
