using System.Drawing;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    // 차량 스폰 포인트들
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
