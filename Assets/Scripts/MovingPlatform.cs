using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform posA;
    [SerializeField] private Transform posB;
    [SerializeField] private float speed = 2f;
    private Vector3 targetPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetPos = posA.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            if (targetPos == posA.position)
            {
                targetPos = posB.position;
            }
            else
            {
                targetPos = posA.position;
            }
        }
    }
}
