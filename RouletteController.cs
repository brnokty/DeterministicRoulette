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
    public int upIndex = 0; // 0 segmenti wheel'ın forward'ında (gözle ayarla)

    [Header("Segment Fine Tune")]
    [Tooltip("Segment merkezine küçük kayma. 0 = tam segment merkezi, -2 gibi değerlerde hizalama yapabilirsin.")]
    public float ANGLE_OFFSET = 0f;
    [Tooltip("0 ise segment birebir, +1 ise bir segment ileri, -1 geri kaydırır.")]
    public int SEGMENT_OFFSET = 0;

    [Header("Animasyon Eğrileri (İsteğe bağlı)")]
    public AnimationCurve wheelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve ballCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Cache the initial ball position relative to wheel
    private Vector3 initialBallDirection;
    private float initialBallRadius;
    private Vector3 wheelCenter;

    public int SegmentCount => wheelOrder.Length;

    private void Start()
    {
        wheelCenter = wheel.position;
        initialBallDirection = (ball.position - wheelCenter).normalized;
        initialBallRadius = Vector3.Distance(ball.position, wheelCenter);
        
        Debug.Log($"[ROULETTE-DEBUG] Initial ball direction: {initialBallDirection}");
        Debug.Log($"[ROULETTE-DEBUG] Initial ball radius: {initialBallRadius}");
        Debug.Log($"[ROULETTE-DEBUG] Wheel center: {wheelCenter}");
    }

    public IEnumerator SpinRoulette(int winningNumber, System.Action onComplete = null)
    {
        debugCount++;
        StringBuilder dbg = new StringBuilder();

        // Reset wheel angle
        wheel.localEulerAngles = Vector3.zero;
        float wheelStartAngle = 0f;

        // Find the winning segment index
        int realWinIndex = System.Array.IndexOf(wheelOrder, winningNumber);
        if (realWinIndex == -1)
        {
            Debug.LogError($"[ROULETTE-DEBUG-{debugCount}] Winning number {winningNumber} not found in wheel order!");
            yield break;
        }

        int winIndex = (realWinIndex + SEGMENT_OFFSET + SegmentCount) % SegmentCount;

        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        int wheelSpins = Random.Range(minWheelRounds, maxWheelRounds + 1);
        
        // Calculate target segment angle
        float segmentAngle = GetAngleForIndex(winIndex);
        float wheelEndAngle = wheelStartAngle + 360f * wheelSpins + segmentAngle;

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] WinningNumber={winningNumber} | realWinIndex={realWinIndex} | winIndex={winIndex}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] segmentAngle={segmentAngle} | wheelEndAngle={wheelEndAngle}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] wheelSpins={wheelSpins} | duration={wheelSpinDuration}");

        // Ball animation parameters
        float ballStartAngle = Random.Range(1440f, 2880f); // Ball spins fast initially
        float ballEndAngle = 0f; // Ball ends at target position

        float elapsed = 0f;

        // Initial position logging
        PlaceBallOnSegment(0, wheelStartAngle, wheelCenter);
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [START] Ball pos: {ball.position:F3}, Wheel pos: {wheel.position:F3}");

        // Main animation loop
        while (elapsed < wheelSpinDuration)
        {
            float t = elapsed / wheelSpinDuration;
            float wheelT = wheelCurve != null ? wheelCurve.Evaluate(t) : t;
            float ballT = ballCurve != null ? ballCurve.Evaluate(t) : t;

            // Update wheel rotation
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

            // FIXED CALCULATION: Ball should end up on the winning segment
            // The key is to position the ball relative to the target segment
            float targetSegmentAngle = GetAngleForIndex(winIndex);
            
            // Ball's total angle combines: wheel rotation + segment offset + ball's own movement
            float totalBallAngle = currentWheelAngle + targetSegmentAngle + ballSpin;
            
            SetBallPosition(totalBallAngle, ballRadius, wheelCenter, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final positioning
        float finalWheelAngle = wheelEndAngle;
        wheel.localEulerAngles = new Vector3(0f, finalWheelAngle, 0f);
        
        // Place ball exactly on the winning segment
        float finalSegmentAngle = GetAngleForIndex(winIndex);
        float finalBallAngle = finalWheelAngle + finalSegmentAngle;
        SetBallPosition(finalBallAngle, ballEndRadius, wheelCenter, 0f);

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [END] Ball pos: {ball.position:F3}, Wheel Y={wheel.localEulerAngles.y:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Final segment angle: {finalSegmentAngle:F2}, Final ball angle: {finalBallAngle:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");

        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

    private void PlaceBallOnSegment(int segIndex, float wheelAngle, Vector3 center)
    {
        float segmentAngle = GetAngleForIndex(segIndex);
        float totalAngle = segmentAngle + wheelAngle;
        SetBallPosition(totalAngle, ballStartRadius, center, 0f);
    }

    private float GetAngleForIndex(int index)
    {
        float segmentSize = 360f / SegmentCount;
        float baseAngle = segmentSize * index;
        
        // Add offset to center the segment
        float centeredAngle = baseAngle + (segmentSize * 0.5f);
        
        // Apply fine-tuning
        float finalAngle = centeredAngle + ANGLE_OFFSET;
        
        Debug.Log($"[ROULETTE-DEBUG-{debugCount}] GetAngleForIndex({index}): base={baseAngle:F2}, centered={centeredAngle:F2}, final={finalAngle:F2}");
        
        return finalAngle;
    }

    public void SetBallPosition(float angle, float radius, Vector3 center, float bounceY = 0f)
    {
        // Convert angle to radians
        float rad = angle * Mathf.Deg2Rad;
        
        // Calculate position using trigonometry
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * radius,
            ballBaseHeight + bounceY,
            Mathf.Sin(rad) * radius
        );
        
        ball.position = center + offset;
    }
}