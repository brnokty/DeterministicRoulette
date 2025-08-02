using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

[System.Serializable]
public class RouletteSetupData
{
    [Header("Scene Objects")]
    public Transform wheelTransform;
    public Transform ballTransform;
    
    [Header("UI Elements")]
    public Canvas uiCanvas;
    public InputField numberInput;
    public Button spinButton;
    public Button testBagerButton;
    public Button resetButton;
    public Text statusText;
    
    [Header("Positioning")]
    public Vector3 wheelPosition = Vector3.zero;
    public Vector3 ballStartPosition = new Vector3(0.35f, 0.05f, 0f);
    
    [Header("Settings")]
    public float spinDuration = 3f;
    public int minWheelTurns = 5;
    public int maxWheelTurns = 8;
    public float ballStartRadius = 0.35f;
    public float ballEndRadius = 0.29f;
}

public class RouletteSetupHelper : MonoBehaviour
{
    [Header("Setup Configuration")]
    public RouletteSetupData setupData = new RouletteSetupData();
    
    [Header("Auto Setup")]
    public bool autoSetupOnStart = true;
    public bool createMissingComponents = true;
    
    [Header("Debug")]
    public bool enableDebugLogs = true;

    private RouletteController rouletteController;
    private BagerTest bagerTest;
    private RouletteTestManager testManager;

    private void Start()
    {
        if (autoSetupOnStart)
        {
            SetupRouletteSystem();
        }
    }

    [ContextMenu("Setup Roulette System")]
    public void SetupRouletteSystem()
    {
        LogDebug("Starting Roulette System Setup...");
        
        // 1. Setup basic components
        SetupRouletteController();
        SetupBagerTest();
        SetupTestManager();
        
        // 2. Configure positions
        ConfigurePositions();
        
        // 3. Setup UI if available
        SetupUI();
        
        LogDebug("Roulette System Setup Complete!");
        
        // 4. Print instructions
        PrintUsageInstructions();
    }

    private void SetupRouletteController()
    {
        // Get or create RouletteController
        rouletteController = GetComponent<RouletteController>();
        if (rouletteController == null && createMissingComponents)
        {
            rouletteController = gameObject.AddComponent<RouletteController>();
            LogDebug("Created RouletteController component");
        }

        if (rouletteController != null)
        {
            // Assign references
            rouletteController.wheel = setupData.wheelTransform;
            rouletteController.ball = setupData.ballTransform;
            
            // Configure settings using reflection for safety
            SetRouletteControllerProperty("spinDuration", setupData.spinDuration);
            SetRouletteControllerProperty("minWheelTurns", setupData.minWheelTurns);
            SetRouletteControllerProperty("maxWheelTurns", setupData.maxWheelTurns);
            SetRouletteControllerProperty("ballStartRadius", setupData.ballStartRadius);
            SetRouletteControllerProperty("ballEndRadius", setupData.ballEndRadius);
            
            LogDebug("RouletteController configured");
        }
    }

