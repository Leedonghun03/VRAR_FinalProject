using UnityEngine;

public class BusCar : MonoBehaviour
{
    public float waitTime = 2f;  // 정차 시간
    public float speed = 8f;
    public Transform exitDirection; // 도로 쪽으로 나가는 방향용 Transform (또는 첫 웨이포인트)

    private bool started = false;
    private float timer = 0f;

    public void Init(Transform startPoint)
    {
        // 필요시 초기화
    }

    void Update()
    {
        if (!started)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                started = true;
            }
        }
        else
        {
            // 승강장에서 빠져나와 도로 쪽으로
            Vector3 dir;
            if (exitDirection)
                dir = (exitDirection.position - transform.position).normalized;
            else
                dir = Vector3.back; // 귀찮으면 그냥 -Z

            transform.position += dir * (speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 5f * Time.deltaTime);
        }
    }
}

