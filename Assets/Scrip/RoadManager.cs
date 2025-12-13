using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public Transform player;
    public GameObject[] roadPrefabs;
    public float segmentLength = 10f;
    public int startSegments = 6;
    public float despawnDistance = 100f;

    private float nextSpawnZ = 0f;
    private Queue<RoadTile> segments = new Queue<RoadTile>();
    private GameObject nextPrefab = null;

    private int specialCooldown = 0;
    private const int minCooldown = 2;
    
    void Start()
    {
        for (int i = 0; i < startSegments; i++)
        {
            SpawnSegment(true);
        }
    }

    void Update()
    {
        // 새 타일 생성
        if (player.position.z + segmentLength * 2f > nextSpawnZ)
        {
            SpawnSegment(false);
        }

        // 각 타일 체크하여 플레이어가 가까워지면 차량 스폰
        foreach (var tile in segments)
        {
            if (!tile.trafficSpawned)
            {
                float dist = tile.transform.position.z - player.position.z;
                if (dist < tile.spawnTriggerDistance)
                {
                    if (TrafficManager.Instance)
                        TrafficManager.Instance.SetupTrafficOnTile(tile);
                    tile.trafficSpawned = true;
                }
            }
        }

        // 오래된 타일 제거
        if (segments.Count > 0)
        {
            RoadTile first = segments.Peek();
            if (first.transform.position.z + despawnDistance < player.position.z)
            {
                RoadTile old = segments.Dequeue();
                Destroy(old.gameObject);
            }
        }
    }
    
    void SpawnSegment(bool isStart)
    {
        if(!nextPrefab)
            nextPrefab = ChooseRandomTile(isStart);
    
        GameObject prefab = nextPrefab;
        
        // 현재 nextSpawnZ 위치에 타일 생성
        Vector3 pos = new Vector3(0f, 0f, nextSpawnZ);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        RoadTile tile = obj.GetComponent<RoadTile>();
        if (!tile)
        {
            Debug.Log("No tile found");
            return;
        }
        
        // 초반 시작 구간은 차량 스폰 안되도록 설정
        if (isStart)
        {
            tile.trafficSpawned = true;  // 차량 스폰 비활성화
        }
        
        segments.Enqueue(tile);

        // 타일 생성 시 차량 스폰은 하지 않음 (Update에서 거리 체크 후 스폰)
        nextPrefab = ChooseRandomTile(false);
        RoadTile nextTile = nextPrefab.GetComponent<RoadTile>();
        
        float spacing;
        if (tile.tileType == RoadTileType.BusStop || (nextTile && nextTile.tileType == RoadTileType.BusStop))
            spacing = 15.0f;
        else
            spacing = segmentLength;

        // 생성된 타일의 길이만큼 다음 스폰 위치 증가
        nextSpawnZ += spacing;
    }
    
    GameObject ChooseRandomTile(bool isStart)
    {
        // 시작 구간은 너무 빡세지 않게 직진만
        if (isStart)
        {
            List<GameObject> safeList = new List<GameObject>();
            foreach (var p in roadPrefabs)
            {
                var t = p.GetComponent<RoadTile>();
                if (t.tileType == RoadTileType.Straight)
                    safeList.Add(p);
            }
            return safeList[Random.Range(0, safeList.Count)];
        }

        // 쿨다운 중이면 일반 패턴만
        if (specialCooldown > 0)
        {
            specialCooldown--;
            return GetNormalTile();
        }

        if (Random.value < 0.3f)
        {
            GameObject specialTile = GetSpecialTile();
            specialCooldown = minCooldown;
            return specialTile;
        }

        return GetNormalTile();
    }

    GameObject GetNormalTile()
    {
        List<GameObject> normalList = new List<GameObject>();
        foreach (var p in roadPrefabs)
        {
            var t = p.GetComponent<RoadTile>();
            if (t.tileType == RoadTileType.Straight)
                normalList.Add(p);
        }
        return normalList[Random.Range(0, normalList.Count)];
    }

    GameObject GetSpecialTile()
    {
        List<GameObject> specialList = new List<GameObject>();
        foreach (var p in roadPrefabs)
        {
            var t = p.GetComponent<RoadTile>();
            if (t.tileType == RoadTileType.LeftOpen ||
                t.tileType == RoadTileType.FourLane ||
                t.tileType == RoadTileType.BusStop)
                specialList.Add(p);
        }
        return specialList[Random.Range(0, specialList.Count)];
    }
}