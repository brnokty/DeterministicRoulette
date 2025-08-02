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

    [Header("Animasyon Eğrileri (İsteğe bağlı)")]
    public AnimationCurve wheelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve ballCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // BagerTest'teki EXACT mantık için
    private Vector3 ballStartPosition;
    private Vector3 wheelCenter;
    private float startRadius;

    public int SegmentCount => wheelOrder.Length;

    private void Start()
    {
        // BagerTest'teki Start mantığını birebir kopyala
        ballStartPosition = ball.position;
        wheelCenter = wheel.position;
        startRadius = Vector3.Distance(ballStartPosition, wheelCenter);
        
        Debug.Log($"[ROULETTE-DEBUG] Ball Start Position: {ballStartPosition}");
        Debug.Log($"[ROULETTE-DEBUG] Wheel Center: {wheelCenter}");
        Debug.Log($"[ROULETTE-DEBUG] Start Radius: {startRadius}");
        Debug.Log($"[ROULETTE-DEBUG] Segment Count: {SegmentCount}");
        Debug.Log($"[ROULETTE-DEBUG] Segment Size: {360f / wheelOrder.Length} degrees");
    }

    public IEnumerator SpinRoulette(int winningNumber, System.Action onComplete = null)
    {
        debugCount++;
        StringBuilder dbg = new StringBuilder();

        // BagerTest'teki Calculate() mantığını birebir kopyala
        int index = FindIndex(wheelOrder, winningNumber);
        if (index == -1)
        {
            Debug.LogError($"[ROULETTE-DEBUG-{debugCount}] Number {winningNumber} not found in wheel order!");
            yield break;
        }

        // BagerTest'teki segment hesaplaması - TAMAMEN AYNI
        float segmentSize = 360f / wheelOrder.Length;
        float targetSegmentAngle = segmentSize * index;

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Target Number: {winningNumber}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Found Index: {index}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Segment Size: {segmentSize}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Target Segment Angle: {targetSegmentAngle}");

        // Animasyon parametreleri
        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        int wheelSpins = Random.Range(minWheelRounds, maxWheelRounds + 1);
        float totalWheelRotation = 360f * wheelSpins;
        
        float wheelStartAngle = 0f;
        float wheelEndAngle = wheelStartAngle + totalWheelRotation;

        // Ball animasyon parametreleri
        float ballStartAngle = Random.Range(1440f, 2880f); // Ball fast spin
        float ballEndAngle = 0f; // Ball ends still

        float elapsed = 0f;

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Wheel animation: {wheelSpins} spins, duration: {wheelSpinDuration}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [START] Ball pos: {ball.position:F3}, Wheel pos: {wheel.position:F3}");

        // Ana animasyon döngüsü
        while (elapsed < wheelSpinDuration)
        {
            float t = elapsed / wheelSpinDuration;
            float wheelT = wheelCurve != null ? wheelCurve.Evaluate(t) : t;
            float ballT = ballCurve != null ? ballCurve.Evaluate(t) : t;

            // Wheel animasyonu - sadece görsel için dönüyor
            float currentWheelAngle = Mathf.Lerp(wheelStartAngle, wheelEndAngle, wheelT);
            wheel.localEulerAngles = new Vector3(0f, currentWheelAngle, 0f);

            // Ball animasyonu
            float ballRadius = Mathf.Lerp(ballStartRadius, ballEndRadius, ballT);
            float ballSpin = Mathf.Lerp(ballStartAngle, ballEndAngle, ballT);

            // Bounce effect
            float bounce = 0f;
            if (t > 0.7f)
            {
                float freq = SegmentCount * 1.8f;
                float localT = Mathf.InverseLerp(0.7f, 1f, t);
                bounce = Mathf.Abs(Mathf.Sin(ballSpin * freq * Mathf.Deg2Rad)) * ballBounceHeight * (1 - localT * 0.6f);
            }

            // ÖNEMLİ: Ball pozisyonu SADECE target segment + ball spin'e bağlı
            // Wheel'ın dönmesi ball pozisyonunu ETKİLEMEZ - BagerTest mantığı
            float ballAngle = targetSegmentAngle + ballSpin;
            
            // BagerTest'teki SetBallPosition'ı BIREBIR kullan
            SetBallPositionExact(ballAngle, ballRadius, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final positioning - BagerTest mantığı ile
        wheel.localEulerAngles = new Vector3(0f, wheelEndAngle, 0f);
        
        // Son durum: sadece target segment açısı (ball spin = 0)
        SetBallPositionExact(targetSegmentAngle, ballEndRadius, 0f);

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [END] Ball pos: {ball.position:F3}, Wheel Y={wheel.localEulerAngles.y:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Final targetSegmentAngle: {targetSegmentAngle:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");

        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

    // BagerTest'teki SetBallPosition'ı TAMAMEN kopyala
    private void SetBallPositionExact(float segmentAngle, float radius, float bounceY = 0f)
    {
        // BagerTest mantığı - BIREBIR
        Vector3 directionToStart = (ballStartPosition - wheelCenter).normalized;
        Vector3 rotatedDirection = Quaternion.Euler(0, segmentAngle, 0) * directionToStart;
        Vector3 newBallPosition = wheelCenter + rotatedDirection * radius;
        
        // Y pozisyonunu ayarla
        newBallPosition.y = ballStartPosition.y + bounceY;
        
        ball.position = newBallPosition;
        
        if (debugCount <= 2) // İlk birkaç test için detaylı log
        {
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] SetBallExact: angle={segmentAngle:F2}, radius={radius:F3}");
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] dirStart={directionToStart:F3}, dirRot={rotatedDirection:F3}");
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] finalPos={newBallPosition:F3}");
            
            // BagerTest'teki verification
            float actualAngle = Vector3.SignedAngle(directionToStart, rotatedDirection, Vector3.up);
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] Actual rotation applied: {actualAngle:F2} degrees");
        }
    }

    // BagerTest'teki FindIndex - BIREBIR
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