    private void SetRouletteControllerProperty(string propertyName, object value)
    {
        try
        {
            // Try direct field access first
            var field = rouletteController.GetType().GetField(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(rouletteController, value);
                LogDebug($"Set {propertyName} = {value}");
                return;
            }

            // Try property access
            var property = rouletteController.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property != null && property.CanWrite)
            {
                property.SetValue(rouletteController, value);
                LogDebug($"Set {propertyName} = {value}");
                return;
            }

            LogDebug($"Warning: Could not find field or property '{propertyName}' in RouletteController");
        }
        catch (System.Exception e)
        {
            LogDebug($"Error setting {propertyName}: {e.Message}");
        }
    }

    private void SetupBagerTest()
    {
        // Find existing BagerTest or create one
        bagerTest = FindObjectOfType<BagerTest>();
        if (bagerTest == null && createMissingComponents)
        {
            GameObject bagerObj = new GameObject("BagerTest");
            bagerTest = bagerObj.AddComponent<BagerTest>();
            LogDebug("Created BagerTest component");
        }

        if (bagerTest != null)
        {
            // Assign references
            bagerTest.wheel = setupData.wheelTransform;
            bagerTest.ball = setupData.ballTransform;
            bagerTest.inputField = setupData.numberInput;
            
            LogDebug("BagerTest configured");
        }
    }

    private void SetupTestManager()
    {
        // Get or create RouletteTestManager
        testManager = GetComponent<RouletteTestManager>();
        if (testManager == null && createMissingComponents)
        {
            testManager = gameObject.AddComponent<RouletteTestManager>();
            LogDebug("Created RouletteTestManager component");
        }

        if (testManager != null)
        {
            // Assign references
            testManager.rouletteController = rouletteController;
            testManager.bagerTest = bagerTest;
            testManager.numberInputField = setupData.numberInput;
            testManager.spinButton = setupData.spinButton;
            testManager.testBagerButton = setupData.testBagerButton;
            testManager.resetButton = setupData.resetButton;
            testManager.statusText = setupData.statusText;
            
            LogDebug("RouletteTestManager configured");
        }
    }

    private void ConfigurePositions()
    {
        if (setupData.wheelTransform != null)
        {
            setupData.wheelTransform.position = setupData.wheelPosition;
            LogDebug($"Wheel positioned at {setupData.wheelPosition}");
        }

        if (setupData.ballTransform != null)
        {
            Vector3 ballPos = setupData.wheelPosition + setupData.ballStartPosition;
            setupData.ballTransform.position = ballPos;
            LogDebug($"Ball positioned at {ballPos}");
        }
    }

    private void SetupUI()
    {
        if (setupData.uiCanvas == null) return;

        // Configure UI elements if they exist
        if (setupData.spinButton != null)
        {
            var buttonText = setupData.spinButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "SPIN";
        }

        if (setupData.testBagerButton != null)
        {
            var buttonText = setupData.testBagerButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "Test BagerTest";
        }

        if (setupData.resetButton != null)
        {
            var buttonText = setupData.resetButton.GetComponentInChildren<Text>();
            if (buttonText != null) buttonText.text = "Reset";
        }

        if (setupData.numberInput != null)
        {
            setupData.numberInput.text = "15";
        }

        if (setupData.statusText != null)
        {
            setupData.statusText.text = "Ready for testing";
        }

        LogDebug("UI configured");
    }

    private void PrintUsageInstructions()
    {
        LogDebug("=== ROULETTE SYSTEM USAGE ===");
        LogDebug("Keyboard Controls:");
        LogDebug("  SPACE - Spin roulette with current input number");
        LogDebug("  B - Test BagerTest directly");
        LogDebug("  R - Reset positions");
        LogDebug("  1,2,3,4 - Quick test specific numbers");
        LogDebug("");
        LogDebug("Code Usage:");
        LogDebug("  rouletteController.StartSpin(15);");
        LogDebug("  rouletteController.SpinForNumber(15);");
        LogDebug("  testManager.StartSpinFromCode(32);");
        LogDebug("");
        LogDebug("Components Setup:");
        LogDebug($"  RouletteController: {(rouletteController != null ? "✅" : "❌")}");
        LogDebug($"  BagerTest: {(bagerTest != null ? "✅" : "❌")}");
        LogDebug($"  TestManager: {(testManager != null ? "✅" : "❌")}");
        LogDebug("==============================");
    }

    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[RouletteSetup] {message}");
        }
    }

    // Utility methods for runtime configuration
    public void SetWinningNumber(int number)
    {
        if (setupData.numberInput != null)
        {
            setupData.numberInput.text = number.ToString();
        }
        
        if (rouletteController != null)
        {
            SetRouletteControllerProperty("testWinningNumber", number);
        }
    }

    public void QuickSpin(int number)
    {
        SetWinningNumber(number);
        if (rouletteController != null)
        {
            // Try multiple methods to ensure compatibility
            try
            {
                // Try StartSpin first (most reliable)
                var startSpinMethod = rouletteController.GetType().GetMethod("StartSpin", BindingFlags.Public | BindingFlags.Instance);
                if (startSpinMethod != null)
                {
                    startSpinMethod.Invoke(rouletteController, new object[] { number });
                    LogDebug($"Called StartSpin({number})");
                    return;
                }

                // Try SpinForNumber
                var spinForNumberMethod = rouletteController.GetType().GetMethod("SpinForNumber", BindingFlags.Public | BindingFlags.Instance);
                if (spinForNumberMethod != null)
                {
                    spinForNumberMethod.Invoke(rouletteController, new object[] { number });
                    LogDebug($"Called SpinForNumber({number})");
                    return;
                }

                // Fallback: call SpinRoulette coroutine directly
                var spinRouletteMethod = rouletteController.GetType().GetMethod("SpinRoulette", BindingFlags.Public | BindingFlags.Instance);
                if (spinRouletteMethod != null)
                {
                    StartCoroutine((System.Collections.IEnumerator)spinRouletteMethod.Invoke(rouletteController, new object[] { number, null }));
                    LogDebug($"Called SpinRoulette({number}) as fallback");
                    return;
                }

                LogDebug("Error: Could not find any spin method in RouletteController");
            }
            catch (System.Exception e)
            {
                LogDebug($"Error calling spin method: {e.Message}");
            }
        }
    }

    // Auto-find components if not assigned
    [ContextMenu("Auto-Find Components")]
    public void AutoFindComponents()
    {
        if (setupData.wheelTransform == null)
        {
            setupData.wheelTransform = GameObject.Find("Wheel")?.transform;
        }
        
        if (setupData.ballTransform == null)
        {
            setupData.ballTransform = GameObject.Find("Ball")?.transform;
        }
        
        if (setupData.uiCanvas == null)
        {
            setupData.uiCanvas = FindObjectOfType<Canvas>();
        }
        
        LogDebug("Auto-find completed");
    }

    // Manual setup method if auto setup fails
    [ContextMenu("Manual Component Setup")]
    public void ManualComponentSetup()
    {
        // Just create components without setting properties
        if (GetComponent<RouletteController>() == null)
        {
            gameObject.AddComponent<RouletteController>();
            LogDebug("Added RouletteController manually");
        }

        if (GetComponent<RouletteTestManager>() == null)
        {
            gameObject.AddComponent<RouletteTestManager>();
            LogDebug("Added RouletteTestManager manually");
        }

        if (FindObjectOfType<BagerTest>() == null)
        {
            GameObject bagerObj = new GameObject("BagerTest");
            bagerObj.AddComponent<BagerTest>();
            LogDebug("Added BagerTest manually");
        }

        LogDebug("Manual setup complete. Please assign references manually in Inspector.");
    }
}