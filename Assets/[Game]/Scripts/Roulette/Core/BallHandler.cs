using System;
using UnityEngine;
using System.Collections;
using DG.Tweening;
using Game.Core;
using Random = UnityEngine.Random;


public class BallHandler : MonoBehaviour
{
    public Transform ball;
    public float rotationSpeed = 180f; // Sürekli dönerkenki hız (derece/sn)
    public float slowDownAngle = 90f; // Son kaç derecede yavaşlayacak

    private Coroutine rotateCoroutine;
    private float currentTotalDegree = 0f; // Başladığından beri dönen toplam derece

    private float defaultZPosition;
    private float targetZPosition = 0.2149f;

    private void Start()
    {
        defaultZPosition = ball.localPosition.z; // Başlangıç Z konumunu kaydet
    }

    // Sonsuz döndür (sen stop veya rotateToTarget demedikçe)
    public void StartRotating()
    {
        StopRotating();
        SoundManager.Instance.PlaySound(SoundManager.SoundType.WheelSpin, true);
        rotateCoroutine = StartCoroutine(RotateInfinite());
        SetZposition(targetZPosition);
    }

    public void StopRotating()
    {
        SoundManager.Instance.StopSound(SoundManager.SoundType.WheelSpin);
        SoundManager.Instance.PlaySound(SoundManager.SoundType.BallStop);
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            SetZposition(defaultZPosition);
            rotateCoroutine = null;
        }
    }

    IEnumerator RotateInfinite()
    {
        // Burada toplam açıyı her frame güncelle!
        while (true)
        {
            float delta = rotationSpeed * Time.deltaTime;
            currentTotalDegree += delta;
            transform.rotation = Quaternion.Euler(0f, currentTotalDegree, 0f);
            yield return null;
        }
    }


    // public void RotateByDegree(float degree, float duration, Action onComplete)
    // {
    //     StopRotating();
    //     float currentY = transform.localEulerAngles.y;
    //     float targetY = degree;
    //     float delta = targetY - currentY;
    //     if (delta <= 0)
    //     {
    //         delta += 360f;
    //     }
    //
    //     // degree = degree < 0 ? 360 + degree : degree; // Negatif ise pozitif yap
    //     float finalY = currentY + delta;
    //     transform.DOLocalRotate(new Vector3(0, finalY, 0), duration, RotateMode.FastBeyond360)
    //         .SetEase(Ease.OutExpo)
    //         .OnComplete(() =>
    //         {
    //             currentTotalDegree = finalY; // Son dereceyi güncelle
    //             onComplete?.Invoke();
    //         });
    // }
    
    public void RotateByDegree(float degree, float duration, Action onComplete)
    {
        StopRotating();
        StartCoroutine(RotateByDegreeCoroutine(degree, duration, onComplete));
    }

    private IEnumerator RotateByDegreeCoroutine(float degree, float duration, Action onComplete)
    {
        float currentY = transform.localEulerAngles.y;
        float targetY = degree;

        // Hedef: 1 tam tur + hedef açı kadar döndürmek
        float finalY = currentY + 360f + Mathf.DeltaAngle(currentY % 360f, targetY % 360f);

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float easedT = 1 - Mathf.Pow(2, -10 * t); // Basit EaseOutExpo
            float lerpedY = Mathf.Lerp(currentY, finalY, easedT);
            transform.localEulerAngles = new Vector3(0, lerpedY, 0);
            yield return null;
        }
        transform.localEulerAngles = new Vector3(0, finalY, 0);
        onComplete?.Invoke();
    }



    public void SetZposition(float zPosition)
    {
        ball.DOLocalMoveZ(zPosition, Random.Range(1f, 2f));
    }
}