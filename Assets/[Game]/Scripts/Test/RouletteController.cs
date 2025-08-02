using UnityEngine;
using System.Collections;
using System.Text;

public class RouletteController : MonoBehaviour
{
    [Header("Wheel & Ball")]
    public Transform wheel;
    public Transform ball;

    [Header("Spin Settings")]
    [SerializeField] public float spinDuration = 3f;
    [SerializeField] public int minWheelTurns = 5;
    [SerializeField] public int maxWheelTurns = 8;

    [Header("Ball Movement")]
    [SerializeField] public float ballStartRadius = 0.35f;
    [SerializeField] public float ballEndRadius = 0.29f;
    [SerializeField] public float ballHeight = 0.05f;
    [SerializeField] public int ballTurns = 12; // Ball kaç tur atacak

    [Header("Wheel Order - BagerTest ile AYNI")]
    public int[] wheelOrder = new int[] {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };

    [Header("Animation Curves")]
    public AnimationCurve wheelSpinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve ballSpinCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Test & Debug")]
    [SerializeField] public int testWinningNumber = 15;
    [SerializeField] public int debugCount = 0;

    // BagerTest mantığı için gerekli değişkenler
    private Vector3 ballStartPosition;
    private Vector3 wheelCenter;
    private float segmentSize;

    private void Start()
    {
        // Wheel center'ı al
        wheelCenter = wheel.position;
        segmentSize = 360f / wheelOrder.Length;

        // Ball'ı wheel etrafında doğru pozisyona yerleştir
        SetupBallStartPosition();

        Debug.Log($"[ROULETTE] Initialized - Ball Start: {ballStartPosition}, Wheel Center: {wheelCenter}");
        Debug.Log($"[ROULETTE] Ball-Wheel distance: {Vector3.Distance(ballStartPosition, wheelCenter)}");
        Debug.Log($"[ROULETTE] Segment Size: {segmentSize} degrees, Total Segments: {wheelOrder.Length}");
    }

