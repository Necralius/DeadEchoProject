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

    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float deactiveAlpha = 0.4f;

    [Header("Player State and Life")]
    [SerializeField] private Image playerSprite;
    [SerializeField] private HealthSlider lifeSlider;

    [SerializeField] private List<CharacterState> _stanceStates;

    // ----------------------------------------------------------------------
    // Name: UpdatePlayerState
    // Desc: This method updates completly the character UI.
    // ----------------------------------------------------------------------
    public void UpdatePlayerState(BodyController controller, CharacterManager manager)
    {
        UpdatePlayerState(controller);
        if (lifeSlider == null)
        {
            Debug.Log("Slider is null");
            return;
        }
        lifeSlider?.UpdateHealth(manager.CurrentHealth);
        lifeSlider.MaxHealth = manager._maxHealth;
    }

    // ----------------------------------------------------------------------
    // Name: UpdatePlayerState (Overcharge)
    // Desc: This method updates the player UI stance state, changing the character
    //       state sprite to the correct state.
    // ----------------------------------------------------------------------
    public void UpdatePlayerState(BodyController controller)
    {
        if (controller is null) return;

        CharacterState state;
        switch(controller.CurrentState)
        {
            case MovementState.Idle:        state = _stanceStates.Find(e => e.type == StateType.Stand);     break;
            case MovementState.Walking:     state = _stanceStates.Find(e => e.type == StateType.Stand);     break;
            case MovementState.Sprinting:   state = _stanceStates.Find(e => e.type == StateType.Stand);     break;
            case MovementState.Crouching:   state = _stanceStates.Find(e => e.type == StateType.Crouch);    break;
            case MovementState.Air:         state = _stanceStates.Find(e => e.type == StateType.Jumping);   break;
            default: state = _stanceStates[0]; break;
        }

        Sprite correctStateSprt = state.stateSprite;

        if (correctStateSprt == null || playerSprite == null) 
            return;

        playerSprite.sprite     = correctStateSprt;
    }
}