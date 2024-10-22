using UnityEngine;

[CreateAssetMenu(fileName = "New Survival Data", menuName = "Dead Echo/Survival/New Survival Data")]
public class SurvivalData : ScriptableObject
{
    [Header("Loss Factors")]
    [Range(0.01f, 1f)] public float staminaFactor = 0.01f;
    [Range(0.01f, 1f)] public float hungerFactor  = 0.01f;
    [Range(0.01f, 1f)] public float thirstFactor  = 0.01f;
    [Range(0.01f, 100f)] private float _damageOnMax = 15f;

    public float DamageOnMax
    {
        get
        {
            float damage = Random.Range(0.85f, 1f);
            damage       *= _damageOnMax;
            return damage;
        }
    }
}