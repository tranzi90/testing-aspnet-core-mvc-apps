using System;
using AtmSimulator.Web.Database;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AtmSimulator.Web.Migrations
{
    public partial class InitialMigration : Migration
    {
        private readonly IDbContextSchema _schema;

        public InitialMigration()
        {
        }

        public InitialMigration(IDbContextSchema schema)
        {
            _schema = schema;
        }

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Atms",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Atms", x => x.Id);
                },
                schema: _schema?.Schema);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Cash = table.Column<decimal>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Name);
                },
                schema: _schema?.Schema);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Atms");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
