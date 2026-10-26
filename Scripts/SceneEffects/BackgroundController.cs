using UnityEngine;

public class ScrollBackground : MonoBehaviour
{
    public float speed = 2.0f;
    public float endX = -3.810997f;
    public float startX = 3.810997f;

    void Update()
    {
        transform.Translate(-speed * Time.deltaTime, 0, 0);

        if (transform.position.x <= endX)
        {
            float overshoot = transform.position.x - endX;

            transform.position = new Vector3(startX + overshoot, transform.position.y, transform.position.z);
        }
    }
}