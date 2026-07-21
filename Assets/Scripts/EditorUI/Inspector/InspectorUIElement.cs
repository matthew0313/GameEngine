using UnityEngine;

public abstract class InspectorUIElement : MonoBehaviour
{
    protected InspectorUI inspectorUI { get; private set; }
    protected IInspectable inspecting => inspectorUI.inspecting;
    protected DataUnit data => inspecting.inspectorData;
    public void Init(InspectorUI inspectorUI)
    {
        this.inspectorUI = inspectorUI;
    }
}
