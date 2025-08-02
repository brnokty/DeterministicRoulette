using UnityEngine;
using System.Collections;
using System.Text;

public class RouletteController : MonoBehaviour
{
    [Header("Wheel & Ball")]
    public Transform wheel;
    public Transform ball;

    [Header("Debug")]
    public int debugCount = 0;

    [Header("Wheel Spin")]
    public float wheelSpinMin = 2.7f;
    public float wheelSpinMax = 4.1f;
    public int minWheelRounds = 5;
    public int maxWheelRounds = 7;

    [Header("Ball Spin (after wheel stops)")]
    public float ballSpinAfterMin = 0.8f;
    public float ballSpinAfterMax = 1.6f;
    public int minExtraBallRounds = 4;
    public int maxExtraBallRounds = 7;

    [Header("Ball Orbit")]
    public float ballStartRadius = 0.325f;
    public float ballEndRadius = 0.29f;
    public float ballBaseHeight = 0.05f;
    public float ballBounceHeight = 0.02f;

    [Header("Wheel Segment Order")]
    public int[] wheelOrder = new int[] {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };

    [Header("Segment Fine Tune")]
    [Tooltip("Segment merkezine küçük kayma. 0 = tam segment merkezi, -2 gibi değerlerde hizalama yapabilirsin.")]
    public float ANGLE_OFFSET = 0f;
    [Tooltip("0 ise segment birebir, +1 ise bir segment ileri, -1 geri kaydırır.")]
    public int SEGMENT_OFFSET = 0;

    [Header("Animasyon Eğrileri (İsteğe bağlı)")]
    public AnimationCurve wheelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve ballCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // BagerTest'teki mantığı kullanmak için
    private Vector3 ballStartPosition;
    private Vector3 wheelCenter;

    public int SegmentCount => wheelOrder.Length;

    private void Start()
    {
        ballStartPosition = ball.position;
        wheelCenter = wheel.position;
        
        Debug.Log($"[ROULETTE-DEBUG] Ball Start Position: {ballStartPosition}");
        Debug.Log($"[ROULETTE-DEBUG] Wheel Center: {wheelCenter}");
        Debug.Log($"[ROULETTE-DEBUG] Segment Count: {SegmentCount}");
    }

    public IEnumerator SpinRoulette(int winningNumber, System.Action onComplete = null)
    {
        debugCount++;
        StringBuilder dbg = new StringBuilder();

        // Reset wheel angle
        wheel.localEulerAngles = Vector3.zero;
        float wheelStartAngle = 0f;

        // Find the winning segment index - BagerTest mantığı
        int winIndex = FindIndex(wheelOrder, winningNumber);
        if (winIndex == -1)
        {
            Debug.LogError($"[ROULETTE-DEBUG-{debugCount}] Winning number {winningNumber} not found in wheel order!");
            yield break;
        }

        // BagerTest'teki segment açısı hesaplaması - bu açı asla değişmez!
        float segmentSize = 360f / wheelOrder.Length;
        float targetSegmentAngle = segmentSize * winIndex;

        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        int wheelSpins = Random.Range(minWheelRounds, maxWheelRounds + 1);
        
        // Wheel'ın döneceği toplam açı - bu değer animasyon için
        float totalWheelRotation = 360f * wheelSpins;
        float wheelEndAngle = wheelStartAngle + totalWheelRotation;

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] WinningNumber={winningNumber} | winIndex={winIndex}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] segmentSize={segmentSize} | targetSegmentAngle={targetSegmentAngle}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] wheelSpins={wheelSpins} | totalWheelRotation={totalWheelRotation}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] wheelEndAngle={wheelEndAngle} | duration={wheelSpinDuration}");

        // Ball animation parameters
        float ballStartAngle = Random.Range(1440f, 2880f); // Ball spins fast initially
        float ballEndAngle = 0f; // Ball ends at target position

        float elapsed = 0f;

        // Initial position logging
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [START] Ball pos: {ball.position:F3}, Wheel pos: {wheel.position:F3}");

        // Main animation loop
        while (elapsed < wheelSpinDuration)
        {
            float t = elapsed / wheelSpinDuration;
            float wheelT = wheelCurve != null ? wheelCurve.Evaluate(t) : t;
            float ballT = ballCurve != null ? ballCurve.Evaluate(t) : t;

            // Update wheel rotation - sadece animasyon için döndür
            float currentWheelAngle = Mathf.Lerp(wheelStartAngle, wheelEndAngle, wheelT);
            wheel.localEulerAngles = new Vector3(0f, currentWheelAngle, 0f);

            // Update ball position
            float ballRadius = Mathf.Lerp(ballStartRadius, ballEndRadius, ballT);
            float ballSpin = Mathf.Lerp(ballStartAngle, ballEndAngle, ballT);

            // Calculate bounce effect for the ball
            float bounce = 0f;
            if (t > 0.7f)
            {
                float freq = SegmentCount * 1.8f;
                float localT = Mathf.InverseLerp(0.7f, 1f, t);
                bounce = Mathf.Abs(Mathf.Sin(ballSpin * freq * Mathf.Deg2Rad)) * ballBounceHeight * (1 - localT * 0.6f);
            }

            // ÖNEMLİ FİX: Ball pozisyonu sadece TARGET SEGMENT açısına göre hesaplanmalı
            // Wheel'ın ne kadar döndüğü ball'ın hedef pozisyonunu etkilemez!
            // Ball sadece hedef segment + kendi spin hareketinde olmalı
            float currentBallAngle = targetSegmentAngle + ballSpin;
            
            // BagerTest'teki SetBallPosition mantığını kullan
            SetBallPositionLikeBagerTest(currentBallAngle, ballRadius, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final positioning - sadece target segment açısı
        wheel.localEulerAngles = new Vector3(0f, wheelEndAngle, 0f);
        SetBallPositionLikeBagerTest(targetSegmentAngle, ballEndRadius, 0f);

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [END] Ball pos: {ball.position:F3}, Wheel Y={wheel.localEulerAngles.y:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Final targetSegmentAngle: {targetSegmentAngle:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");

        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

    // BagerTest'teki SetBallPosition mantığını kopyala
    private void SetBallPositionLikeBagerTest(float segmentAngle, float radius, float bounceY = 0f)
    {
        // BagerTest'teki mantık:
        // 1. Ball'ı start position'a resetle (direction hesabı için)
        // 2. Wheel center'dan ball start'a direction hesapla
        // 3. Bu direction'ı segment açısı kadar döndür
        // 4. Yeni pozisyonu uygula
        
        Vector3 directionToStart = (ballStartPosition - wheelCenter).normalized;
        Vector3 rotatedDirection = Quaternion.Euler(0, segmentAngle, 0) * directionToStart;
        Vector3 newBallPosition = wheelCenter + rotatedDirection * radius;
        
        // Y pozisyonunu ayarla (bounce effect ile)
        newBallPosition.y = ballStartPosition.y + bounceY;
        
        ball.position = newBallPosition;
        
        if (debugCount <= 2) // İlk birkaç debug için detaylı log
        {
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] SetBallPos: angle={segmentAngle:F2}, radius={radius:F3}, dirStart={directionToStart:F3}, dirRot={rotatedDirection:F3}, finalPos={newBallPosition:F3}");
        }
    }

    // BagerTest'teki FindIndex fonksiyonu
    public int FindIndex(int[] array, int value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
            {
                return i;
            }
        }
        return -1;
    }
}