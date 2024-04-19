using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// --------------------------------------------------------------------------
// Name : InputManager
// Desc : This class handle all the game input.
// --------------------------------------------------------------------------
[RequireComponent(typeof(PlayerInput))]
public class InputManager : MonoBehaviour
{
    #region - Singleton Pattern -
    public static InputManager Instance;
    #endregion

    private PlayerInput playerInput => GetComponent<PlayerInput>();

    #region - Input Class Data -

    //Private Data
    public InputActionMap currentMap = null;

    //Movment Actions
    [HideInInspector] public InputContent MoveAction        = new InputContent();
    [HideInInspector] public InputContent LookAction        = new InputContent();
    [HideInInspector] public InputContent SpaceAction       = new InputContent();
    [HideInInspector] public InputContent CtrlAction        = new InputContent();
    [HideInInspector] public InputContent LeftShiftAction   = new InputContent();

    [HideInInspector] public InputContent mouseLeftAction   = new InputContent();
    [HideInInspector] public InputContent mouseRightAction  = new InputContent();
    [HideInInspector] public InputContent R_Action          = new InputContent();
    [HideInInspector] public InputContent B_Action          = new InputContent();
    [HideInInspector] public InputContent T_Action          = new InputContent();
    [HideInInspector] public InputContent C_Action          = new InputContent();

    [HideInInspector] public InputContent F_Action          = new InputContent();

    [HideInInspector] public InputContent One_Action        = new InputContent();
    [HideInInspector] public InputContent Two_Action        = new InputContent();

    [HideInInspector] public InputContent Escape_Action     = new InputContent();
    [HideInInspector] public InputContent E_Action          = new InputContent();
    [HideInInspector] public InputContent Z_Action          = new InputContent();
    [HideInInspector] public InputContent Tab_Action        = new InputContent();

    #endregion

    // ---------------------------- Methods ----------------------------

    #region - BuiltIn Methods -
    // ----------------------------------------------------------------------
    // Name : Awake
    // Desc : This method its called in the very first application frame,
    //        also this method get all the map actions and translate them
    //        in to literal game inputs.
    // ----------------------------------------------------------------------
    private void Awake()
    {
        if (Instance != null) 
            Destroy(Instance.gameObject);
        Instance = this;

        currentMap      = playerInput.currentActionMap;

        //Movment Actions
        MoveAction.Action                   = currentMap.FindAction("Move");
        LookAction.Action                   = currentMap.FindAction("Look");
        LeftShiftAction.Action              = currentMap.FindAction("LeftShiftAction");
        SpaceAction.Action                  = currentMap.FindAction("SpaceAction");
        CtrlAction.Action                   = currentMap.FindAction("CtrlAction");

        //Gun Behavior Actions
        R_Action.Action                     = currentMap.FindAction("R_Action");
        mouseLeftAction.Action              = currentMap.FindAction("LeftMouseAction");
        mouseRightAction.Action             = currentMap.FindAction("RightMouseAction");

        One_Action.Action                   = currentMap.FindAction("1_Action");
        Two_Action.Action                   = currentMap.FindAction("2_Action");
        Escape_Action.Action                = currentMap.FindAction("EscapeAction");
        Tab_Action.Action                   = currentMap.FindAction("Tab_Action");
        T_Action.Action                     = currentMap.FindAction("T_Action");
        B_Action.Action                     = currentMap.FindAction("B_Action");
        C_Action.Action                     = currentMap.FindAction("C_Action");
        F_Action.Action                     = currentMap.FindAction("F_Action");
        E_Action.Action                     = currentMap.FindAction("E_Action");
        Z_Action.Action                     = currentMap.FindAction("Z_Action");

        MoveAction.Action.performed         += on_Move;
        LookAction.Action.performed         += on_Look;
        LeftShiftAction.Action.performed    += on_LeftShift;
        mouseLeftAction.Action.performed    += on_MouseLeft;
        mouseRightAction.Action.performed   += on_MouseRight;
        SpaceAction.Action.performed        += on_Jump;
        CtrlAction.Action.performed         += on_Crouch;
        Escape_Action.Action.performed      += on_PauseMenu;
        Tab_Action.Action.performed         += on_Tab;
        R_Action.Action.performed           += on_R;
        B_Action.Action.performed           += on_B;
        T_Action.Action.performed           += on_T;
        F_Action.Action.performed           += on_F;
        E_Action.Action.performed           += on_E;
        Z_Action.Action.performed           += on_Z;
        C_Action.Action.performed           += on_C;

        MoveAction.Action.canceled          += on_Move;
        LookAction.Action.canceled          += on_Look;
        LeftShiftAction.Action.canceled     += on_LeftShift;
        mouseLeftAction.Action.canceled     += on_MouseLeft;
        mouseRightAction.Action.canceled    += on_MouseRight;
        SpaceAction.Action.canceled         += on_Jump;
        CtrlAction.Action.canceled          += on_Crouch;
        Escape_Action.Action.canceled       += on_PauseMenu;
        Tab_Action.Action.canceled          += on_Tab;
        R_Action.Action.canceled            += on_R;
        B_Action.Action.canceled            += on_B;
        T_Action.Action.canceled            += on_T;
        F_Action.Action.canceled            += on_F;
        E_Action.Action.canceled            += on_E;
        Z_Action.Action.canceled            += on_Z;
        C_Action.Action.canceled            += on_C;
    }
    #endregion

    #region - Input Gethering -
    private void on_Move(InputAction.CallbackContext context)           => MoveAction.Vector            = context.ReadValue<Vector2>();
    private void on_Look(InputAction.CallbackContext context)           => LookAction.Vector            = context.ReadValue<Vector2>();
    private void on_LeftShift(InputAction.CallbackContext context)      => LeftShiftAction.State        = context.ReadValueAsButton();
    private void on_MouseLeft(InputAction.CallbackContext context)      => mouseLeftAction.State        = context.ReadValueAsButton();
    private void on_MouseRight(InputAction.CallbackContext context)     => mouseRightAction.State       = context.ReadValueAsButton();
    private void on_Jump(InputAction.CallbackContext context)           => SpaceAction.State            = context.ReadValueAsButton();
    private void on_Crouch(InputAction.CallbackContext context)         => CtrlAction.State             = context.ReadValueAsButton();
    private void on_PauseMenu(InputAction.CallbackContext context)      => Escape_Action.State          = context.ReadValueAsButton();
    private void on_Tab(InputAction.CallbackContext context)            => Tab_Action.State             = context.ReadValueAsButton();
    private void on_R(InputAction.CallbackContext context)              => R_Action.State               = context.ReadValueAsButton();
    private void on_B(InputAction.CallbackContext context)              => B_Action.State               = context.ReadValueAsButton();
    private void on_T(InputAction.CallbackContext context)              => T_Action.State               = context.ReadValueAsButton();
    private void on_F(InputAction.CallbackContext context)              => F_Action.State               = context.ReadValueAsButton();
    private void on_E(InputAction.CallbackContext context)              => E_Action.State               = context.ReadValueAsButton();
    private void on_Z(InputAction.CallbackContext context)              => Z_Action.State               = context.ReadValueAsButton();
    private void on_C(InputAction.CallbackContext context)              => C_Action.State               = context.ReadValueAsButton();
    #endregion
}

[Serializable]
public class InputContent
{
    public InputAction Action;
    public bool        State;
    public Vector2     Vector;
}