using UnityEngine;

public class BusCar : MonoBehaviour
{
    public float waitTime = 1f;      // 정차 시간
    public float speed = 8f;
    public Transform[] waypoints;    // 승강장 → 도로 진입 경로

    private bool started = false;
    private float timer = 0f;
    private int currentIndex = 0;

    public void Init(Transform[] exitPath)
    {
        waypoints = exitPath;
    }

    void Update()
    {
        // 정차 중
        if (!started)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                started = true;
                Debug.Log("[BusCar] 출발!");
            }
            return;
        }

        // 출발 후 이동
        if (waypoints != null && waypoints.Length > 0)
        {
            MoveWithWaypoints();
        }
        else
        {
            // 웨이포인트 없으면 그냥 -Z 직진
            transform.Translate(Vector3.back * (speed * Time.deltaTime), Space.World);
        }
    }

    void MoveWithWaypoints()
    {
        if (currentIndex >= waypoints.Length)
        {
            // 웨이포인트 끝나면 직진 모드
            transform.Translate(Vector3.back * (speed * Time.deltaTime), Space.World);
            return;
        }

        Transform target = waypoints[currentIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        
        // 이동
        transform.position += dir * (speed * Time.deltaTime);
        
        // 회전
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, 15f * Time.deltaTime);
        }

        // 웨이포인트 도달 체크
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentIndex++;
        }
    }
}