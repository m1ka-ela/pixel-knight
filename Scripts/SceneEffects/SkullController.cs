using UnityEngine;

public class SkullController : MonoBehaviour
{
    public float rotateSpeed = -45.0f;

    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}