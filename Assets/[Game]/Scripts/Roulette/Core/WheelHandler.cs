using UnityEngine;
using System.Collections;
using System;
using Game.Core;

public class WheelHandler : MonoBehaviour
{
    #region INSPECTOR PROPERTIES

    public AnimationCurve easeCurve;
    [SerializeField] private Renderer wheelRenderer;
    [SerializeField] private Material europeanWheelMaterial;
    [SerializeField] private Material americanWheelMaterial;

    #endregion

    #region PRIVATE PROPERTIES

    private Coroutine spinCoroutine;

    #endregion

    #region PUBLIC METHODS

    public void SpinTo(float deltaDegrees, float duration, Action<float> onEnd)
    {
        if (spinCoroutine != null) StopCoroutine(spinCoroutine);
        spinCoroutine = StartCoroutine(SpinToCoroutine(deltaDegrees, duration, onEnd));
    }


    public void SetWheelOrder(GameType gameType)
    {
        if (wheelRenderer == null) return;

        Material[] materials = wheelRenderer.materials;
        if (gameType == GameType.EuropeanRoulette)
        {
            materials[1] = europeanWheelMaterial;
        }
        else if (gameType == GameType.AmericanRoulette)
        {
            materials[1] = americanWheelMaterial;
        }

        wheelRenderer.materials = materials;
    }

    #endregion

    #region PRIVATE METHODS

    private IEnumerator SpinToCoroutine(float deltaDegrees, float duration, Action<float> onEnd)
    {
        SoundManager.Instance.PlaySound(SoundType.WheelSpin, true);
        float startY = Mathf.Repeat(transform.eulerAngles.y, 360f);
        float targetY = startY + deltaDegrees;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            float easedT = (easeCurve != null) ? easeCurve.Evaluate(t) : t;
            float currentY = Mathf.Lerp(startY, targetY, easedT);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentY, transform.eulerAngles.z);
            yield return null;
        }

        float normalizedY = Mathf.Repeat(targetY, 360f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, normalizedY, transform.eulerAngles.z);

        spinCoroutine = null;
        onEnd?.Invoke(normalizedY);
    }

    #endregion
}