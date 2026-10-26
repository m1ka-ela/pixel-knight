using UnityEngine;

public class GameBackgroundController : MonoBehaviour
{
    [Header("패럴랙스 속도 (1이면 카메라와 같이 이동, 0이면 고정)")]
    public Vector2 parallaxEffectMultiplier;

    private Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y, 0);

        lastCameraPosition = cameraTransform.position;
    }
}