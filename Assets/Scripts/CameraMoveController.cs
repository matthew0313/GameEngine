using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMoveController : MonoBehaviour, IPointerDownHandler
{
    public Camera sceneCamera;

    public float moveSpeed = 0.5f;

    public float zoomSpeed = 5.0f;  
    public float minSize = 2.0f;   
    public float maxSize = 20.0f;  
    bool dragging = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Middle)
        {
            dragging = true;
        }
    }

    void Update()
    {
        if (dragging && !Input.GetMouseButton(2)) dragging = false;
        if (dragging)
        {
            Vector2 delta = Input.mousePositionDelta;
            float sensitivity = sceneCamera.orthographicSize * 0.01f;
            float moveX = -delta.x * moveSpeed * sensitivity;
            float moveY = -delta.y * moveSpeed * sensitivity;

            sceneCamera.transform.Translate(new Vector3(moveX, moveY, 0));
        }
        bool hitSelf = false;
        foreach (var hit in EditorSceneManager.Instance.RaycastUI(Input.mousePosition))
        {
            if (hit.gameObject == gameObject || hit.gameObject.transform.IsChildOf(transform))
            {
                hitSelf = true; break;
            }
        }
        if (hitSelf)
        {
            if (sceneCamera == null) return;
            float scroll = Input.mouseScrollDelta.y;

            if (scroll != 0)
            {
                float newSize = sceneCamera.orthographicSize - (scroll * zoomSpeed);
                sceneCamera.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
            }
        }
    }
}