using UnityEngine;

public class EagleFollower : MonoBehaviour
{
    private Transform player;
    public Vector3 offset = new Vector3(-1.5f, 1.5f, 0f);
    public float followSpeed = 5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 targetPos = player.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }
}
