using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public PlayerController player;
    public float eyeHeight = 1.62f;
    public float minPitch = -85f;
    public float maxPitch = 85f;

    float pitch;

    void Start()
    {
        if (player == null) player = GetComponentInParent<PlayerController>();
        transform.localPosition = new Vector3(0f, eyeHeight, 0f);
    }

    void LateUpdate()
    {
        pitch = Mathf.Clamp(pitch - player.LookInput.y, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
