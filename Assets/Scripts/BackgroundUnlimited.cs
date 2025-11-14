using UnityEngine;

public class BackgroundUnlimited : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform midBg;
    [SerializeField] private Transform sideBg;
    [SerializeField] private float length;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (cameraTransform.position.x > midBg.position.x)
        {
            UpdateBackground(Vector3.right);
        }
        else if (cameraTransform.position.x < midBg.position.x)
        {
            UpdateBackground(Vector3.left);
        }
    }
    void UpdateBackground(Vector3 direction)
    {
        sideBg.position = midBg.position + direction * length;
        Transform temp = midBg;
        midBg = sideBg;
        sideBg = temp;
    }
}
