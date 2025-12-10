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
            case RoadTileType.TrafficLight:
                SetupForwardCars(tile);
                break;

            case RoadTileType.LeftOpen:
                SetupForwardCars(tile);
                SetupSideCars(tile);   // 옆길에서 회전해서 진입
                break;

            case RoadTileType.FourLane:
                SetupFourLanePattern(tile);
                break;

            case RoadTileType.BusStop:
                SetupBus(tile);
                break;
        }
    }

    void SetupForwardCars(RoadTile tile)
    {
        foreach (var sp in tile.forwardSpawnPoints)
        {
            if (Random.value < 0.4f) // 40% 확률로 스폰
            {
                SpawnCar(sp.position, sp.rotation, false);
            }
        }
    }

    void SetupSideCars(RoadTile tile)
    {
        foreach (var sp in tile.sideSpawnPoints)
        {
            if (Random.value < 0.6f)
            {
                SpawnCar(sp.position, sp.rotation, true); // sideCar = true
            }
        }
    }

    void SetupFourLanePattern(RoadTile tile)
    {
        // 예시: 왼, 왼, 오, 왼, 오 느낌의 패턴
        // laneSpawnPoints[0] = 제일 왼쪽, [3] = 제일 오른쪽 이런 식으로 배치했다고 가정
        var spawns = tile.forwardSpawnPoints;
        if (spawns == null || spawns.Length == 0) return;

        // 간단한 랜덤 패턴 예시
        // ex) L L R L R 느낌 내기 (0 = 왼, 3 = 오른쪽)
        int[] pattern = new int[] { 0, 0, 3, 0, 3 };

        int count = Mathf.Min(pattern.Length, spawns.Length);
        for (int i = 0; i < count; i++)
        {
            if (Random.value < 0.7f) // 70% 정도는 스폰되게
            {
                Transform lanePoint = spawns[pattern[i]];
                SpawnCar(lanePoint.position, lanePoint.rotation, false);
            }
        }
    }

    void SetupBus(RoadTile tile)
    {
        if (tile.busSpawnPoints == null || tile.busSpawnPoints.Length == 0)
            return;

        Transform busPoint = tile.busSpawnPoints[0];
        GameObject busObj = Instantiate(busPrefab, busPoint.position, busPoint.rotation);

        BusCar bus = busObj.GetComponent<BusCar>();
        if (bus != null)
        {
            bus.Init(busPoint); // 정차 → 출발 로직 안에서 처리
        }
    }

    GameObject SpawnCar(Vector3 pos, Quaternion rot, bool isFromSide)
    {
        GameObject obj = Instantiate(carPrefab, pos, rot);
        CarController car = obj.GetComponent<CarController>();
        if (car != null)
        {
            car.isFromSide = isFromSide;
        }
        return obj;
    }
}
