using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RouletteTestManager : MonoBehaviour
{
    [Header("References")]
    public RouletteController rouletteController;
    public BagerTest bagerTest;
    
    [Header("UI")]
    public InputField numberInputField;
    public Button spinButton;
    public Button testBagerButton;
    public Button resetButton;
    public Text statusText;
    
    [Header("Auto Test")]
    public bool autoTest = false;
    public float autoTestInterval = 3f;
    public int[] testNumbers = { 0, 15, 32, 19, 4, 21, 2, 25 };
    
    private int currentTestIndex = 0;
    private bool isSpinning = false;

    private void Start()
    {
        SetupUI();
        UpdateStatus("Ready for testing");
        
        if (autoTest)
        {
            StartCoroutine(AutoTestRoutine());
        }
    }

    private void SetupUI()
    {
        if (spinButton != null)
            spinButton.onClick.AddListener(OnSpinButtonClick);
            
        if (testBagerButton != null)
            testBagerButton.onClick.AddListener(OnTestBagerClick);
            
        if (resetButton != null)
            resetButton.onClick.AddListener(OnResetClick);
            
        if (numberInputField != null)
            numberInputField.text = "15";
    }

    private void Update()
    {
        // Keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.Space) && !isSpinning)
        {
            OnSpinButtonClick();
        }
        
        if (Input.GetKeyDown(KeyCode.B) && !isSpinning)
        {
            OnTestBagerClick();
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            OnResetClick();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) TestSpecificNumber(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TestSpecificNumber(15);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TestSpecificNumber(32);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TestSpecificNumber(19);
    }

    public void OnSpinButtonClick()
    {
        if (isSpinning) return;
        
        int number = GetInputNumber();
        if (number == -1) return;
        
        StartSpin(number);
    }

    public void OnTestBagerClick()
    {
        if (isSpinning) return;
        
        int number = GetInputNumber();
        if (number == -1) return;
        
        TestBagerDirect(number);
    }

    public void OnResetClick()
    {
        if (rouletteController != null)
        {
            rouletteController.ball.position = rouletteController.transform.GetChild(0).position; // Assume ball start pos
            rouletteController.wheel.localEulerAngles = Vector3.zero;
        }
        
        UpdateStatus("Reset to initial positions");
    }

    private void StartSpin(int number)
    {
        if (rouletteController == null)
        {
            UpdateStatus("Error: RouletteController not assigned!");
            return;
        }
        
        isSpinning = true;
        UpdateStatus($"Spinning for number: {number}...");
        
        StartCoroutine(rouletteController.SpinRoulette(number, () => {
            isSpinning = false;
            UpdateStatus($"Spin completed for number: {number}");
            
            // Verify result
            StartCoroutine(VerifyResult(number));
        }));
    }

    private void TestBagerDirect(int number)
    {
        if (bagerTest == null)
        {
            UpdateStatus("Error: BagerTest not assigned!");
            return;
        }
        
        if (bagerTest.inputField == null)
        {
            UpdateStatus("Error: BagerTest input field not assigned!");
            return;
        }
        
        string oldText = bagerTest.inputField.text;
        bagerTest.inputField.text = number.ToString();
        
        UpdateStatus($"Testing BagerTest directly with number: {number}");
        
        bagerTest.debugCount++;
        bagerTest.Calculate();
        
        bagerTest.inputField.text = oldText;
        
        UpdateStatus($"BagerTest completed for number: {number}");
    }

    private IEnumerator VerifyResult(int expectedNumber)
    {
        yield return new WaitForSeconds(0.5f);
        
        // Get ball's current segment
        int currentSegment = GetBallCurrentSegment();
        
        if (currentSegment == expectedNumber)
        {
            UpdateStatus($"✅ SUCCESS: Ball is on segment {currentSegment} (Expected: {expectedNumber})");
        }
        else
        {
            UpdateStatus($"❌ FAILED: Ball is on segment {currentSegment} (Expected: {expectedNumber})");
        }
    }

    private int GetBallCurrentSegment()
    {
        if (rouletteController == null) return -1;
        
        Vector3 ballPos = rouletteController.ball.position;
        Vector3 wheelCenter = rouletteController.wheel.position;
        
        // Calculate angle from wheel center to ball
        Vector3 direction = (ballPos - wheelCenter).normalized;
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
        
        if (angle < 0) angle += 360f;
        
        // Convert angle to segment index
        float segmentSize = 360f / rouletteController.wheelOrder.Length;
        int segmentIndex = Mathf.RoundToInt(angle / segmentSize) % rouletteController.wheelOrder.Length;
        
        return rouletteController.wheelOrder[segmentIndex];
    }

    private void TestSpecificNumber(int number)
    {
        if (isSpinning) return;
        
        if (numberInputField != null)
            numberInputField.text = number.ToString();
            
        StartSpin(number);
    }

    private int GetInputNumber()
    {
        if (numberInputField == null)
        {
            UpdateStatus("Error: Input field not assigned!");
            return -1;
        }
        
        if (int.TryParse(numberInputField.text, out int number))
        {
            return number;
        }
        
        UpdateStatus("Error: Invalid number in input field!");
        return -1;
    }

    private void UpdateStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
            
        Debug.Log($"[RouletteTestManager] {message}");
    }

    private IEnumerator AutoTestRoutine()
    {
        while (autoTest)
        {
            yield return new WaitForSeconds(autoTestInterval);
            
            if (isSpinning) continue;
            
            int testNumber = testNumbers[currentTestIndex];
            currentTestIndex = (currentTestIndex + 1) % testNumbers.Length;
            
            UpdateStatus($"Auto Test: Testing number {testNumber}");
            
            if (numberInputField != null)
                numberInputField.text = testNumber.ToString();
                
            StartSpin(testNumber);
        }
    }

    // Public methods for external use
    public void StartSpinFromCode(int number)
    {
        if (numberInputField != null)
            numberInputField.text = number.ToString();
        StartSpin(number);
    }

    public void SetAutoTest(bool enabled)
    {
        autoTest = enabled;
        if (enabled && !isSpinning)
        {
            StartCoroutine(AutoTestRoutine());
        }
    }
}