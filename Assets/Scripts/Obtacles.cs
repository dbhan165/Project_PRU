using UnityEngine;

public class Obtacles : MonoBehaviour
{
    public float leftBoundary = -20f;
    public float gameSpeed = 2f;

    // Update is called once per frame
    void Update()
    {
        MoveObticles();
    }
    private void MoveObticles()
    {
        transform.position += Vector3.left * gameSpeed * Time.deltaTime;
        if (transform.position.x < leftBoundary)
        {
            Destroy(gameObject);
        }
    }
}
