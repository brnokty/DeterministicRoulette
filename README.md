# Unity Deterministic Roulette 

Bu proje, Unity kullanılarak geliştirilmiş, Amerikan ve Avrupa ruleti kurallarını içeren eksiksiz bir rulet oyunudur. Oyuncular gerçekçi bir masada farklı bahis türleriyle oynayabilir, sonuçlar fiziksel animasyonlarla canlandırılır, istatistikler ve kazançlar tutulur ve görsel/sesli kutlama efektleriyle zenginleştirilmiştir.

---

## 📋 Özellikler

- **Amerikan & Avrupa ruleti desteği**  
- **Tüm klasik bahis çeşitleri:** Tekli sayı (Straight), Split, Street (3'lü), Corner (Köşe), Line (6'lı), Column, Dozen, Red/Black, Odd/Even, High/Low, Double Zero
- **Gerçekçi çark ve top animasyonu**
- **Chip sürükle & bırak, kolay bahis yerleştirme**
- **Kullanıcıya özel deterministic outcome (sayıyı zorla seçebilme)**
- **Kazanan sayı ve renk vurgulama, kutlama efektleri (confetti, highlight, animasyon)**
- **Anlık bakiye takibi, toplam istatistikler ve geçmiş kazananlar listesi**
- **Oyun kayıt/yükle (persistency) ve sıfırlama**
- **Temiz ve kullanıcı dostu arayüz**

---

## 🚀 Kurulum & Kullanım

1. **Unity Projesini Açın**  
   Unity 2021 veya üzeri bir sürüm ile projeyi açabilirsiniz.

2. **Sahne Ayarları**  
   Ana sahnede bütün ana GameObject'ler (GameManager, UIManager, BetArea'lar, vb.) otomatik olarak hazırdır.

3. **Oyunu Çalıştırın**  
   Play tuşuna bastığınızda oyunu masaüstünde veya editörde oynayabilirsiniz.

4. **Bahis Yapmak**  
   - Sağdan istediğiniz chip değerini seçin.
   - Masadaki bahis alanlarından birine chip bırakın (sürükle & bırak ile).
   - Birden fazla bahis yapılabilir.
   - "Spin" butonuna basarak oyunu başlatın.

5. **Kazanma/Kayıp ve Sonuç**  
   - Sonuç ekranda sayı ve renk ile gösterilir.
   - Kazandıysanız kutlama efekti (confetti, highlight vs.) çıkar, bakiyeniz güncellenir.
   - İstatistik panelinden tüm spin geçmişinizi görebilirsiniz.

---

## 🎮 Kontroller

- **Mouse**: Chip sürükle & bırak, butonlara tıklama
- **Spin Butonu**: Bahisler kapatılır ve çark döner
- **Reset Butonu**: Tüm bahisler temizlenir, istatistikler sıfırlanır

---

## 🧩 Oyun Mantığı ve Kısa Akış

1. Oyuncu chip seçer ve masada dilediği bahis tipine bırakır.
2. Spin tuşuna basınca çark ve top animasyon başlar.
3. Sonuç deterministic ise seçilen sayı, değilse rastgele gelir.
4. Sonuç ekranda vurgulanır, kazanç/istatistikler güncellenir, kutlama efektleri oynar.
5. İstenirse yeni bahis yapılarak oyun tekrar başlatılabilir.

---

## 🛠️ OOP ve Yazılım Mimarisi

- **Singleton pattern**: GameManager, UIManager, BetManager vb.  
- **Encapsulation & Separation**: Bahis mantığı, çark/top animasyonu, istatistik yönetimi, ses/görsel efektler ayrı scriptlerde modüler biçimde yazıldı.
- **Event ve delegate kullanımı**: İstatistik güncellemeleri ve oyun sonuçları için observer/event pattern.
- **Enum & Struct**: Bahis türleri ve masa alanları için tip güvenliği.
- **Persistency**: SaveManager ile oyun kaydı ve istatistik saklama.
- **UI Separation**: Tüm UI kontrolü ve panel logic'i ayrı tutuldu.

---

## 🔥 Bilinen Eksikler / Geliştirme Fikirleri

- Oyun mobile cihazlar için optimize edilmemiştir.
- Daha gelişmiş ses efektleri ve özelleştirilebilir VFX eklenebilir.
- Multiplayer (çoklu oyuncu) modu eklenebilir.
- Cihaz sarsıntısı veya fizikle daha detaylı top animasyonu için geliştirme yapılabilir.

---

## 👤 Geliştirici

- **Baran Oktay** (BageR)  
- [LinkedIn veya iletişim isteğe göre buraya eklenebilir]

---