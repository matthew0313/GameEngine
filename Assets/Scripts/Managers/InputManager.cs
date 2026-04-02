using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public static bool PC_LeftClick() => Instance.inputs.PC.LeftClick.triggered;
    public static bool PC_LeftClickHold() => Instance.inputs.PC.LeftClick.IsPressed();
    public static Vector2 PC_MousePos() => Instance.inputs.PC.MousePos.ReadValue<Vector2>();
    public static Vector2 PC_MouseDelta() => Instance.inputs.PC.MouseDelta.ReadValue<Vector2>();
    public static Vector2 PC_ScrollDelta() => Instance.inputs.PC.Scroll.ReadValue<Vector2>();
    public static bool PC_MiddleMouseHeld() => Instance.inputs.PC.Pan.IsPressed();

    public static bool Mobile_Touch0() => Instance.inputs.Mobile.Touch0.triggered;
    public static bool Mobile_Touch0Hold() => Instance.inputs.Mobile.Touch0.IsPressed();
    public static Vector2 Mobile_Touch0Pos() => Instance.inputs.Mobile.Touch0Pos.ReadValue<Vector2>();
    public static Vector2 Mobile_Touch0Delta() => Instance.inputs.Mobile.Touch0Delta.ReadValue<Vector2>();
    public static bool Mobile_Touch1() => Instance.inputs.Mobile.Touch1.triggered;
    public static bool Mobile_Touch1Hold() => Instance.inputs.Mobile.Touch1.IsPressed();
    public static Vector2 Mobile_Touch1Pos() => Instance.inputs.Mobile.Touch1Pos.ReadValue<Vector2>();
    public static Vector2 Mobile_Touch1Delta() => Instance.inputs.Mobile.Touch1Delta.ReadValue<Vector2>();

    public static bool LeftClickInput()
    {
        switch (DeviceManager.deviceType)
        {
            case DeviceType.Handheld: return Mobile_Touch0();
            default: return PC_LeftClick();
        }
    }
    public static bool LeftClickHoldInput()
    {
        switch(DeviceManager.deviceType)
        {
            case DeviceType.Handheld: return Mobile_Touch0Hold();
            default: return PC_LeftClickHold();
        }
    }
    public static Vector2 MousePosInput()
    {
        switch (DeviceManager.deviceType)
        {
            case DeviceType.Handheld: return Mobile_Touch0Pos();
            default: return PC_MousePos();
        }
    }
    public static Vector2 MouseDeltaInput()
    {
        switch (DeviceManager.deviceType)
        {
            case DeviceType.Handheld: return Mobile_Touch0Delta();
            default: return PC_MouseDelta();
        }
    }

    MyInputs inputs;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<InputManager>("InputManager"));
        DontDestroyOnLoad(Instance);
    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        inputs ??= new MyInputs();
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs?.Disable();
    }

    void OnDestroy()
    {
        inputs?.Dispose();
        inputs = null;
    }
}
