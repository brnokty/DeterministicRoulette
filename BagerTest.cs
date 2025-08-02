using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BagerTest : MonoBehaviour
{
    public Transform wheel;
    public Transform ball;
    [Header("UI")] public InputField inputField;
   
    [Header("Debug")]
    public int debugCount = 0;

    [Header("Rulet Sıralaması")] public int[] wheelOrder = new int[]
    {
        0, 32, 15, 19, 4, 21, 2, 25, 17, 34, 6, 27,
        13, 36, 11, 30, 8, 23, 10, 5, 24,
        16, 33, 1, 20, 14, 31, 9, 22, 18,
        29, 7, 28, 12, 35, 3, 26
    };
    
    Vector3 ballStartPosition;
    Vector3 wheelCenter;
    
    private void Start()
    {
        ballStartPosition = ball.position;
        wheelCenter = wheel.position;
        
        Debug.Log($"[DEBUG-{debugCount}] Ball Start Position: {ballStartPosition}");
        Debug.Log($"[DEBUG-{debugCount}] Wheel Center: {wheelCenter}");
        Debug.Log($"[DEBUG-{debugCount}] Wheel Order Length: {wheelOrder.Length}");
        Debug.Log($"[DEBUG-{debugCount}] Segment Size: {360f / wheelOrder.Length} degrees");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            debugCount++;
            Calculate();
        }
    }

    public void Calculate()
    {
        var inputText = inputField.text;
        if (string.IsNullOrEmpty(inputText))
        {
            Debug.LogError($"[DEBUG-{debugCount}] Input field is empty!");
            return;
        }

        int targetNumber = int.Parse(inputText);
        var index = FindIndex(wheelOrder, targetNumber);
        
        if (index == -1)
        {
            Debug.LogError($"[DEBUG-{debugCount}] Number {targetNumber} not found in wheel order!");
            return;
        }

        Debug.Log($"[DEBUG-{debugCount}] ===========================================");
        Debug.Log($"[DEBUG-{debugCount}] Target Number: {targetNumber}");
        Debug.Log($"[DEBUG-{debugCount}] Found Index: {index}");
        
        // Current wheel angle
        var currentWheelAngle = wheel.localEulerAngles.y;
        Debug.Log($"[DEBUG-{debugCount}] Current Wheel Angle: {currentWheelAngle}");
        
        // Calculate target angle for this segment
        float segmentSize = 360f / wheelOrder.Length;
        float targetSegmentAngle = segmentSize * index;
        
        Debug.Log($"[DEBUG-{debugCount}] Segment Size: {segmentSize}");
        Debug.Log($"[DEBUG-{debugCount}] Target Segment Angle: {targetSegmentAngle}");
        
        // Calculate the angle we need to spin to align this segment with the reference point
        // The reference point is where the ball should be when the wheel stops
        float angleToSpin = targetSegmentAngle;
        
        Debug.Log($"[DEBUG-{debugCount}] Angle to Spin: {angleToSpin}");
        
        SetBallPosition(angleToSpin);
        
        Debug.Log($"[DEBUG-{debugCount}] ===========================================");
    }

    private void SetBallPosition(float segmentAngle)
    {
        // Reset ball to start position first
        ball.position = ballStartPosition;
        
        Debug.Log($"[DEBUG-{debugCount}] Ball reset to start: {ball.position}");
        
        // Calculate the direction from wheel center to ball start position
        Vector3 directionToStart = (ballStartPosition - wheelCenter).normalized;
        float startRadius = Vector3.Distance(ballStartPosition, wheelCenter);
        
        Debug.Log($"[DEBUG-{debugCount}] Direction to start: {directionToStart}");
        Debug.Log($"[DEBUG-{debugCount}] Start radius: {startRadius}");
        
        // Rotate this direction by the segment angle
        Vector3 rotatedDirection = Quaternion.Euler(0, segmentAngle, 0) * directionToStart;
        
        Debug.Log($"[DEBUG-{debugCount}] Rotated direction: {rotatedDirection}");
        
        // Place ball at the rotated position
        Vector3 newBallPosition = wheelCenter + rotatedDirection * startRadius;
        ball.position = newBallPosition;
        
        Debug.Log($"[DEBUG-{debugCount}] Final Ball Position: {ball.position}");
        
        // Additional verification
        float actualAngle = Vector3.SignedAngle(directionToStart, rotatedDirection, Vector3.up);
        Debug.Log($"[DEBUG-{debugCount}] Actual rotation applied: {actualAngle} degrees");
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
        return -1;
    }
}