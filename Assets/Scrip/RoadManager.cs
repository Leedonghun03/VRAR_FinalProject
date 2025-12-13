using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public Transform player;
    public GameObject[] roadPrefabs; // 1~5번 타일 프리팹들 배열로 넣기
    public float segmentLength = 10f;
    public int startSegments = 6;
    public float despawnDistance = 40f;

    private float nextSpawnZ = 0f;
    private Queue<RoadTile> segments = new Queue<RoadTile>();
    private GameObject nextPrefab = null;

    private int specialCooldown = 0;
    private const int minCooldown = 2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < startSegments; i++)
        {
            SpawnSegment(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.z + segmentLength * 2f > nextSpawnZ)
        {
            SpawnSegment(false);
        }

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
        
        segments.Enqueue(tile);

        // 타일이 생성될 때 차량 스폰도 같이 세팅
        if (TrafficManager.Instance)
            TrafficManager.Instance.SetupTrafficOnTile(tile);

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
        // 시작 구간은 너무 빡세지 않게 직진/4차선 위주
        if (isStart)
        {
            List<GameObject> safeList = new List<GameObject>();
            foreach (var p in roadPrefabs)
            {
                var t = p.GetComponent<RoadTile>();
                if (t.tileType == RoadTileType.Straight || t.tileType == RoadTileType.FourLane)
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
