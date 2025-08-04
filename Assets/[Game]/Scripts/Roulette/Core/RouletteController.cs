using System;
using Game.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameType
{
    EuropeanRoulette,
    AmericanRoulette,
}

public class RouletteController : MonoBehaviour
{
    [Header("References")] public WheelHandler wheelHandler;
    public BallHandler ballHandler;
    [SerializeField] private GameObject europeanBetArea;
    [SerializeField] private GameObject americanBetArea;
    [SerializeField] private ParticleSystem confettiParticleSystem;
    [Header("Settings")] public GameType gameType = GameType.EuropeanRoulette;
    public float ruletMinDegree = 1080f;
    public float ruletMaxDegree = 2160f;
    public float ruletMinDuration = 2f;
    public float ruletMaxDuration = 4f;
    public float minBallSpeed = 2f;
    public float maxBallSpeed = 4f;
    public float angleOffset = 0f;

    [Header("European Wheel Segment Order")]
    public string[] europeanWheelOrder = new string[]
    {
        "0", "32", "15", "19", "4", "21", "2", "25", "17", "34", "6", "27",
        "13", "36", "11", "30", "8", "23", "10", "5", "24",
        "16", "33", "1", "20", "14", "31", "9", "22", "18",
        "29", "7", "28", "12", "35", "3", "26"
    };

    [Header("American Wheel Segment Order")]
    public string[] americanWheelOrder = new string[]
    {
        "0", "28", "9", "26", "30", "11", "7", "20", "32", "17", "5", "22",
        "34", "15", "3", "24", "36", "13", "1", "00", "27", "10", "25",
        "29", "12", "8", "19", "31", "18", "6", "21", "33", "16", "4",
        "23", "35", "14", "2"
    };
    

    public void StartSpin(string number, System.Action onComplete)
    {
        ballHandler.StartRotating();
        float ruletDelta = Random.Range(ruletMinDegree, ruletMaxDegree);
        float ruletDuration = Random.Range(ruletMinDuration, ruletMaxDuration);

        wheelHandler.SpinTo(ruletDelta, ruletDuration, (ruletFinalY) =>
        {
            var ballduration = Random.Range(minBallSpeed, maxBallSpeed);
            var calculatedAngle = CalculateAngle(number);
            print("Calculated Angle: " + calculatedAngle);
            ballHandler.RotateByDegree(calculatedAngle, ballduration, onComplete);
        });
    }

    public float CalculateAngle(string number)
    {
        var index = -1;
        var targetAngle = 0f;
        switch (gameType)
        {
            case GameType.EuropeanRoulette:
                index = FindIndex(europeanWheelOrder, number);
                if (index == -1)
                    index = Random.Range(0, europeanWheelOrder.Length);
                targetAngle = index * (360f / europeanWheelOrder.Length);
                break;
            case GameType.AmericanRoulette:
                index = FindIndex(americanWheelOrder, number);
                if (index == -1)
                    index = Random.Range(0, americanWheelOrder.Length);
                targetAngle = index * (360f / americanWheelOrder.Length);
                break;
            default:
                Debug.LogError("Unknown game type: " + gameType);
                return 0f;
        }

        var wheelAngle = wheelHandler.transform.localEulerAngles.y;
        // if (wheelAngle < 0)
        // {
        //     wheelAngle += 360f;
        // }

        var angleToSpin = targetAngle + wheelAngle;

        print("target angle: " + targetAngle + " wheel angle: " + wheelAngle + " angle to spin: " + angleToSpin);

        if (angleToSpin < 0)
        {
            angleToSpin += 360f;
        }

        return angleToSpin + angleOffset;
    }

    public int FindIndex(string[] array, string value)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
            {
                print("index found: " + i + " for value: " + value);
                return i;
            }
        }

        // Eleman bulunamazsa -1 döndürür
        return -1;
    }


    public void SetGameType(GameType type)
    {
        gameType = type;
        wheelHandler.SetWheelOrder(gameType);

        if (gameType == GameType.EuropeanRoulette)
        {
            europeanBetArea.SetActive(true);
            americanBetArea.SetActive(false);
        }
        else
        {
            americanBetArea.SetActive(true);
            europeanBetArea.SetActive(false);
        }
    }

    public void PlayConfetti()
    {
        if (confettiParticleSystem != null)
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Confetti);
            confettiParticleSystem.Play();
        }
        else
        {
            Debug.LogWarning("Confetti Particle System is not assigned!");
        }
    }
}