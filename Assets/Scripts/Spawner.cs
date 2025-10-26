using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obtacles;
    [SerializeField] private Transform lowPos;
    private float timer = 0;
    [SerializeField] private float spawnRate = 2f;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnRate)
        {
            SpawnObtacles();
            timer = 0;
        }
    }
    private void SpawnObtacles()
    {
        int index = Random.Range(0, obtacles.Length);
        if (index == 0)
        {
            GameObject obtacle = Instantiate(obtacles[index], lowPos.position, Quaternion.identity);
        }
    }
}