    private void SetupBallStartPosition()
    {
        // Ball'ı wheel'ın sağında, uygun mesafede konumlandır
        Vector3 offset = new Vector3(ballStartRadius, ballHeight, 0f);
        ballStartPosition = wheelCenter + offset;
        ball.position = ballStartPosition;
        
        Debug.Log($"[ROULETTE] Ball positioned at distance {ballStartRadius} from wheel center");
        Debug.Log($"[ROULETTE] Ball world position: {ballStartPosition}");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log($"[ROULETTE] Starting spin for number: {testWinningNumber}");
            StartCoroutine(SpinRoulette(testWinningNumber));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // Reset pozisyonu
            SetupBallStartPosition(); // Doğru pozisyona resetle
            wheel.localEulerAngles = Vector3.zero;
            Debug.Log("[ROULETTE] Reset to initial positions");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            // Test ball positioning
            TestBallPositioning();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            // Toggle gizmo debug info
            Debug.Log($"[ROULETTE] Ball-Wheel Distance: {Vector3.Distance(ball.position, wheelCenter):F3}");
            Debug.Log($"[ROULETTE] Expected Distance: {ballStartRadius:F3}");
        }
    }

    private void TestBallPositioning()
    {
        Debug.Log("[ROULETTE] Testing ball positioning around wheel...");
        
        for (int i = 0; i < 8; i++)
        {
            float testAngle = i * 45f; // Every 45 degrees
            SetBallPositionTrigonometric(testAngle, ballStartRadius);
            Debug.Log($"Angle {testAngle}° -> Ball pos: {ball.position:F2}");
        }
        
        // Reset to start position
        SetupBallStartPosition();
    }

    public IEnumerator SpinRoulette(int winningNumber, System.Action onComplete = null)
    {
        debugCount++;
        
        Debug.Log($"[ROULETTE-{debugCount}] ========== SPIN START ==========");
        Debug.Log($"[ROULETTE-{debugCount}] Winning Number: {winningNumber}");

        // BagerTest'teki FindIndex mantığını kullan
        int winIndex = FindIndex(wheelOrder, winningNumber);
        if (winIndex == -1)
        {
            Debug.LogError($"[ROULETTE-{debugCount}] Number {winningNumber} not found!");
            yield break;
        }

        // BagerTest'teki segment hesaplama mantığını kullan
        float targetSegmentAngle = segmentSize * winIndex;
        
        Debug.Log($"[ROULETTE-{debugCount}] Win Index: {winIndex}, Target Angle: {targetSegmentAngle}");

        // Animasyon parametreleri
        int wheelTurns = Random.Range(minWheelTurns, maxWheelTurns + 1);
        float totalWheelRotation = wheelTurns * 360f;
        
        // Wheel son açısı: toplam dönüş + hedef segment açısı
        float wheelFinalAngle = totalWheelRotation + targetSegmentAngle;

        Debug.Log($"[ROULETTE-{debugCount}] Wheel turns: {wheelTurns}, Final angle: {wheelFinalAngle}");
        Debug.Log($"[ROULETTE-{debugCount}] Ball turns: {ballTurns}");

        float elapsed = 0f;
        float wheelStartAngle = wheel.localEulerAngles.y;
        float ballTotalRotation = ballTurns * 360f;

        Vector3 initialBallPos = ball.position;
        Debug.Log($"[ROULETTE-{debugCount}] Initial ball pos: {initialBallPos}");

        // Animasyon döngüsü
        while (elapsed < spinDuration)
        {
            float t = elapsed / spinDuration;
            
            // Wheel dönüşü
            float wheelT = wheelSpinCurve.Evaluate(t);
            float currentWheelAngle = Mathf.Lerp(wheelStartAngle, wheelFinalAngle, wheelT);
            wheel.localEulerAngles = new Vector3(0, currentWheelAngle, 0);

            // Ball dönüşü ve radius
            float ballT = ballSpinCurve.Evaluate(t);
            float ballRadius = Mathf.Lerp(ballStartRadius, ballEndRadius, t);
            
            // Ball açısı - başta hızlı, sonda yavaş
            float ballAngle = ballTotalRotation * (1 - ballT);

            // Ball pozisyonunu hesapla - trigonometrik
            SetBallPositionTrigonometric(ballAngle, ballRadius);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Animasyon bitti - şimdi BagerTest mantığını kullan
        wheel.localEulerAngles = new Vector3(0, wheelFinalAngle, 0);

        Debug.Log($"[ROULETTE-{debugCount}] Animation finished. Using BagerTest logic...");
        Debug.Log($"[ROULETTE-{debugCount}] Ball pos before BagerTest logic: {ball.position}");

        // BagerTest'teki SetBallPosition mantığını BIREBIR kullan
        SetBallPositionLikeBagerTest(targetSegmentAngle);

        Debug.Log($"[ROULETTE-{debugCount}] Ball pos after BagerTest logic: {ball.position}");
        Debug.Log($"[ROULETTE-{debugCount}] ========== SPIN END ==========");

        onComplete?.Invoke();
    }

    // BagerTest'teki SetBallPosition mantığını TAMAMEN kopyala
    private void SetBallPositionLikeBagerTest(float segmentAngle)
    {
        // BagerTest'teki EXACT kod:
        
        // Reset ball to start position first
        ball.position = ballStartPosition;
        
        // Calculate the direction from wheel center to ball start position
        Vector3 directionToStart = (ballStartPosition - wheelCenter).normalized;
        float startRadius = Vector3.Distance(ballStartPosition, wheelCenter);
        
        // Rotate this direction by the segment angle
        Vector3 rotatedDirection = Quaternion.Euler(0, segmentAngle, 0) * directionToStart;
        
        // Place ball at the rotated position
        Vector3 newBallPosition = wheelCenter + rotatedDirection * startRadius;
        ball.position = newBallPosition;
        
        Debug.Log($"[ROULETTE-{debugCount}] BagerTest Logic Applied:");
        Debug.Log($"[ROULETTE-{debugCount}] - Segment Angle: {segmentAngle}");
        Debug.Log($"[ROULETTE-{debugCount}] - Direction: {directionToStart} -> {rotatedDirection}");
        Debug.Log($"[ROULETTE-{debugCount}] - Radius: {startRadius}");
        Debug.Log($"[ROULETTE-{debugCount}] - Final Position: {newBallPosition}");
    }

    // Animasyon sırasında ball pozisyonu - trigonometrik
    private void SetBallPositionTrigonometric(float angle, float radius)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 pos = wheelCenter + new Vector3(
            Mathf.Cos(rad) * radius,
            ballHeight,
            Mathf.Sin(rad) * radius
        );
        ball.position = pos;
        
        // Debug visualization (first few frames only)
        if (debugCount <= 2 && Time.frameCount % 30 == 0) // Every 30 frames
        {
            Debug.Log($"[ROULETTE-{debugCount}] Ball Animation: angle={angle:F1}°, radius={radius:F3}, pos={pos:F2}");
        }
    }

    // Visual debug gizmo
    private void OnDrawGizmos()
    {
        if (wheel != null && ball != null)
        {
            // Draw wheel center
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(wheelCenter, 0.02f);
            
            // Draw ball start position
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(ballStartPosition, 0.015f);
            
            // Draw ball current position
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(ball.position, 0.01f);
            
            // Draw ball orbit circles
            Gizmos.color = Color.yellow;
            DrawCircle(wheelCenter, ballStartRadius, 32);
            
            Gizmos.color = Color.orange;
            DrawCircle(wheelCenter, ballEndRadius, 32);
            
            // Draw line from wheel center to ball
            Gizmos.color = Color.white;
            Gizmos.DrawLine(wheelCenter, ball.position);
        }
    }

    private void DrawCircle(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(radius, 0, 0);
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                0,
                Mathf.Sin(angle) * radius
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
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

    // Public metod - dışarıdan çağırılabilir
    public void SpinForNumber(int number)
    {
        StartCoroutine(SpinRoulette(number));
    }

    // Alternative public method for external calls
    public void StartSpin(int number)
    {
        StartCoroutine(SpinRoulette(number));
    }

    // Coroutine wrapper for external use
    public void StartSpinWithCallback(int number, System.Action callback)
    {
        StartCoroutine(SpinRoulette(number, callback));
    }

    // Editor'da test için
    [ContextMenu("Test Spin")]
    private void TestSpin()
    {
        StartCoroutine(SpinRoulette(testWinningNumber));
    }
}