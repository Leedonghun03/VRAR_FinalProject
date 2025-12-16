using UnityEngine;
using System.Collections.Generic;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager Instance { get; private set; }

    public GameObject[] carPrefabs; // 일반 차량 리스트
    public GameObject[] busPrefabs; // 버스 리스트
    public Transform player; // 플레이어 참조 (차량 삭제 체크용)

    [Header("직진 차량 스폰 거리 설정")] public float forwardCarSpawnDistanceMin = 15f; // 스폰 포인트보다 뒤쪽 최소 거리
    public float forwardCarSpawnDistanceMax = 25f; // 스폰 포인트보다 뒤쪽 최대 거리

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
                // 4차선은 좌/우 한쪽만 선택
                SetupFourLaneCars(tile);
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

    // 4차선 전용: 좌/우 한쪽만 랜덤 선택
    void SetupFourLaneCars(RoadTile tile)
    {
        if (tile.fourLaneCarPaths == null || tile.fourLaneCarPaths.Length == 0)
            return;

        // 경로를 좌측/우측으로 분리
        List<RoadTile.CarPath> leftPaths = new List<RoadTile.CarPath>();
        List<RoadTile.CarPath> rightPaths = new List<RoadTile.CarPath>();

        foreach (var path in tile.fourLaneCarPaths)
        {
            // 스폰 포인트의 X 위치로 좌/우 판단 (0보다 작으면 좌측, 크면 우측)
            if (path.spawnPoint.position.x < 0)
                leftPaths.Add(path);
            else
                rightPaths.Add(path);
        }

        // 50% 확률로 좌측 또는 우측 선택
        bool useLeft = Random.value < 0.5f;
        List<RoadTile.CarPath> selectedPaths = useLeft ? leftPaths : rightPaths;

        // 선택된 쪽의 경로만 차량 생성
        SetupPathCars(selectedPaths.ToArray(), 0.6f);
    }

    // 경로가 있는 차량 (사이드, 4차선 등 - 웨이포인트 사용)
    void SetupPathCars(RoadTile.CarPath[] paths, float spawnChance = 1.0f)
    {
        if (paths == null) return;

        foreach (var path in paths)
        {
            if (Random.value < spawnChance)
            {
                SpawnCar(
                    path.spawnPoint.position,
                    path.spawnPoint.rotation,
                    true, // isFromSide = true (웨이포인트 사용)
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
            GameObject selectedBusPrefab = GetRandomBusPrefab();
            if (!selectedBusPrefab) return;
            
            // 버스는 항상 생성 (확률 없음)
            GameObject busObj = Instantiate(
                selectedBusPrefab,
                path.spawnPoint.position,
                path.spawnPoint.rotation
            );

            BusCar bus = busObj.GetComponent<BusCar>();
            if (bus)
            {
                bus.Init(path.waypoints); // 웨이포인트 전달
                bus.player = player; // 플레이어 참조 전달
            }
        }
    }

    // 차량 생성
    GameObject SpawnCar(Vector3 pos, Quaternion rot, bool useWaypoints, Transform[] waypoints)
    {
        GameObject selectedCarPrefab = GetRandomCarPrefab();
        if (!selectedCarPrefab) return null;
        
        GameObject obj = Instantiate(selectedCarPrefab, pos, rot);
        CarController car = obj.GetComponent<CarController>();
        if (car)
        {
            car.isFromSide = useWaypoints;
            car.waypoints = waypoints;
            car.player = player; // 플레이어 참조 전달
        }

        return obj;
    }

    GameObject GetRandomCarPrefab()
    {
        if (carPrefabs == null || carPrefabs.Length == 0)
        {
            Debug.LogWarning("[TrafficManager] 차량 프리팹이 설정되어 있지 않습니다.");
            return null;
        }

        return carPrefabs[Random.Range(0, carPrefabs.Length)];
    }
    
    GameObject GetRandomBusPrefab()
    {
        if (busPrefabs == null || busPrefabs.Length == 0)
        {
            Debug.LogWarning("[TrafficManager] 버스 프리팹이 설정되어 있지 않습니다.");
            return null;
        }

        return busPrefabs[Random.Range(0, busPrefabs.Length)];
    }
}