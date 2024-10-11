using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraUsable : MonoBehaviour, ICustomUpdateMono
{
    public HeroCharacter trackingTarget;
    public SubCameraUsable subCameraUsable;
    private new Camera camera;
    private Transform cameraTransform;
    private const float DirectionForceReduceRate = 0.935f; // ���Ӻ���
    private const float DirectionForceMin = 0.001f; // ����ġ ������ ��� �������� ����
    private Vector3 startPosition;  // �Է� ���� ��ġ�� ���
    private Vector3 directionForce; // ������ �������� ������ �����ϸ鼭 �̵� ��Ű��

    [Header("Bool_Variable")]
    public bool isDontMove; //UI�� �� ���� �� �������� �ʰ�
    public bool isCrossLimitLine; //true�϶� ī�޶� �� �ٱ����� ������ �� ����
    public bool isCameraMove; // ���� ������ �ϰ��ִ��� Ȯ���� ���� ����
    public bool isTrackingTarget; //true�϶� Ŭ���� ������ ����
    private bool onStopTracking = false;

    [Header("CameraViewBox")]
    public Vector3 boxSize = new Vector3(1f, 1f, 1f);
    public float viewSize_Default = 0f;
    public float viewSize_Tracking = 0f;
    private float xMin, xMax, yMin, yMax; //ī�޶� �̵��� �����ϴ� 4���� ��ǥ

    void Awake()
    {
        camera = GetComponent<Camera>();
        cameraTransform = camera.transform;

        subCameraUsable = GetComponentInChildren<SubCameraUsable>();
    }
    private void OnEnable()
    {
        CustomUpdateManager.customUpdateMonos.Add(this);
    }
    private void OnDisable()
    {
        CustomUpdateManager.customUpdateMonos.Remove(this);
    }

    public void CustomUpdate()
    {
        StatusUpdate();

        if (!isDontMove)
        {
            // ī�޶� ������ �̵�
            ControlCameraPosition();

            // ������ �������� ����
            ReduceDirectionForce();

            // ī�޶� ��ġ ������Ʈ
            UpdateCameraPosition();
        }

        if (!isCrossLimitLine)
        {
            //ī�޶� �̵� ������ǥ ������Ʈ
            LimitPositionSet();

            //ī�޶� �� ���� ������ ���ϰ� �̵�
            CameraMoveLimit();
        }
    }

    protected void StatusUpdate()
    {
        if (trackingTarget != null)
        {
            isTrackingTarget = true;
            camera.orthographicSize = viewSize_Tracking;
        }
        else
        {
            isTrackingTarget = false;
            camera.orthographicSize = viewSize_Default;
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
        //�����Ϳ��� Scene, Simulator���� �� ��� �����޽��� ������
        if (EditorWindow.focusedWindow == null || EditorWindow.focusedWindow.titleContent.text != "Game" && EditorWindow.focusedWindow.titleContent.text != "Simulator")
        {
            return;
        }
#endif

        if (isTrackingTarget)
        {
            Vector3 pos = trackingTarget.myObject.position;
            cameraTransform.position = new Vector3(pos.x, pos.y, cameraTransform.position.z);

            // ����� ��ġ �Ǵ� PC Ŭ��
            if (IsTouchOrClick() && !IsPointerOverUI())
            {
                Vector3 clickPosition = GetTouchOrMouseWorldPosition();
                StartCoroutine(StopTracking(clickPosition));
            }
        }
        else
        {
            Vector3 mouseWorldPosition = GetTouchOrMouseWorldPosition();

            if (IsTouchOrClick() && !IsPointerOverUI())
            {
                CameraPositionMoveStart(mouseWorldPosition);
            }
            else if (IsTouchHeld() && !IsPointerOverUI())
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
        isDontMove = true;

        Ray ray = camera.ScreenPointToRay(clickPosition); // Camera.main ��� camera ���
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // �ʿ��� ��쿡�� ó��
            yield return null;
        }
        else if (trackingTarget != null)
        {
            trackingTarget = null;
        }

        yield return new WaitForSeconds(0.3f);

        isDontMove = false;
        onStopTracking = false;
    }

    void CameraPositionMoveStart(Vector3 startPosition)
    {
        isCameraMove = true;
        this.startPosition = startPosition;
        directionForce = Vector2.zero;
    }

    void CameraPositionMoveProgress(Vector3 targetPosition)
    {
        if (!isCameraMove)
        {
            CameraPositionMoveStart(targetPosition);
            return;
        }
        // directionForce ���� �ּ�ȭ
        directionForce = startPosition - targetPosition;
    }

    void CameraPositionMoveEnd()
    {
        isCameraMove = false;
    }

    protected void ReduceDirectionForce()
    {
        // ���� ���϶��� �ƹ��͵� ����
        if (isCameraMove)
        {
            return;
        }

        // ���� ��ġ ����
        directionForce *= DirectionForceReduceRate;

        // ���� ��ġ�� �Ǹ� ������ ����
        if (directionForce.magnitude < DirectionForceMin)
        {
            directionForce = Vector3.zero;
        }
    }

    protected void UpdateCameraPosition()
    {
        // �̵� ��ġ�� ������ �ƹ��͵� ����
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

        // �������� ��� ����
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

    // ��ġ �Ǵ� ���콺 �Է� ���� �Լ�
    private bool IsTouchOrClick()
    {
        return Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
    }

    // ��ġ �Ǵ� ���콺 �巡�� ���� �Լ�
    private bool IsTouchHeld()
    {
        return Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved);
    }

    // ��ġ �Ǵ� ���콺 �巡�װ� UI������ �۵��ߴ��� ���� �Լ�
    bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)  //�����
        {
            Touch touch = Input.GetTouch(0);
            return EventSystem.current.IsPointerOverGameObject(touch.fingerId); // ��ġ�� fingerId�� ����Ͽ� UI �˻�
        }
        else //PC ���콺
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    // ��ġ �Ǵ� ���콺 �Է� ��ġ ��ȯ �Լ�
    private Vector3 GetTouchOrMouseWorldPosition()
    {
        if (Input.touchCount > 0)
        {
            return camera.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            return camera.ScreenToWorldPoint(Input.mousePosition);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        //Ž�� �þ�
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
#endif
}
