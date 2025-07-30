using UnityEngine;
using System.Collections;
using System.Text;

public class RouletteController : MonoBehaviour
{
    [Header("Wheel & Ball")]
    public Transform wheel;
    public Transform ball;

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

    public int SegmentCount => wheelOrder.Length;

    public IEnumerator SpinRoulette(int winningNumber, System.Action onComplete = null)
    {
        StringBuilder dbg = new StringBuilder();

        // Başlangıç açılarını resetle
        wheel.localEulerAngles = Vector3.zero;
        float wheelStartAngle = 0f;

        int realWinIndex = System.Array.IndexOf(wheelOrder, winningNumber);
        int winIndex = (realWinIndex + SEGMENT_OFFSET + SegmentCount) % SegmentCount;

        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        float ballSpinAfterWheel = Random.Range(ballSpinAfterMin, ballSpinAfterMax);

        int wheelSpins = Random.Range(minWheelRounds, maxWheelRounds + 1);
        float segmentAngle = GetAngleForIndex(winIndex);

        float wheelEndAngle = wheelStartAngle + 360f * wheelSpins + segmentAngle;

        // Ball açısı burada (başlangıç açısı): "wheel ve ball ters yönlere dönecek" rulet fiziği!
        // Ball ilk başta hızlıca çemberde gezsin, sonunda wheel ile birlikte yavaşlasın.
        float ballStartAngle = Random.Range(1440f, 2880f); // Topun başta hızlı dönmesi için (4~8 tam tur)
        float ballEndAngle = 0f; // En sonunda, ball segment üstünde sıfırlansın

        float elapsed = 0f;
        Vector3 wheelCenter = wheel.position;

        dbg.AppendLine($"[START] WinningNumber={winningNumber} | winIndex={winIndex} | wheelStartAngle={wheelStartAngle} | segmentAngle={segmentAngle}");
        PlaceBallOnSegment(0, wheelStartAngle, wheelCenter);
        dbg.AppendLine($"[START] Ball pos: {ball.position} (should be 0'ın üstü), Wheel pos: {wheel.position}");

        // Wheel ve Ball birlikte döner
        while (elapsed < wheelSpinDuration)
        {
            float t = elapsed / wheelSpinDuration;
            float wheelT = wheelCurve != null ? wheelCurve.Evaluate(t) : t;
            float ballT = ballCurve != null ? ballCurve.Evaluate(t) : t;

            float wheelAngle = Mathf.Lerp(wheelStartAngle, wheelEndAngle, wheelT);
            wheel.localEulerAngles = new Vector3(0f, wheelAngle, 0f);

            float ballRadius = Mathf.Lerp(ballStartRadius, ballEndRadius, ballT);
            float ballSpin = Mathf.Lerp(ballStartAngle, ballEndAngle, ballT);

            // Topun gerçek rulet mantığı: wheelAngle + hedef segment açısı + ballSpin
            float targetSegmentAngle = GetAngleForIndex(winIndex);
            float ballAngle = targetSegmentAngle + wheelAngle + ballSpin;

            float bounce = 0f;
            if (t > 0.7f)
            {
                float freq = SegmentCount * 1.8f;
                float localT = Mathf.InverseLerp(0.7f, 1f, t);
                bounce = Mathf.Abs(Mathf.Sin(ballSpin * freq * Mathf.Deg2Rad)) * ballBounceHeight * (1 - localT * 0.6f);
            }

            
            float topSpinStart = Random.Range(1800f, 3240f);
            float topSpinEnd = 0f;
            float tBall = Mathf.Clamp01(elapsed / 2f);
            float easeBall = 1 - Mathf.Pow(1 - tBall, 2);
            float currentBallSpin = Mathf.Lerp(topSpinStart, topSpinEnd, easeBall);

            float wheelY = wheel.transform.eulerAngles.y;
            
            
            var angleToSpin = targetSegmentAngle + wheelY;

            if (angleToSpin < 0)
            {
                angleToSpin += 360f;
            }
            
            
            
            SetBallPosition(angleToSpin + currentBallSpin, ballRadius, wheelCenter, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Wheel durduktan sonra top küçük bir zıplama ile hedef segmentte yavaşça durur
        float wheelFinalAngle = wheelEndAngle;
        // yield return BallSettleAnimation(winIndex, wheelFinalAngle, wheelCenter, dbg);

        dbg.AppendLine($"[END] Ball pos: {ball.position}, Wheel Y={wheel.localEulerAngles.y}");

        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

    // Ball'ı segmentin üstüne ilk kez koymak için (sadece pozisyon, açı animasyonu yok)
    private void PlaceBallOnSegment(int segIndex, float wheelAngle, Vector3 center)
    {
        float segmentAngle = GetAngleForIndex(segIndex);
        float totalAngle = segmentAngle + wheelAngle;
        float rad = totalAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * ballStartRadius,
            ballBaseHeight,
            Mathf.Sin(rad) * ballStartRadius
        );
        ball.position = center + offset;
    }

    // Ball durma animasyonu (segmentte zıplama/oturma efekti)
    private IEnumerator BallSettleAnimation(int winIndex, float wheelAngle, Vector3 wheelCenter, StringBuilder dbg)
    {
        float settleTime = 0.45f;
        float t = 0f;
        float baseRad = (GetAngleForIndex(winIndex) + wheelAngle) * Mathf.Deg2Rad;
        float baseRadius = ballEndRadius;
        float bounceH = ballBounceHeight * 0.7f;
        while (t < 1f)
        {
            float bounce = Mathf.Sin(t * Mathf.PI * 2f) * bounceH * (1 - t);
            Vector3 finalOffset = new Vector3(
                Mathf.Cos(baseRad) * baseRadius,
                ballBaseHeight + bounce,
                Mathf.Sin(baseRad) * baseRadius
            );
            ball.position = wheelCenter + finalOffset;
            t += Time.deltaTime / settleTime;
            yield return null;
        }
        Vector3 stopOffset = new Vector3(
            Mathf.Cos(baseRad) * baseRadius,
            ballBaseHeight,
            Mathf.Sin(baseRad) * baseRadius
        );
        ball.position = wheelCenter + stopOffset;

        dbg.AppendLine($"[SETTLE END] Ball pos: {ball.position}, Segment açısı: {(GetAngleForIndex(winIndex) + wheelAngle) % 360f}");
    }

    // İstenilen segmentin sahnedeki açısını döndürür
    private float GetAngleForIndex(int index)
    {
        float segmentSize = 360f / SegmentCount;
        return (segmentSize * ((index + upIndex) % SegmentCount)) + ANGLE_OFFSET + segmentSize / 2f;
    }

    // Topun world pozisyonunu hesaplar ve uygular
    private void SetBallPosition(float ballAngle, float radius, Vector3 center, float bounceY = 0f)
    {
        float rad = ballAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * radius,
            ballBaseHeight + bounceY,
            Mathf.Sin(rad) * radius
        );
        ball.position = center + offset;
    }
}
