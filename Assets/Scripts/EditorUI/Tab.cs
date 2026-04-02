using UnityEngine;
using UnityEngine.UI;

public abstract class Tab : MonoBehaviour
{
    public virtual void Open() { }
    public virtual void Close() { }
}