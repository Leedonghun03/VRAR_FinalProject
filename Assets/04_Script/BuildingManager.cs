using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // RoadManager가 타일 생성 시 호출
    public void SetupBuildingsOnTile(RoadTile tile)
    {
        if (tile.buildingSpawnPoints == null || tile.buildingSpawnPoints.Length == 0)
        {
            return;
        }

        // 각 스폰 포인트에서 건물 생성 (리스트에서 랜덤하게 하나 선택)
        foreach (BuildingSpawnPoint spawnPoint in tile.buildingSpawnPoints)
        {
            if (spawnPoint != null)
            {
                spawnPoint.SpawnBuilding();
            }
        }
    }
}
