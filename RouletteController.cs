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
    [Tooltip("BagerTest script'ini buraya sürükle - animasyon bitince onun Calculate metodunu çağıracağız")]
    public BagerTest bagerTest;

    [Header("Wheel Spin")]
    public float wheelSpinMin = 2.7f;
    public float wheelSpinMax = 4.1f;
    public int minWheelRounds = 5;
    public int maxWheelRounds = 7;

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

    [Header("Animasyon Eğrileri")]
    public AnimationCurve wheelCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve ballCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Test")]
    [Tooltip("Test için bir sayı gir ve T tuşuna bas")]
    public int testNumber = 15;

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

    private void Update()
    {
        // Test için T tuşu
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"[TEST] Starting RouletteController test with number: {testNumber}");
            StartCoroutine(SpinRoulette(testNumber));
        }
        
        // BagerTest direkt testi için B tuşu
        if (Input.GetKeyDown(KeyCode.B) && bagerTest != null)
        {
            Debug.Log($"[TEST] Testing BagerTest directly with number: {testNumber}");
            if (bagerTest.inputField != null)
            {
                bagerTest.inputField.text = testNumber.ToString();
                bagerTest.Calculate();
            }
        }
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

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Starting animation for winning number: {winningNumber}");

        // Animasyon parametreleri
        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        int wheelSpins = Random.Range(minWheelRounds, maxWheelRounds + 1);
        float totalWheelRotation = 360f * wheelSpins;
        
        float wheelStartAngle = 0f;
        float wheelEndAngle = wheelStartAngle + totalWheelRotation;

        // Ball animasyon parametreleri - gerçekçi hareket için
        float ballStartSpeed = Random.Range(720f, 1440f); // Başlangıç hızı
        float ballEndSpeed = 0f; // Bitiş hızı
        float ballCurrentAngle = 0f; // Ball'ın mevcut açısı

        float elapsed = 0f;

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Animation: {wheelSpins} wheel spins, {wheelSpinDuration:F2}s duration");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Ball start speed: {ballStartSpeed:F0} deg/s");
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

            // Ball gerçekçi hareket - hız yavaş yavaş azalır
            float currentBallSpeed = Mathf.Lerp(ballStartSpeed, ballEndSpeed, ballT);
            ballCurrentAngle += currentBallSpeed * Time.deltaTime;
            
            float ballRadius = Mathf.Lerp(ballStartRadius, ballEndRadius, ballT);

            // Bounce effect
            float bounce = 0f;
            if (t > 0.7f)
            {
                float freq = SegmentCount * 1.8f;
                float localT = Mathf.InverseLerp(0.7f, 1f, t);
                bounce = Mathf.Abs(Mathf.Sin(ballCurrentAngle * freq * Mathf.Deg2Rad)) * ballBounceHeight * (1 - localT * 0.6f);
            }

            // Ball pozisyonu - çemberde gerçekçi hareket
            RealisticBallMovement(ballCurrentAngle, ballRadius, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final wheel position
        wheel.localEulerAngles = new Vector3(0f, wheelEndAngle, 0f);
        
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Animation finished. Wheel final angle: {wheelEndAngle:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Ball final animation angle: {ballCurrentAngle:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Ball position before BagerTest.Calculate(): {ball.position:F3}");

        // ÖNCE BagerTest'in doğru çalışıp çalışmadığını test et
        if (bagerTest.inputField != null)
        {
            string oldText = bagerTest.inputField.text;
            
            dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Input field old text: '{oldText}'");
            
            bagerTest.inputField.text = winningNumber.ToString();
            
            dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Input field set to: '{bagerTest.inputField.text}'");
            dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Calling BagerTest.Calculate()...");
            
            // BagerTest'in debug count'unu da artır
            bagerTest.debugCount++;
            bagerTest.Calculate();
            
            dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] BagerTest.Calculate() completed.");
            dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Ball position after BagerTest.Calculate(): {ball.position:F3}");
            
            // Input field'ı eski haline döndür
            bagerTest.inputField.text = oldText;
            
            dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] Input field restored to: '{bagerTest.inputField.text}'");
        }
        else
        {
            Debug.LogError($"[ROULETTE-DEBUG-{debugCount}] BagerTest.inputField is null!");
        }

        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] [FINAL] Ball pos: {ball.position:F3}, Wheel Y: {wheel.localEulerAngles.y:F2}");
        dbg.AppendLine($"[ROULETTE-DEBUG-{debugCount}] ===========================================");

        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

    // Gerçekçi ball hareketi - çemberde sürekli döner
    private void RealisticBallMovement(float ballAngle, float radius, float bounceY = 0f)
    {
        float rad = ballAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * radius,
            ballBaseHeight + bounceY,
            Mathf.Sin(rad) * radius
        );
        ball.position = wheelCenter + offset;
    }
}