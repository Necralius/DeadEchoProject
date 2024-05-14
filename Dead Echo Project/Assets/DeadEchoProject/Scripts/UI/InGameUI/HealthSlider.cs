using UnityEngine;
using UnityEngine.UI;
using NekraByte;

[RequireComponent(typeof(Image))]
public class HealthSlider : MonoBehaviour
{
    private Image _img          => GetComponent<Image>();

    [SerializeField, Range(0f, 10f)] private float _travelSpeed  = 1f;

    [SerializeField, Range(0f, 300f)]   private float _maxHealth      = 100f;
    [SerializeField, Range(0f, 300f)]   private float _currentHealth  = 100f;
    private float                                     _targetHealth   = 100f;

    public float MaxHealth      { get { return _maxHealth; } set {  _maxHealth = value; } }
    public float CurrentHealth  { get { return _currentHealth; } }

    float segmentsCount = 0f;

    public void UpdateHealth(float health)
    {
        segmentsCount = _maxHealth / 25;

        _img.material.SetFloat("_SegmentCount", segmentsCount);
        _currentHealth  = health;
        _targetHealth   = health;
    }

    private void Update()
    {
        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        _targetHealth = Mathf.MoveTowards(_currentHealth, _targetHealth, _travelSpeed * Time.deltaTime);

        _img.material.SetFloat("_RemovedSegments", ConvertHealthToSegments(_targetHealth, (int)segmentsCount, _maxHealth));
    }

    float ConvertHealthToSegments(float vidaAlvo, int segmentCount, float maxHealth)
    {
        float vidaRestante = maxHealth - vidaAlvo;

        if (vidaRestante <= 0) 
            return 0;
        else
        {
            float removedSegments = vidaRestante / 25; // Cada segmento representa 25 de vida
            return Mathf.Min(removedSegments, segmentCount);
        }
    }
}