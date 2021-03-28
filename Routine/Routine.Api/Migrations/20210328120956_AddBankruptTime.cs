using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Routine.Api.Migrations
{
    public partial class AddBankruptTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees");

            migrationBuilder.AddColumn<DateTime>(
                name: "BankruptTime",
                table: "Companies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("1861341e-b42b-410c-ae21-cf11f36fc574"),
                column: "DateOfBirth",
                value: new DateTime(1957, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("4b501cb3-d168-4cc0-b375-48fb33f318a4"),
                column: "DateOfBirth",
                value: new DateTime(1976, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("679dfd33-32e4-4393-b061-f7abb8956f53"),
                column: "DateOfBirth",
                value: new DateTime(1967, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("72457e73-ea34-4e02-b575-8d384e82a481"),
                column: "DateOfBirth",
                value: new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("7644b71d-d74e-43e2-ac32-8cbadd7b1c3a"),
                column: "DateOfBirth",
                value: new DateTime(1977, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("7eaa532c-1be5-472c-a738-94fd26e5fad6"),
                column: "DateOfBirth",
                value: new DateTime(1981, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "BankruptTime",
                table: "Companies");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("1861341e-b42b-410c-ae21-cf11f36fc574"),
                column: "DateOfBirth",
                value: new DateTimeOffset(new DateTime(1957, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("4b501cb3-d168-4cc0-b375-48fb33f318a4"),
                column: "DateOfBirth",
                value: new DateTimeOffset(new DateTime(1976, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("679dfd33-32e4-4393-b061-f7abb8956f53"),
                column: "DateOfBirth",
                value: new DateTimeOffset(new DateTime(1967, 1, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("72457e73-ea34-4e02-b575-8d384e82a481"),
                column: "DateOfBirth",
                value: new DateTimeOffset(new DateTime(1986, 11, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("7644b71d-d74e-43e2-ac32-8cbadd7b1c3a"),
                column: "DateOfBirth",
                value: new DateTimeOffset(new DateTime(1977, 4, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: new Guid("7eaa532c-1be5-472c-a738-94fd26e5fad6"),
                column: "DateOfBirth",
                value: new DateTimeOffset(new DateTime(1981, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 8, 0, 0, 0)));

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
