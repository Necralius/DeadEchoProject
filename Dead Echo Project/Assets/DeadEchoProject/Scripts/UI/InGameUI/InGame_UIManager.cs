using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NekraByte.Core.Enumerators;
using static NekraByte.Core.DataTypes;

public class InGame_UIManager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static InGame_UIManager Instance;
    private void Awake()
    {
        if (Instance != null) 
            Destroy(Instance.gameObject);
        Instance = this;
    }
    #endregion

    [SerializeField] private float activeAlpha   = 1f;
    [SerializeField] private float deactiveAlpha = 0.4f;

    [Header("Player State and Life")]
    [SerializeField] private HealthSlider lifeSlider;

    // ----------------------------------------------------------------------
    // Name: UpdatePlayerState
    // Desc: This method updates completly the character UI.
    // ----------------------------------------------------------------------
    public void UpdatePlayerState(BodyController controller, CharacterManager manager)
    {
        if (lifeSlider == null)
        {
            Debug.Log("Slider is null");
            return;
        }
        lifeSlider?.UpdateHealth(manager.CurrentHealth);
        lifeSlider.MaxHealth = manager._maxHealth;
    }
}