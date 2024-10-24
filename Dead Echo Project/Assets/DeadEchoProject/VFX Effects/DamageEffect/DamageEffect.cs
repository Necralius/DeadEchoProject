using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class DamageEffect : MonoBehaviour
{
    public static DamageEffect Instance;

    public Material screenDamageMat;
    private Coroutine screenDamageForwardTask;
    private Coroutine screenDamageBackwardTask;
    private Coroutine screenDamageFullActionTask;

    [SerializeField] private float forwardSpeed  = 1f;
    [SerializeField] private float backwardSpeed = 1f;

    [SerializeField] private bool _debug = false;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        ResetEffect();
    }
    private void OnEnable()
    {
        ResetEffect();
    }

    public void ResetEffect()
    {
        screenDamageMat.SetFloat("_Radius", 1);
    }
    private void Update()
    {
        if (_debug)
        {
            if (Input.GetKeyDown(KeyCode.I)) 
                ScreenDamageEffect(Random.Range(0.1f, 1f));

            if (Input.GetKeyDown(KeyCode.K))
                ScreenDamageEffectForward(Random.Range(0.1f, 1f));

            if (Input.GetKeyDown(KeyCode.H))
                ScreenDamageEffectBackward(Random.Range(0.1f, 1f));
        }
    }

    public void ScreenDamageEffect(float intensity, float forwardSpeed, float backwardSpeed)
    {
        if (screenDamageFullActionTask != null)
            StopCoroutine(screenDamageFullActionTask);
        screenDamageFullActionTask = StartCoroutine(screenDamage(intensity, forwardSpeed, backwardSpeed));
    }

    public void ScreenDamageEffect(float intensity)
    {
        if (screenDamageFullActionTask != null)
            StopCoroutine(screenDamageFullActionTask);
        screenDamageFullActionTask = StartCoroutine(screenDamage(intensity));
    }

    public void ScreenDamageEffectForward(float intensity)
    {
        if (screenDamageForwardTask != null)
            StopCoroutine(screenDamageForwardTask);
        screenDamageBackwardTask = StartCoroutine(screenDamageForward(intensity));
    }

    public void ScreenDamageEffectBackward(float intensity)
    {
        if (screenDamageBackwardTask != null)
            StopCoroutine(screenDamageBackwardTask);
        screenDamageBackwardTask = StartCoroutine(screenDamageBackward(intensity));
    }

    IEnumerator screenDamage(float intensity)
    {
        var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.15f);
        var curRadius = 1f;

        for (float t = 0; curRadius != targetRadius; t += (Time.deltaTime * forwardSpeed))
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            screenDamageMat.SetFloat("_Radius", curRadius);
            yield return null;
        }

        for (float t = 0; curRadius < 1f; t += (Time.deltaTime * backwardSpeed))
        {
            curRadius = Mathf.Lerp(targetRadius, 1, t);
            screenDamageMat.SetFloat("_Radius", curRadius);
            yield return null;
        }
    }

    IEnumerator screenDamage(float intensity, float forwardSpeed, float backwardSpeed)
    {
        var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.15f);
        var curRadius = 1f;

        for (float t = 0; curRadius != targetRadius; t += (Time.deltaTime * forwardSpeed))
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            screenDamageMat.SetFloat("_Radius", curRadius);
            yield return null;
        }

        for (float t = 0; curRadius < 1f; t += (Time.deltaTime * backwardSpeed))
        {
            curRadius = Mathf.Lerp(targetRadius, 1, t);
            screenDamageMat.SetFloat("_Radius", curRadius);
            yield return null;
        }
    }

    IEnumerator screenDamageForward(float intensity)
    {
        var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.15f);
        var curRadius    = 1f;

        for (float t = 0; curRadius != targetRadius; t += (Time.deltaTime * forwardSpeed))
        {
            curRadius = Mathf.Lerp(1, targetRadius, t);
            screenDamageMat.SetFloat("_Radius", curRadius);
            yield return null;
        }
    }

    IEnumerator screenDamageBackward(float intensity)
    {
        var targetRadius = Remap(intensity, 0, 1, 0.4f, -0.15f);
        var curRadius    = screenDamageMat.GetFloat("_Radius");

        for (float t = 0; curRadius < 1f; t += (Time.deltaTime * backwardSpeed))
        {
            curRadius = Mathf.Lerp(targetRadius, 1, t);
            screenDamageMat.SetFloat("_Radius", curRadius);
            yield return null;
        }
    }

    private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax) 
        => Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));

}
