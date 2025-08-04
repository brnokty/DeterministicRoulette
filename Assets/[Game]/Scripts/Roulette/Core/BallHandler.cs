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

    public void RotateByDegree(float degree, float duration, Action onComplete)
    {
        StopRotating();
        StartCoroutine(RotateByDegreeCoroutine(degree, duration, onComplete));
    }

    private IEnumerator RotateByDegreeCoroutine(float degree, float duration, Action onComplete)
    {
        float currentY = transform.localEulerAngles.y;
        float targetY = degree;
        float finalY = currentY + 360f + Mathf.DeltaAngle(currentY % 360f, targetY % 360f);

        float timer = 0f;

        Vector3 startPos = transform.localPosition;
        float bounceHeight = 0.02f; 
        float bounceFrequency = Random.Range(4f, 6f); 
        float minBounce = 0.005f; 

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            // Ease-out expo ile yavaşlama
            float easedT = 1 - Mathf.Pow(2, -10 * t);
            float lerpedY = Mathf.Lerp(currentY, finalY, easedT);
            transform.localEulerAngles = new Vector3(0, lerpedY, 0);

            // Sadece yukarı zıplama: abs(sin)
            float bounceDamping = Mathf.Lerp(bounceHeight, minBounce, t); // Sekme azalır
            float bounce = Mathf.Abs(Mathf.Sin(t * Mathf.PI * bounceFrequency)) * bounceDamping;
            transform.localPosition = startPos + new Vector3(0, bounce, 0);

            yield return null;
        }

        // transform.localEulerAngles = new Vector3(0, finalY, 0);
        // transform.localPosition = startPos;
        onComplete?.Invoke();
    }


    public void SetZposition(float zPosition)
    {
        // Eğer aynı anda birden fazla animasyon başlamasın diye eski coroutine'i durdur
        if (moveZCoroutine != null)
            StopCoroutine(moveZCoroutine);

        float duration = Random.Range(1f, 2f);
        moveZCoroutine = StartCoroutine(MoveZCoroutine(zPosition, duration));
    }

    private Coroutine moveZCoroutine;

    private IEnumerator MoveZCoroutine(float zPosition, float duration)
    {
        float timer = 0f;
        Vector3 startPos = ball.localPosition;
        Vector3 endPos = new Vector3(startPos.x, startPos.y, zPosition);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float smoothT = 1 - Mathf.Pow(1 - t, 2.5f); 
            ball.localPosition = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
        ball.localPosition = endPos;
    }
}