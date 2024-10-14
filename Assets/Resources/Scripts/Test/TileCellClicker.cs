using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class TileCellClicker : MonoBehaviour
{
    public Tilemap tilemap; // Ÿ�ϸ� ����

    void Start()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap is not assigned!");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���� ���콺 ��ư Ŭ��
        {
            Vector3 mouseWorldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0; // 2D Ÿ�ϸʿ� ���߱�

            Vector3Int tilePosition = tilemap.WorldToCell(mouseWorldPosition);
            Vector3 cellCenterWorldPosition = tilemap.GetCellCenterWorld(tilePosition);

            TileBase tile = tilemap.GetTile(tilePosition);
            if (tile != null)
            {
                // Ÿ���� z���� ��� ���� ���
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
        // Ÿ�� �߾��� ���� ��ǥ�� �������� Z ���� ����ϴ� ����
        Vector3 cellCenterWorldPosition = tilemap.GetCellCenterWorld(tilePosition);

        // Ÿ���� Z �������� ����ϴ� ��� (��: Tilemap�� Z ���� �������� ����ϴ� ���)
        // �̴� Ÿ�ϸ� ������ ���� �޶��� �� �ֽ��ϴ�. ���������� Z ���� ���� �����Ѵٰ� �����մϴ�.
        // �����δ� Ÿ���� Z �������� �����ϰų�, Ÿ���� Ư�� �Ӽ� ���� ��ȸ�ϴ� ����� �ʿ��� �� �ֽ��ϴ�.
        return cellCenterWorldPosition.z; // �� �κ��� Ÿ�ϸ� ������ �°� ������ �� �ֽ��ϴ�.
    }
}
