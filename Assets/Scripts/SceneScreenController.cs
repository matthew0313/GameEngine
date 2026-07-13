using PrimeTween;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneScreenController : MonoBehaviour, IPointerDownHandler
{
    public Camera sceneCamera;
    public GridGraphic grid;

    public float moveSpeed = 0.5f;

    public float zoomSpeed = 5.0f;  
    public float minSize = 2.0f;   
    public float maxSize = 20.0f;

    public Color sceneColor, prefabColor;
    float baseSpacing;
    void Awake()
    {
        baseSpacing = grid.spacing;
    }

    bool dragging = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Middle)
        {
            eventData.Use();
            dragging = true;
        }
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.Use();
            EditorSceneManager.Instance.rightClickMenu.Open(Input.mousePosition, MakeRightClickMenu());
        }
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Move to Center",
            ctx =>
            {
                MoveTo(Vector2.zero, true);
            });
    }
    const float lerpTime = 0.3f;
    Tween moveTween;
    public void MoveTo(Vector2 pos, bool lerp = false)
    {
        if (lerp)
        {
            moveTween.Stop();
            moveTween = Tween.Position(sceneCamera.transform, new Vector3(pos.x, pos.y, sceneCamera.transform.position.z), lerpTime, Ease.OutCirc).OnUpdate(sceneCamera.transform, (cam, tween) =>
            {
                grid.offset = -sceneCamera.transform.position * grid.spacing;
                grid.SetVerticesDirty();
            });
        }
        else
        {
            sceneCamera.transform.position = new Vector3(pos.x, pos.y, sceneCamera.transform.position.z);
            grid.offset = -sceneCamera.transform.position * grid.spacing;
            grid.SetVerticesDirty();
        }
    }
    void Update()
    {
        sceneCamera.backgroundColor = EditorSceneManager.Instance.myScene.prefabMode ? prefabColor : sceneColor;
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
                newSize = Mathf.Clamp(newSize, minSize, maxSize);
                sceneCamera.orthographicSize = newSize;
                grid.spacing = baseSpacing / (newSize / minSize);
                grid.offset = -sceneCamera.transform.position * grid.spacing;
                grid.SetVerticesDirty();
            }
        }

        if (dragging && !Input.GetMouseButton(2)) dragging = false;
        if (dragging)
        {
            moveTween.Stop();
            Vector2 delta = Input.mousePositionDelta;
            float sensitivity = sceneCamera.orthographicSize * 0.01f;
            float moveX = -delta.x * moveSpeed * sensitivity;
            float moveY = -delta.y * moveSpeed * sensitivity;

            sceneCamera.transform.Translate(new Vector3(moveX, moveY, 0));

            grid.offset = -sceneCamera.transform.position * grid.spacing;
            grid.SetVerticesDirty();
        }
    }
}