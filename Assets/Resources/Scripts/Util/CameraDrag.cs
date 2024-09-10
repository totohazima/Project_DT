using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public HeroCharacter trackingTarget;

    public bool isCameraMove; // 현재 조작을 하고있는지 확인을 위한 변수
    public bool isDontMove; //UI가 떠 있을 때 움직이지 않게
    public bool isCrossLimitLine; //true일때 카메라가 맵 바깥으로 움직일 수 있음
    public bool isTrackingTarget; //true일때 클릭한 유닛을 따라감
    [SerializeField] private bool onStopTracking = false;

    private new Camera camera;
    private Transform cameraTransform;
    private const float DirectionForceReduceRate = 0.935f; // 감속비율
    private const float DirectionForceMin = 0.001f; // 설정치 이하일 경우 움직임을 멈춤
    private Vector3 startPosition;  // 입력 시작 위치를 기억
    private Vector3 directionForce; // 조작을 멈췄을때 서서히 감속하면서 이동 시키기

    [Header("CameraViewBox")]
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    private float xMin, xMax, yMin, yMax; //카메라 이동을 제한하는 4방향 좌표
    [SerializeField] private float viewSize_Default = 0f;
    [SerializeField] private float viewSize_Tracking = 0f;
    
    void Awake()
    {
        camera = Camera.main;
        cameraTransform = camera.transform;
    }
    void Update()
    {
        StatusUpdate();

        if (!isDontMove)
        {
            // 카메라 포지션 이동
            ControlCameraPosition();

            // 조작을 멈췄을때 감속
            ReduceDirectionForce();

            // 카메라 위치 업데이트
            UpdateCameraPosition();
        }

        if (!isCrossLimitLine)
        {
            //카메라 이동 제한좌표 업데이트
            LimitPositionSet();

            //카메라가 맵 밖을 비추지 못하게 이동
            CameraMoveLimit();
        }
        
    }
    protected void StatusUpdate()
    {
        if (trackingTarget != null)
        {
            isTrackingTarget = true;
            Camera.main.orthographicSize = viewSize_Tracking;
        }
        else
        {
            isTrackingTarget = false;
            Camera.main.orthographicSize = viewSize_Default;
        }
    }
    protected void LimitPositionSet()
    {
        xMin = -boxSize.x / 2;
        xMax = boxSize.x / 2;
        yMin = -boxSize.y / 2;
        yMax = boxSize.y / 2;
    }

    protected void ControlCameraPosition()
    {
#if UNITY_EDITOR
        //에디터에서 Scene, Simulator으로 볼 경우 에러메시지 대응용
        if(EditorWindow.focusedWindow == null || EditorWindow.focusedWindow.titleContent.text != "Game" && EditorWindow.focusedWindow.titleContent.text != "Simulator")
        {
            return;
        }
#endif
        if (isTrackingTarget)
        {
            Vector3 pos = trackingTarget.myObject.position;
            cameraTransform.position = new Vector3(pos.x, pos.y, cameraTransform.position.z);

            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0) && !onStopTracking)
            {
                StartCoroutine(StopTracking(mouseWorldPosition));
            }
        }
        else
        {
            Vector3 mouseWorldPosition = camera.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                CameraPositionMoveStart(mouseWorldPosition);
            }
            else if (Input.GetMouseButton(0))
            {
                CameraPositionMoveProgress(mouseWorldPosition);
            }
            else
            {
                CameraPositionMoveEnd();
            }
        }
    }
    protected IEnumerator StopTracking(Vector3 clickPosition)
    {
        onStopTracking = true;

        Ray ray = Camera.main.ScreenPointToRay(clickPosition);
        RaycastHit hit;
        isDontMove = true;

        if (Physics.Raycast(ray, out hit))
        {
            yield return 0;
        }
        else if(trackingTarget != null) 
        {
            trackingTarget = null;
        }

        //딜레이를 주지 않으면 CameraDrag 스크립트에서 카메라를 움직여버림
        yield return new WaitForSeconds(0.3f);
        isDontMove = false;
        onStopTracking = false;
    }

    protected void CameraPositionMoveStart(Vector3 startPosition)
    {
        isCameraMove = true;
        this.startPosition = startPosition;
        directionForce = Vector2.zero;
    }
    protected void CameraPositionMoveProgress(Vector3 targetPosition)
    {
        if (isCameraMove == false)
        {
            CameraPositionMoveStart(targetPosition);
            return;
        }

        directionForce = startPosition - targetPosition;
    }
    protected void CameraPositionMoveEnd()
    {
        isCameraMove = false;
    }
    protected void ReduceDirectionForce()
    {
        // 조작 중일때는 아무것도 안함
        if (isCameraMove == true)
        {
            return;
        }

        // 감속 수치 적용
        directionForce *= DirectionForceReduceRate;

        // 작은 수치가 되면 강제로 멈춤
        if (directionForce.magnitude < DirectionForceMin)
        {
            directionForce = Vector3.zero;
        }
    }
    protected void UpdateCameraPosition()
    {
        // 이동 수치가 없으면 아무것도 안함
        if (directionForce == Vector3.zero)
        {
            return;
        }

        var currentPosition = transform.position;
        var targetPosition = currentPosition + directionForce;
        transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.5f);
    }

    protected void CameraMoveLimit()
    {
        Vector3 CameraPos = cameraTransform.position;
        
        float cameraHalfWidth = camera.orthographicSize * camera.aspect;
        float cameraHalfHeight = camera.orthographicSize;

        // 스테이지 경계 제한
        float minX = xMin + cameraHalfWidth;
        float maxX = xMax - cameraHalfWidth;
        float minY = yMin + cameraHalfHeight;
        float maxY = yMax - cameraHalfHeight;

        // Check if the camera bounds are smaller than the stage bounds
        bool isCameraWiderThanStage = cameraHalfWidth * 2 >= (xMax - xMin);
        bool isCameraTallerThanStage = cameraHalfHeight * 2 >= (yMax - yMin);

        if (isCameraWiderThanStage)
        {
            // Center the camera horizontally if the stage is narrower than the camera
            CameraPos.x = (xMin + xMax) / 2;
        }
        else
        {
            // Otherwise, clamp the camera position horizontally
            CameraPos.x = Mathf.Clamp(CameraPos.x, minX, maxX);
        }

        if (isCameraTallerThanStage)
        {
            // Center the camera vertically if the stage is shorter than the camera
            CameraPos.y = (yMin + yMax) / 2;
        }
        else
        {
            // Otherwise, clamp the camera position vertically
            CameraPos.y = Mathf.Clamp(CameraPos.y, minY, maxY);
        }

        transform.position = CameraPos;
    }

#if UNITY_EDITOR
    int segments = 100;
    bool drawWhenSelected = true;

    void OnDrawGizmosSelected()
    {
        if (drawWhenSelected)
        {
            //탐지 시야
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, boxSize);
        }
    }


#endif

}
