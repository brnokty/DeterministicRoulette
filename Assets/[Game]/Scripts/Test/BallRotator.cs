using UnityEngine;
using System.Collections;

public class BallRotator : MonoBehaviour
{
    public Transform pivotObject;
    public float radius = 1.0f;
    public float height = 0.1f;
    public AnimationCurve easeCurve;

    private bool followWheel = false;
    private Coroutine settleCoroutine;
    private float currentAngle = 0f;

    public void StartFollowing()
    {
        followWheel = true;
        if (settleCoroutine != null) StopCoroutine(settleCoroutine);
    }

    public void SetAngleDirect(float angle)
    {
        currentAngle = angle;
        UpdateBallPosition();
    }

    public void SettleTo(float finalAngle, float duration)
    {
        if (settleCoroutine != null) StopCoroutine(settleCoroutine);
        followWheel = false;
        settleCoroutine = StartCoroutine(SettleCoroutine(finalAngle, duration));
    }

    IEnumerator SettleCoroutine(float finalAngle, float duration)
    {
        currentAngle = pivotObject.eulerAngles.y;
        float startAngle = currentAngle;
        float delta = finalAngle - startAngle;
        if (delta <= 0) delta += 360f; // Saat yönünün tersine, kısa yol yok
        float target = startAngle + delta;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float easedT = (easeCurve != null) ? easeCurve.Evaluate(t) : t;
            currentAngle = Mathf.Lerp(startAngle, target, easedT);
            UpdateBallPosition();
            yield return null;
        }
        currentAngle = finalAngle;
        UpdateBallPosition();
        settleCoroutine = null;
    }

    void LateUpdate()
    {
        if (followWheel && pivotObject != null)
        {
            currentAngle = pivotObject.eulerAngles.y;
            UpdateBallPosition();
        }
    }

    void UpdateBallPosition()
    {
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad) * radius, height, Mathf.Sin(rad) * radius);
        transform.position = pivotObject.position + offset;
    }
}
