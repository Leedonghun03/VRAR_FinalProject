using System;
using TMPro;
using Unity.Hierarchy;
using UnityEngine;

public class ForceMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField]  private float rotSpeed = 5.0f;

    [SerializeField] private int viewMove = 0;

    [SerializeField] private GameObject[] goPointsObj;
    [SerializeField] private Transform[] goPointsTranform;
    private int curGoPointIdx = 0;
    private Transform curTransform;

    private CharacterController curCharacterController;
    private Camera curPlayerCamera;

    private int Count = 1;

    private void Awake()
    {
        curTransform = transform;

        curCharacterController = GetComponent<CharacterController>();
        curPlayerCamera = Camera.main;

        goPointsObj = GameObject.FindGameObjectsWithTag("GoPoint");
        goPointsTranform = new Transform[goPointsObj.Length];

        SortGoPoint();

        for (int i = 0; i < goPointsObj.Length; i++)
        {
            goPointsTranform[i] = goPointsObj[i].transform;
            Debug.Log($"Current GameObject Name : {goPointsObj[i].name}");
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (viewMove == 0)
        {
            MovePlayer();
        }
        else if (viewMove == 1)
        {
            LookAtMovePlayer();
        }
    }

    private void MovePlayer()
    {
        Vector3 goDirection = goPointsTranform[curGoPointIdx].position - curTransform.position;

        if (goDirection != Vector3.zero)
        {
            Quaternion goRotation = Quaternion.LookRotation(goDirection);
            curTransform.rotation = Quaternion.Slerp(curTransform.rotation, goRotation, Time.deltaTime * rotSpeed);
            curTransform.Translate(Vector3.forward * Time.deltaTime * moveSpeed);
        }
    }

    void LookAtMovePlayer()
    {
        // 현재 바라보고 있는 Camera의 방향이 플레이어가 움직이는 방향
        Vector3 ForwardDir = curPlayerCamera.transform.forward;
        // 해당 방향의 높이 값을 0으로 주어서 움직일때 높이와 상관 없는 방향성 제시
        ForwardDir.y = 0;
        // 현재 CharacterController를 SimpleMove를 통해서 이동
        curCharacterController.SimpleMove(ForwardDir * moveSpeed);
    }

    private void SortGoPoint()
    {
        if(goPointsObj == null) 
            return;

        Array.Sort(goPointsObj, (a, b) =>
        {
            string nameA = a.name;
            string nameB = b.name;

            char lastNameA = nameA[nameA.Length - 1];
            char lastNameB = nameB[nameB.Length - 1];

            return lastNameA.CompareTo(lastNameB);
        });

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoPoint"))
        {
            curGoPointIdx = (curGoPointIdx + 1) % goPointsTranform.Length;
            Debug.Log($"Trigger Enter : {other.name}");

            if (curGoPointIdx == 0)
            {
                Count++;
                Debug.Log($"Count : {Count}");
            
                if (Count == 3)
                    viewMove = 1;
            }
        }
    }
}
