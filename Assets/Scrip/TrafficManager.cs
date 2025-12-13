using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager Instance { get; private set; }

    public GameObject carPrefab;     // 일반 차량
    public GameObject busPrefab;     // 버스
    public Transform player;         // 플레이어 참조 (차량 삭제 체크용)
    
    [Header("직진 차량 스폰 거리 설정")]
    public float forwardCarSpawnDistanceMin = 15f;  // 스폰 포인트보다 뒤쪽 최소 거리
    public float forwardCarSpawnDistanceMax = 25f;  // 스폰 포인트보다 뒤쪽 최대 거리

    private void Awake()
    {
        Instance = this;
    }

    // RoadManager가 타일을 생성할 때 호출
    public void SetupTrafficOnTile(RoadTile tile)
    {
        switch (tile.tileType)
        {
            case RoadTileType.Straight:
                SetupForwardCars(tile);
                break;

            case RoadTileType.LeftOpen:
                SetupForwardCars(tile);
                SetupPathCars(tile.sideCarPaths);
                break;

            case RoadTileType.FourLane:
                SetupPathCars(tile.fourLaneCarPaths);
                break;

            case RoadTileType.BusStop:
                SetupBuses(tile);
                break;
        }
    }

    // 직진 차량 (웨이포인트 없음) - 타일 뒷부분에서 생성
    // 한 번에 최대 1대만 생성하여 피할 수 있는 패턴 보장
    void SetupForwardCars(RoadTile tile)
    {
        if (tile.forwardSpawnPoints == null || tile.forwardSpawnPoints.Length == 0) 
            return;

        // 40% 확률로 차량 생성 여부 결정
        if (Random.value < 0.4f)
        {
            // 랜덤하게 하나의 차선만 선택
            Transform selectedSpawnPoint = tile.forwardSpawnPoints[Random.Range(0, tile.forwardSpawnPoints.Length)];
            
            // 스폰 포인트보다 뒤쪽(+Z 방향)에서 생성하여 멀리서 다가오는 느낌
            float spawnOffset = Random.Range(forwardCarSpawnDistanceMin, forwardCarSpawnDistanceMax);
            Vector3 spawnPos = selectedSpawnPoint.position + Vector3.forward * spawnOffset;
            
            SpawnCar(spawnPos, selectedSpawnPoint.rotation, false, null);
        }
    }

    // 경로가 있는 차량 (사이드, 4차선 등 - 웨이포인트 사용)
    void SetupPathCars(RoadTile.CarPath[] paths, float spawnChance = 0.6f)
    {
        if (paths == null) return;

        foreach (var path in paths)
        {
            if (Random.value < spawnChance)
            {
                SpawnCar(
                    path.spawnPoint.position, 
                    path.spawnPoint.rotation, 
                    true,  // isFromSide = true (웨이포인트 사용)
                    path.waypoints
                );
            }
        }
    }

    // 버스 (웨이포인트 사용)
    void SetupBuses(RoadTile tile)
    {
        if (tile.busCarPaths == null) return;

        foreach (var path in tile.busCarPaths)
        {
            // 버스는 항상 생성 (확률 없음)
            GameObject busObj = Instantiate(
                busPrefab, 
                path.spawnPoint.position, 
                path.spawnPoint.rotation
            );

            BusCar bus = busObj.GetComponent<BusCar>();
            if (bus)
            {
                bus.Init(path.waypoints);  // 웨이포인트 전달
                bus.player = player;        // 플레이어 참조 전달
            }
        }
    }

    // 차량 생성
    GameObject SpawnCar(Vector3 pos, Quaternion rot, bool useWaypoints, Transform[] waypoints)
    {
        GameObject obj = Instantiate(carPrefab, pos, rot);
        CarController car = obj.GetComponent<CarController>();
        if (car)
        {
            car.isFromSide = useWaypoints;
            car.waypoints = waypoints;
            car.player = player;  // 플레이어 참조 전달
        }
        return obj;
    }
}
