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
    //Public Data
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }

    public bool sprint              = false;
    public bool R_State             = false;
    public bool mouseLeft           = false;
    public bool mouseRight          = false;
    public bool jumping             = false;
    public bool crouching           = false;
    public bool changingGunMode     = false;
    public bool isHoldingRock       = false;
    public bool flashlightActive    = false;
    public bool pauseMenu           = false;
    public bool E_State             = false;
    public bool Z_State             = false;
    public bool permanentHolst      = false;
    public bool Tab_State           = false;

    //Private Data
    public InputActionMap currentMap = null;

    //Movment Actions
    [HideInInspector] public InputAction moveAction     = null;
    [HideInInspector] public InputAction lookAction     = null;
    [HideInInspector] public InputAction jumpAction     = null;
    [HideInInspector] public InputAction crouchAction   = null;
    [HideInInspector] public InputAction sprintAction   = null;

    //Gun Behavior Actions
    [HideInInspector] public InputAction R_Action           = null;
    [HideInInspector] public InputAction mouseLeftAction    = null;
    [HideInInspector] public InputAction mouseRightAction   = null;
    [HideInInspector] public InputAction gunModeAction      = null;
    [HideInInspector] public InputAction throwRockAction    = null;
    [HideInInspector] public InputAction permaHolstAction   = null;

    [HideInInspector] public InputAction flashLightAction   = null;

    [HideInInspector] public InputAction primaryGun         = null;
    [HideInInspector] public InputAction secondaryGun       = null;

    [HideInInspector] public InputAction pauseMenuAction    = null;
    [HideInInspector] public InputAction E_Action           = null;
    [HideInInspector] public InputAction Z_Action           = null;
    [HideInInspector] public InputAction Tab_Action         = null;

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
        moveAction          = currentMap.FindAction("Move");
        lookAction          = currentMap.FindAction("Look");
        sprintAction        = currentMap.FindAction("SprintAction");
        jumpAction          = currentMap.FindAction("JumpAction");
        crouchAction        = currentMap.FindAction("CrouchAction");
        throwRockAction     = currentMap.FindAction("ThrowRockAction");

        //Gun Behavior Actions
        R_Action            = currentMap.FindAction("R_Action");
        mouseLeftAction     = currentMap.FindAction("LeftMouseAction");
        mouseRightAction    = currentMap.FindAction("RightMouseAction");
        gunModeAction       = currentMap.FindAction("ChangeGunMode");
        permaHolstAction    = currentMap.FindAction("PermanentHolst");

        flashLightAction    = currentMap.FindAction("FlashLightAction");

        primaryGun          = currentMap.FindAction("PrimaryGun");
        secondaryGun        = currentMap.FindAction("SecondaryGun");
        pauseMenuAction     = currentMap.FindAction("PauseMenu");
        E_Action            = currentMap.FindAction("E_Action");
        Z_Action            = currentMap.FindAction("Z_Action");
        Tab_Action          = currentMap.FindAction("Tab_Action");

        moveAction.performed        += onMove;
        lookAction.performed        += onLook;
        sprintAction.performed      += onSprint;
        R_Action.performed          += on_R;
        mouseLeftAction.performed   += onMouseLeft;
        mouseRightAction.performed  += onMouseRight;
        jumpAction.performed        += onJump;
        crouchAction.performed      += onCrouch;
        gunModeAction.performed     += onModeChanged;
        throwRockAction.performed   += onThrowRocked;
        flashLightAction.performed  += onFlashlight;
        pauseMenuAction.performed   += onPauseMenu;
        E_Action.performed          += on_E;
        Z_Action.performed          += on_Z;
        permaHolstAction.performed  += onPermaHolst;
        Tab_Action.performed        += on_Tab;

        moveAction.canceled         += onMove;
        lookAction.canceled         += onLook;
        sprintAction.canceled       += onSprint;
        R_Action.canceled           += on_R;
        mouseLeftAction.canceled    += onMouseLeft;
        mouseRightAction.canceled   += onMouseRight;
        jumpAction.canceled         += onJump;
        crouchAction.canceled       += onCrouch;
        gunModeAction.canceled      += onModeChanged;
        throwRockAction.canceled    += onThrowRocked;
        flashLightAction.canceled   += onFlashlight;
        pauseMenuAction.canceled    += onPauseMenu;
        E_Action.canceled           += on_E;
        Z_Action.canceled           += on_Z;
        permaHolstAction.canceled   += onPermaHolst;
        Tab_Action.canceled         += on_Tab;
    }
    #endregion

    #region - Input Gethering -
    private void onMove(InputAction.CallbackContext context)        => Move             = context.ReadValue<Vector2>();
    private void onLook(InputAction.CallbackContext context)        => Look             = context.ReadValue<Vector2>();
    private void onSprint(InputAction.CallbackContext context)      => sprint           = context.ReadValueAsButton();
    private void on_R(InputAction.CallbackContext context)          => R_State          = context.ReadValueAsButton();
    private void onMouseLeft(InputAction.CallbackContext context)   => mouseLeft        = context.ReadValueAsButton();
    private void onMouseRight(InputAction.CallbackContext context)  => mouseRight       = context.ReadValueAsButton();
    private void onJump(InputAction.CallbackContext context)        => jumping          = context.ReadValueAsButton();
    private void onCrouch(InputAction.CallbackContext context)      => crouching        = context.ReadValueAsButton();
    private void onModeChanged(InputAction.CallbackContext context) => changingGunMode  = context.ReadValueAsButton();
    private void onThrowRocked(InputAction.CallbackContext context) => isHoldingRock    = context.ReadValueAsButton();
    private void onFlashlight(InputAction.CallbackContext context)  => flashlightActive = context.ReadValueAsButton();
    private void onPauseMenu(InputAction.CallbackContext context)   => pauseMenu        = context.ReadValueAsButton();
    private void on_E(InputAction.CallbackContext context)          => E_State          = context.ReadValueAsButton();
    private void on_Z(InputAction.CallbackContext context)          => Z_State          = context.ReadValueAsButton();
    private void onPermaHolst(InputAction.CallbackContext context)  => permanentHolst   = context.ReadValueAsButton();
    private void on_Tab(InputAction.CallbackContext context)        => Tab_State        = context.ReadValueAsButton();
    #endregion
}