using UnityEngine;
using UnityEngine.UI;

public class RuletGameManager : MonoBehaviour
{
    public RuletController ruletController;
    public BallRotator ballRotator;

    public float ruletMinDegree = 1080f;
    public float ruletMaxDegree = 2160f;
    public float ruletMinDuration = 2f;
    public float ruletMaxDuration = 4f;
    public float ballSettleExtraTime = 2f; // Topun dönüş süresi ruletten ne kadar fazla olsun
    public InputField inputField;
    public SmoothRotator smoothRotator;
    public float ballSpeed = 0.5f;
    
    [Header("Wheel Segment Order")]
    public int[] wheelOrder = new int[] {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartSpin();
            smoothRotator.StartRotating();
        }
    }

    public void StartSpin()
    {
        float ruletDelta = Random.Range(ruletMinDegree, ruletMaxDegree);
        float ruletDuration = Random.Range(ruletMinDuration, ruletMaxDuration);

        // 1) Spin başlarken top ruleti takip etsin
        ballRotator.StartFollowing();

        ruletController.SpinTo(ruletDelta, ruletDuration, (ruletFinalY) =>
        {
            // 2) Rulet durunca topun açısını ruletle eşitle!
            var calculatedAngle = CalculateAngle();
            print("Calculated Angle: " + calculatedAngle);
            // smoothRotator.RotateToTarget(calculatedAngle);
            smoothRotator.BagerRotateByDegree(calculatedAngle,ballSpeed);
            ballRotator.SetAngleDirect(calculatedAngle);

            // 3) Topa hedef açıyı ve süreyi söyle, easing ile dönmeye başlasın
            float ballFinalAngle = Random.Range(0f, 360f); // örnek olarak random
            float ballSettleDuration = ruletDuration + ballSettleExtraTime;
            ballRotator.SettleTo(ballFinalAngle, ballSettleDuration);
        });
    }
    
    public float CalculateAngle()
    {
        var inputText = inputField.text;

        var index = FindIndex(wheelOrder, int.Parse(inputText));
        print("BGR Index: " + index);
        var wheelAngle = ruletController.transform.localEulerAngles.y;
        var targetAngle = index * (360f / wheelOrder.Length);
        var angleToSpin = targetAngle + wheelAngle;

        print("target angle: " + targetAngle);
        
        if (angleToSpin < 0)
        {
            angleToSpin += 360f;
        }

        return angleToSpin;
    }
    
    public int FindIndex(int[] array, int value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
            {
                return i;
            }
        }

        // Eleman bulunamazsa -1 döndürür
        return -1;
    }
}