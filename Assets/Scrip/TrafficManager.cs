using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager Instance { get; private set; }

    public GameObject carPrefab;     // 일반 차량
    public GameObject busPrefab;     // 버스

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

    // 직진 차량 (웨이포인트 없음)
    void SetupForwardCars(RoadTile tile)
    {
        if (tile.forwardSpawnPoints == null) return;

        foreach (var sp in tile.forwardSpawnPoints)
        {
            if (Random.value < 0.4f) // 40% 확률로 스폰
            {
                SpawnCar(sp.position, sp.rotation, false, null);
            }
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
        }
        return obj;
    }
}
