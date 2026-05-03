using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBS_Projesi.Migrations
{
    /// <inheritdoc />
    public partial class Baslangic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bolum",
                columns: table => new
                {
                    BolumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BolumAdi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bolum", x => x.BolumID);
                });

            migrationBuilder.CreateTable(
                name: "Derslik",
                columns: table => new
                {
                    DerslikID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Kapasite = table.Column<int>(type: "int", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    Kat = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Derslik", x => x.DerslikID);
                });

            migrationBuilder.CreateTable(
                name: "Oturum",
                columns: table => new
                {
                    OturumID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tanim = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaslangicSaat = table.Column<TimeSpan>(type: "time", nullable: false),
                    BitisSaat = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oturum", x => x.OturumID);
                });

            migrationBuilder.CreateTable(
                name: "Ders",
                columns: table => new
                {
                    DersID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DersKodu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DersTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DersAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OgrenciSayisi = table.Column<int>(type: "int", nullable: false),
                    Yariyil = table.Column<int>(type: "int", nullable: false),
                    BolumID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ders", x => x.DersID);
                    table.ForeignKey(
                        name: "FK_Ders_Bolum_BolumID",
                        column: x => x.BolumID,
                        principalTable: "Bolum",
                        principalColumn: "BolumID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Personel",
                columns: table => new
                {
                    PersonelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Unvan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BolumID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personel", x => x.PersonelID);
                    table.ForeignKey(
                        name: "FK_Personel_Bolum_BolumID",
                        column: x => x.BolumID,
                        principalTable: "Bolum",
                        principalColumn: "BolumID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sinav",
                columns: table => new
                {
                    SinavID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DersID = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OturumID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sinav", x => x.SinavID);
                    table.ForeignKey(
                        name: "FK_Sinav_Ders_DersID",
                        column: x => x.DersID,
                        principalTable: "Ders",
                        principalColumn: "DersID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sinav_Oturum_OturumID",
                        column: x => x.OturumID,
                        principalTable: "Oturum",
                        principalColumn: "OturumID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Kullanici",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sifre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PersonelID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanici", x => x.KullaniciID);
                    table.ForeignKey(
                        name: "FK_Kullanici_Personel_PersonelID",
                        column: x => x.PersonelID,
                        principalTable: "Personel",
                        principalColumn: "PersonelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonelMazeret",
                columns: table => new
                {
                    MazeretID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonelID = table.Column<int>(type: "int", nullable: false),
                    OturumID = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MazeretTuru = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UygunMu = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonelMazeret", x => x.MazeretID);
                    table.ForeignKey(
                        name: "FK_PersonelMazeret_Oturum_OturumID",
                        column: x => x.OturumID,
                        principalTable: "Oturum",
                        principalColumn: "OturumID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonelMazeret_Personel_PersonelID",
                        column: x => x.PersonelID,
                        principalTable: "Personel",
                        principalColumn: "PersonelID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SinavLog",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SinavID = table.Column<int>(type: "int", nullable: false),
                    PersonelID = table.Column<int>(type: "int", nullable: false),
                    IslemTuru = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EskiDeger = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    YeniDeger = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IslemTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavLog", x => x.LogID);
                    table.ForeignKey(
                        name: "FK_SinavLog_Personel_PersonelID",
                        column: x => x.PersonelID,
                        principalTable: "Personel",
                        principalColumn: "PersonelID");
                    table.ForeignKey(
                        name: "FK_SinavLog_Sinav_SinavID",
                        column: x => x.SinavID,
                        principalTable: "Sinav",
                        principalColumn: "SinavID");
                });

            migrationBuilder.CreateTable(
                name: "SinavSalonu",
                columns: table => new
                {
                    SinavSalonuID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SinavID = table.Column<int>(type: "int", nullable: false),
                    DerslikID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinavSalonu", x => x.SinavSalonuID);
                    table.ForeignKey(
                        name: "FK_SinavSalonu_Derslik_DerslikID",
                        column: x => x.DerslikID,
                        principalTable: "Derslik",
                        principalColumn: "DerslikID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SinavSalonu_Sinav_SinavID",
                        column: x => x.SinavID,
                        principalTable: "Sinav",
                        principalColumn: "SinavID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GozetmenAtama",
                columns: table => new
                {
                    AtamaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SinavSalonuID = table.Column<int>(type: "int", nullable: false),
                    PersonelID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GozetmenAtama", x => x.AtamaID);
                    table.ForeignKey(
                        name: "FK_GozetmenAtama_Personel_PersonelID",
                        column: x => x.PersonelID,
                        principalTable: "Personel",
                        principalColumn: "PersonelID");
                    table.ForeignKey(
                        name: "FK_GozetmenAtama_SinavSalonu_SinavSalonuID",
                        column: x => x.SinavSalonuID,
                        principalTable: "SinavSalonu",
                        principalColumn: "SinavSalonuID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ders_BolumID",
                table: "Ders",
                column: "BolumID");

            migrationBuilder.CreateIndex(
                name: "IX_GozetmenAtama_PersonelID",
                table: "GozetmenAtama",
                column: "PersonelID");

            migrationBuilder.CreateIndex(
                name: "IX_GozetmenAtama_SinavSalonuID",
                table: "GozetmenAtama",
                column: "SinavSalonuID");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanici_PersonelID",
                table: "Kullanici",
                column: "PersonelID");

            migrationBuilder.CreateIndex(
                name: "IX_Personel_BolumID",
                table: "Personel",
                column: "BolumID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMazeret_OturumID",
                table: "PersonelMazeret",
                column: "OturumID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonelMazeret_PersonelID",
                table: "PersonelMazeret",
                column: "PersonelID");

            migrationBuilder.CreateIndex(
                name: "IX_Sinav_DersID",
                table: "Sinav",
                column: "DersID");

            migrationBuilder.CreateIndex(
                name: "IX_Sinav_OturumID",
                table: "Sinav",
                column: "OturumID");

            migrationBuilder.CreateIndex(
                name: "IX_SinavLog_PersonelID",
                table: "SinavLog",
                column: "PersonelID");

            migrationBuilder.CreateIndex(
                name: "IX_SinavLog_SinavID",
                table: "SinavLog",
                column: "SinavID");

            migrationBuilder.CreateIndex(
                name: "IX_SinavSalonu_DerslikID",
                table: "SinavSalonu",
                column: "DerslikID");

            migrationBuilder.CreateIndex(
                name: "IX_SinavSalonu_SinavID",
                table: "SinavSalonu",
                column: "SinavID");

            // 1. SQL klasörünün yolunu belirle
            var sqlFolder = Path.Combine(Directory.GetCurrentDirectory(), "database");

            // 2. Dosyaları çalışma sırasına (bağımlılıklara) göre listele
            // Önce Fonksiyonlar (UDF), sonra View'lar, sonra Prosedürler (SP) ve en son Trigger'lar (T)
            string[] sqlFiles = {
        "UDF_1_GozetmenKontrol.sql",
        "UDF_2_DersCakisirMi.sql",
        "UDF_3_KapasiteKontrol.sql",
        "V_1_SinavProgramDetayi.sql",
        "V_2_DerslikDoluluk.sql",
        "V_3_PersonelYuku.sql",
        "SP_1_SalonHesapla.sql",
        "SP_2_HavuzdanAtama.sql",
        "SP_3_Loglama.sql",
        "T_1_SalonCakisirMi.sql",
        "T_2_SinavGuncelleme.sql"
    };

            // 3. Dosyaları sırayla oku ve veritabanında çalıştır
            foreach (var fileName in sqlFiles)
            {
                var filePath = Path.Combine(sqlFolder, fileName);
                if (File.Exists(filePath))
                {
                    migrationBuilder.Sql(File.ReadAllText(filePath));
                }
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GozetmenAtama");

            migrationBuilder.DropTable(
                name: "Kullanici");

            migrationBuilder.DropTable(
                name: "PersonelMazeret");

            migrationBuilder.DropTable(
                name: "SinavLog");

            migrationBuilder.DropTable(
                name: "SinavSalonu");

            migrationBuilder.DropTable(
                name: "Personel");

            migrationBuilder.DropTable(
                name: "Derslik");

            migrationBuilder.DropTable(
                name: "Sinav");

            migrationBuilder.DropTable(
                name: "Ders");

            migrationBuilder.DropTable(
                name: "Oturum");

            migrationBuilder.DropTable(
                name: "Bolum");
        }
    }
}
