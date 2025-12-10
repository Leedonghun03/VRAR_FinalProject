using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public Transform player;
    public GameObject[] roadPrefabs; // 1~5번 타일 프리팹들 배열로 넣기
    public float segmentLength = 20f;
    public int startSegments = 6;
    public float despawnDistance = 40f;

    private float nextSpawnZ = 0f;
    private Queue<RoadTile> segments = new Queue<RoadTile>();
    
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
        GameObject prefab = ChooseRandomTile(isStart);

        Vector3 pos = new Vector3(0f, 0f, nextSpawnZ);
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        RoadTile tile = obj.GetComponent<RoadTile>();
        segments.Enqueue(tile);

        // 타일이 생성될 때 차량 스폰도 같이 세팅
        TrafficManager.Instance.SetupTrafficOnTile(tile);

        nextSpawnZ += segmentLength;
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

        // 그 외는 그냥 랜덤
        int idx = Random.Range(0, roadPrefabs.Length);
        return roadPrefabs[idx];
    }
}
