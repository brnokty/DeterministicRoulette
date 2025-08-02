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

    [Header("BagerTest Reference")]
    [Tooltip("BagerTest script'ini buraya sürükle - onun çalışan SetBallPosition metodunu kullanacağız")]
    public BagerTest bagerTest;

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

    private Vector3 ballStartPosition;
    private Vector3 wheelCenter;

    public int SegmentCount => wheelOrder.Length;

    private void Start()
    {
        ballStartPosition = ball.position;
        wheelCenter = wheel.position;
        
        if (bagerTest == null)
        {
            Debug.LogError("[ROULETTE] BagerTest reference is missing! Drag BagerTest script to the field.");
        }
        
        Debug.Log($"[ROULETTE-DEBUG] Ball Start Position: {ballStartPosition}");
        Debug.Log($"[ROULETTE-DEBUG] Wheel Center: {wheelCenter}");
        Debug.Log($"[ROULETTE-DEBUG] BagerTest reference: {(bagerTest != null ? "OK" : "MISSING")}");
    }

    public IEnumerator SpinRoulette(int winningNumber, System.Action onComplete = null)
    {
        debugCount++;
        StringBuilder dbg = new StringBuilder();

        if (bagerTest == null)
        {
            Debug.LogError($"[ROULETTE-DEBUG-{debugCount}] BagerTest reference is missing!");
            yield break;
        }

        // BagerTest'teki segment hesaplama mantığını kullan
        int index = bagerTest.FindIndex(wheelOrder, winningNumber);
        if (index == -1)
        {
            Debug.LogError($"[ROULETTE-DEBUG-{debugCount}] Number {winningNumber} not found in wheel order!");
            yield break;
        }

        // BagerTest'teki segment hesaplaması
        float segmentSize = 360f / wheelOrder.Length;
        float targetSegmentAngle = segmentSize * index;

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Using BagerTest logic for: {winningNumber}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Found Index: {index}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Target Segment Angle: {targetSegmentAngle}");

        // Animasyon parametreleri
        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        int wheelSpins = Random.Range(minWheelRounds, maxWheelRounds + 1);
        float totalWheelRotation = 360f * wheelSpins;
        
        float wheelStartAngle = 0f;
        float wheelEndAngle = wheelStartAngle + totalWheelRotation;

        // Ball animasyon parametreleri
        float ballStartAngle = Random.Range(1440f, 2880f); 
        float ballEndAngle = 0f; 

        float elapsed = 0f;

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [START] Ball pos: {ball.position:F3}");

        // Ana animasyon döngüsü
        while (elapsed < wheelSpinDuration)
        {
            float t = elapsed / wheelSpinDuration;
            float wheelT = wheelCurve != null ? wheelCurve.Evaluate(t) : t;
            float ballT = ballCurve != null ? ballCurve.Evaluate(t) : t;

            // Wheel animasyonu
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

            // BagerTest'in çalışan Calculate metodunu çağır ama ball spin ekle
            float ballAngle = targetSegmentAngle + ballSpin;
            
            // BagerTest'teki SetBallPosition mantığını direkt kullan ama radius ve bounce ekle
            UseBagerTestLogic(ballAngle, ballRadius, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final positioning - BagerTest'teki tam mantık
        wheel.localEulerAngles = new Vector3(0f, wheelEndAngle, 0f);
        
        // SON DURUM: BagerTest'teki Calculate() metodunu direkt çağır
        // Ama önce input field'ı ayarla
        if (bagerTest.inputField != null)
        {
            bagerTest.inputField.text = winningNumber.ToString();
            bagerTest.Calculate(); // BagerTest'teki tam çalışan metod!
        }
        else
        {
            // Input field yoksa manuel hesapla
            UseBagerTestLogic(targetSegmentAngle, ballEndRadius, 0f);
        }

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [END] Ball pos: {ball.position:F3}, Wheel Y={wheel.localEulerAngles.y:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Final calculation done by BagerTest.Calculate()");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");

        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

    // BagerTest'teki SetBallPosition mantığını kullan ama radius kontrolü ekle
    private void UseBagerTestLogic(float segmentAngle, float radius, float bounceY = 0f)
    {
        // BagerTest'teki mantığı birebir kopyala
        Vector3 ballStartPos = ballStartPosition;
        Vector3 wheelCenter = this.wheelCenter;
        
        // Reset ball to start position first (BagerTest'teki gibi)
        ball.position = ballStartPos;
        
        // Calculate the direction from wheel center to ball start position
        Vector3 directionToStart = (ballStartPos - wheelCenter).normalized;
        
        // Rotate this direction by the segment angle
        Vector3 rotatedDirection = Quaternion.Euler(0, segmentAngle, 0) * directionToStart;
        
        // Place ball at the rotated position with custom radius
        Vector3 newBallPosition = wheelCenter + rotatedDirection * radius;
        
        // Y pozisyonunu ayarla
        newBallPosition.y = ballStartPos.y + bounceY;
        
        ball.position = newBallPosition;
        
        if (debugCount <= 2)
        {
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] UseBagerTestLogic: angle={segmentAngle:F2}, radius={radius:F3}");
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] ballStartPos={ballStartPos:F3}, wheelCenter={wheelCenter:F3}");
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] directionToStart={directionToStart:F3}, rotatedDirection={rotatedDirection:F3}");
            Debug.Log($"[ROULETTE-DEBUG-{debugCount}] finalPos={newBallPosition:F3}");
        }
    }
}