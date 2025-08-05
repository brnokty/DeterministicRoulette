# Unity Roulette

## Ekran Görüntüsü
<img width="1195" height="668" alt="Screenshot 2025-08-05 052228" src="https://github.com/user-attachments/assets/87d32d01-1308-4042-b6df-c5a93dac7959" />

Bu projede, Unity kullanarak sıfırdan gerçekçi ve işlevsel bir rulet oyunu geliştirdim. Hem Amerikan hem Avrupa ruleti destekli, klasik masadan ilham alarak tamamen kendi mantığıma ve oyun zevkime göre kodladım.

---

## 🎬 Oyun Videosu

[YouTube'da İzle](https://youtu.be/PQ3nmsk2N1s)

---

## Özellikler

- **Amerikan ve Avrupa ruleti** tek sahnede (RouletteManager ile aralarında geçiş yapabiliriz)
- **Tüm bahis türleri var:** Tek sayı (Straight), split, corner, 6’lı (line), column, dozen, renk, tek/çift, high/low, double zero...
- **Chip sürükle bırak**: Farklı değerlerde chip seçip masadaki herhangi bir alana kolayca bırakabiliyorsun.
- **Deterministic selection**: İstersen sonucu seçip direkt o sayıda durmasını sağlayabilirsin
- **Gerçekçi çark ve top animasyonu**: Top ve çark fiziksel olarak döner, sonlara doğru yavaşlar ve küçük sekmelerle durur.
- **Kazanan sayı ve renk vurgusu**: Sonuç ekranda ve geçmişte renkli şekilde gösterilir, ayrıca kutlama efekti var (confetti).
- **İstatistik paneli**: Toplam spin, win/lose, profit ve geçmişte çıkan sayıları her an görebilirsin.
- **Kayıt ve devam etme**: Oyunun istatistikleri ve bakiye kaydediliyor, tekrar açınca kaldığın yerden devam.
- **Modern, sade arayüz**: Temiz UI, büyük tuşlar, renkli chipler ve klasik masa görüntüsü.

---

## Kurulum ve Çalıştırma

1. **Projeyi Unity ile aç (2021 veya üstü tavsiye ederim).**
2. Ana sahnedeki tüm gerekli objeler (GameManager, UIManager, BetAreas, vb.) otomatik yüklü.
3. Oynamak için editörde “Play” tuşuna basman yeterli.
4. Mobil veya masaüstü için ekstra bir ayar yapmana gerek yok, ancak bu versiyon masaüstü optimizasyonlu.

---

## Oynanış

- Aşağıdan istediğin chip’i seç.
- Masadaki herhangi bir bahis alanına chip’i sürükleyip bırak.
- Birden fazla alana bahis yapabilirsin.
- “Spin” tuşuna basınca animasyon başlar ve sonuç ekrana gelir.
- Sonuç, hem sağ üstte rulet üzerinde hem de geçmişte listelenir.
- Kazanırsan kutlama efekti ve “You won” popup’ı çıkar, bakiyen güncellenir.
- “Deterministic selection” kutusuna sayı girersen, spin o sayıda biter.

---

## Oyun Mantığı

- Klasik rulet masası ve kuralları birebir uygulandı.
- Bahis türlerinin payout oranları ve kuralları orijinal ruletle aynı.
- Çark ve top animasyonu DOTween’siz tamamen coroutine ile fiziksel olarak döner, sonda yavaşlayıp sekerek durur.
- Amerikan ve Avrupa ruleti arasında geçiş yapmak mümkün (çift sıfır da destekli).

---

## Kod ve Mimari Hakkında

- GameManager ve UIManager gibi ana sistemler Singleton yapıda.
- Bet mantığı, çark/top animasyonu, istatistikler ve sesler ayrı scriptlerde yazıldı. Kod tamamen modüler.
- Bahis tipleri enum ve class’larla tip güvenli ve okunabilir.
- Kayıt işlemleri PlayerPrefs ile yapıldı.
- Tüm animasyonlar, DOTween gibi plugin’ler olmadan Unity coroutine’leriyle kodlandı.
- UI elementleri kolayca genişletilebilir, tüm değerler inspector’dan ayarlanabilir.

---

## Geliştirme ve Eksikler

- Proje masaüstü için hazırlandı, mobil uyumluluk için ek dokunmatik kontroller eklenebilir.
- Ses efektleri temel seviyede, istenirse VFX ve ekstra sesler eklenebilir.
- Multiplayer, online leaderboard, farklı masa temaları gibi ek özellikler ileride düşünülebilir.
- Top animasyonunda fiziksel “rebound” veya çarpışma efektleri eklenebilir.

---

## Not

Projeyi tamamen kendi geliştirdim ve sade, anlaşılır bir kod yapısı oluşturmaya özen gösterdim. Herhangi bir yerde hata, bug veya geliştirme önerin olursa bana yazabilirsin.

---

# Detaylar

## Top Detay Kamerası
<img width="115" height="134" alt="Screenshot 2025-08-05 053119" src="https://github.com/user-attachments/assets/dbd474e6-72c2-4324-a036-d9004650efd8" />

Ekranın sağ üst köşesinde topun durduğu yeri daha rahat izleyebilmemiz için kameraya odak olan bir kamera görüntüsü ve durduğunda nerde durduğunu söyleyen bir Textimiz bulunmakta.

## Chip Mantığı
<img width="611" height="109" alt="Screenshot 2025-08-05 053434" src="https://github.com/user-attachments/assets/eac9839a-867a-489b-abcf-d34f4a21f159" />

Toplam 9 farklı chip imiz mevcut. chipi tutup sürükleyerek istediğimiz yerde bete koyabiliyoruz.
<img width="956" height="358" alt="Screenshot 2025-08-05 053804" src="https://github.com/user-attachments/assets/383c5b76-1f87-4ebf-bb66-c2fe366a7e7f" />

Paramızın yetmediği Chipleri alamıyoruz bunu kullancıya chiplerin rengini soluklaştırarak gösteriyoruz.

## Klasör yapısı
<img width="725" height="153" alt="Screenshot 2025-08-05 054053" src="https://github.com/user-attachments/assets/47a58768-0a33-4811-b129-0cbe8583c695" />

Projenin daha düzenli olabilmesi adına **[Game]** adında bir klasör oluşturarak klasör kalabalıklığını önlüyoruz.

<img width="645" height="135" alt="Screenshot 2025-08-05 054239" src="https://github.com/user-attachments/assets/7cd7bd95-807a-4760-8dc6-4ecfeefe616a" />

**[Game]** Göründüğü gibi sade bir klasör yapısı elde edildi.

<img width="253" height="144" alt="Screenshot 2025-08-05 054339" src="https://github.com/user-attachments/assets/b8921f3e-c8c5-4efb-ad59-cccb4557a37c" />

Scriptlerimide 3 ana başlıkta topladım.

### Manager Scriptlerim
<img width="559" height="138" alt="Screenshot 2025-08-05 054433" src="https://github.com/user-attachments/assets/a917f0b8-75a7-4757-87d5-aa6501042545" />

### Roulette İle İlgili Scriptlerim
<img width="409" height="140" alt="Screenshot 2025-08-05 054509" src="https://github.com/user-attachments/assets/1aef6c33-a4e1-4345-afd5-051cc6e0673c" />

### UI Scriptlerim
<img width="485" height="152" alt="Screenshot 2025-08-05 054609" src="https://github.com/user-attachments/assets/6deffe10-723a-436a-8ca0-82cc610c68d7" />

### Unity Scene'im
<img width="209" height="135" alt="Screenshot 2025-08-05 054753" src="https://github.com/user-attachments/assets/e0d92918-2c60-4d4b-b22f-5981c974abac" />

## Amerikan European Değişimi
<img width="211" height="250" alt="Screenshot 2025-08-05 055033" src="https://github.com/user-attachments/assets/444213d1-b277-476b-9a67-1db9a1b8e1d6" />

Hierarchy'den RouletteManager'imizi seçiyoruz.
<img width="515" height="394" alt="Screenshot 2025-08-05 055151" src="https://github.com/user-attachments/assets/159c5939-f0dc-4f84-89b2-58ac527a70cc" />

Inspector'den GameType ı değiştirerek başlattığımızda oyun otomatik olarak GameType a göre başlatıyor.

### European Roulette
<img width="1198" height="666" alt="Screenshot 2025-08-05 055348" src="https://github.com/user-attachments/assets/bc2377dd-fb41-4e27-bd7c-47bd173c4469" />

### American Roulette
<img width="1197" height="670" alt="Screenshot 2025-08-05 055459" src="https://github.com/user-attachments/assets/05a31504-f0cc-4923-9b02-06cd849fbe98" />






