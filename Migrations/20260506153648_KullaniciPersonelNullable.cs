using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBS_Projesi.Migrations
{
    /// <inheritdoc />
    public partial class KullaniciPersonelNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanici_Personel_PersonelID",
                table: "Kullanici");

            migrationBuilder.AlterColumn<int>(
                name: "PersonelID",
                table: "Kullanici",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanici_Personel_PersonelID",
                table: "Kullanici",
                column: "PersonelID",
                principalTable: "Personel",
                principalColumn: "PersonelID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kullanici_Personel_PersonelID",
                table: "Kullanici");

            migrationBuilder.AlterColumn<int>(
                name: "PersonelID",
                table: "Kullanici",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Kullanici_Personel_PersonelID",
                table: "Kullanici",
                column: "PersonelID",
                principalTable: "Personel",
                principalColumn: "PersonelID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
