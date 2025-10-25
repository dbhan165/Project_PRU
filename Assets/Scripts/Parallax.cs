using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Material material;
    [SerializeField] private float parallaxVector = 0.1f;
    private float offset;
    public float gameSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        ParallaxScroll();
    }
    private void ParallaxScroll()
    {
        float speed = gameSpeed * parallaxVector;
        offset += speed * Time.deltaTime;
        material.SetTextureOffset("_MainTex", Vector2.right * offset);
    }
}
