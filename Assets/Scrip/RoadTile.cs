using UnityEngine;

public enum RoadTileType
{
    Straight,
    LeftOpen,
    TrafficLight,
    FourLane,
    BusStop
}

public class RoadTile : MonoBehaviour
{
    public RoadTileType tileType;
    
    // 차량 스폰 포인트들
    public Transform[] forwardSpawnPoints; // 정면에서 -z로 오는 차량
    public Transform[] sideSpawnPoints;    // 왼쪽/오른쪽에서 회전해서 나오는 차량
    public Transform[] busSpawnPoints;     // 버스 승강장용
}
