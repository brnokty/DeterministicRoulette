using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class RuletGameManager : MonoBehaviour
{
    public WheelHandler wheelHandler;
    public float ruletMinDegree = 1080f;
    public float ruletMaxDegree = 2160f;
    public float ruletMinDuration = 2f;
    public float ruletMaxDuration = 4f;
    public BallHandler ballHandler;
    private float ballSpeed = 0.5f;
    public float minBallSpeed = 2f;
    public float maxBallSpeed = 4f;

    [Header("Wheel Segment Order")] public int[] wheelOrder = new int[]
    {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };


    public void StartSpin(int number, System.Action onComplete)
    {
        ballHandler.StartRotating();
        float ruletDelta = Random.Range(ruletMinDegree, ruletMaxDegree);
        float ruletDuration = Random.Range(ruletMinDuration, ruletMaxDuration);

        wheelHandler.SpinTo(ruletDelta, ruletDuration, (ruletFinalY) =>
        {
            ballSpeed = Random.Range(minBallSpeed, maxBallSpeed);
            var calculatedAngle = CalculateAngle(number);
            print("Calculated Angle: " + calculatedAngle);
            ballHandler.BagerRotateByDegree(calculatedAngle, ballSpeed,onComplete);
        });
    }

    public float CalculateAngle(int number = 0)
    {
        var index = FindIndex(wheelOrder, number);
        print("BGR Index: " + index);
        var wheelAngle = wheelHandler.transform.localEulerAngles.y;
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