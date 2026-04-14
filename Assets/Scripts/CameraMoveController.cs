using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMoveController : MonoBehaviour, IDragHandler
{
    public Camera sceneCamera;

    [Header("이동 설정")]
    public float moveSpeed = 0.5f;

    [Header("줌 설정")]
    public float zoomSpeed = 5.0f;  // 휠을 돌릴 때 확대/축소되는 속도
    public float minSize = 2.0f;   // 가장 많이 확대했을 때 (숫자가 작을수록 확대)
    public float maxSize = 20.0f;  // 가장 많이 축소했을 때 (숫자가 클수록 축소)

    // 마우스 드래그 이동 (기존 기능)
    public void OnDrag(PointerEventData eventData)
    {
        if (sceneCamera == null) return;

        // 카메라의 현재 크기(Orthographic Size)에 맞춰 이동 속도를 보정하면
        // 확대했을 때와 축소했을 때 이동 속도가 일정하게 느껴집니다.
        float sensitivity = sceneCamera.orthographicSize * 0.01f;
        float moveX = -eventData.delta.x * moveSpeed * sensitivity;
        float moveY = -eventData.delta.y * moveSpeed * sensitivity;

        sceneCamera.transform.Translate(new Vector3(moveX, moveY, 0));
    }

    // 매 프레임마다 실행되는 함수 (휠 감지용)
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) { 
            if (sceneCamera == null) return;

            // 마우스 휠의 움직임을 가져옵니다 (위로 올리면 +, 아래로 내리면 -)
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                // 카메라의 Orthographic Size를 조절합니다.
                float newSize = sceneCamera.orthographicSize - (scroll * zoomSpeed);

                // 크기가 너무 커지거나 작아지지 않게 한계치를 정해줍니다.
                sceneCamera.orthographicSize = Mathf.Clamp(newSize, minSize, maxSize);
            }
        }
    }
}