using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BagerTest : MonoBehaviour
{
    public Transform wheel;
    public Transform ball;
    [Header("UI")] public InputField inputField;
   

    [Header("Rulet Sıralaması")] public int[] wheelOrder = new int[]
    {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };
    Vector3 ballStartPosition;
    
    private void Start()
    {
        ballStartPosition = ball.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Calculate();
        }
    }

    public void Calculate()
    {
        var inputText = inputField.text;

        var index = FindIndex(wheelOrder, int.Parse(inputText));
        print("BGR Index: " + index);
        var wheelAngle = wheel.localEulerAngles.y;
        var targetAngle = index * (360f / wheelOrder.Length);
        var angleToSpin = targetAngle + wheelAngle;

        if (angleToSpin < 0)
        {
            angleToSpin += 360f;
        }

        SetBallPosition(angleToSpin);
    }

    //set ball position based on the wheel angle
    private void SetBallPosition(float wheelAngle)
    {
        ball.position= ballStartPosition; // Reset position to start
        // ball.RotateAround(wheel.position, Vector3.up, wheelAngle);
        Vector3 dir = ball.position - wheel.position;
        dir = Quaternion.Euler(0, wheelAngle, 0) * dir; // 90 derece Y ekseninde döndür
        ball.position = wheel.position + dir;
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