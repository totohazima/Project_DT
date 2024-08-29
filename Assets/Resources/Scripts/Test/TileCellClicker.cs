using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileCellClicker : MonoBehaviour
{
    public Tilemap tilemap; // 타일맵 참조

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is not assigned!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 버튼 클릭
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // 2D 타일맵에 맞추기

            Vector3Int tilePosition = tilemap.WorldToCell(mouseWorldPosition);
            Vector3 cellCenterWorldPosition = tilemap.GetCellCenterWorld(tilePosition);

            TileBase tile = tilemap.GetTile(tilePosition);
            if (tile != null)
            {
                // 타일의 z값을 얻기 위한 방법
                float tileZ = GetTileZPosition(tilePosition);
                Debug.Log($"Tile clicked at: {tilePosition} with Z position: {tileZ}");
            }
            else
            {
                Debug.Log("No tile found at position: " + tilePosition);
            }
        }
    }

    float GetTileZPosition(Vector3Int tilePosition)
    {
        // 타일 중앙의 월드 좌표를 기준으로 Z 값을 계산하는 로직
        Vector3 cellCenterWorldPosition = tilemap.GetCellCenterWorld(tilePosition);

        // 타일의 Z 포지션을 계산하는 방법 (예: Tilemap의 Z 레벨 조정값을 사용하는 경우)
        // 이는 타일맵 설정에 따라 달라질 수 있습니다. 예제에서는 Z 값을 직접 설정한다고 가정합니다.
        // 실제로는 타일의 Z 포지션을 저장하거나, 타일의 특정 속성 값을 조회하는 방식이 필요할 수 있습니다.
        return cellCenterWorldPosition.z; // 이 부분은 타일맵 설정에 맞게 조정할 수 있습니다.
    }
}
