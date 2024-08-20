using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    GameInput input;
    public InputCache InputCache { get; private set; }
    //
    protected void Awake()
    {
        input = new GameInput();
        InputCache = new InputCache();
        input.Player.Move.performed += OnMoveInput;
        input.Player.Look.performed += OnLookInput;
        input.Player.Zoom.performed += OnZoomInput;
        input.Player.Rise.performed += OnRiseInput;
        input.Player.Sprint.performed += OnSprintInput;
        input.Player.LeftClick.performed += OnLeftClickInput;
        input.Player.RightClick.performed += OnRightClickInput;
    }
    //
    protected void OnEnable()
    {
        input.Player.Enable();
    }
    //
    protected void OnDisable()
    {
        input.Player.Disable();
    }
    //
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<Vector2>());
        InputCache.moveInput = context.ReadValue<Vector2>();
    }
    public void OnLookInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<Vector2>());
        InputCache.lookInput = context.ReadValue<Vector2>();
    }
    public void OnZoomInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<float>());
        InputCache.zoomInput = context.ReadValue<float>();
    }
    public void OnRiseInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValueAsButton());
        InputCache.riseInput = context.ReadValueAsButton();
    }
    public void OnSprintInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValueAsButton());
        InputCache.sprintInput = context.ReadValueAsButton();
    }
    public void OnLeftClickInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValueAsButton());
        InputCache.leftClickInput = context.ReadValueAsButton();
    }
    public void OnRightClickInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValueAsButton());
        InputCache.rightClickInput = context.ReadValueAsButton();
    }
}
public class InputCache
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public float zoomInput;
    public bool riseInput;
    public bool sprintInput;
    public bool leftClickInput;
    public bool rightClickInput;
}