using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BallHandler : MonoBehaviour
{
    public float rotationSpeed = 180f; // Sürekli dönerkenki hız (derece/sn)
    public float slowDownAngle = 90f; // Son kaç derecede yavaşlayacak

    private Coroutine rotateCoroutine;
    private float currentTotalDegree = 0f; // Başladığından beri dönen toplam derece

    // Sonsuz döndür (sen stop veya rotateToTarget demedikçe)
    public void StartRotating()
    {
        StopRotating();
        rotateCoroutine = StartCoroutine(RotateInfinite());
    }

    public void StopRotating()
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
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

    IEnumerator RotateRoutine(float from, float to, float duration)
    {
        float total = to - from;
        float elapsed = 0f;
        currentTotalDegree = from; // Animasyona başlarken güncelle

        float slowStart = to - slowDownAngle;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentDegree;

            // Lineer kısım (slowdown'dan önce)
            if (currentTotalDegree < slowStart)
            {
                float linearT = Mathf.InverseLerp(from, slowStart, currentTotalDegree);
                currentDegree = Mathf.Lerp(from, slowStart, linearT);
            }
            else
            {
                // Ease out kısım (yavaşlayarak)
                float slowT = Mathf.InverseLerp(slowStart, to, currentTotalDegree);
                slowT = Mathf.Clamp01(slowT);
                currentDegree = Mathf.Lerp(slowStart, to, EaseOutCubic(slowT));
            }

            currentTotalDegree = Mathf.MoveTowards(currentTotalDegree, to, (total / duration) * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, currentDegree, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        currentTotalDegree = to;
        transform.rotation = Quaternion.Euler(0, to, 0);
    }

    float EaseOutCubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }
    
    
    public void RotateByDegree(float degree, float duration,System.Action onComplete)
    {
        StopRotating();
        degree=degree<0 ? 360 + degree : degree; // Negatif ise pozitif yap
        transform.DOLocalRotate(
                new Vector3(0, degree, 0),
                duration
            )
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                currentTotalDegree = degree; // Son dereceyi güncelle
                onComplete?.Invoke();
            });
        
    }
}