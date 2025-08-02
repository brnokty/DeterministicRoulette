using UnityEngine;
using System.Collections;

public class SmoothRotator : MonoBehaviour
{
    public float minDegree = 720f; // Minimum ekstra döneceği derece
    public float maxDegree = 1080f; // Maksimum ekstra döneceği derece
    public float minDuration = 2f;
    public float maxDuration = 4f;
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

    // Animasyonun devamını toplam dereceden başlat!
    public void RotateToTarget(float targetAngle)
    {
        StopRotating(); // Önce sonsuz döndürmeyi durdur

        // O anda objenin döndürülmüş toplam açısını bilmemiz gerekiyor
        // (başladığından beri kaç derece döndü?)
        // Onu kaydederek ilerliyoruz.
        float currentY = transform.eulerAngles.y;
        // Burası önemli: currentTotalDegree'nin kalan tamsayı kısmını ekle!
        float baseTotalDegree = currentTotalDegree - Mathf.Repeat(currentY, 360f) + currentY;

        // Şimdi buradan devam edeceğiz:
        float extraDegree = Random.Range(minDegree, maxDegree);
        float targetGlobalDegree = baseTotalDegree + extraDegree + Mathf.DeltaAngle(Mathf.Repeat(baseTotalDegree + extraDegree, 360f), targetAngle);

        float duration = Random.Range(minDuration, maxDuration);

        rotateCoroutine = StartCoroutine(RotateRoutine(baseTotalDegree, targetGlobalDegree, duration));
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
    
    public void SmoothRotateByDegree(float degree, float duration)
    {
        StopRotating(); // Olası animasyonları durdur
        rotateCoroutine = StartCoroutine(SmoothRotateByDegreeRoutine(degree, duration));
    }

    IEnumerator SmoothRotateByDegreeRoutine(float degree, float duration)
    {
        // Başlangıç açısını kendi tut!
        float from = currentTotalDegree;
        float to = from + Mathf.Abs(degree); // Hep pozitif yönde (saat yönü tersine)

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float curDeg = Mathf.Lerp(from, to, EaseOutCubic(t));
            currentTotalDegree = curDeg; // Her frame güncelle!
            transform.rotation = Quaternion.Euler(0, curDeg, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Tam olarak hedef açıda bitir
        currentTotalDegree = to;
        transform.rotation = Quaternion.Euler(0, to, 0);
    }
    
    public void BagerRotateByDegree(float degree, float speed)
    {
        StopRotating(); // Olası animasyonları durdur
        rotateCoroutine = StartCoroutine(BagerRotateByDegreeRoutine(degree, speed));
    }
    
    IEnumerator BagerRotateByDegreeRoutine(float degree, float duration)
    {
        degree=degree<0?degree+360:degree;
        while (degree-transform.localEulerAngles.y> 0.1f)
        {
            print("calculated degree: " + degree + " current: " + transform.localEulerAngles.y);
            transform.localEulerAngles= Vector3.Lerp(transform.localEulerAngles,new Vector3(0, degree, 0),Time.deltaTime*duration);
            yield return null;
        }
    }
    
    
}
