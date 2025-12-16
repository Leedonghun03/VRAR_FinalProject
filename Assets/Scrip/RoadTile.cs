using UnityEngine;

public enum RoadTileType
{
    Straight,
    LeftOpen,
    FourLane,
    BusStop
}

public class RoadTile : MonoBehaviour
{
    public RoadTileType tileType;
    
    // 차량 스폰 관리
    [HideInInspector] public bool trafficSpawned = false;
    public float spawnTriggerDistance = 30f; // 플레이어가 이 거리 안에 들어오면 차량 스폰
    
    [Header("건물 스폰")]
    [Tooltip("각 스폰 포인트에 BuildingSpawnPoint 컴포넌트가 있어야 함")]
    public BuildingSpawnPoint[] buildingSpawnPoints;
    
    // 차량 스폰 포인트들
    [Header("차량 스폰")]
    public Transform[] forwardSpawnPoints; // 정면에서 -z로 오는 차량
    
    // 사이드 차량용 웨이포인트 경로
    [System.Serializable]
    public class CarPath
    {
        public Transform spawnPoint;
        public Transform[] waypoints;
    }

    public CarPath[] sideCarPaths;
    public CarPath[] fourLaneCarPaths;
    public CarPath[] busCarPaths;
}
