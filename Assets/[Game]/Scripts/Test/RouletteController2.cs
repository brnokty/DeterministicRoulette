using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RouletteController2 : MonoBehaviour
{
    [Header("Wheel & Ball")]
    public Transform wheel;       // Wheel objesi (pivot merkezde)
    public Transform ball;        // Ball objesi (pivot merkezde)
    public float wheelSpinMinRotations = 6f;
    public float wheelSpinMaxRotations = 10f;
    public float spinDuration = 4f;
    public float ballRadius = 1.35f;      // Kendi modeline göre ayarla!
    public float ballHeight = 0.11f;      // Ball yüksekliği

    [Header("UI")]
    public InputField numberInput;
    public Button spinButton;
    public Text infoText;

    [Header("Wheel Config")]
    public int[] wheelOrder = new int[]
    {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    }; // European roulette

    private bool isSpinning = false;
    private int targetNumber = -1;

    void Start()
    {
        spinButton.onClick.AddListener(OnSpinButton);
        infoText.text = "Bir sayı girin veya boş bırakın, 'Spin' ile çevirin.";
    }

    public void OnSpinButton()
    {
        if (isSpinning) return;

        // Kullanıcı sayı girdi mi kontrol et
        if (!string.IsNullOrEmpty(numberInput.text))
        {
            int num;
            if (int.TryParse(numberInput.text, out num) && System.Array.IndexOf(wheelOrder, num) >= 0)
            {
                targetNumber = num;
            }
            else
            {
                infoText.text = "Geçerli bir sayı girin!";
                return;
            }
        }
        else
        {
            // Rastgele bir sayı
            int rndIdx = Random.Range(0, wheelOrder.Length);
            targetNumber = wheelOrder[rndIdx];
        }

        infoText.text = $"Dönüyor... Hedef: {targetNumber}";
        StartCoroutine(SpinRoutine());
    }

    IEnumerator SpinRoutine()
    {
        isSpinning = true;

        // Hedef segment açısı
        int winIdx = System.Array.IndexOf(wheelOrder, targetNumber);
        float segmentAngle = 360f / wheelOrder.Length;

        // --- MODELİNDE Z+ HANGİ SEGMENTE BAKIYORSA OTOMATİK OFFSET HESABI ---
        int zeroIdx = System.Array.IndexOf(wheelOrder, 0);
        int segmentOffsetToZero = (0 - zeroIdx + wheelOrder.Length) % wheelOrder.Length;
        // Eğer modelinde 0 değil başka bir segment Z+ yönündeyse, segmentOffsetToZero o kadar kayar.
        float targetWheelAngle = ((winIdx + segmentOffsetToZero) % wheelOrder.Length) * segmentAngle;

        float wheelRotations = Random.Range(wheelSpinMinRotations, wheelSpinMaxRotations);
        float wheelStartAngle = wheel.eulerAngles.y;
        float wheelEndAngle = wheelStartAngle + (wheelRotations * 360f) + Mathf.DeltaAngle(wheelStartAngle % 360, targetWheelAngle);

        float ballStartRelativeAngle = Random.Range(2 * 360f, 4 * 360f);
        float ballEndRelativeAngle = 0f; // spin sonunda wheel'e göre hedef segmentte olacak

        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            float t = Mathf.Clamp01(elapsed / spinDuration);
            float easedT = 1f - Mathf.Pow(1f - t, 3); // Cubic ease out

            // Wheel world açısı
            float currentWheelAngle = Mathf.Lerp(wheelStartAngle, wheelEndAngle, easedT);
            wheel.eulerAngles = new Vector3(0, currentWheelAngle, 0);

            // Ball tekerleğe göre açısı (spin boyunca wheel'e göre 2-4 tur yapar ve sonunda wheel ile aynı açıda olur)
            float currentBallRelativeAngle = Mathf.Lerp(ballStartRelativeAngle, ballEndRelativeAngle, easedT);

            // Ball world açısı = wheel world açısı + ball relative açısı
            float currentBallWorldAngle = currentWheelAngle + currentBallRelativeAngle;
            SetBallPosition(currentBallWorldAngle, ballRadius, wheel.position, ballHeight);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // --- DİKKAT: ARTIK HİÇBİR ŞEKİLDE AÇIYI TEKRAR SET ETMİYORUZ ---
        // wheel.eulerAngles = new Vector3(0, targetWheelAngle, 0);
        // SetBallPosition(targetWheelAngle, ballRadius, wheel.position, ballHeight);

        isSpinning = false;
        infoText.text = $"Kazanan: {targetNumber}";
    }

    // Ball'un world pozisyonunu ayarlar
    private void SetBallPosition(float angle, float radius, Vector3 center, float yOffset)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Sin(rad) * radius,
            yOffset,
            Mathf.Cos(rad) * radius
        );
        ball.position = center + offset;
    }
}
