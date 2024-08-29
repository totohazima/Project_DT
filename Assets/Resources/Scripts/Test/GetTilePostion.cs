using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GetTilePostion : MonoBehaviour
{
    public Tilemap tilemap; // 타일맵을 참조
    public Transform player; // 플레이어의 Transform
    [SerializeField] Vector3 currentTilePosition;
    void Update()
    {
        // 플레이어의 현재 위치에서 타일 좌표를 얻음
        Vector3Int tilePosition = GetTilePositionUnderPlayer(player.position);
        currentTilePosition = tilePosition;
        // 디버그 목적으로 콘솔에 출력
        Debug.Log("Player is standing on tile at: " + tilePosition);
    }

    public Vector3Int GetTilePositionUnderPlayer(Vector3 worldPosition)
    {
        // 월드 좌표를 타일맵의 셀 좌표로 변환
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        // 타일맵의 범위를 확인
        BoundsInt bounds = tilemap.cellBounds;

        // 가장 가까운 z 값을 찾기 위한 변수 초기화
        float closestZ = float.MaxValue;
        Vector3Int closestCellPosition = cellPosition;

        // Z 값을 기준으로 순회하며 가장 가까운 타일 찾기
        for (int z = bounds.zMin; z < bounds.zMax; z++)
        {
            Vector3Int checkPosition = new Vector3Int(cellPosition.x, cellPosition.y, z);

            if (tilemap.HasTile(checkPosition))
            {
                // 현재 Z의 월드 좌표 구하기
                Vector3 tileWorldPosition = tilemap.GetCellCenterWorld(checkPosition);

                // 월드 좌표와 타일의 Z 포지션 비교
                float distance = Mathf.Abs(worldPosition.z - tileWorldPosition.z);

                if (distance < closestZ)
                {
                    closestZ = distance;
                    closestCellPosition = checkPosition;
                }
            }
        }

        // 가장 가까운 Z 포지션의 타일 셀 좌표 반환
        return closestCellPosition;
    }
}

