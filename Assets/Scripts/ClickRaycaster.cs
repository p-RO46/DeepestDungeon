using UnityEngine;
using UnityEngine.InputSystem;

public class ClickRaycaster : MonoBehaviour
{
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Debug.Log("마우스 클릭됨");

            // 1. 마우스 위치를 월드 좌표로 변환
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // 2. 2D 전용 레이캐스트 (마우스 위치에서 화면 안쪽 방향으로 쏘기)
            // Raycast 대신 GetRayIntersection을 쓰면 마우스 위치에 있는 2D 오브젝트를 바로 찾습니다.
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(mousePos));

            if (hit.collider != null)
            {
                GameObject clickedObject = hit.collider.gameObject;
                //Debug.Log("맞춘 2D 오브젝트: " + clickedObject.name);

                // GetComponent를 반복 호출하지 않도록 TryGetComponent 권장 (성능 최적화)
                if (clickedObject.TryGetComponent<MapObjClick>(out var mapObj))
                {
                    mapObj.ObjClicked();
                }
            }
        }
    }
}
