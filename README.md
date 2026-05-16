# OBS Projesi

OBS Projesi, Veritabanı Sistemleri dersi kapsamında geliştirilen bir sınav planlama ve gözetmen atama sistemidir.

Sistem; ders, bölüm, oturum, derslik, sınav ve gözetmen bilgilerini kullanarak sınav programı oluşturmayı, salon atamayı, gözetmen görevlendirmeyi ve raporlamayı amaçlar.

---

## Projenin Amacı

Bu projenin amacı, bir fakültedeki sınav planlama sürecini daha düzenli ve kontrollü hale getirmektir.

Sistem ile:

- Sınavlar belirli oturumlara atanır.
- Derslik kapasitesine göre uygun salonlar seçilir.
- Aynı derslikte aynı oturumda birden fazla sınav yapılması engellenir.
- Aynı bölüm ve yarıyıldaki derslerin çakışması engellenir.
- Gözetmen atamalarında uygunluk, mazeret ve görev yükü kontrolleri yapılır.
- Yönetici ve görüntüleyici rolleri ayrılır.
- Rapor ekranları üzerinden sınav, derslik ve gözetmen bilgileri görüntülenir.

---

## Kullanılan Teknolojiler

- ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- Razor View Engine
- Bootstrap / CSS
- Cookie Authentication
- Role Based Authorization
- T-SQL
- Stored Procedure
- User Defined Function
- View
- Trigger
- Index
- Transaction

---

## Proje Modülleri

### 1. Kullanıcı Girişi ve Yetkilendirme

Sistemde kullanıcı giriş sistemi bulunmaktadır.

Roller:

- Admin
- Viewer

Admin kullanıcıları sistemde ekleme, düzenleme ve silme işlemleri yapabilir.

Viewer kullanıcıları sadece görüntüleme işlemleri yapabilir.

Admin yetkisi gerektiren bazı işlemler:

- Sınav oluşturma
- Sınav düzenleme
- Sınav silme
- Gözetmen atama
- Gözetmen ataması düzenleme
- Gözetmen ataması silme
- Veritabanı programlama ekranındaki işlem butonları

---

### 2. Sınav Planlama

Sınav Planlama modülü ile derslere sınav tarihi ve oturum atanır.

Sistem sınav oluştururken şu kontrolleri yapar:

- Ders seçimi zorunludur.
- Oturum seçimi zorunludur.
- Aynı ders aynı gün ve aynı oturumda tekrar planlanamaz.
- Aynı bölüm ve aynı yarıyıldaki dersler aynı tarih ve oturuma konulamaz.
- Aynı bölüm ve aynı yarıyıl için bir günde ikiden fazla sınav planlanırsa uyarı verilir.
- Sınav için yeterli derslik kapasitesi yoksa işlem geri alınır.

---

### 3. Akıllı Salon Atama

Sistem sınav oluşturulduğunda dersin öğrenci sayısına göre uygun derslikleri otomatik seçer.

Salon atama mantığı:

1. Aynı tarih ve oturumda kullanılan derslikler tespit edilir.
2. Boş ve aktif derslikler listelenir.
3. Öncelikle aynı katta yeterli kapasite olup olmadığı kontrol edilir.
4. Aynı katta yeterli kapasite varsa o kattaki salonlar seçilir.
5. Aynı katta kapasite yetmiyorsa genel kapasite sıralamasına göre salon seçilir.
6. Toplam kapasite öğrenci sayısını karşılamıyorsa sınav oluşturma işlemi iptal edilir.

Bu yapı sayesinde derslik kullanımı daha düzenli hale getirilmiştir.

---

### 4. Gözetmen Atama

Gözetmen Atama modülü ile sınav salonlarına personel atanır.

Atama sırasında yapılan kontroller:

- Geçerli sınav salonu seçilmelidir.
- Geçerli gözetmen seçilmelidir.
- Aynı personel aynı sınav salonuna tekrar atanamaz.
- Aynı personel aynı tarih ve oturumda başka salonda görev alamaz.
- Personel izinli veya uygun değilse atama yapılamaz.
- Bir personel aynı gün arka arkaya en fazla üç oturumda görev alabilir.

---

### 5. Ortak Havuz Sistemi

Gözetmen atama ekranında ortak havuz mantığı uygulanmıştır.

Sistem önce sınavın ait olduğu bölümdeki gözetmenleri listeler.

Eğer gerekli görülürse diğer bölümlerdeki uygun gözetmenler de ortak havuzdan seçilebilir.

Gözetmen listesinde:

- Önce ilgili bölümdeki gözetmenler
- Sonra Mühendislik Fakültesi Ortak Havuzu

görünür.

Ayrıca gözetmenler görev sayılarına göre sıralanır. Böylece daha dengeli görev dağılımı yapılması desteklenir.

---

### 6. Raporlar

Sistemde raporlama ekranları bulunmaktadır.

Rapor ekranları:

- Sınav Programı Detayı
- Derslik Durumu
- Gözetmen Görev Yükü

Bu ekranlar üzerinden sınav planı, derslik kullanımı ve gözetmen görev dağılımı görüntülenebilir.

---

### 7. Veritabanı Programlama Ekranı

Sistemde veritabanı işlemlerini test etmek için ayrı bir yönetim ekranı bulunmaktadır.

Bu ekranda şu işlemler yer alır:

- Akıllı Salon Ata
- Havuzdan Gözetmen Ata
- Yedek Al
- Kapasite Kontrol
- Yarıyıl Çakışma Kontrol
- Gözetmen Uygunluk Kontrol

Admin kullanıcıları işlem çalıştırabilir.

Viewer kullanıcıları veri değiştiren işlemleri çalıştıramaz.

---

### 8. Log Kayıtları

Sistemde sınav değişikliklerini takip etmek için log ekranı bulunmaktadır.

Log ekranında sınav üzerinde yapılan değişikliklerin geçmişi görüntülenir.

Örnek log bilgileri:

- İşlem tarihi
- İşlem türü
- İlgili sınav
- Eski değer
- Yeni değer
- Değiştiren kişi

Not: Log trigger tarafının doğru çalışması için ilgili SQL scriptinin doğru veritabanında çalıştırılmış olması gerekir.

---

## Veritabanı Özellikleri

Projede ilişkisel veritabanı yapısı kullanılmıştır.

Başlıca tablolar:

- Bolum
- Ders
- Derslik
- Oturum
- Sinav
- SinavSalonu
- Personel
- PersonelMazeret
- GozetmenAtama
- SinavLog
- Kullanici

---

## Veritabanı Scriptleri

Veritabanı scriptleri `database` klasörü altında tutulmaktadır.

Önemli script dosyaları:

```text
database/
├── SP_1_SalonHesapla.sql
├── SP_2_HavuzdanAtama.sql
├── SP_3_Loglama.sql
├── UDF_1_GozetmenKontrol.sql
├── UDF_2_DersCakisirMi.sql
├── UDF_3_KapasiteKontrol.sql
├── V_1_SinavProgramDetayi.sql
├── V_2_DerslikDoluluk.sql
├── V_3_PersonelYuku.sql
├── T_1_SalonCakisma.sql
├── T_2_SinavGuncelleme.sql
├── SEC_1_RolesAndPermissions.sql
└── IDX_1_PerformanceIndexes.sql
