# Bantuan SmartTicker

Panduan ini berlaku untuk SmartTicker 1.0.3. Panduan ini menjelaskan ticker utama, Pengaturan Aplikasi,
kuotasi, aturan peringatan, izin situs web, cadangan, dan masalah umum.

SmartTicker membaca HTML statis publik dari halaman web yang Anda konfigurasi. SmartTicker tidak
menyediakan umpan data pasar, dan informasi yang diekstrak dapat tertunda, tidak lengkap, atau
keliru. Verifikasikan informasi keuangan penting dengan sumber yang berwenang.

## Navigasi cepat

| Area | Lompat ke |
| --- | --- |
| Memulai | [Membuka jendela Bantuan dan konfigurasi](#membuka-jendela-bantuan-dan-konfigurasi) |
| Ticker utama | [Kontrol](#kontrol-ticker-utama) · [Tampilan bergulir atau statis](#memilih-tampilan-kuotasi-bergulir-atau-statis) · [Pindahkan](#memindahkan-ticker) · [Ubah ukuran](#mengubah-ukuran-ticker) · [Jeda](#menjeda-dan-melanjutkan) · [Referensi menu](#referensi-menu-utama) |
| Kuotasi dan berita | [Kuotasi](#kuotasi) · [Tambah entri](#menambahkan-entri-kuotasi-atau-berita) · [Kelompokkan kuotasi](#mengelompokkan-kuotasi) · [URL sumber](#preset-sumber-dan-url) · [Selektor](#referensi-bidang-selektor) · [Penemuan](#menemukan-selektor) · [Validasi](#memvalidasi-sumber) |
| Preferensi aplikasi | [Pengaturan Aplikasi](#pengaturan-aplikasi) · [Baris dan kecepatan](#baris-dan-kecepatan-ticker) · [Mulai otomatis](#memulai-smartticker-saat-masuk) · [Akses situs web](#akses-situs-web) · [Tampilan](#tampilan) · [Pencadangan dan pemulihan](#pencadangan-dan-pemulihan) · [Edit file konfigurasi](#mengedit-file-konfigurasi-secara-langsung) |
| Peringatan harga | [Aturan peringatan](#aturan-peringatan) · [Buat aturan](#membuat-aturan) · [Perilaku pemicuan](#saat-aturan-terpicu) · [Keluaran peringatan](#pengaturan-keluaran-peringatan) · [Kelola aturan](#mengelola-aturan-yang-dikonfigurasi) |
| Data dan dukungan | [File lokal dan privasi](#file-lokal-dan-privasi) · [Pemecahan masalah](#pemecahan-masalah) · [Dukungan](#dukungan) |

## Membuka jendela Bantuan dan konfigurasi

Klik kanan ticker untuk membuka menunya. Perintah konfigurasi utama adalah:

- **Quotes...** (*Kuotasi...*): menambah, menguji, mengedit, mengurutkan, dan menghapus sumber kuotasi atau berita.
- **Quote groups...** (*Grup kuotasi...*): membuat, memperbarui, atau menghapus grup dan mengaitkan kuotasi dengannya.
- **Alerts** (*Peringatan*): membuat dan mengelola aturan peringatan harga.
- **App Settings...** (*Pengaturan Aplikasi...*): mengonfigurasi baris, kecepatan, interval penyegaran, mulai otomatis, akses
	situs web, warna, transparansi, dan cadangan.
- **View** (*Tampilan*): memilih satu dari empat kombinasi yang saling eksklusif: bergulir atau statis,
	dengan Prices only (*Harga saja*) atau Prices with News (*Harga dengan Berita*).
- **Help** (*Bantuan*): membuka panduan ini di dalam SmartTicker.
- **About SmartTicker** (*Tentang SmartTicker*): menampilkan versi yang terpasang dan pemberitahuan lisensi.
- **Exit** (*Keluar*): menutup SmartTicker sepenuhnya.

Jendela Bantuan segera memformat panduan bawaan untuk bahasa aplikasi yang dipilih,
lalu memeriksa panduan online yang sesuai setiap kali Anda membuka Bantuan atau
mengubah **Language** (*Bahasa*). Panduan online bahasa Indonesia adalah:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.id.md>

Panduan terjemahan menggunakan `help/HELPME.<language-code>.md` di repositori yang sama.
Jika dokumen online tidak dapat diunduh, SmartTicker tetap menampilkan terjemahan bawaan
yang sesuai dalam aplikasi terpasang. Mengubah **Language** (*Bahasa*) segera memperbarui
judul, status, navigasi, dan seluruh panduan pada jendela Bantuan yang terbuka. Tutup
Bantuan dengan kontrol tutup normal pada bilah judulnya.

## Kontrol ticker utama

### Memilih tampilan kuotasi bergulir atau statis

SmartTicker menyediakan empat mode tampilan yang saling eksklusif. Klik kanan ticker, buka
**View** (*Tampilan*), lalu pilih salah satunya. Tata letak segera berubah dan pilihan Anda disimpan.

| Opsi tampilan | Hasil |
| --- | --- |
| **Left-to-right scroll: Prices only** (*Gulir kiri ke kanan: Harga saja*) | Teks harga berjalan pada ticker utama; tanpa tampilan berita. Ini adalah pengaturan bawaan. |
| **Left-to-right scroll: Prices with News** (*Gulir kiri ke kanan: Harga dengan Berita*) | Teks harga dan berita berjalan pada ticker utama. |
| **Static view: Prices only** (*Tampilan statis: Harga saja*) | Ubin harga responsif di jendela utama; tanpa jendela Berita. |
| **Static view: Prices with News** (*Tampilan statis: Harga dengan Berita*) | Ubin harga responsif serta jendela statis **SmartTicker News** yang terpisah. |

File pengaturan yang dibuat sebelum pilihan ini ditambahkan dipetakan ke kombinasi yang sesuai
dari pengaturan bergulir/statis dan berita yang tersimpan. Mode tampilan hanya dikelola dari
menu **View** (*Tampilan*) yang dibuka dengan mengklik kanan ticker.

- Dalam kedua mode bergulir, harga menggunakan teks berjalan horizontal serta jumlah baris harga
	dan kecepatan gulir yang dikonfigurasi.
- Dalam kedua mode statis, grup muncul sebagai ubin responsif yang ditata dari kiri ke kanan. Ubin
  berlanjut ke baris berikutnya hanya jika jendela terlalu sempit. Harga tidak bergerak
  secara otomatis.
- Setiap ubin kuotasi memiliki kolom **Symbol** (*Simbol*), **Last** (*Terakhir*), **Chg** (*Perubahan*), dan **Chg%** (*Perubahan%*)
  sendiri yang sejajar. **Chg** diperoleh
	dari Last dan Chg% karena halaman sumber menyediakan selektor persentase, bukan
	selektor perubahan absolut yang terpisah. Nilai ini menampilkan `—` jika salah satu nilai tidak tersedia.
- Pilih kepala grup untuk menciutkan atau memperluasnya. Urutan grup mengikuti kemunculan pertama
	kuotasinya dalam urutan entri yang dikonfigurasi; baris di dalam grup mempertahankan urutan tersebut.
- Entri tanpa grup muncul di bawah **Ungrouped** (*Tanpa grup*).
- Arahkan penunjuk ke Last untuk melihat nilai prapasar dan setelah jam perdagangan yang tersedia. Klik dua kali
	baris kuotasi untuk membuka halaman sumbernya.
- Kedipan peringatan dan warna naik/turun berfungsi dalam kedua mode harga.
- Berita terbuka secara otomatis dalam jendela **SmartTicker News** terpisah yang berisi ubin grup statis
	**Symbol / Headline** (*Simbol / Judul berita*). Berita tidak berjalan dalam mode statis. Jendela News
	memiliki bilah judul dan batas pengubahan ukuran normal, sehingga jendela Quotes dan News dapat
	dipindahkan secara terpisah ke monitor yang berbeda. Klik dua kali baris judul berita untuk membuka
	sumbernya.
- Pada peluncuran awal, News menggunakan ukuran ringkas 680×340. SmartTicker menempatkannya di monitor lain
	jika tersedia; pada satu monitor, SmartTicker terlebih dahulu mencoba area kosong di bawah,
	kanan, atas, atau kiri Prices. Setelah itu, Anda dapat memindahkan dan mengubah ukurannya seperti biasa.
- Di dalam setiap grup News, judul berita diselang-seling berdasarkan kuotasi: satu judul berita dari
	kuotasi pertama, lalu satu dari kuotasi berikutnya, dan berlanjut dalam putaran. Dengan demikian, kuotasi dengan banyak
	judul berita tidak dapat memenuhi seluruh bagian atas grupnya.
- Buka daftar tarik-turun satu baris **Show news for** (*Tampilkan berita untuk*) lalu centang atau hapus centang setiap kuotasi
	secara terpisah. Kombinasi kuotasi apa pun dapat terlihat, termasuk semua atau tidak satu pun. Tombol
	merangkum pilihan saat ini, dan entri mencantumkan kuotasi serta sumber agar
	simbol duplikat tetap independen. Kuotasi yang tidak dicentang disimpan dalam file pengaturan Anda
	sebagai `hiddenNewsQuotes`, sehingga tetap berlaku setelah aplikasi dimulai ulang dan ikut dalam cadangan pengaturan.
- Seret pegangan bertitik di samping judul ubin kuotasi atau berita lalu jatuhkan pada bagian kiri
	atau kanan ubin lain. Urutan berubah di kedua jendela dan disimpan dengan
	mengurutkan ulang entri dasar yang dikonfigurasi.
- Grup dengan banyak baris bergulir di dalam ubinnya sendiri yang dibatasi. Tampilan keseluruhan hanya bergulir
	secara vertikal jika baris ubin yang terbungkus tidak muat dalam tinggi jendela saat ini.

Menutup **SmartTicker News** tidak menonaktifkan pengumpulan berita. Untuk membukanya kembali, klik kanan
jendela Prices lalu pilih **View > Open static news window** (*Tampilan > Buka jendela berita statis*). Memilih **Static
view: Prices only** (*Tampilan statis: Harga saja*) akan menutupnya; memilih **Static view: Prices with News** (*Tampilan statis: Harga dengan Berita*) akan membukanya
kembali. Kedua pilihan bergulir menutup jendela News yang terpisah; pilihan bergulir
Prices-with-News memulihkan teks berita berjalan di ticker utama.

Pergantian mode menerapkan ukuran yang tersimpan untuk tampilan tersebut. Ticker bergulir, jendela Prices
statis, dan jendela News statis masing-masing menyimpan lebar dan tinggi secara independen.

### Memindahkan ticker

Tekan dan tahan pegangan bertitik vertikal di bagian atas bidang kiri yang sempit, seret
ticker, lalu lepaskan tombol mouse. Teks ticker bukan permukaan seret, sehingga memilih
atau mengklik konten tidak akan secara tidak sengaja mulai memindahkan jendela.

### Mengubah ukuran ticker

Pindahkan penunjuk ke tepi atau sudut mana pun sampai kursor pengubah ukuran muncul, lalu tekan dan
seret. Sudut kanan bawah memiliki tanda kecil pengubahan ukuran yang terlihat. Lebar minimum jendela
adalah 420 piksel. Tinggi mode bergulir adalah 50 hingga 900 piksel, tinggi Prices statis adalah 420
hingga 4320 piksel, dan tinggi News statis adalah 240 hingga 4320 piksel.

Pengubahan ukuran secara manual memperbarui dimensi yang tersimpan untuk tampilan aktif setelah penyeretan selesai.
Ketiga pasangan ukuran disertakan dalam cadangan pengaturan. Posisi jendela tidak disimpan.
Jika ukuran bergulir terlalu pendek bagi baris Price/News dan ukuran font bergulir yang dipilih,
SmartTicker menaikkan tinggi tersimpan tersebut secara otomatis. Dengan demikian, memilih **Left-to-right
scroll: Prices with News** (*Gulir kiri ke kanan: Harga dengan Berita*) selalu menyediakan ruang untuk baris News, alih-alih
menyembunyikannya tanpa pemberitahuan.
Setiap kali jendela dibuka atau dipindahkan, SmartTicker menjaga setidaknya sudut kiri atas 32 pikselnya
tetap berada di dalam area kerja layar dan membatasi X serta Y global ke nilai minimum 1. Hal ini menjaga pegangan
pemindah atau sudut judul tetap dapat dijangkau dengan mouse, bahkan setelah monitor dilepas.

### Menjeda dan melanjutkan

Pilih tombol status di bawah pegangan pemindah, atau klik kanan lalu pilih
**Pause / Resume** (*Jeda / Lanjutkan*). Menjeda akan menghentikan penyegaran harga dan berita otomatis serta membekukan
teks berjalan. Tindakan ini juga mencegah kedua perintah penyegaran manual memulai pekerjaan baru. Permintaan sumber
yang sudah berlangsung tidak dibatalkan secara paksa hanya karena Pause dan mungkin
selesai sebelum seluruh aktivitas benar-benar berhenti. Resume memulai kembali timer otomatis.

Di Windows, SmartTicker secara otomatis menetapkan prioritas proses sistem operasinya ke **Low** (*Rendah*) dan mengaktifkan
**Efficiency mode** (*Mode efisiensi*) Windows (EcoQoS) sebelum memulai antarmuka pengguna. Tidak ada pengaturan aplikasi untuk
perilaku ini. SmartTicker juga menggunakan jalur perenderan perangkat lunak dengan beban rendah. Waktu teks berjalan menyesuaikan
dengan kecepatan yang dikonfigurasi, dan teks berjalan yang dijeda, kosong, atau dilepas akan menghentikan timer
animasinya. Baris yang tidak berubah menekan notifikasi visual berulang. Kedipan peringatan dan
sorotan perubahan berwarna cokelat selama tiga detik memang disengaja dan tidak menjeda pengguliran. Penjadwalan proses
Linux diserahkan kepada sistem operasi. Jika Windows menolak salah satu pengaturan proses,
SmartTicker melaporkan kegagalan tersebut ke pelacakan diagnostik dan melanjutkan proses memulai aplikasi.

### Membuka tautan

Klik dua kali teks ticker yang memiliki tautan, termasuk judul berita, untuk membuka sumbernya di
browser bawaan Anda. SmartTicker tidak membuka tautan dengan satu klik.

### Sorotan perubahan

Setelah setiap penyegaran, SmartTicker menandai secara singkat apa yang berubah dengan latar belakang cokelat selama tiga
detik:

- Kuotasi yang harganya berbeda dari sinkronisasi sebelumnya.
- Setiap judul berita yang tidak ada pada sinkronisasi sebelumnya untuk kuotasi tersebut.

Sinkronisasi pertama setelah aplikasi dimulai tidak menyorot apa pun karena tidak ada nilai terdahulu untuk
dibandingkan. Peringatan yang terpicu mempertahankan warna kedip peringatannya sendiri dan mendapat prioritas.

### Referensi menu utama

| Perintah | Efek |
| --- | --- |
| **Refresh prices now** (*Segarkan harga sekarang*) | Memulai ulang siklus harga bertahap dan meminta slot waktu pertamanya ketika SmartTicker tidak dijeda. |
| **Refresh news now** (*Segarkan berita sekarang*) | Memulai ulang siklus News bertahap dan meminta slot waktu pertamanya ketika SmartTicker tidak dijeda. |
| **Pause / Resume** (*Jeda / Lanjutkan*) | Mengalihkan penyegaran dan pergerakan teks berjalan. |
| **View > Left-to-right scroll: Prices only** (*Tampilan > Gulir kiri ke kanan: Harga saja*) | Hanya menggunakan teks harga berjalan horizontal. Ini adalah pengaturan bawaan. |
| **View > Left-to-right scroll: Prices with News** (*Tampilan > Gulir kiri ke kanan: Harga dengan Berita*) | Menggunakan kedua teks berjalan horizontal. |
| **View > Static view: Prices only** (*Tampilan > Tampilan statis: Harga saja*) | Hanya menggunakan ubin kuotasi statis yang responsif. |
| **View > Static view: Prices with News** (*Tampilan > Tampilan statis: Harga dengan Berita*) | Menggunakan ubin kuotasi serta jendela News statis yang terpisah. |
| **View > Open static news window** (*Tampilan > Buka jendela berita statis*) | Membuka kembali jendela News terpisah setelah ditutup. Tersedia dalam mode statis saat berita diaktifkan. |
| **Language** (*Bahasa*) | Memilih salah satu dari 16 bahasa untuk menu, teks status, dan panduan Bantuan lengkap. Jendela Bantuan yang sedang terbuka langsung diperbarui. |

Visibilitas baris, bahasa, dan nilai konfigurasi lainnya disimpan secara otomatis.

## Kuotasi

Buka **Quotes...** (*Kuotasi...*) dari menu klik kanan. Setiap entri yang dikonfigurasi mewakili satu
simbol dan satu halaman web. Simbol duplikat diizinkan dan tetap independen karena
setiap entri memiliki sumber, selektor, opsi pengumpulan, dan peringatannya sendiri.

### Mulai cepat dengan sampel yang dipublikasikan

Jika belum ada entri, jendela Quotes menawarkan **Import sample quotes from GitHub** (*Impor kuotasi sampel dari GitHub*).
Tindakan ini mengunduh sampel repositori dan mengganti pengaturan aplikasi saat ini.
Tinjau setiap URL yang diimpor dan ketentuan terkini setiap situs web sebelum menggunakannya. Anda dapat
mengedit atau menghapus entri sampel apa pun setelahnya.

**Import Sample Quotes Config** (*Impor Konfigurasi Kuotasi Sampel*) di bagian atas jendela Quotes dan App Settings
melakukan hal yang sama kapan pun, setelah konfirmasi:

- SmartTicker menanyakan **Are you sure?** (*Apakah Anda yakin?*) dan memperingatkan bahwa unduhan akan mengganti
	kuotasi, grup kuotasi, persetujuan sumber, tampilan, penampilan, dan pengaturan aplikasi lainnya yang ada.
	Aturan peringatan tersimpan dalam file sendiri dan tidak dihapus.
- **Export existing config...** (*Ekspor konfigurasi yang ada...*) bersifat opsional. Tindakan ini menyimpan konfigurasi saat ini ke
file JSON lokal, lalu kembali ke konfirmasi yang sama.
- **Import Sample Quotes Config** (*Impor Konfigurasi Kuotasi Sampel*) mengunduh sampel dari internet dan mengganti
	konfigurasi Anda.
- **Cancel** (*Batal*) tidak mengubah apa pun.

### Menambahkan entri kuotasi atau berita

1. Masukkan label **Ticker**, misalnya `MSFT`. SmartTicker memangkasnya dan menyimpannya dalam
	 huruf besar.
2. Secara opsional, pilih **Group** (*Grup*) yang ada dari daftar pencarian, atau ketik nama baru seperti
	 `Nasdaq`, `Precious Metals`, atau `Mag 7`. Biarkan kosong untuk **Ungrouped** (*Tanpa grup*).
3. Pilih preset **Source** (*Sumber*).
4. Masukkan **URL suffix** (*Akhiran URL*), atau URL lengkap saat menggunakan **Custom URL** (*URL Kustom*).
5. Pilih **Price** (*Harga*), **News** (*Berita*), atau keduanya di bawah **Collect** (*Kumpulkan*). Setidaknya satu pilihan diwajibkan.
6. Masukkan selektor secara manual, gunakan tombol penemuan, atau biarkan selektor opsional
	 kosong untuk menggunakan deteksi bawaan.
7. Pilih **Validate URL** (*Validasi URL*) untuk menguji harga reguler dan/atau judul berita.
8. Jika SmartTicker meminta persetujuan sumber, tinjau situs web dan konfirmasikan hanya jika
	 Anda diizinkan mengumpulkan data darinya.
9. Pilih **Add independent entry** (*Tambah entri independen*). SmartTicker menyimpan entri dan segera menyegarkan
	 data yang diaktifkan.

### Mengelompokkan kuotasi

Grup adalah koleksi bernama yang Anda tentukan. Grup tidak terikat pada bursa atau kategori
bawaan, sehingga Anda dapat mengatur entri berdasarkan pasar, jenis aset, strategi, portofolio,
wilayah, atau skema lainnya. Nama dipangkas, dapat menggunakan Unicode, dan dapat berisi hingga
80 karakter. Setiap kuotasi hanya dapat menjadi anggota paling banyak satu grup.

Gunakan **Manage groups** (*Kelola grup*) di samping bidang Group, atau pilih **Quote groups...** (*Grup kuotasi...*) dari
menu klik kanan ticker. Jendela ini memiliki tiga area kerja:

- Di sebelah kiri, masukkan **Group name** (*Nama grup*), lalu pilih **Create** (*Buat*). Pilih grup yang ada,
	edit namanya, lalu pilih **Update** (*Perbarui*), atau pilih **Delete** (*Hapus*). Grup kosong tetap dipertahankan.
- Di sebelah kanan, pilih kuotasi. Grupnya saat ini ditampilkan di kolom **Current group** (*Grup saat ini*);
	**Ungrouped** (*Tanpa grup*) berarti kuotasi tersebut tidak memiliki kaitan.
- Di tengah, pilih **Associate** (*Kaitkan*) setelah memilih satu grup dan satu kuotasi. Jika
	kuotasi tersebut sudah menjadi anggota grup lain, SmartTicker memindahkannya ke grup yang dipilih.
- Pilih **Remove association** (*Hapus kaitan*) untuk mengembalikan hanya kuotasi yang dipilih ke **Ungrouped** (*Tanpa grup*).
- Menghapus grup mengembalikan semua kuotasinya ke **Ungrouped** (*Tanpa grup*). Kuotasi, sumber, data saat ini,
	dan peringatan tidak dihapus.
- Anda juga dapat memilih grup yang ada dari daftar pencarian ketika menambah atau mengedit kuotasi,
	atau mengetik nama grup baru di sana.
- Gunakan kontrol naik/turun dalam Configured entries (*Entri yang dikonfigurasi*) untuk menentukan urutan grup dan baris dalam
	tabel statis.
- Dalam mode statis, seret judul ubin untuk mengurutkan ulang grup lengkap secara langsung. Urutan yang sama
	digunakan oleh jendela Quotes dan News yang terpisah.

Sampel yang dipublikasikan berisi enam grup contoh, sedangkan mode statis tetap nonaktif secara
bawaan. Aktifkan tampilan statis setelah mengimpornya untuk melihat grup tersebut sebagai tabel.

### Preset sumber dan URL

| Sumber | Yang harus dimasukkan | Kebijakan yang ditampilkan SmartTicker |
| --- | --- | --- |
| **Yahoo Finance** | Akhiran setelah `https://finance.yahoo.com/`, misalnya `quote/MSFT/`. | Izin tertulis diwajibkan. Ketentuan Yahoo melarang pengumpulan otomatis tanpa izin sebelumnya. |
| **CNBC** | Akhiran setelah `https://www.cnbc.com/`. | Periksa kebijakan terkini dan arahan robots situs tersebut. |
| **Trading Economics** | Akhiran setelah `https://tradingeconomics.com/`. | Utamakan API terdokumentasi atau umpan resmi dan periksa kebijakan terkini situs tersebut. |
| **Custom URL** (*URL Kustom*) | URL lengkap halaman publik `http://` atau `https://`. | Tinjau ketentuan, kebijakan privasi, dan aturan akses otomatis situs tersebut. |

Hanya URL HTTP dan HTTPS absolut yang diterima. URL yang memuat nama pengguna atau
kata sandi tertanam akan ditolak. Masuk melalui browser tidak memberi SmartTicker izin untuk mengumpulkan
halaman, dan SmartTicker tidak menggunakan sesi browser terautentikasi.

Baris **Full URL** (*URL Lengkap*) menampilkan alamat akhir yang dihasilkan dari awalan preset dan
akhiran Anda. Periksa sebelum validasi atau penemuan.

### Opsi pengumpulan

- **Price** (*Harga*) meminta harga reguler. Selektor opsional untuk perubahan, prapasar, dan setelah jam perdagangan
	dievaluasi dari halaman unduhan yang sama.
- **News** (*Berita*) meminta tautan judul berita dari halaman tersebut.
- Memilih keduanya memungkinkan satu entri berkontribusi pada kedua area ticker.
- Menghapus kedua pilihan tidak valid.

### Referensi bidang selektor

Selektor CSS mengidentifikasi elemen dalam HTML statis halaman web. Selektor bersifat
opsional, kecuali jika deteksi otomatis tidak dapat menemukan nilai yang Anda perlukan.

| Bidang | Nilai yang diekstrak SmartTicker |
| --- | --- |
| **Price selector** (*Selektor harga*) | Harga reguler atau penutupan. |
| **Price change** (*Perubahan harga*) | Persentase perubahan sesi reguler. Jika kosong, deteksi perubahan bawaan akan dicoba. |
| **Pre-market selector** (*Selektor prapasar*) | Harga prapasar, jika sesi tersebut tersedia pada halaman. |
| **Pre-market change** (*Perubahan prapasar*) | Persentase perubahan prapasar. |
| **After-hours selector** (*Selektor setelah jam perdagangan*) | Harga pascapasar atau setelah jam perdagangan. |
| **After-hours change** (*Perubahan setelah jam perdagangan*) | Persentase perubahan pascapasar atau setelah jam perdagangan. |
| **News selector** (*Selektor berita*) | Tautan judul berita. Pilih elemen jangkar atau kontainer yang hasilnya mencakup tautan. |

Nilai prapasar dan setelah jam perdagangan melengkapi harga reguler; nilai tersebut tidak
menggantikannya. Halaman mungkin menghilangkan elemen tersebut di luar sesi pasar yang sesuai.

Contoh selektor Yahoo Finance yang digunakan oleh sampel yang dipublikasikan adalah:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

Markup situs web berubah seiring waktu. Perlakukan contoh sebagai titik awal, bukan kontrak
permanen.

### Menemukan selektor

Setiap bidang selektor memiliki tombol **Discover** (*Temukan*) yang sesuai.

1. Lengkapi URL sumber dan setujui situs web jika persetujuan diperlukan.
2. Pilih tombol penemuan untuk jenis nilai yang tepat.
3. SmartTicker mengunduh HTML statis publik dan mencantumkan selektor yang mungkin beserta contoh
	 nilai, persentase keyakinan, dan alasan dalam tooltip.
4. Pilih **Use** (*Gunakan*) di samping saran untuk menyalinnya ke bidang yang sesuai.
5. Validasi atau amati hasilnya sebelum mengandalkannya.

Discovery tidak menjalankan JavaScript, masuk ke akun, melewati kontrol akses, atau memeriksa
browser Anda. Nilai yang hanya tersedia melalui JavaScript mungkin tidak memiliki selektor yang dapat ditemukan. Jenis penemuan
yang terpisah sengaja mencegah tercampurnya nilai prapasar dan setelah jam perdagangan.

### Memvalidasi sumber

**Validate URL** (*Validasi URL*) meminta halaman dan melaporkan harga reguler dan/atau jumlah
judul berita yang dapat dibaca. Fitur ini aman digunakan sebelum memasukkan ticker karena SmartTicker
menggunakan label sementara untuk pengujian.

Validasi ini saat ini tidak memverifikasi keempat bidang selektor prapasar dan setelah jam perdagangan.
Gunakan nilai sampel penemuannya, lalu konfirmasikan data sesi yang ditampilkan.

Kegagalan umum meliputi kesalahan HTTP, waktu habis, nilai yang hilang, nol judul berita, izin sumber
yang belum disetujui, konten yang hanya tersedia melalui JavaScript, atau selektor usang.

### Batas pengulangan berita

**Show max _N_ times** (*Tampilkan maksimal _N_ kali*) menerima 1 hingga 100 dan memiliki nilai bawaan 5. SmartTicker menghitung satu
penayangan untuk setiap siklus penyegaran News yang selesai ketika judul berita yang sama
dikembalikan. Setelah judul tersebut muncul dalam jumlah siklus yang dikonfigurasi, judul itu dihentikan
untuk sisa sesi aplikasi saat ini. Mengedit atau menghapus entri tersebut akan menghapus
riwayat pengulangannya.

### Mengedit, mengurutkan, dan menghapus entri

Daftar **Configured entries** (*Entri yang dikonfigurasi*) menampilkan simbol, grup, sumber, URL, lencana pengumpulan,
selektor harga reguler, selektor berita, dan batas pengulangan berita.

- **Edit** (*Edit*) memuat entri ke formulir. Pilih **Save changes** (*Simpan perubahan*) untuk menerapkannya atau
	**Cancel edit** (*Batalkan edit*) untuk membuang perubahan formulir.
- Tombol panah atas dan bawah mengubah urutan ticker dan segera menyimpannya.
- **Remove** (*Hapus*) menghapus entri dan data yang sedang ditampilkan.
- Jika aturan peringatan menargetkan entri tersebut, SmartTicker menanyakan apakah aturan itu juga akan dihapus. Peringatan
	tanpa kuotasi terkonfigurasi yang cocok tidak dapat terpicu.
- Mengganti nama entri memperbarui simbol tampilan aturan peringatan yang terpasang pada entri tersebut.

## Pengaturan Aplikasi

Buka **App Settings...** (*Pengaturan Aplikasi...*) dari menu klik kanan. Perubahan diterapkan dan disimpan
secara otomatis; tidak ada tombol Apply (*Terapkan*).

### Baris dan kecepatan ticker

| Pengaturan | Pilihan | Bawaan | Efek |
| --- | --- | --- | --- |
| Baris harga | 1 hingga 8 | 1 | Jumlah baris teks harga berjalan yang paralel. |
| Kecepatan gulir harga | 20, 30, 40, 50, 65, 80, 100, atau 120 px/sec | 50 | Kecepatan teks harga berjalan. |
| Baris berita | 1 hingga 8 | 1 | Jumlah baris judul berita berjalan yang paralel. |
| Kecepatan gulir berita | 20, 30, 40, 50, 65, 80, 100, atau 120 px/sec | 40 | Kecepatan teks berita berjalan. |
| Ukuran font bergulir | 9 hingga 24 pt | 14 pt | Teks Price dan News dalam baris bergulir. |
| Ukuran font statis | 9 hingga 24 pt | 13 pt | Teks kuotasi dan judul berita dalam baris statis. |
| Penyegaran harga | 30 hingga 300 detik, dalam kelipatan 15 detik | 60 detik | Waktu untuk setiap entri harga berizin menerima satu penyegaran terjadwal. |
| Penyegaran berita | 30 hingga 300 detik, dalam kelipatan 15 detik | 300 detik | Waktu untuk setiap entri News berizin menerima satu penyegaran terjadwal. |

Baris harga dan kecepatan gulir harga dinonaktifkan ketika tabel statis berkelompok aktif
karena mode tersebut menampilkan semua entri harga dan tidak pernah menggulir otomatis kedua jendela.
Pengaturan baris dan kecepatan News dipertahankan untuk tampilan bergulir.

Permintaan Price dan News didistribusikan secara independen ke slot satu detik sepanjang
intervalnya, bukan dimulai bersamaan. Misalnya, 60 entri selama 30 detik
menjadwalkan dua entri per detik; lima entri selama 30 detik menjadwalkan sekitar satu entri
setiap enam detik. Maksimal empat permintaan sumber berjalan sekaligus, pekerjaan duplikat untuk entri
dan aliran yang sama dilewati, dan slot yang terlewat tidak diputar ulang sekaligus. **Refresh
prices now** (*Segarkan harga sekarang*) atau **Refresh news now** (*Segarkan berita sekarang*) hanya memulai ulang aliran tersebut dan meminta slot pertamanya.
Harga dan judul berita yang sebelumnya berhasil tetap terlihat saat data pengganti dibaca.

Setiap permintaan HTTP memiliki batas waktu tetap 20 detik. Sumber yang lambat tidak menahan dispatcher
UI atau mencegah slot berikutnya menggunakan kapasitas permintaan yang tersisa. SmartTicker
melaporkan kegagalan seperti HTTP 403 dan 429 serta tidak melewati pembatasan. SmartTicker tidak
secara otomatis mengurai atau memberlakukan arahan robots,
nilai crawl-delay, atau instruksi backoff server, jadi pilih sumber yang patuh dan hindari
permintaan yang terlalu sering tanpa keperluan.

### Ukuran jendela

App Settings menyimpan tiga pasangan ukuran independen:

| Jendela | Lebar | Tinggi | Bawaan |
| --- | --- | --- | --- |
| Tampilan bergulir | 420–7680 px | 50–900 px | 980 × 64 px |
| Tampilan Prices statis | 420–7680 px | 420–4320 px | 980 × 420 px |
| Tampilan News statis | 420–7680 px | 240–4320 px | 680 × 340 px |

Mengubah nilai akan segera diterapkan ketika jendela atau tampilan tersebut aktif. Sampel yang dipublikasikan
menunjukkan 1200 × 96 untuk mode bergulir, 1200 × 720 untuk Prices statis, dan 760 × 480 untuk News
statis, dengan teks bergulir 15 poin dan teks statis 14 poin. Tinggi bergulir di bawah
ruang yang diperlukan oleh baris aktif akan dinaikkan secara otomatis ke nilai minimum yang diperlukan.

Gunakan empat pilihan di bawah **View** (*Tampilan*) untuk memilih apakah News ditampilkan dan apakah
tata letak bergulir atau tetap statis. Mengubah tampilan tidak pernah menghapus entri yang dikonfigurasi.

### Memulai SmartTicker saat masuk

Aktifkan **Start SmartTicker when I sign in** (*Mulai SmartTicker saat saya masuk*) untuk mendaftarkan file yang dapat dieksekusi dan
terpasang hanya bagi pengguna saat ini.

- Di Windows, SmartTicker menggunakan kunci registri `Run` milik pengguna saat ini.
- Pada desktop Linux yang mendukung konvensi autostart freedesktop, SmartTicker
	menulis `smartticker.desktop` di direktori autostart pengguna.
- Opsi ini dinonaktifkan pada platform yang tidak memiliki mekanisme pendaftaran
	yang didukung SmartTicker.

Sistem operasi merupakan sumber kebenaran. Jika pengaturan mulai otomatis diubah di luar SmartTicker,
kotak centang mencerminkan status sistem operasi saat pengaturan dimuat kembali.

### Akses situs web

**Allow website cookies and cross-host redirects** (*Izinkan cookie situs web dan pengalihan lintas host*) dinonaktifkan secara bawaan.

Jika dinonaktifkan:

- SmartTicker memerlukan satu persetujuan eksplisit untuk setiap host situs web sebelum memintanya.
- Cookie situs web tidak diterima.
- Pengalihan ke host berbeda diblokir.
- Host yang disetujui diingat dalam pengaturan lokal.

Jika diaktifkan:

- SmartTicker melewati langkah persetujuan per host.
- Cookie yang ditetapkan oleh situs web yang diminta hanya disimpan dalam kontainer memori terisolasi
	dan hilang ketika SmartTicker ditutup.
- Pengalihan ke host lain dapat diikuti.
- SmartTicker tetap tidak membaca cookie browser, mengirim kredensial, atau mengirim
	formulir masuk.

Menonaktifkan opsi ini akan menghapus data yang saat ini ditampilkan dari sumber yang belum disetujui
sampai host tersebut disetujui dan disegarkan.

#### Pilihan privasi situs web

Jika respons dikenali sebagai formulir privasi/cookie yang memuat pilihan positif dan
negatif, SmartTicker berhenti sementara dan menampilkan judul halaman, URL yang diminta,
URL persetujuan, ringkasan formulir, dan label Accept/Reject milik situs web.

- **Accept** (*Terima*) mengirim bidang tersembunyi yang disediakan formulir tersebut beserta kontrol Accept
	yang persis Anda pilih.
- **Reject** (*Tolak*) mengirim bidang tersembunyi tersebut beserta kontrol Reject yang persis Anda pilih.
- **Cancel** (*Batal*) tidak mengirim apa pun.

Ini adalah pilihan privasi situs web, bukan persetujuan izin per sumber milik SmartTicker.

#### Memvalidasi semua sumber

Pilih **Validate all sources** (*Validasi semua sumber*) untuk meninjau dan menguji setiap entri yang dikonfigurasi.

1. Jika akses situs web dibatasi, SmartTicker mengelompokkan entri yang belum disetujui menurut nama host
	 dan menampilkan satu dialog peninjauan sumber per host.
2. Tinjau host, ringkasan kebijakan, panduan, nama sumber, dan simbol.
3. Centang konfirmasi hanya jika Anda telah meninjau situs web dan diizinkan menggunakannya.
4. Pilih **Approve this source** (*Setujui sumber ini*), **Skip this source** (*Lewati sumber ini*), atau **Cancel validation** (*Batalkan validasi*).
5. SmartTicker menguji setiap entri yang diizinkan dan melaporkan jumlah yang lulus, gagal, dan dilewati.
	 Masalah individual muncul di bawah baris status.

Catatan persetujuan merekam izin di dalam SmartTicker; catatan tersebut tidak memberikan hak hukum atau
mengesampingkan ketentuan situs web.

### Tampilan

**Window transparency** (*Transparansi jendela*) hanya mengubah latar belakang ticker. Teks tetap legap. Rentangnya
20% hingga 100%, dalam kelipatan 5%, dan nilai bawaannya adalah 100%.

Bidang warna menerima nilai heksadesimal `#RRGGBB` dan juga menyediakan pemilih warna.

| Warna | Bawaan | Digunakan untuk |
| --- | --- | --- |
| Latar belakang | `#10151D` | Latar belakang ticker sebelum transparansi diterapkan. |
| Nama kuotasi | `#79C0FF` | Label simbol/sumber. |
| Harga penutupan | `#FFA657` | Harga reguler. |
| Setelah jam perdagangan | `#00E5FF` | Harga prapasar dan setelah jam perdagangan. |
| Berita ke-1 | `#FFFFFF` | Judul berita 1, 5, 9, dan seterusnya. |
| Berita ke-2 | `#00E5FF` | Judul berita 2, 6, 10, dan seterusnya. |
| Berita ke-3 | `#A3E635` | Judul berita 3, 7, 11, dan seterusnya. |
| Berita ke-4 | `#79C0FF` | Judul berita 4, 8, 12, dan seterusnya. |
| Perubahan naik | `#3FB950` | Perubahan persentase positif. |
| Perubahan turun | `#F85149` | Perubahan persentase negatif. |
| Kedip peringatan | `#FF00FF` | Peringatan harga yang terpicu, bergantian dengan hitam. |

**Reset to defaults** (*Atur ulang ke bawaan*) memulihkan setiap warna di atas dan opasitas latar belakang 100%. Tindakan ini
tidak mengatur ulang baris, kecepatan, ukuran font, ukuran jendela, sumber, interval penyegaran, peringatan, atau
bahasa.

### Pencadangan dan pemulihan

SmartTicker menyimpan pengaturan aplikasi dan aturan peringatan dalam file JSON terpisah serta
menyediakan tombol terpisah untuk setiap jenis cadangan.

#### Mengekspor dan mengimpor pengaturan

- **Export settings...** (*Ekspor pengaturan...*) menulis entri yang dikonfigurasi, penetapan grup, definisi grup,
	kuotasi berita tersembunyi, urutan entri, selektor, pilihan tampilan kuotasi bergulir/statis,
	host yang disetujui, visibilitas baris, baris, kecepatan, ukuran font bergulir/statis, ketiga
	pasangan ukuran jendela, interval penyegaran, preferensi mulai otomatis,
	opsi akses situs web, warna termasuk warna kedip peringatan, transparansi, dan
	bahasa.
- **Import settings...** (*Impor pengaturan...*) memvalidasi seluruh file sebelum mengubah apa pun. File yang ditolak
	membiarkan pengaturan saat ini tidak berubah.
- Impor yang berhasil mengganti setiap entri yang dikonfigurasi dan preferensi aplikasi. Impor tersebut
	tidak mengganti file aturan peringatan yang terpisah.
- Grup disertakan sebagai penetapan kuotasi dalam file pengaturan, bersama dengan definisi grup
	itu sendiri, sehingga grup tanpa kuotasi juga tetap tersedia dalam cadangan. Tidak ada
	file ekspor atau impor khusus grup yang terpisah.
- Preferensi mulai otomatis ada dalam cadangan pengaturan, tetapi mengimpornya tidak
	diam-diam mengubah pendaftaran mulai otomatis sistem operasi. Sistem operasi tetap menjadi sumber kebenaran;
	gunakan kotak centang Startup (*Mulai otomatis*) untuk mengubah pendaftaran pada komputer saat ini.
- File impor dibatasi hingga 1 MiB, versi skema 1, dan paling banyak 200 langganan.
	Properti yang tidak dikenal, ID duplikat, URL salah format, warna tidak valid, rentang tidak valid,
	atau kode bahasa yang tidak didukung akan ditolak, bukan diabaikan tanpa pemberitahuan.

#### Mengekspor dan mengimpor aturan peringatan- **Export alert rules...** (*Ekspor aturan peringatan...*) menulis semua aturan beserta Buzz, jumlah buzz, dan durasi kedip.
- **Import alert rules...** (*Impor aturan peringatan...*) memvalidasi seluruh file, lalu mengganti semua aturan saat ini
	dan pengaturan pemicuan peringatan.
- Aturan terlebih dahulu terhubung kembali berdasarkan ID langganan. Jika ID berbeda, SmartTicker mencoba
	pencocokan simbol tanpa membedakan huruf besar dan kecil.
- Aturan yang diimpor tanpa kuotasi yang cocok dipertahankan tetapi tidak dapat terpicu. Status impor
	melaporkan jumlah aturan yang ditautkan kembali atau tetap tidak cocok.
- File impor peringatan dibatasi hingga 1 MiB.

Untuk memindahkan ke komputer lain, impor pengaturan aplikasi terlebih dahulu dan aturan peringatan
setelahnya. Mengimpor peringatan setelah pengaturan memungkinkan aturan terhubung kembali dengan ID langganan baru
berdasarkan simbol.

### Mengedit file konfigurasi secara langsung

**Edit Current App Config** (*Edit Konfigurasi Aplikasi Saat Ini*) dan **Edit Current Alert Rules** (*Edit Aturan Peringatan Saat Ini*) di App Settings membuka
file JSON aktif di editor teks apa pun yang diasosiasikan sistem Anda dengan `.json`. Fitur ini ditujukan bagi
pengguna tingkat lanjut; jendela dalam SmartTicker mencakup pengaturan yang sama tanpa risiko tersebut.

Kedua tombol terlebih dahulu menampilkan konfirmasi yang meminta Anda mengekspor file saat ini. Lakukan
ekspor tersebut: pengeditan manual dapat merusak file dan tidak dapat dibatalkan.

- **Export existing config...** (*Ekspor konfigurasi yang ada...*) menyimpan file saat ini, lalu kembali ke permintaan yang sama.
- **Open in text editor** (*Buka di editor teks*) membuka file aktif.
- **Cancel** (*Batal*) tidak mengubah apa pun.

SmartTicker memantau file dan memuatnya kembali segera setelah editor Anda menyimpannya:

- File yang valid langsung diterapkan, dan ticker diperbarui tanpa memulai ulang.
- JSON salah format, pelanggaran skema, atau kesalahan validasi lainnya akan ditolak. Konfigurasi
	yang sedang berjalan tidak berubah dan jendela App Settings melaporkan
	masalahnya.
- Setelah edit ditolak, perbaiki file atau pulihkan ekspor yang valid dengan
	**Import settings...** (*Impor pengaturan...*) atau **Import alert rules...** (*Impor aturan peringatan...*).
- File yang tetap dikunci oleh program lain akan dicoba kembali sebentar, lalu dilaporkan.

Mengedit file aturan peringatan mengikuti aturan yang sama dan tidak memengaruhi pengaturan
aplikasi karena kedua file tersebut terpisah.

## Aturan peringatan

Buka **Alerts** (*Peringatan*) dari menu klik kanan. Aturan dievaluasi setelah setiap penyegaran
harga yang berhasil dan hanya memantau harga reguler, bukan nilai prapasar atau setelah jam perdagangan.

### Membuat aturan

1. Pilih **Quote** (*Kuotasi*) yang dikonfigurasi. Entri dengan simbol yang sama tetap berbeda.
2. Pilih **Condition** (*Kondisi*) dan masukkan ambang numerik menggunakan desimal invarian, seperti
	 `250.50`.
3. Secara opsional, pilih **Active from** (*Aktif mulai*). Biarkan kosong untuk langsung mengaktifkannya.
4. Biarkan **Never expires** (*Tidak pernah kedaluwarsa*) dicentang, atau hapus centangnya lalu pilih tanggal kedaluwarsa.
5. Pilih **Add rule** (*Tambah aturan*).

Perbandingan yang tersedia adalah:

| Pilihan | Arti |
| --- | --- |
| `LessThan` | Harga `<` ambang. |
| `LessThanOrEqual` | Harga `<=` ambang. |
| `GreaterThan` | Harga `>` ambang. |
| `GreaterThanOrEqual` | Harga `>=` ambang. |
| `EqualTo` | Harga sama persis dengan ambang. |
| `NotEqualTo` | Harga berbeda dari ambang. |

Batas mulai bersifat inklusif. Batas kedaluwarsa juga inklusif; setelah batas tersebut
terlewati, aturan tidak lagi terpicu. SmartTicker menolak tanggal kedaluwarsa yang lebih awal dari tanggal mulai.

### Saat aturan terpicu

Aturan terjadwal yang diaktifkan terpicu satu kali ketika kondisinya berubah dari salah menjadi benar.
Aturan tidak memberi notifikasi pada setiap penyegaran selama kondisinya tetap benar. Setelah harga
keluar dari kondisi, aturan dipersenjatai kembali dan dapat terpicu ketika harga memasuki kondisi itu lagi.

Mengedit aturan atau menonaktifkan lalu mengaktifkannya kembali juga mempersenjatainya kembali. Oleh karena itu, aturan yang diaktifkan
dapat langsung terpicu jika harga reguler terbaru sudah memenuhi
kondisinya. Harga yang gagal atau hilang tidak dapat memicu aturan.

Jika satu atau beberapa aturan terpicu:

- Entri harga yang terpengaruh bergantian antara warna kedip peringatan yang dikonfigurasi dan hitam selama
	durasi yang dikonfigurasi. Warna kedip bawaan adalah magenta (`#FF00FF`).
- Jika **Buzz** diaktifkan, SmartTicker memutar urutan buzz yang dikonfigurasi.
- Pesan peringatan mengidentifikasi satu aturan atau melaporkan jumlah aturan yang terpicu bersamaan.
- Ticker tetap bergulir selama sorotan peringatan aktif.

### Pengaturan keluaran peringatan

| Pengaturan | Rentang | Bawaan |
| --- | --- | --- |
| **Buzz** | Aktif atau nonaktif | Aktif |
| Jumlah buzz | 1 hingga 20 | 15 |
| **Blink for** (*Kedip selama*) | 5 hingga 900 detik, dalam kelipatan 15 detik | 60 detik |

Menonaktifkan Buzz tidak menonaktifkan peringatan visual. Jika beberapa aturan terpicu dalam evaluasi yang sama,
SmartTicker memulai satu urutan buzz yang dikonfigurasi untuk evaluasi tersebut.
Ubah **Alert blink** (*Kedip peringatan*) di bawah **App Settings > Appearance** (*Pengaturan Aplikasi > Tampilan*). Ini adalah preferensi
tampilan aplikasi, sehingga ekspor/impor Settings menyertakannya, bukan file
aturan peringatan yang terpisah.

### Mengelola aturan yang dikonfigurasi

- **Edit** (*Edit*) memuat aturan ke formulir. Pilih **Update rule** (*Perbarui aturan*) untuk menyimpan atau **Cancel** (*Batal*) untuk
	membiarkannya tidak berubah.
- **Disable** (*Nonaktifkan*) mempertahankan aturan tetapi menghentikan pencocokannya. **Enable** (*Aktifkan*) mempersenjatainya kembali dan
	mengevaluasinya terhadap harga reguler terbaru.
- **Remove** (*Hapus*) menghapus aturan.
- Daftar menampilkan status aktif, simbol, ringkasan kondisi, dan jadwal.

Perubahan aturan peringatan dan pengaturan keluaran peringatan disimpan secara otomatis.

## File lokal dan privasi

SmartTicker menyimpan konfigurasi secara lokal dan tidak menyinkronkannya ke layanan
pengembang.

Di Windows, file bawaannya adalah:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

Di Linux, .NET menggunakan direktori data aplikasi lokal milik pengguna saat ini, biasanya:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Menggunakan direktori data terisolasi

Diagnostik tingkat lanjut dan proses pengujian dapat menetapkan `SMARTTICKER_DATA_DIRECTORY` sebelum menjalankan
SmartTicker. Jika nilainya tidak kosong, kedua file ditempatkan langsung dalam direktori hasil resolusi tersebut
sebagai `settings.json` dan `alerts.json`; lokasi bawaan platform di atas tidak digunakan
untuk proses tersebut. Utamakan jalur absolut dan pastikan direktori dapat ditulisi.

Contoh PowerShell:

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Contoh shell Linux:

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Tetapkan variabel sebelum proses dimulai. SmartTicker tidak menyalin profil bawaan
ke direktori yang dipilih, sehingga direktori kosong dimulai dengan konfigurasi kosong.
Instans yang diarahkan ke direktori yang sama dapat mengamati edit tersimpan satu sama lain. Gunakan
perintah ekspor/impor Settings dan Alert Rules biasa untuk pencadangan dan pemindahan profil.

Jendela Alerts menampilkan jalur persis file peringatan yang sedang digunakan. Penulisan menggunakan file
sementara yang kemudian diganti, sehingga file yang hanya tertulis sebagian tidak dianggap sebagai
konfigurasi saat ini.

SmartTicker tidak memiliki akun, telemetri, analitik, iklan, atau sinkronisasi cloud. Situs web
sumber menerima informasi jaringan normal, seperti alamat IP Anda, ketika SmartTicker
meminta sumber tersebut. Membuka Bantuan meminta panduan mentah dari GitHub. Untuk detail
lengkap, baca `PRIVACY.md` di repositori.

Anda bertanggung jawab memastikan bahwa setiap URL sumber dan selektor digunakan sesuai dengan
ketentuan, lisensi, arahan robots, dan hukum yang berlaku pada situs web tersebut.

## Pemecahan masalah

### Kuotasi menampilkan tidak tersedia atau tanpa harga

Permintaan sumber kehabisan waktu setelah 20 detik. Jika kuotasi tersebut memiliki snapshot berhasil sebelumnya,
penyegaran yang gagal mempertahankannya agar tetap terlihat; jika tidak, kuotasi menampilkan **Unavailable** (*Tidak tersedia*)
sampai penyegaran berikutnya berhasil. Baca kesalahan validasi atau penyegaran sebelum mengubah
selektor.

1. Buka **Quotes...** (*Kuotasi...*), edit entri, lalu periksa Full URL (*URL Lengkap*).
2. Pastikan **Price** (*Harga*) dipilih.
3. Setujui situs web jika diminta.
4. Pilih **Validate URL** (*Validasi URL*) dan baca hasil persisnya.
5. Jalankan **Discover price** (*Temukan harga*), atau periksa HTML statis halaman dan perbarui selektor.
6. Periksa apakah halaman memerlukan JavaScript, autentikasi, atau persetujuan yang
	 tidak dapat ditangani SmartTicker dengan aman.
7. Patuhi HTTP 403, 429, pembatasan robots, dan kebijakan akses otomatis situs tersebut.

### Data prapasar atau setelah jam perdagangan tidak tersedia

- Sesi pasar yang sesuai mungkin sedang tidak aktif.
- Halaman mungkin menghilangkan elemen sesi jika tidak ada nilai sesi.
- Pastikan selektor prapasar menargetkan elemen prapasar dan selektor setelah jam perdagangan
	menargetkan elemen pascapasar.
- Jalankan kembali perintah penemuan yang sesuai karena markup situs web mungkin telah berubah.

### Berita kosong

- Pastikan **News** (*Berita*) dipilih.
- Validasi sumber dan jalankan **Discover news** (*Temukan berita*).
- Pastikan selektor mengembalikan tautan dengan teks judul berita yang terlihat.
- Permintaan News yang gagal atau kehabisan waktu mempertahankan judul berita lama yang sebelumnya berhasil jika tersedia.
	Sumber tanpa hasil berhasil tetap kosong sampai slot berikutnya berhasil.
- Judul berita menghilang setelah mencapai batas pengulangan yang dikonfigurasi untuk sesi ini.
- Dalam News statis, pastikan kuotasi yang dimaksud dicentang di bawah **Show news for** (*Tampilkan berita untuk*).

### Penemuan selektor tidak menemukan apa pun

Discovery hanya membaca HTML statis yang diunduh. Fitur ini tidak dapat melihat nilai yang dibuat kemudian oleh
JavaScript halaman. Masukkan selektor terverifikasi secara manual, pilih halaman/umpan statis, atau gunakan
API resmi yang terdokumentasi melalui halaman publik yang kompatibel.

### Peringatan tidak terpicu

- Pastikan kuotasi yang terpasang masih ada, mengumpulkan Price, dan memiliki harga reguler yang
	berhasil.
- Pastikan aturan Enabled (*Diaktifkan*) dan berada dalam jadwal mulai/kedaluwarsanya.
- Periksa perbandingan dan ambang. `EqualTo` memerlukan kesamaan desimal yang persis.
- Ingat bahwa kondisi yang terus-menerus benar hanya terpicu sekali; kondisi harus menjadi salah sebelum
	dapat terpicu lagi, kecuali Anda mengedit atau mengaktifkan kembali aturan tersebut.
- Harga prapasar dan setelah jam perdagangan tidak menggerakkan aturan peringatan.

### SmartTicker tidak dapat dipindahkan atau diubah ukurannya

- Pindahkan hanya dari pegangan bertitik vertikal di bidang kiri.
- Ubah ukuran dari tepi atau sudut; gunakan tanda kanan bawah yang terlihat jika tepi sulit
	ditemukan.
- Konten ticker memang sengaja bukan permukaan pemindah.

### Grup atau nilai statis tidak seperti yang diharapkan

- Buka **Quotes...** (*Kuotasi...*) dan konfirmasikan nilai Group setiap entri.
- Buka **Quote groups...** (*Grup kuotasi...*) untuk mengelola definisi grup dan meninjau kaitan
	setiap kuotasi saat ini.
- Entri dengan Group kosong muncul di bawah **Ungrouped** (*Tanpa grup*).
- **Chg** dihitung dari Last dan Chg%; nilai ini tidak diekstrak secara independen dari
	halaman. Nilai tetap `—` jika persentase tidak tersedia.
- Urutkan ulang entri dengan kontrol naik/turun untuk mengubah urutan grup dan baris.
- Seret pegangan bertitik pada judul ubin untuk memindahkan seluruh grup. Jatuhkan pada bagian kiri
  ubin lain untuk menempatkannya sebelum ubin tersebut, atau bagian kanan untuk menempatkannya setelah ubin tersebut.
- Pilih **Refresh prices now** (*Segarkan harga sekarang*) saat SmartTicker tidak dijeda untuk memperbarui tabel.

### Teks Bantuan tidak diformat atau navigasi tidak bergerak

- Jendela Bantuan seharusnya menampilkan judul, paragraf, daftar, tabel, tautan,
	dan blok kode yang diformat, bukan tanda baca Markdown.
- Gunakan **On this page** (*Di halaman ini*) di sebelah kiri untuk melompat ke bagian utama. Tautan dalam tabel Navigasi
	cepat juga menggulir di dalam dokumen.
- Tutup dan buka kembali Bantuan, atau ubah **Language** (*Bahasa*), untuk meminta panduan
	terbitan yang sesuai. Sambil menunggunya, SmartTicker menampilkan panduan terformat yang
	disematkan dalam aplikasi terpasang.

### Bantuan online tidak tersedia atau kedaluwarsa

- Tutup dan buka kembali Bantuan untuk meminta lagi panduan yang dipublikasikan.
- Buka alamat GitHub mentah yang ditampilkan di dekat awal panduan ini dalam browser untuk
	memeriksa file yang dipublikasikan secara langsung.
- SmartTicker menggunakan panduan yang disematkan ketika permintaan gagal atau mengembalikan file kosong.
- Perubahan online hanya muncul setelah `HELPME.md` atau file
  `help/HELPME.<language-code>.md` yang sesuai dipublikasikan pada cabang `main` repositori.

## Dukungan

Laporkan masalah yang dapat direproduksi di:

<https://github.com/bulentozkir/smartticker/issues>

Sertakan versi SmartTicker, sistem operasi, nama host sumber, status validasi,
dan teks kesalahan yang persis. Hapus URL privat atau informasi sensitif lainnya sebelum memposting.