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

    [Tooltip("Wheel'ın Z+ yönü (mavi ok) SAHNENDE tam hangi segmenti gösteriyorsa ona göre ayarla. 0:0, 1:32, 2:15, ...")]
    public int upIndex = 0; 
    [Tooltip("Segment ortasına/kenarına tam hizalamak için inspector'dan ayar yap. (Testte eksik geliyorsa segmentSize/2 ekle/çıkar!)")]
    public float angleOffset = 0f;

    public int SegmentCount => wheelOrder.Length;

    public IEnumerator SpinRoulette(int winningNumber, System.Action onComplete = null)
    {
        StringBuilder dbg = new StringBuilder();

        // BAŞLANGIÇ
        wheel.localEulerAngles = Vector3.zero;
        float wheelStartAngle = 0f;

        // Hedef segmentin indexi (wheelOrder'da)
        int realWinIndex = System.Array.IndexOf(wheelOrder, winningNumber);
        int winIndex = (realWinIndex + upIndex + SegmentCount) % SegmentCount;

        float wheelSpinDuration = Random.Range(wheelSpinMin, wheelSpinMax);
        int wheelSpins = Random.Range(minWheelRounds, maxWheelRounds + 1);

        float segmentSize = 360f / SegmentCount;
        float targetAngle = winIndex * segmentSize;
        float wheelEndAngle = wheelStartAngle + 360f * wheelSpins + targetAngle;

        float elapsed = 0f;
        Vector3 wheelCenter = wheel.position;

        dbg.AppendLine($"[START] WinningNumber={winningNumber} | realWinIndex={realWinIndex} | winIndex={winIndex} | segmentSize={segmentSize} | targetAngle={targetAngle}");
        PlaceBallOnSegment(winIndex, wheelStartAngle, wheelCenter);
        dbg.AppendLine($"[START] Ball pos: {ball.position} (should be {winningNumber}'nın üstü), Wheel pos: {wheel.position}");

        while (elapsed < wheelSpinDuration)
        {
            float t = elapsed / wheelSpinDuration;
            float easeT = 1 - Mathf.Pow(1 - t, 3); // cubic-out easing

            float wheelAngle = Mathf.Lerp(wheelStartAngle, wheelEndAngle, easeT);
            wheel.localEulerAngles = new Vector3(0f, wheelAngle, 0f);

            float ballRadius = Mathf.Lerp(ballStartRadius, ballEndRadius, easeT);

            // *** DÜZGÜN SEGMENT HESABI ***
            float angleToSpin = winIndex * segmentSize + wheelAngle + angleOffset;
            // Eğer top segmentin ORTASINDA dursun istiyorsan: angleToSpin += segmentSize / 2f;

            SetBallPosition(angleToSpin, ballRadius, wheelCenter);

            // DEBUG: Son frame'lerde log bas
            if (t > 0.98f)
            {
                dbg.AppendLine($"[SPIN LOOP] winIndex: {winIndex}, segmentSize: {segmentSize}, angleToSpin: {angleToSpin}, WheelAngle: {wheelAngle}, BallPos: {ball.position}");
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Son pozisyonu garantiye al
        float finalWheelAngle = wheelEndAngle % 360f;
        float finalAngleToSpin = winIndex * segmentSize + finalWheelAngle + angleOffset;
        SetBallPosition(finalAngleToSpin, ballEndRadius, wheelCenter);

        dbg.AppendLine($"[FINAL] Ball pos: {ball.position}, angleToSpin: {finalAngleToSpin}, Wheel Y={finalWheelAngle}");
        Debug.Log(dbg.ToString());

        onComplete?.Invoke();
    }

    private void PlaceBallOnSegment(int segIndex, float wheelAngle, Vector3 center)
    {
        float segmentSize = 360f / SegmentCount;
        float angle = segIndex * segmentSize + wheelAngle + angleOffset;
        // Eğer segment ORTASINDA dursun istiyorsan: angle += segmentSize / 2f;
        SetBallPosition(angle, ballStartRadius, center);
    }

    private void SetBallPosition(float angle, float radius, Vector3 center)
    {
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(
            Mathf.Cos(rad) * radius,
            ballBaseHeight,
            Mathf.Sin(rad) * radius
        );
        ball.position = center + offset;
    }
}
