using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MarbleHouse.Data.Migrations
{
    public partial class AddAppointmentandProdForAppToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentDate = table.Column<DateTime>(nullable: false),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerPhoneNumber = table.Column<string>(nullable: true),
                    CustomerEMail = table.Column<string>(nullable: true),
                    isConfirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductsSelectedForAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(nullable: false),
                    AppointmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsSelectedForAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsSelectedForAppointments_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsSelectedForAppointments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsSelectedForAppointments_AppointmentId",
                table: "ProductsSelectedForAppointments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsSelectedForAppointments_ProductId",
                table: "ProductsSelectedForAppointments",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsSelectedForAppointments");

            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
