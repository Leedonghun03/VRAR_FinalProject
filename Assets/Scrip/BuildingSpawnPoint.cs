using UnityEngine;

public class BuildingSpawnPoint : MonoBehaviour
{
    [Header("이 스폰 포인트 전용 건물 리스트")]
    public GameObject[] buildingPrefabs;

    // 이 스폰 포인트에 건물 생성
    public GameObject SpawnBuilding()
    {
        // 건물 프리팹이 없으면 생성 안 함
        if (buildingPrefabs == null || buildingPrefabs.Length == 0)
        {
            return null;
        }

        // 랜덤하게 건물 프리팹 선택
        GameObject selectedPrefab = buildingPrefabs[Random.Range(0, buildingPrefabs.Length)];
        
        // 건물 생성 (이 스폰 포인트를 부모로 설정)
        GameObject building = Instantiate(selectedPrefab, transform.position, transform.rotation, transform);
        
        return building;
    }
}
