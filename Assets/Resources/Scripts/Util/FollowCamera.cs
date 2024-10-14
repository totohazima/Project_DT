using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public UnityEngine.Camera mainCamera;
    public Transform player;
    [HideInInspector] public float xMin, xMax, yMin, yMax;
    private void Update()
    {
        if (player == null)
            return;

        Vector3 playerPos = player.position;
        Vector3 targetPos = new Vector3(playerPos.x, playerPos.y, -10);


        float cameraHalfWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float cameraHalfHeight = mainCamera.orthographicSize;

        // �������� ��� ����
        float minX = xMin + cameraHalfWidth;
        float maxX = xMax - cameraHalfWidth;
        float minY = yMin + cameraHalfHeight;
        float maxY = yMax - cameraHalfHeight;

        // Check if the camera bounds are smaller than the stage bounds 
        bool isCameraWiderThanStage = cameraHalfWidth * 2 >= (xMax - xMin);
        bool isCameraTallerThanStage = cameraHalfHeight * 2 >= (yMax - yMin);

        //���� ���α��̰� ī�޶󺸴� �� ���
        if (isCameraWiderThanStage)
        {
            // Center the camera horizontally if the stage is narrower than the camera
            targetPos.x = (xMin + xMax) / 2;
        }
        else
        {
            // Otherwise, clamp the camera position horizontally
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
        }

        //���� ���α��̰� ī�޶󺸴� �� ���
        if (isCameraTallerThanStage)
        {
            // Center the camera vertically if the stage is shorter than the camera
            targetPos.y = (yMin + yMax) / 2;
        }
        else
        {
            // Otherwise, clamp the camera position vertically
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        transform.position = targetPos;
    }

}


