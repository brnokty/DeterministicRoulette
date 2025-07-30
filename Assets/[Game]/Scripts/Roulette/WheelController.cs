using UnityEngine;
using System.Collections;
using System.Text;

public class WheelController : MonoBehaviour
{
    [Header("Wheel & Ball")]
    public Transform wheel;
    public Transform ball;

    [Header("Wheel Spin Time (seconds)")]
    public float wheelSpinMin = 2.7f;
    public float wheelSpinMax = 4.1f;
    public float ballSpinAfterMin = 0.8f;
    public float ballSpinAfterMax = 1.6f;

    [Header("Ball Settings")]
    public float ballStartRadius = 0.325f;
    public float ballEndRadius = 0.29f;
    public float ballBaseHeight = 0.05f;
    public float ballBounceHeight = 0.02f;

    [Header("Animation")]
    public AnimationCurve wheelCurve;
    public AnimationCurve ballCurve;

    [Header("Wheel Segment Order")]
    public int[] wheelOrder = new int[] {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };
    public int SegmentCount => wheelOrder.Length;

    [Header("Segment Fine Tune")]
    public int SEGMENT_OFFSET = 0;
    public float ANGLE_OFFSET = 0f; // segment merkezi için küçük ince ayar (ör. -1.5)

    public IEnumerator SpinWheel(int winningNumber, System.Action onComplete)
    {
        StringBuilder dbg = new StringBuilder();

        // Wheel Y rotasyonu her zaman 0 olmalı (parent objede ayarla!)
        wheel.localEulerAngles = Vector3.zero;
        float wheelStartAngle = 0f;

        int realWinIndex = System.Array.IndexOf(wheelOrder, winningNumber);
        int winIndex = (realWinIndex + SEGMENT_OFFSET + SegmentCount) % SegmentCount;

        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        float ballSpinAfterWheel = Random.Range(ballSpinAfterMin, ballSpinAfterMax);

        int wheelSpins = Random.Range(5, 7);
        float segmentAngle = GetAngleForIndex(winIndex);

        float wheelEndAngle = wheelStartAngle + 360f * wheelSpins + segmentAngle;

        float ballStartAngle = 0f;
        int ballSpins = wheelSpins + Random.Range(4, 7);
        float ballEndAngle = ballStartAngle - 360f * ballSpins - segmentAngle;

        float elapsed = 0f;
        Vector3 wheelCenter = wheel.position;

        dbg.AppendLine($"[START] WinningNumber={winningNumber} | winIndex={winIndex} | wheelStartAngle={wheelStartAngle} | segmentAngle={segmentAngle}");
        PlaceBallOnSegment(0, wheelStartAngle, wheelCenter);
        dbg.AppendLine($"[START] Ball pos: {ball.position} (should be 0'ın üstü), Wheel pos: {wheel.position}");

        while (elapsed < wheelSpinDuration)
        {
            float t = elapsed / wheelSpinDuration;
            float wheelT = wheelCurve != null ? wheelCurve.Evaluate(t) : t;
            float ballT = wheelT;

            float wheelAngle = Mathf.Lerp(wheelStartAngle, wheelEndAngle, wheelT);
            wheel.localEulerAngles = new Vector3(0f, wheelAngle, 0f);

            float ballRadius = Mathf.Lerp(ballStartRadius, ballEndRadius, ballT);
            float ballAngle = Mathf.Lerp(ballStartAngle, ballEndAngle, ballT);

            float bounce = 0f;
            if (t > 0.7f)
            {
                float freq = SegmentCount * 1.8f;
                float localT = Mathf.InverseLerp(0.7f, 1f, t);
                bounce = Mathf.Abs(Mathf.Sin(ballAngle * freq * Mathf.Deg2Rad)) * ballBounceHeight * (1 - localT * 0.6f);
            }

            SetBallPosition(ballAngle, wheelAngle, ballRadius, wheelCenter, bounce);

            elapsed += Time.deltaTime;
            yield return null;
        }

        float ballRadiusFinal = Mathf.Lerp(ballStartRadius, ballEndRadius, 1f);
        float ballAngleAtWheelStop = Mathf.Lerp(ballStartAngle, ballEndAngle, 1f);
        float wheelFinalAngle = wheelEndAngle;

        float ballSpinElapsed = 0f;
        float ballExtraAngle = 360f * Random.Range(1.3f, 2.2f);
        float ballStopAngle = ballAngleAtWheelStop - ballExtraAngle;

        while (ballSpinElapsed < ballSpinAfterWheel)
        {
            float t = ballSpinElapsed / ballSpinAfterWheel;
            float ballT = ballCurve != null ? ballCurve.Evaluate(t) : t;
            float ballAngle = Mathf.Lerp(ballAngleAtWheelStop, ballStopAngle, ballT);

            float bounce = 0f;
            float freq = SegmentCount * Mathf.Lerp(1.5f, 0.6f, ballT);
            bounce = Mathf.Abs(Mathf.Sin(ballAngle * freq * Mathf.Deg2Rad)) * ballBounceHeight * (1 - ballT * 0.8f);

            SetBallPosition(ballAngle, wheelFinalAngle, ballRadiusFinal, wheelCenter, bounce);

            ballSpinElapsed += Time.deltaTime;
            yield return null;
        }

        dbg.AppendLine($"[SETTLE START] winIndex={winIndex} | wheelFinalAngle={wheelFinalAngle}");

        yield return BallSettleAnimation(winIndex, wheelFinalAngle, wheelCenter, dbg);

        dbg.AppendLine($"[END] Ball pos: {ball.position}, Wheel Y={wheel.localEulerAngles.y}");

        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

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

    private float GetAngleForIndex(int index)
    {
        float segmentSize = 360f / SegmentCount;
        return (segmentSize * index) + ANGLE_OFFSET + segmentSize / 2f;
    }

    private void SetBallPosition(float ballAngle, float wheelAngle, float radius, Vector3 center, float bounceY = 0f)
    {
        float totalAngle = ballAngle + wheelAngle;
        float rad = totalAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * radius,
            ballBaseHeight + bounceY,
            Mathf.Sin(rad) * radius
        );
        ball.position = center + offset;
    }
}
