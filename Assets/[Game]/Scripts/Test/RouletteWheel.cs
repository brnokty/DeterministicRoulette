using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RouletteWheel : MonoBehaviour
{
    [Header("References")]
    public Transform wheel;       // Çark
    public Transform ball;        // Top (wheel'in üstünde, çarkın merkezinden uzağa yerleştir)
    public Button spinButton;     // Spin butonu
    public InputField inputField; // InputField

    [Header("Wheel Settings")]
    public int[] wheelOrder = new int[] {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };
    public float spinTime = 4f;
    public int spinRounds = 4;
    public int ballRounds = 8; // Top daha hızlı/çok dönecek (roulette gibi)

    private bool isSpinning = false;

    void Start()
    {
        spinButton.onClick.AddListener(SpinToInputNumber);
    }

    public void SpinToInputNumber()
    {
        if (isSpinning) return;

        int inputNumber;
        if (!int.TryParse(inputField.text, out inputNumber))
        {
            Debug.LogWarning("Geçerli bir sayı girilmedi!");
            return;
        }

        int winIndex = System.Array.IndexOf(wheelOrder, inputNumber);
        if (winIndex < 0)
        {
            Debug.LogWarning("Girilen sayı segmentlerde yok!");
            return;
        }

        float segmentAngle = 360f / wheelOrder.Length;
        float targetAngle = winIndex * segmentAngle + segmentAngle / 2f;
        float wheelTargetAngle = spinRounds * 360f + targetAngle;
        float ballTargetAngle = -ballRounds * 360f - targetAngle; // Ters yöne dönecek!

        StartCoroutine(SpinRoutine(wheelTargetAngle, ballTargetAngle));
    }

    IEnumerator SpinRoutine(float wheelTarget, float ballTarget)
    {
        isSpinning = true;

        float wheelStart = wheel.localEulerAngles.y;
        float ballStart = ball.localEulerAngles.y;

        // Unity'de açılar 0-360 arası çalıştığı için, 360 üzerini sarmalamak gerekebilir.
        float wheelEnd = wheelStart + wheelTarget;
        float ballEnd = ballStart + ballTarget;

        float duration = spinTime;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float easedT = Mathf.SmoothStep(0, 1, t);

            float currentWheelAngle = Mathf.Lerp(wheelStart, wheelEnd, easedT);
            float currentBallAngle = Mathf.Lerp(ballStart, ballEnd, easedT);

            wheel.localEulerAngles = new Vector3(0, currentWheelAngle, 0);
            ball.localEulerAngles = new Vector3(0, currentBallAngle, 0);

            yield return null;
        }

        // Sabitle
        wheel.localEulerAngles = new Vector3(0, wheelEnd, 0);
        ball.localEulerAngles = new Vector3(0, ballEnd, 0);

        isSpinning = false;
    }
}
