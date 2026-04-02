using UnityEngine;

public class DeviceManager : MonoBehaviour
{
    public static DeviceManager Instance { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<DeviceManager>("DeviceManager"));
        DontDestroyOnLoad(Instance);
    }
    [SerializeField] bool overrideDevice = false;
    [SerializeField] DeviceType overridden = DeviceType.Desktop;
    public static DeviceType deviceType => Instance.overrideDevice ? Instance.overridden : SystemInfo.deviceType;
}