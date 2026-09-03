# SmartTicker Yardımı

Bu kılavuz SmartTicker 1.0.3 için geçerlidir. Ana bilgi şeridini, Uygulama Ayarlarını,
kotasyonları, uyarı kurallarını, web sitesi izinlerini, yedeklemeleri ve yaygın sorunları açıklar.

SmartTicker, yapılandırdığınız web sayfalarındaki herkese açık statik HTML'yi okur. Bir
piyasa verisi akışı sağlamaz ve ayıklanan bilgiler gecikmiş, eksik veya hatalı olabilir.
Önemli finansal bilgileri yetkili bir kaynakla doğrulayın.

## Hızlı gezinme

| Alan | Git |
| --- | --- |
| Başlarken | [Yardım ve yapılandırma pencerelerini açma](#yardım-ve-yapılandırma-pencerelerini-açma) |
| Ana bilgi şeridi | [Denetimler](#ana-bilgi-şeridi-denetimleri) · [Kayan veya statik görünüm](#kayan-veya-statik-kotasyon-görünümünü-seçme) · [Taşıma](#bilgi-şeridini-taşıma) · [Yeniden boyutlandırma](#bilgi-şeridini-yeniden-boyutlandırma) · [Duraklatma](#duraklatma-ve-sürdürme) · [Menü başvurusu](#ana-menü-başvurusu) |
| Kotasyonlar ve haberler | [Kotasyonlar](#kotasyonlar) · [Girdi ekleme](#kotasyon-veya-haber-girdisi-ekleme) · [Kotasyonları gruplama](#kotasyonları-gruplama) · [Kaynak URL'leri](#kaynak-önayarları-ve-urller) · [Seçiciler](#seçici-alanları-başvurusu) · [Keşif](#seçicileri-keşfetme) · [Doğrulama](#bir-kaynağı-doğrulama) |
| Uygulama tercihleri | [Uygulama Ayarları](#uygulama-ayarları) · [Satırlar ve hız](#bilgi-şeridi-satırları-ve-hızı) · [Başlangıç](#oturum-açarken-smarttickerı-başlatma) · [Web sitesi erişimi](#web-sitesi-erişimi) · [Görünüm](#görünüm) · [Yedekleme ve geri yükleme](#yedekleme-ve-geri-yükleme) · [Yapılandırma dosyalarını düzenleme](#yapılandırma-dosyalarını-yerinde-düzenleme) |
| Fiyat uyarıları | [Uyarı kuralları](#uyarı-kuralları) · [Kural oluşturma](#kural-oluşturma) · [Tetiklenme davranışı](#bir-kural-tetiklendiğinde) · [Uyarı çıktısı](#uyarı-çıktısı-ayarları) · [Kuralları yönetme](#yapılandırılmış-kuralları-yönetme) |
| Veriler ve destek | [Yerel dosyalar ve gizlilik](#yerel-dosyalar-ve-gizlilik) · [Sorun giderme](#sorun-giderme) · [Destek](#destek) |

## Yardım ve yapılandırma pencerelerini açma

Menüsünü açmak için bilgi şeridine sağ tıklayın. Ana yapılandırma komutları şunlardır:

- **Kotasyonlar... (Quotes...)**: kotasyon veya haber kaynakları ekleyin, test edin, düzenleyin, sıralayın ve kaldırın.
- **Kotasyon grupları... (Quote groups...)**: gruplar oluşturun, güncelleyin veya silin ve kotasyonları bunlarla ilişkilendirin.
- **Uyarılar (Alerts)**: fiyat uyarısı kuralları oluşturun ve yönetin.
- **Uygulama Ayarları... (App Settings...)**: satırları, hızları, yenileme aralıklarını, başlangıcı, web sitesi
	erişimini, renkleri, saydamlığı ve yedeklemeleri yapılandırın.
- **Görünüm (View)**: birbirini dışlayan dört birleşimden birini seçin: kayan veya statik,
	yalnızca Fiyatlar ya da Haberlerle birlikte Fiyatlar.
- **Yardım (Help)**: bu kılavuzu SmartTicker içinde açın.
- **SmartTicker Hakkında (About SmartTicker)**: yüklü sürümü ve lisans bildirimini gösterin.
- **Çıkış (Exit)**: SmartTicker uygulamasını tamamen kapatın.

Yardım penceresi, seçilen uygulama diline ait gömülü kılavuzu hemen biçimlendirip görüntüler;
ardından Yardım'ı her açtığınızda veya **Dil (Language)** ayarını değiştirdiğinizde eşleşen
çevrimiçi kılavuzu denetler. Türkçe çevrimiçi kılavuz:

<https://raw.githubusercontent.com/bulentozkir/smartticker/refs/heads/main/help/HELPME.tr.md>

Çevrimiçi belge indirilemezse SmartTicker, seçilen dille eşleşen gömülü çeviriyi göstermeyi
sürdürür. **Dil (Language)** değiştirildiğinde açık Yardım penceresinin başlığı, durumu, gezinme
bölümü ve tam kılavuzu hemen güncellenir. Yardım'ı normal başlık çubuğu kapatma denetimiyle kapatın.

## Ana bilgi şeridi denetimleri

### Kayan veya statik kotasyon görünümünü seçme

SmartTicker birbirini dışlayan dört görüntüleme modu sunar. Bilgi şeridine sağ tıklayın,
**Görünüm'ü (View)** açın ve birini seçin. Düzen hemen değişir ve seçiminiz kaydedilir.

| Görünüm seçeneği | Sonuç |
| --- | --- |
| **Soldan sağa kaydırma: Yalnızca Fiyatlar (Left-to-right scroll: Prices only)** | Ana bilgi şeridinde kayan fiyat bandı; haber gösterilmez. Bu varsayılandır. |
| **Soldan sağa kaydırma: Haberlerle birlikte Fiyatlar (Left-to-right scroll: Prices with News)** | Ana bilgi şeridinde kayan fiyat ve haber bantları. |
| **Statik görünüm: Yalnızca Fiyatlar (Static view: Prices only)** | Ana pencerede duyarlı fiyat kutucukları; Haberler penceresi yoktur. |
| **Statik görünüm: Haberlerle birlikte Fiyatlar (Static view: Prices with News)** | Duyarlı fiyat kutucuklarına ek olarak ayrı bir statik **SmartTicker News** penceresi. |

Bu seçenekler eklenmeden önce oluşturulan ayar dosyaları, kaydedilmiş kayan/statik ve
haber ayarlarının karşılık gelen birleşimine eşlenir. Görüntüleme modu yalnızca
bilgi şeridinin sağ tıklama **Görünüm (View)** menüsünden yönetilir.

- Her iki kayan modda da fiyatlar yatay kayan bandı ve yapılandırılmış fiyat satırı
	sayısı ile kaydırma hızını kullanır.
- Her iki statik modda da gruplar soldan sağa yerleştirilen duyarlı kutucuklar olarak görünür. Kutucuklar
  yalnızca pencere çok dar olduğunda başka bir satıra kayar. Fiyatlar otomatik olarak
  hareket etmez.
- Her kotasyon kutucuğunun kendi hizalı **Sembol (Symbol)**, **Son (Last)**, **Değ. (Chg)** ve **Değ.% (Chg%)**
  sütunları vardır. **Değ. (Chg)** değeri Son ve Değ.% değerlerinden türetilir,
	çünkü kaynak sayfalar ayrı bir mutlak değişim seçicisi yerine yüzde seçicisi sağlar.
	Değerlerden biri kullanılamıyorsa `—` gösterir.
- Daraltmak veya genişletmek için bir grup başlığını seçin. Gruplar, kotasyonlarının
	yapılandırılmış girdi sırasındaki ilk görünümünü izler; grup içindeki satırlar bu sırayı korur.
- Grubu olmayan girdiler **Gruplanmamış (Ungrouped)** altında görünür.
- Kullanılabilir piyasa öncesi ve mesai sonrası değerleri görmek için Son'un üzerine gelin. Kaynak
	sayfasını açmak için bir kotasyon satırına çift tıklayın.
- Uyarı yanıp sönmesi ve yükseliş/düşüş renkleri her iki fiyat modunda da çalışır.
- Haberler, statik **Sembol / Başlık (Symbol / Headline)** grup kutucuklarını içeren ayrı bir **SmartTicker News**
	penceresinde otomatik olarak açılır. Statik modda kayan bant kullanmaz. Haberler
	penceresinin normal başlık çubuğu ve yeniden boyutlandırma kenarlığı vardır; böylece Kotasyonlar ve Haberler pencereleri
	farklı monitörlere birbirinden bağımsız taşınabilir. Kaynağını açmak için bir başlık satırına çift tıklayın.
- Haberler ilk açılışta kompakt 680×340 boyutunu kullanır. SmartTicker, kullanılabilir başka bir
	monitör varsa pencereyi oraya yerleştirir; tek monitörde önce Fiyatlar'ın altında,
	sağında, üstünde veya solunda boş bir alan dener. Ardından normal şekilde taşıyıp yeniden boyutlandırabilirsiniz.
- Her Haber grubunda başlıklar kotasyona göre dönüşümlü yerleştirilir: ilk kotasyondan bir başlık,
	sonra bir sonraki kotasyondan bir başlık alınarak turlar hâlinde devam edilir. Bu nedenle çok sayıda
	başlığı olan bir kotasyon grubunun üst kısmının tamamını kaplayamaz.
- Tek satırlı **Şunlar için haberleri göster (Show news for)** açılır listesini açın ve her kotasyonu
	bağımsız olarak işaretleyin veya temizleyin. Tümü veya hiçbiri dâhil her türlü kotasyon birleşimi görünür olabilir. Düğme
	geçerli seçimi özetler; girdiler kotasyonu ve kaynağı içerdiğinden
	yinelenen semboller bağımsız kalır. Temizlenen kotasyonlar ayar dosyanıza
	`hiddenNewsQuotes` olarak kaydedilir; böylece yeniden başlatmadan sonra korunur ve ayar yedeğiyle taşınır.
- Herhangi bir kotasyon veya haber kutucuğu başlığının yanındaki noktalı tutamacı sürükleyin ve başka bir kutucuğun sol
	veya sağ yarısına bırakın. Sıra her iki pencerede de değişir ve
	temeldeki yapılandırılmış girdiler yeniden sıralanarak kaydedilir.
- Çok sayıda satırı olan bir grup kendi sınırlı kutucuğunun içinde kayar. Genel görünüm yalnızca
	sarmalanan kutucuk satırları geçerli pencere yüksekliğine sığmadığında dikey olarak kayar.

**SmartTicker News** penceresini kapatmak haber toplamayı devre dışı bırakmaz. Yeniden açmak için
Fiyatlar penceresine sağ tıklayın ve **Görünüm > Statik haber penceresini aç (View > Open static news window)** seçeneğini belirleyin. **Statik
görünüm: Yalnızca Fiyatlar (Static view: Prices only)** seçildiğinde kapanır; **Statik görünüm: Haberlerle birlikte Fiyatlar (Static view: Prices with News)** seçildiğinde
yeniden açılır. Kayan seçeneklerden biri ayrı Haberler penceresini kapatır; kayan
Haberlerle birlikte Fiyatlar seçeneği ana bilgi şeridindeki haber bandını geri getirir.

Modlar arasında geçiş yapıldığında o görünüm için kaydedilmiş boyut uygulanır. Kayan bilgi şeridi, statik Fiyatlar
penceresi ve statik Haberler penceresinin her biri bağımsız genişlik ve yüksekliğini korur.

### Bilgi şeridini taşıma

Dar sol şeridin üstündeki dikey noktalı tutamacı basılı tutun, bilgi şeridini
sürükleyin ve fare düğmesini bırakın. Bilgi şeridi metni bir sürükleme yüzeyi değildir; böylece içerik seçmek
veya içeriğe tıklamak yanlışlıkla pencereyi taşımayı başlatamaz.

### Bilgi şeridini yeniden boyutlandırma

Yeniden boyutlandırma imleci görünene kadar işaretçiyi herhangi bir kenar veya köşeye götürün, ardından basıp
sürükleyin. Sağ alt köşede küçük, görünür bir yeniden boyutlandırma işareti vardır. En düşük pencere genişliği
420 pikseldir. Kayan görünüm yüksekliği 50 ile 900 piksel, statik Fiyatlar yüksekliği 420
ile 4320 piksel ve statik Haberler yüksekliği 240 ile 4320 piksel arasındadır.

Elle yeniden boyutlandırma, sürükleme durulduktan sonra etkin görünümün kaydedilmiş boyutlarını günceller.
Üç boyut çiftinin tümü bir ayar yedeğine dâhil edilir. Pencere konumları saklanmaz.
Kayan görünüm boyutu seçili Fiyat/Haber satırları ve kayan yazı tipi
boyutu için çok kısaysa SmartTicker kaydedilmiş yüksekliği otomatik olarak artırır. Bu nedenle **Soldan sağa
kaydırma: Haberlerle birlikte Fiyatlar (Left-to-right scroll: Prices with News)** seçildiğinde Haber satırları sessizce
gizlenmek yerine her zaman bunlara yer açılır.
Bir pencere her açıldığında veya taşındığında SmartTicker, en az sol üstteki 32 piksellik köşesini
bir ekranın çalışma alanı içinde tutar ve genel X ile Y değerlerini en az 1 olacak şekilde sınırlar. Bu, bir monitörün
bağlantısı kesildikten sonra bile taşıma tutamacının veya başlık köşesinin fareyle erişilebilir kalmasını sağlar.

### Duraklatma ve sürdürme

Taşıma tutamacının altındaki durum düğmesini seçin veya sağ tıklayıp
**Duraklat / Sürdür (Pause / Resume)** seçeneğini belirleyin. Duraklatma otomatik fiyat ve haber yenilemelerini durdurur ve
kayan bandı dondurur. Ayrıca iki el ile yenileme komutunun da yeni iş başlatmasını önler. Yalnızca Duraklat nedeniyle,
hâlihazırda sürmekte olan bir kaynak isteği zorla iptal edilmez ve etkinlik tamamen
durulmadan önce tamamlanabilir. Sürdürme otomatik zamanlayıcıları yeniden başlatır.

Windows'ta SmartTicker, kullanıcı arayüzünü başlatmadan önce işletim sistemi işlem önceliğini otomatik olarak **Düşük (Low)** düzeyine ayarlar ve
Windows **Verimlilik modunu (Efficiency mode)** (EcoQoS) etkinleştirir. Bu davranış için uygulama ayarı yoktur.
Ayrıca düşük ek yüklü bir yazılım işleme yolu kullanır. Kayan bant zamanlaması
yapılandırılmış hıza uyarlanır ve duraklatılmış, boş veya ayrılmış bir kayan bant animasyon
zamanlayıcısını durdurur. Değişmeyen satırlar gereksiz görsel bildirimleri engeller. Uyarının yanıp sönmesi ve
üç saniyelik kahverengi değişiklik vurgusu kasıtlıdır ve kaymayı duraklatmaz. Linux
işlem zamanlaması işletim sistemine bırakılır. Windows işlem ayarlarından birini
reddederse SmartTicker hatayı tanılama izlemesine bildirir ve başlatmaya devam eder.

### Bağlantıları açma

Bir haber başlığı dâhil bağlantılı bilgi şeridi metnine çift tıklayarak kaynağını varsayılan
tarayıcınızda açın. SmartTicker bağlantıları tek tıklamayla açmaz.

### Değişiklik vurguları

SmartTicker her yenilemeden sonra neyin değiştiğini üç saniyeliğine kahverengi bir arka planla kısaca işaretler:

- Fiyatı önceki eşitlemeden farklı olan bir kotasyon.
- İlgili kotasyon için önceki eşitlemede bulunmayan her başlık.

Başlangıçtan sonraki ilk eşitlemede karşılaştırılacak önceki bir değer olmadığından hiçbir şey vurgulanmaz.
Tetiklenmiş bir uyarı kendi uyarı yanıp sönme rengini korur ve önceliklidir.

### Ana menü başvurusu

| Komut | Etki |
| --- | --- |
| **Fiyatları şimdi yenile (Refresh prices now)** | SmartTicker duraklatılmamışken kademeli fiyat döngüsünü yeniden başlatır ve ilk zaman dilimini ister. |
| **Haberleri şimdi yenile (Refresh news now)** | SmartTicker duraklatılmamışken kademeli Haber döngüsünü yeniden başlatır ve ilk zaman dilimini ister. |
| **Duraklat / Sürdür (Pause / Resume)** | Yenilemeyi ve kayan bant hareketini açıp kapatır. |
| **Görünüm > Soldan sağa kaydırma: Yalnızca Fiyatlar (View > Left-to-right scroll: Prices only)** | Yalnızca yatay fiyat bandını kullanır. Bu varsayılandır. |
| **Görünüm > Soldan sağa kaydırma: Haberlerle birlikte Fiyatlar (View > Left-to-right scroll: Prices with News)** | Her iki yatay kayan bandı kullanır. |
| **Görünüm > Statik görünüm: Yalnızca Fiyatlar (View > Static view: Prices only)** | Yalnızca duyarlı statik kotasyon kutucuklarını kullanır. |
| **Görünüm > Statik görünüm: Haberlerle birlikte Fiyatlar (View > Static view: Prices with News)** | Kotasyon kutucuklarını ve ayrı statik Haberler penceresini kullanır. |
| **Görünüm > Statik haber penceresini aç (View > Open static news window)** | Kapatıldıktan sonra ayrı Haberler penceresini yeniden açar. Haberler etkin olduğunda statik modda kullanılabilir. |
| **Dil (Language)** | Menüler, durum metni ve tam Yardım kılavuzu için desteklenen 16 dilden birini seçer. Açık Yardım penceresi hemen güncellenir. |

Satır görünürlüğü, dil ve diğer yapılandırma değerleri otomatik olarak kaydedilir.

## Kotasyonlar

Sağ tıklama menüsünden **Kotasyonlar... (Quotes...)** seçeneğini açın. Yapılandırılmış her girdi bir
sembolü ve bir web sayfasını temsil eder. Yinelenen sembollere izin verilir ve her girdinin
kendi kaynağı, seçicileri, toplama seçenekleri ve uyarıları olduğundan bağımsız kalırlar.

### Yayımlanan örnekle hızlı başlangıç

Hiç girdi yoksa Kotasyonlar penceresi **GitHub'dan örnek kotasyonları içe aktar (Import sample quotes from GitHub)** seçeneğini sunar.
Bu işlem depo örneğini indirir ve geçerli uygulama ayarlarının yerine koyar.
Kullanmadan önce içe aktarılan her URL'yi ve her web sitesinin geçerli koşullarını inceleyin. Daha sonra
herhangi bir örnek girdiyi düzenleyebilir veya kaldırabilirsiniz.

Hem Kotasyonlar hem de Uygulama Ayarları pencerelerinin üstündeki **Örnek Kotasyon Yapılandırmasını İçe Aktar (Import Sample Quotes Config)**
seçeneği, bir onayın ardından herhangi bir zamanda aynı işlemi yapar:

- SmartTicker **Emin misiniz? (Are you sure?)** diye sorar ve indirmenin mevcut
	kotasyonlarınızı, kotasyon gruplarınızı, kaynak onaylarınızı, görünümünüzü, görünüşünüzü ve diğer uygulama ayarlarınızı değiştireceği konusunda uyarır.
	Uyarı kuralları kendi dosyalarında bulunur ve silinmez.
- **Mevcut yapılandırmayı dışa aktar... (Export existing config...)** isteğe bağlıdır. Geçerli yapılandırmanızı
	yerel bir JSON dosyasına kaydeder ve ardından aynı onaya döner.
- **Örnek Kotasyon Yapılandırmasını İçe Aktar (Import Sample Quotes Config)** örneği internetten indirir ve
	yapılandırmanızın yerine koyar.
- **İptal (Cancel)** hiçbir şeyi değiştirmez.

### Kotasyon veya haber girdisi ekleme

1. `MSFT` gibi bir **Sembol (Ticker)** etiketi girin. SmartTicker baştaki ve sondaki boşlukları kaldırır ve değeri
	 büyük harfle saklar.
2. İsteğe bağlı olarak aramadan mevcut bir **Grup (Group)** seçin veya
	 `Nasdaq`, `Precious Metals` ya da `Mag 7` gibi yeni bir ad yazın. **Gruplanmamış (Ungrouped)** için boş bırakın.
3. Bir **Kaynak (Source)** önayarı seçin.
4. **URL son ekini (URL suffix)** veya **Özel URL (Custom URL)** kullanırken tam bir URL'yi girin.
5. **Topla (Collect)** altında **Fiyat (Price)**, **Haberler (News)** veya her ikisini seçin. En az biri gereklidir.
6. Seçicileri elle girin, keşif düğmelerini kullanın veya yerleşik algılamayı kullanmak için isteğe bağlı seçicileri
	 boş bırakın.
7. Normal fiyatı ve/veya başlıkları sınamak için **URL'yi Doğrula (Validate URL)** seçeneğini belirleyin.
8. SmartTicker kaynak onayı isterse web sitesini inceleyin ve yalnızca
	 veri toplama izniniz olduğunda onaylayın.
9. **Bağımsız girdi ekle (Add independent entry)** seçeneğini belirleyin. SmartTicker girdiyi kaydeder ve
	 etkin verilerini hemen yeniler.

### Kotasyonları gruplama

Grup, sizin tanımladığınız adlandırılmış bir koleksiyondur. Bir borsaya veya yerleşik
kategoriye bağlı değildir; bu nedenle girdileri piyasaya, varlık türüne, stratejiye, portföye,
bölgeye veya başka herhangi bir düzene göre düzenleyebilirsiniz. Adların başındaki ve sonundaki boşluklar kaldırılır, Unicode kullanabilir ve en fazla
80 karakter içerebilir. Her kotasyon en fazla bir gruba ait olabilir.

Grup alanının yanındaki **Grupları yönet (Manage groups)** seçeneğini kullanın veya bilgi şeridinin
sağ tıklama menüsünden **Kotasyon grupları... (Quote groups...)** seçeneğini belirleyin. Pencerede üç çalışma alanı vardır:

- Solda bir **Grup adı (Group name)** girin, ardından **Oluştur'u (Create)** seçin. Mevcut bir grubu seçin,
	adını düzenleyip **Güncelle'yi (Update)** seçin veya **Sil'i (Delete)** seçin. Boş gruplar korunur.
- Sağda bir kotasyon seçin. Geçerli grubu **Geçerli grup (Current group)**
	sütununda gösterilir; **Gruplanmamış (Ungrouped)** herhangi bir ilişkisinin olmadığı anlamına gelir.
- Ortada bir grup ve bir kotasyon seçtikten sonra **İlişkilendir'i (Associate)** seçin. Bu
	kotasyon zaten başka bir gruba aitse SmartTicker onu seçilen gruba taşır.
- Yalnızca seçili kotasyonu **Gruplanmamış'a (Ungrouped)** döndürmek için **İlişkilendirmeyi kaldır'ı (Remove association)** seçin.
- Bir grubun silinmesi tüm kotasyonlarını **Gruplanmamış'a (Ungrouped)** döndürür. Kotasyonlar, kaynaklar, geçerli
	veriler ve uyarılar silinmez.
- Bir kotasyon eklerken veya düzenlerken aramadan mevcut bir grubu da seçebilir
	ya da buraya yeni bir grup adı yazabilirsiniz.
- Statik tablodaki grup ve satır sırasını belirlemek için Yapılandırılmış girdiler bölümündeki yukarı/aşağı denetimlerini kullanın.
- Statik modda tüm grupları doğrudan yeniden sıralamak için bir kutucuk başlığını sürükleyin. Ayrı
	Kotasyonlar ve Haberler pencereleri aynı sırayı kullanır.

Yayımlanan örnek, statik modu varsayılan olarak kapalı bırakırken altı örnek grup içerir.
Bu grupları tablo olarak görmek için içe aktardıktan sonra statik görünümü etkinleştirin.

### Kaynak önayarları ve URL'ler

| Kaynak | Girilecek değer | SmartTicker tarafından gösterilen politika |
| --- | --- | --- |
| **Yahoo Finance** | `https://finance.yahoo.com/` sonrasındaki bir son ek; örneğin `quote/MSFT/`. | Yazılı izin gereklidir. Yahoo'nun koşulları önceden izin alınmadan otomatik veri toplamayı yasaklar. |
| **CNBC** | `https://www.cnbc.com/` sonrasındaki bir son ek. | Sitenin geçerli politikasını ve robots yönergelerini denetleyin. |
| **Trading Economics** | `https://tradingeconomics.com/` sonrasındaki bir son ek. | Belgelenmiş bir API'yi veya yetkili veri akışını tercih edin ve sitenin geçerli politikasını denetleyin. |
| **Özel URL (Custom URL)** | Tam ve herkese açık bir `http://` veya `https://` sayfa URL'si. | Sitenin koşullarını, gizlilik politikasını ve otomatik erişim kurallarını inceleyin. |

Yalnızca mutlak HTTP ve HTTPS URL'leri kabul edilir. Gömülü kullanıcı adı veya
parola içeren URL'ler reddedilir. Tarayıcı oturumu açmak SmartTicker uygulamasına bir
sayfadan veri toplama yetkisi vermez ve SmartTicker kimliği doğrulanmış tarayıcı oturumlarını kullanmaz.

**Tam URL (Full URL)** satırı, önayar önekiyle son ekinizden üretilen son adresi gösterir.
Doğrulama veya keşiften önce bunu denetleyin.

### Toplama seçenekleri

- **Fiyat (Price)** normal fiyatı ister. İsteğe bağlı değişim, piyasa öncesi ve mesai sonrası
	seçicileri aynı indirilen sayfadan değerlendirilir.
- **Haberler (News)** sayfadaki başlık bağlantılarını ister.
- Her ikisini seçmek, tek bir girdinin bilgi şeridinin iki alanına da katkıda bulunmasını sağlar.
- İkisini de temizlemek geçersizdir.

### Seçici alanları başvurusu

CSS seçicisi, bir web sayfasının statik HTML'sindeki bir öğeyi tanımlar. Seçiciler,
otomatik algılama ihtiyacınız olan değeri bulamadığı sürece isteğe bağlıdır.

| Alan | SmartTicker tarafından ayıklanan değer |
| --- | --- |
| **Fiyat seçicisi (Price selector)** | Normal veya kapanış fiyatı. |
| **Fiyat değişimi (Price change)** | Normal seans yüzde değişimi. Boş olduğunda yerleşik değişim algılama denenir. |
| **Piyasa öncesi seçicisi (Pre-market selector)** | Sayfada ilgili seans bulunduğunda piyasa öncesi fiyatı. |
| **Piyasa öncesi değişimi (Pre-market change)** | Piyasa öncesi yüzde değişimi. |
| **Mesai sonrası seçicisi (After-hours selector)** | Piyasa sonrası veya mesai sonrası fiyatı. |
| **Mesai sonrası değişimi (After-hours change)** | Piyasa sonrası veya mesai sonrası yüzde değişimi. |
| **Haber seçicisi (News selector)** | Başlık bağlantıları. Bir bağlantı öğesi veya sonuçlarında bağlantı bulunan bir kapsayıcı seçin. |

Piyasa öncesi ve mesai sonrası değerleri normal fiyatı tamamlar; onun yerine
geçmez. Bir sayfa, ilgili piyasa seansı dışında bu öğeleri içermeyebilir.

Yayımlanan örnekte kullanılan Yahoo Finance seçicileri şöyledir:

```text
Price:                  [data-testid="qsp-price"]
Price change:           section.primary span[data-testid="qsp-price-change-percent"]
Pre-market price:       section.secondary span[data-testid="qsp-pre-price"]
Pre-market change:      section.secondary span[data-testid="qsp-pre-price-change-percent"]
After-hours price:      section.secondary span[data-testid="qsp-post-price"]
After-hours change:     section.secondary span[data-testid="qsp-post-price-change-percent"]
```

Web sitesi işaretlemesi zamanla değişir. Örnekleri kalıcı sözleşmeler olarak değil,
başlangıç noktaları olarak değerlendirin.

### Seçicileri keşfetme

Her seçici alanının eşleşen bir **Keşfet (Discover)** düğmesi vardır.

1. Kaynak URL'sini tamamlayın ve onay gerekiyorsa web sitesini onaylayın.
2. Tam değer türüne ait keşif düğmesini seçin.
3. SmartTicker herkese açık statik HTML'yi indirir ve olası seçicileri örnek bir
	 değer, güven yüzdesi ve araç ipucunda bir gerekçeyle listeler.
4. Bir önerinin yanındaki **Kullan'ı (Use)** seçerek onu eşleşen alana kopyalayın.
5. Sonuca güvenmeden önce doğrulayın veya sonucu gözlemleyin.

Keşif JavaScript çalıştırmaz, oturum açmaz, erişim denetimlerini atlamaz veya
tarayıcınızı incelemez. Yalnızca JavaScript ile oluşturulan bir değerin keşfedilebilir seçicisi olmayabilir. Ayrı keşif
türleri, piyasa öncesi ve mesai sonrası değerlerinin karıştırılmasını özellikle önler.

### Bir kaynağı doğrulama

**URL'yi Doğrula (Validate URL)** sayfayı ister ve okuyabildiği normal fiyatı ve/veya
başlık sayısını bildirir. SmartTicker test için geçici bir etiket kullandığından
bir sembol girmeden önce güvenle kullanılabilir.

Bu doğrulama şu anda dört piyasa öncesi ve mesai sonrası seçici
alanını denetlemez. Bunların keşif örnek değerlerini kullanın ve ardından görüntülenen seans verilerini doğrulayın.

Yaygın hatalar arasında HTTP hatası, zaman aşımı, eksik değer, sıfır başlık, onaylanmamış kaynak
izni, yalnızca JavaScript ile oluşturulan içerik veya eski bir seçici bulunur.

### Haber tekrar sınırı

**En fazla _N_ kez göster (Show max _N_ times)** 1 ile 100 arasındaki değerleri kabul eder ve varsayılanı 5'tir. SmartTicker, aynı başlık metninin
döndürüldüğü tamamlanmış her Haber yenileme döngüsünde bir gösterim sayar. Başlık yapılandırılmış
döngü sayısında göründüğünde geçerli uygulama oturumunun geri kalanı için kullanımdan kaldırılır.
Bu girdiyi düzenlemek veya kaldırmak tekrar geçmişini temizler.

### Girdileri düzenleme, sıralama ve kaldırma

**Yapılandırılmış girdiler (Configured entries)** listesi sembolü, grubu, kaynağı, URL'yi, toplama rozetlerini,
normal fiyat seçicisini, haber seçicisini ve haber tekrar sınırını gösterir.

- **Düzenle (Edit)** girdiyi forma yükler. Uygulamak için **Değişiklikleri kaydet (Save changes)** veya
	form değişikliklerini atmak için **Düzenlemeyi iptal et (Cancel edit)** seçeneğini belirleyin.
- Yukarı ve aşağı ok düğmeleri bilgi şeridi sırasını değiştirir ve hemen kaydeder.
- **Kaldır (Remove)** girdiyi ve o anda görüntülenen verilerini siler.
- Uyarı kuralları girdiyi hedefliyorsa SmartTicker bu kuralların silinmesini sorar. Eşleşen
	yapılandırılmış kotasyonu olmayan bir uyarı tetiklenemez.
- Bir girdinin yeniden adlandırılması, o girdiye bağlı kuralların uyarı kuralı görüntü sembollerini günceller.

## Uygulama Ayarları

Sağ tıklama menüsünden **Uygulama Ayarları... (App Settings...)** seçeneğini açın. Değişiklikler uygulanır ve
otomatik olarak kaydedilir; Uygula düğmesi yoktur.

### Bilgi şeridi satırları ve hızı

| Ayar | Seçenekler | Varsayılan | Etki |
| --- | --- | --- | --- |
| Fiyat satırları | 1 ile 8 | 1 | Paralel kayan fiyat bandı satırlarının sayısı. |
| Fiyat kaydırma hızı | 20, 30, 40, 50, 65, 80, 100 veya 120 px/sec | 50 | Kayan fiyat bandı hızı. |
| Haber satırları | 1 ile 8 | 1 | Paralel kayan başlık bandı satırlarının sayısı. |
| Haber kaydırma hızı | 20, 30, 40, 50, 65, 80, 100 veya 120 px/sec | 40 | Kayan haber bandı hızı. |
| Kayan yazı tipi boyutu | 9 ile 24 pt | 14 pt | Kayan satırlardaki Fiyat ve Haber metni. |
| Statik yazı tipi boyutu | 9 ile 24 pt | 13 pt | Statik satırlardaki kotasyon ve başlık metni. |
| Fiyat yenileme | 15 saniyelik adımlarla 30 ile 300 saniye | 60 saniye | İzinli her fiyat girdisinin bir zamanlanmış yenileme aldığı süre. |
| Haber yenileme | 15 saniyelik adımlarla 30 ile 300 saniye | 300 saniye | İzinli her Haber girdisinin bir zamanlanmış yenileme aldığı süre. |

Statik gruplanmış tablolar etkinken fiyat satırları ve fiyat kaydırma hızı devre dışıdır,
çünkü bu mod tüm fiyat girdilerini gösterir ve iki pencereyi de otomatik kaydırmaz.
Haber satırı ve hız ayarları kayan görünüm için korunur.

Fiyat ve Haber istekleri birlikte başlamak yerine tüm aralıkları boyunca
bir saniyelik zaman dilimlerine bağımsız olarak dağıtılır. Örneğin 30 saniye boyunca 60 girdi
saniyede iki girdi; 30 saniye boyunca beş girdi yaklaşık her altı saniyede bir girdi olarak zamanlanır. Aynı anda en fazla dört
kaynak isteği çalışır, aynı girdi ve akış için yinelenen işler atlanır ve kaçırılan zaman dilimleri toplu olarak yeniden oynatılmaz. **Fiyatları
şimdi yenile (Refresh prices now)** veya **Haberleri şimdi yenile (Refresh news now)** yalnızca ilgili akışı yeniden başlatır ve ilk zaman dilimini ister.
Yeni veriler okunurken mevcut başarılı fiyatlar ve başlıklar görünür kalır.

Her HTTP isteğinin sabit 20 saniyelik zaman aşımı vardır. Yavaş bir kaynak kullanıcı arayüzü
dağıtıcısını tutmaz veya sonraki zaman dilimlerinin kalan istek kapasitesini kullanmasını engellemez. SmartTicker
HTTP 403 ve 429 gibi hataları bildirir ve kısıtlamaları atlamaz. Robots yönergelerini,
crawl-delay değerlerini veya sunucu geri çekilme talimatlarını otomatik olarak ayrıştırmaz ya da uygulamaz; bu nedenle uyumlu kaynaklar seçin ve
gereksiz sıklıkta istek göndermekten kaçının.

### Pencere boyutları

Uygulama Ayarları üç bağımsız boyut çifti saklar:

| Pencere | Genişlik | Yükseklik | Varsayılan |
| --- | --- | --- | --- |
| Kayan görünüm | 420–7680 px | 50–900 px | 980 × 64 px |
| Statik Fiyatlar görünümü | 420–7680 px | 420–4320 px | 980 × 420 px |
| Statik Haberler görünümü | 420–7680 px | 240–4320 px | 680 × 340 px |

Bir değerin değiştirilmesi, ilgili pencere veya görünüm etkinken hemen uygulanır. Yayımlanan
örnek 1200 × 96 kayan görünüm, 1200 × 720 statik Fiyatlar ve 760 × 480 statik
Haberler boyutlarını, 15 punto kayan metni ve 14 punto statik metni gösterir. Etkin satırların gerektirdiği
alandan daha düşük bir kayan görünüm yüksekliği otomatik olarak gerekli en düşük değere çıkarılır.

Haberlerin gösterilip gösterilmeyeceğini ve düzenin kayıp kaymayacağını veya statik kalacağını seçmek için **Görünüm (View)** altındaki dört seçeneği kullanın.
Görünümü değiştirmek yapılandırılmış girdileri asla silmez.

### Oturum açarken SmartTicker'ı başlatma

Yüklü yürütülebilir dosyayı yalnızca geçerli kullanıcı için kaydetmek üzere
**Oturum açtığımda SmartTicker'ı başlat (Start SmartTicker when I sign in)** seçeneğini etkinleştirin.

- Windows'ta SmartTicker geçerli kullanıcının `Run` kayıt defteri anahtarını kullanır.
- Freedesktop otomatik başlatma kuralını destekleyen Linux masaüstlerinde SmartTicker,
	kullanıcının otomatik başlatma dizinine `smartticker.desktop` yazar.
- SmartTicker tarafından desteklenen kayıt mekanizması bulunmayan platformlarda seçenek devre dışıdır.

İşletim sistemi yetkili kaynaktır. Başlangıç SmartTicker dışında değiştirilirse
ayarlar bir sonraki yüklendiğinde onay kutusu işletim sisteminin durumunu yansıtır.

### Web sitesi erişimi

**Web sitesi çerezlerine ve ana makineler arası yönlendirmelere izin ver (Allow website cookies and cross-host redirects)** varsayılan olarak devre dışıdır.

Devre dışıyken:

- SmartTicker her web sitesi ana makinesini istemeden önce bir kez açık onay gerektirir.
- Web sitesi çerezleri kabul edilmez.
- Farklı bir ana makineye yönlendirmeler engellenir.
- Onaylanan ana makineler yerel ayarlarda hatırlanır.

Etkin olduğunda:

- SmartTicker ana makine başına onay adımını atlar.
- İstenen web sitelerinin ayarladığı çerezler yalnızca yalıtılmış bir bellek içi kapsayıcıda tutulur
	ve SmartTicker kapandığında kaybolur.
- Diğer ana makinelere yönlendirmeler izlenebilir.
- SmartTicker yine de tarayıcı çerezlerini okumaz, kimlik bilgilerini veya oturum açma
	formlarını göndermez.

Bu seçeneğin kapatılması, onaylanmamış kaynakların o anda görüntülenen verilerini
bu ana makineler onaylanıp yenilenene kadar kaldırır.

#### Web sitesi gizlilik seçenekleri

Bir yanıt hem olumlu hem olumsuz seçenekler içeren bir gizlilik/çerez formu olarak
tanınırsa SmartTicker duraklar ve sayfa başlığını, istenen URL'yi,
onay URL'sini, form özetini ve web sitesinin Kabul/Reddet etiketlerini görüntüler.

- **Kabul (Accept)** o formun sağladığı gizli alanlarla birlikte seçtiğiniz Kabul
	denetiminin tam değerini gönderir.
- **Reddet (Reject)** bu gizli alanlarla birlikte seçtiğiniz Reddet denetiminin tam değerini gönderir.
- **İptal (Cancel)** hiçbir şey göndermez.

Bu, SmartTicker uygulamasının kaynak başına izin onayı değil, bir web sitesinin gizlilik tercihidir.

#### Tüm kaynakları doğrulama

Yapılandırılmış her girdiyi incelemek ve sınamak için **Tüm kaynakları doğrula (Validate all sources)** seçeneğini belirleyin.

1. Web sitesi erişimi kısıtlıysa SmartTicker onaylanmamış girdileri ana makine adına göre gruplar
	 ve her ana makine için bir kaynak inceleme iletişim kutusu görüntüler.
2. Ana makineyi, politika özetini, kılavuzu, kaynak adlarını ve sembolleri inceleyin.
3. Onayı yalnızca web sitesini incelediyseniz ve kullanma izniniz varsa işaretleyin.
4. **Bu kaynağı onayla (Approve this source)**, **Bu kaynağı atla (Skip this source)** veya **Doğrulamayı iptal et (Cancel validation)** seçeneğini belirleyin.
5. SmartTicker izinli her girdiyi sınar ve başarılı, başarısız ve atlanan
	 toplamları bildirir. Ayrı sorunlar durum satırının altında görünür.

Onay, izni SmartTicker içinde kaydeder; yasal hak vermez veya
web sitesinin koşullarını geçersiz kılmaz.

### Görünüm

**Pencere saydamlığı (Window transparency)** yalnızca bilgi şeridi arka planını değiştirir. Metin opak kalır. Aralık
5% adımlarla 20% ile 100% arasındadır ve varsayılan 100%'dir.

Renk alanları `#RRGGBB` onaltılık değerlerini kabul eder ve ayrıca bir renk seçici sunar.

| Renk | Varsayılan | Kullanım amacı |
| --- | --- | --- |
| Arka plan | `#10151D` | Saydamlık uygulanmadan önceki bilgi şeridi arka planı. |
| Kotasyon adı | `#79C0FF` | Sembol/kaynak etiketi. |
| Kapanış fiyatı | `#FFA657` | Normal fiyat. |
| Mesai sonrası | `#00E5FF` | Piyasa öncesi ve mesai sonrası fiyatları. |
| Haberler 1. | `#FFFFFF` | Başlıklar 1, 5, 9 ve devamı. |
| Haberler 2. | `#00E5FF` | Başlıklar 2, 6, 10 ve devamı. |
| Haberler 3. | `#A3E635` | Başlıklar 3, 7, 11 ve devamı. |
| Haberler 4. | `#79C0FF` | Başlıklar 4, 8, 12 ve devamı. |
| Yükseliş | `#3FB950` | Pozitif yüzde değişimleri. |
| Düşüş | `#F85149` | Negatif yüzde değişimleri. |
| Uyarı yanıp sönmesi | `#FF00FF` | Siyahla dönüşümlü olarak tetiklenmiş fiyat uyarıları. |

**Varsayılanlara sıfırla (Reset to defaults)** yukarıdaki tüm renkleri ve 100% arka plan opaklığını geri yükler. Bu işlem
satırları, hızları, yazı tipi boyutlarını, pencere boyutlarını, kaynakları, yenileme aralıklarını, uyarıları veya
dili sıfırlamaz.

### Yedekleme ve geri yükleme

SmartTicker uygulama ayarlarını ve uyarı kurallarını ayrı JSON dosyalarında tutar ve
her yedekleme türü için ayrı düğmeler sağlar.

#### Ayarları dışa ve içe aktarma

- **Ayarları dışa aktar... (Export settings...)** yapılandırılmış girdileri, grup atamalarını, grup tanımlarını,
	gizli haber kotasyonlarını, girdi sırasını, seçicileri, kayan/statik kotasyon görünümü seçimini,
	onaylanmış ana makineleri, satır görünürlüğünü, satırları, hızları, kayan/statik yazı tipi boyutlarını, üç
	pencere boyutu çiftinin tümünü, yenileme aralıklarını, başlangıç tercihini,
	web sitesi erişim seçeneğini, uyarı yanıp sönme rengi dâhil renkleri, saydamlığı ve
	dili yazar.
- **Ayarları içe aktar... (Import settings...)** herhangi bir şeyi değiştirmeden önce dosyanın tamamını doğrular. Reddedilen
	dosya geçerli ayarları değiştirmez.
- Başarılı bir içe aktarma her yapılandırılmış girdinin ve uygulama tercihinin yerini alır. Ayrı
	uyarı kuralları dosyasının yerini almaz.
- Gruplar, ayar dosyasında grup tanımlarının kendileriyle birlikte kotasyon atamaları olarak
	bulunur; böylece kotasyonu olmayan bir grup da yedekte korunur. Ayrı bir
	yalnızca grup dışa veya içe aktarma dosyası yoktur.
- Başlangıç tercihi bir ayar yedeğinde bulunur ancak içe aktarılması işletim sistemi başlangıç
	kaydını sessizce değiştirmez. İşletim sistemi yetkili kaynak olarak kalır;
	geçerli bilgisayardaki kaydı değiştirmek için Başlangıç onay kutusunu kullanın.
- İçe aktarma dosyaları 1 MiB, şema sürümü 1 ve en fazla 200 abonelikle sınırlıdır.
Bilinmeyen özellikler, yinelenen kimlikler, hatalı URL'ler, geçersiz renkler, geçersiz aralıklar
	veya desteklenmeyen dil kodları sessizce yok sayılmak yerine reddedilir.

#### Uyarı kurallarını dışa ve içe aktarma- **Uyarı kurallarını dışa aktar... (Export alert rules...)** tüm kurallarla birlikte Buzz, buzz count ve blink duration değerlerini yazar.
- **Uyarı kurallarını içe aktar... (Import alert rules...)** dosyanın tamamını doğrular, ardından tüm geçerli kuralların
ve uyarı tetikleme ayarlarının yerini alır.
- Kurallar önce abonelik kimliğiyle yeniden bağlanır. Kimlikler farklı olduğunda SmartTicker büyük/küçük harfe duyarsız
sembol eşleşmesi dener.
- Eşleşen kotasyonu olmayan içe aktarılmış bir kural korunur ancak tetiklenemez. İçe aktarma
durumu kaç kuralın yeniden bağlandığını veya eşleşmeden kaldığını bildirir.
- Uyarı içe aktarma dosyaları 1 MiB ile sınırlıdır.

Başka bir bilgisayara aktarım için önce uygulama ayarlarını, ardından uyarı kurallarını
içe aktarın. Uyarıların ikinci olarak içe aktarılması, kuralların yeni abonelik kimliklerine
sembol üzerinden yeniden bağlanmasını sağlar.

### Yapılandırma dosyalarını yerinde düzenleme

Uygulama Ayarlarındaki **Geçerli Uygulama Yapılandırmasını Düzenle (Edit Current App Config)** ve **Geçerli Uyarı Kurallarını Düzenle (Edit Current Alert Rules)** seçenekleri,
canlı JSON dosyasını sisteminizin `.json` ile ilişkilendirdiği metin düzenleyicide açar. Bu özellik
ileri düzey kullanıcılar içindir; SmartTicker içindeki pencereler aynı ayarları risk olmadan kapsar.

Her iki düğme de önce geçerli dosyayı dışa aktarmanızı isteyen bir onay gösterir. Bu
dışa aktarmayı yapın: elle düzenleme dosyayı bozabilir ve geri alma yoktur.

- **Mevcut yapılandırmayı dışa aktar... (Export existing config...)** geçerli dosyayı kaydeder, ardından aynı isteme döner.
- **Metin düzenleyicide aç (Open in text editor)** canlı dosyayı açar.
- **İptal (Cancel)** hiçbir şeyi değiştirmez.

SmartTicker dosyayı izler ve düzenleyiciniz kaydeder kaydetmez yeniden yükler:

- Geçerli bir dosya hemen uygulanır ve bilgi şeridi yeniden başlatılmadan güncellenir.
- Hatalı biçimlendirilmiş JSON, şema ihlali veya başka herhangi bir doğrulama hatası reddedilir. Çalışan
	yapılandırmanız değiştirilmez ve Uygulama Ayarları penceresi
	sorunu bildirir.
- Reddedilen bir düzenlemeden sonra dosyayı düzeltin veya geçerli bir dışa aktarmayı
	**Ayarları içe aktar... (Import settings...)** ya da **Uyarı kurallarını içe aktar... (Import alert rules...)** ile geri yükleyin.
- Başka bir program tarafından kilitli kalan dosya kısa süre yeniden denenir ve ardından bildirilir.

Uyarı kuralları dosyasını düzenlemek aynı kurallara uyar ve uygulama
ayarlarını etkilemez; çünkü iki dosya ayrıdır.

## Uyarı kuralları

Sağ tıklama menüsünden **Uyarıları (Alerts)** açın. Kurallar her başarılı
fiyat yenilemesinden sonra değerlendirilir ve piyasa öncesi veya mesai sonrası değerleri değil yalnızca normal fiyatı izler.

### Kural oluşturma

1. Yapılandırılmış bir **Kotasyon (Quote)** seçin. Aynı sembole sahip girdiler ayrı kalır.
2. Bir **Koşul (Condition)** seçin ve `250.50` gibi değişmez bir ondalık gösterim kullanarak sayısal bir eşik
	 girin.
3. İsteğe bağlı olarak **Etkinlik başlangıcını (Active from)** seçin. Hemen etkinleştirmek için boş bırakın.
4. **Hiçbir zaman sona ermez (Never expires)** işaretini bırakın veya temizleyip bir sona erme tarihi seçin.
5. **Kural ekle (Add rule)** seçeneğini belirleyin.

Kullanılabilir karşılaştırmalar şunlardır:

| Seçenek | Anlamı |
| --- | --- |
| `LessThan` | Fiyat eşikten `<` küçüktür. |
| `LessThanOrEqual` | Fiyat eşikten `<=` küçük veya eşittir. |
| `GreaterThan` | Fiyat eşikten `>` büyüktür. |
| `GreaterThanOrEqual` | Fiyat eşikten `>=` büyük veya eşittir. |
| `EqualTo` | Fiyat eşiğe tam olarak eşittir. |
| `NotEqualTo` | Fiyat eşikten farklıdır. |

Başlangıç sınırı dâhildir. Sona erme sınırı da dâhildir; bu sınır
geçtikten sonra kural artık tetiklenmez. SmartTicker başlangıçtan önceki bir sona ermeyi reddeder.

### Bir kural tetiklendiğinde

Etkin ve zamanlanmış bir kural, koşulu yanlış durumdan doğru duruma geçtiğinde bir kez tetiklenir.
Koşul doğru kalırken her yenilemede bildirim göndermez. Fiyat
koşuldan çıktıktan sonra kural yeniden kurulur ve fiyat koşula tekrar girdiğinde tetiklenebilir.

Bir kuralı düzenlemek veya devre dışı bırakıp yeniden etkinleştirmek de kuralı yeniden kurar. Bu nedenle etkin
bir kural, en son normal fiyat koşulunu zaten karşılıyorsa hemen tetiklenebilir.
Başarısız veya eksik bir fiyat kuralı tetikleyemez.

Bir veya daha fazla kural tetiklendiğinde:

- Etkilenen fiyat girdisi, yapılandırılmış uyarı yanıp sönme rengiyle siyah arasında
	yapılandırılmış süre boyunca dönüşümlü görünür. Varsayılan yanıp sönme rengi macentadır (`#FF00FF`).
- **Buzz** etkinse SmartTicker yapılandırılmış buzz dizisini çalar.
- Uyarı iletisi bir kuralı tanımlar veya birlikte tetiklenen kural sayısını bildirir.
- Uyarı vurgusu etkinken bilgi şeridi kaymaya devam eder.

### Uyarı çıktısı ayarları

| Ayar | Aralık | Varsayılan |
| --- | --- | --- |
| **Buzz** | Açık veya kapalı | Açık |
| Buzz count | 1 ile 20 | 15 |
| **Şu süre boyunca yanıp sön (Blink for)** | 15 saniyelik adımlarla 5 ile 900 saniye | 60 saniye |

Buzz'ın devre dışı bırakılması görsel uyarıyı etkin bırakır. Aynı
değerlendirmede birden çok kural tetiklenirse SmartTicker o değerlendirme için yapılandırılmış tek bir buzz dizisi başlatır.
**Uygulama Ayarları > Görünüm (App Settings > Appearance)** altındaki **Uyarı yanıp sönmesi (Alert blink)** değerini değiştirin. Bu bir uygulama
görünüm tercihidir; bu nedenle Ayarlar dışa/içe aktarma işlemi, ayrı
uyarı kuralları dosyası yerine bu değeri içerir.

### Yapılandırılmış kuralları yönetme

- **Düzenle (Edit)** kuralı forma yükler. Kaydetmek için **Kuralı güncelle (Update rule)** veya
	değiştirmeden bırakmak için **İptal (Cancel)** seçeneğini belirleyin.
- **Devre dışı bırak (Disable)** kuralı korur ancak eşleşmesini durdurur. **Etkinleştir (Enable)** kuralı yeniden kurar ve
	en son normal fiyata göre değerlendirir.
- **Kaldır (Remove)** kuralı siler.
- Liste etkin durumu, sembolü, koşul özetini ve zamanlamayı gösterir.

Uyarı kuralı değişiklikleri ve uyarı çıktısı ayarları otomatik olarak kaydedilir.

## Yerel dosyalar ve gizlilik

SmartTicker yapılandırmayı yerel olarak saklar ve bir geliştirici
hizmetiyle eşitlemez.

Windows'ta varsayılan dosyalar şunlardır:

```text
%LocalAppData%\SmartTicker\settings.json
%LocalAppData%\SmartTicker\alerts.json
```

Linux'ta .NET geçerli kullanıcının yerel uygulama verileri dizinini kullanır; bu normalde şöyledir:

```text
~/.local/share/SmartTicker/settings.json
~/.local/share/SmartTicker/alerts.json
```

### Yalıtılmış bir veri dizini kullanma

İleri düzey tanılama ve test çalıştırmaları SmartTicker başlatılmadan önce `SMARTTICKER_DATA_DIRECTORY` değerini ayarlayabilir.
Değer boş değilse her iki dosya da doğrudan çözümlenmiş dizine
`settings.json` ve `alerts.json` olarak yerleştirilir; yukarıdaki platform varsayılanları bu işlem için kullanılmaz.
Mutlak bir yol tercih edin ve yazılabilir olduğundan emin olun.

PowerShell örneği:

```powershell
$env:SMARTTICKER_DATA_DIRECTORY = 'D:\SmartTicker-Profile'
& 'C:\Program Files\SmartTicker\SmartTicker.Desktop.exe'
```

Linux kabuk örneği:

```bash
SMARTTICKER_DATA_DIRECTORY="$HOME/.local/share/SmartTicker-Test" smartticker
```

Değişkeni işlem başlatılmadan önce ayarlayın. SmartTicker varsayılan profili
seçilen dizine kopyalamaz; bu nedenle boş bir dizin boş bir yapılandırmayla başlar.
Aynı dizine yönlendirilmiş örnekler birbirlerinin kaydedilmiş düzenlemelerini görebilir. Yedekleme ve profil aktarımı için
normal Ayarlar ve Uyarı Kuralları dışa/içe aktarma komutlarını kullanın.

Uyarılar penceresi kullanılan uyarı dosyasının tam yolunu görüntüler. Yazma işlemleri, kısmen
yazılmış bir dosyanın geçerli yapılandırma olarak değerlendirilmemesi için geçici bir dosya ve ardından değiştirme kullanır.

SmartTicker uygulamasında hesap, telemetri, analiz, reklam veya bulut eşitleme yoktur. Bir kaynak
web sitesi, SmartTicker bu kaynağı istediğinde IP adresiniz gibi normal ağ bilgilerini alır.
Yardım'ı açmak GitHub'dan ham kılavuzu ister. Tüm ayrıntılar için depodaki
`PRIVACY.md` dosyasını okuyun.

Her kaynak URL'si ve seçicisinin web sitesinin koşullarına, lisansına, robots
yönergelerine ve geçerli hukuka uygun kullanılmasını sağlamak sizin sorumluluğunuzdadır.

## Sorun giderme

### Bir kotasyon kullanılamıyor veya fiyat yok olarak görünüyor

Bir kaynak isteği 20 saniye sonra zaman aşımına uğrar. Bu kotasyonun daha önce başarılı bir
anlık görüntüsü varsa başarısız yenileme onu görünür tutar; aksi takdirde kotasyon daha sonraki bir yenileme başarılı olana kadar
**Kullanılamıyor (Unavailable)** gösterir. Seçicileri değiştirmeden önce doğrulama veya yenileme hatasını okuyun.

1. **Kotasyonlar... (Quotes...)** penceresini açın, girdiyi düzenleyin ve Tam URL'yi denetleyin.
2. **Fiyatın (Price)** seçili olduğunu doğrulayın.
3. İstenirse web sitesini onaylayın.
4. **URL'yi Doğrula (Validate URL)** seçeneğini belirleyin ve tam sonucu okuyun.
5. **Fiyatı keşfet (Discover price)** komutunu çalıştırın veya sayfanın statik HTML'sini inceleyip seçiciyi güncelleyin.
6. Sayfanın SmartTicker tarafından güvenle işlenemeyen JavaScript, kimlik doğrulaması veya onay gerektirip gerektirmediğini
	 denetleyin.
7. HTTP 403, 429, robots kısıtlamaları ve sitenin otomatik erişim politikasına uyun.

### Piyasa öncesi veya mesai sonrası verileri eksik

- İlgili piyasa seansı etkin olmayabilir.
- Seans değeri olmadığında sayfa seans öğesini içermeyebilir.
- Piyasa öncesi seçicilerin piyasa öncesi öğeleri, mesai sonrası seçicilerin ise
	piyasa sonrası öğeleri hedeflediğini doğrulayın.
- Web sitesi işaretlemesi değişmiş olabileceğinden eşleşen keşif komutunu yeniden çalıştırın.

### Haberler boş

- **Haberlerin (News)** seçili olduğunu doğrulayın.
- Kaynağı doğrulayın ve **Haberleri keşfet (Discover news)** komutunu çalıştırın.
- Seçicinin görünür başlık metni olan bağlantılar döndürdüğünden emin olun.
- Başarısız veya zaman aşımına uğrayan bir Haber isteği, kullanılabiliyorsa önceki başarılı başlıkları korur.
	Başarılı sonucu olmayan bir kaynak, sonraki bir zaman dilimi başarılı olana kadar boş kalır.
- Bir başlık, bu oturum için yapılandırılmış tekrar sınırına ulaştıktan sonra kaybolur.
- Statik Haberlerde, amaçlanan kotasyonun **Şunlar için haberleri göster (Show news for)** altında işaretli olduğunu doğrulayın.

### Seçici keşfi hiçbir şey bulamıyor

Keşif yalnızca indirilen statik HTML'yi okur. Daha sonra sayfa JavaScript'i tarafından
oluşturulan değerleri göremez. Doğrulanmış bir seçiciyi elle girin, statik bir sayfa/veri akışı seçin veya
uyumlu bir herkese açık sayfa üzerinden yetkili ve belgelenmiş bir API kullanın.

### Bir uyarı tetiklenmiyor

- Bağlı kotasyonun hâlâ var olduğunu, Fiyat topladığını ve başarılı bir normal
	fiyata sahip olduğunu doğrulayın.
- Kuralın Etkin olduğunu ve başlangıç/sona erme zamanlaması içinde bulunduğunu doğrulayın.
- Karşılaştırmayı ve eşiği denetleyin. `EqualTo` tam ondalık eşitliği gerektirir.
- Sürekli doğru olan bir koşulun bir kez tetiklendiğini unutmayın; kuralı düzenlemediğiniz veya yeniden etkinleştirmediğiniz sürece
	yeniden tetiklenmeden önce yanlış duruma gelmelidir.
- Piyasa öncesi ve mesai sonrası fiyatları uyarı kurallarını çalıştırmaz.

### SmartTicker taşınamıyor veya yeniden boyutlandırılamıyor

- Yalnızca sol şeritteki dikey noktalı tutamaçtan taşıyın.
- Bir kenar veya köşeden yeniden boyutlandırın; bir kenara erişmek zorsa görünür sağ alt işareti
	kullanın.
- Bilgi şeridi içeriği kasıtlı olarak taşıma yüzeyi değildir.

### Statik gruplar veya değerler beklediğim gibi değil

- **Kotasyonlar... (Quotes...)** penceresini açın ve her girdinin Grup değerini doğrulayın.
- Grup tanımlarını yönetmek ve her kotasyonun geçerli
	ilişkisini incelemek için **Kotasyon grupları... (Quote groups...)** penceresini açın.
- Grup alanı boş olan girdiler **Gruplanmamış (Ungrouped)** altında görünür.
- **Değ. (Chg)** Son ve Değ.% değerlerinden hesaplanır; sayfadan bağımsız olarak ayıklanmaz.
	Yüzde kullanılamadığında `—` olarak kalır.
- Grup ve satır sırasını değiştirmek için girdileri yukarı/aşağı denetimleriyle yeniden sıralayın.
- Tüm grubu taşımak için kutucuk başlığındaki noktalı tutamacı sürükleyin. Öncesine yerleştirmek için başka bir kutucuğun sol
  yarısına, sonrasına yerleştirmek için sağ yarısına bırakın.
- Tabloyu güncellemek için SmartTicker duraklatılmamışken **Fiyatları şimdi yenile (Refresh prices now)** seçeneğini belirleyin.

### Yardım metni biçimlendirilmemiş veya gezinme çalışmıyor

- Yardım penceresi Markdown noktalama işaretleri yerine biçimlendirilmiş başlıklar, paragraflar, listeler, tablolar, bağlantılar
	ve kod blokları göstermelidir.
- Önemli bir bölüme gitmek için soldaki **Bu sayfada (On this page)** seçeneğini kullanın. Hızlı
	gezinme tablosundaki bağlantılar da belge içinde kaydırır.
- Eşleşen yayımlanmış kılavuzu istemek için Yardım'ı kapatıp yeniden açın veya **Dil (Language)**
	ayarını değiştirin. Kılavuz gelene kadar SmartTicker, yüklü uygulamadaki biçimlendirilmiş gömülü
	kılavuzu gösterir.

### Çevrimiçi Yardım kullanılamıyor veya güncel değil

- Yayımlanmış kılavuzu yeniden istemek için Yardım'ı kapatıp yeniden açın.
- Yayımlanmış dosyayı doğrudan incelemek için bu kılavuzun başlarında gösterilen ham GitHub adresini
	bir tarayıcıda açın.
- İstek başarısız olursa veya boş dosya döndürürse SmartTicker gömülü kılavuzu kullanır.
- Çevrimiçi değişiklikler yalnızca `HELPME.md` veya eşleşen yerelleştirilmiş
	`help/HELPME.<language-code>.md` dosyası deponun `main` dalında yayımlandıktan sonra görünür.

## Destek

Yeniden üretilebilir sorunları şu adreste bildirin:

<https://github.com/bulentozkir/smartticker/issues>

SmartTicker sürümünü, işletim sistemini, kaynak ana makine adını, doğrulama durumunu
ve tam hata metnini ekleyin. Göndermeden önce özel URL'leri veya diğer hassas bilgileri kaldırın.