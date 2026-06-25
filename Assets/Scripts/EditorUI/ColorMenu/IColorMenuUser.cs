using UnityEngine;

// Anything that can drive the shared ColorMenu: it provides the current color
// and receives updates while the menu is open. Positioning of the menu is the
// caller's responsibility (set ColorMenu.transform.position when opening).
public interface IColorMenuUser
{
    Color color { get; }
    void SetColor(Color color);
}
