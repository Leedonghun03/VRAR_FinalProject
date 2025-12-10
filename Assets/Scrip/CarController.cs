using UnityEngine;

public class CarController : MonoBehaviour
{
    public float speed = 10f;
    public bool isFromSide;          // 옆에서 진입하는 타입인지
    public Transform[] waypoints;    // 회전 경로 (왼쪽 뚫린 길, 4차선 진입 등)

    private int currentIndex = 0;

    void Update()
    {
        if (waypoints != null && waypoints.Length > 0 && isFromSide)
        {
            // 사이드에서 들어와서 차선으로 합류하는 방식 (웨이포인트 따라가기)
            MoveWithWaypoints();
        }
        else
        {
            // 그냥 -Z 방향으로 직진
            transform.Translate(Vector3.back * (speed * Time.deltaTime), Space.World);
        }
    }

    void MoveWithWaypoints()
    {
        Transform target = waypoints[currentIndex];
        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * (speed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentIndex++;
            if (currentIndex >= waypoints.Length)
            {
                // 웨이포인트 다 끝나면 그냥 -Z 방향으로 직진 모드로 전환
                waypoints = null;
            }
        }
    }
}