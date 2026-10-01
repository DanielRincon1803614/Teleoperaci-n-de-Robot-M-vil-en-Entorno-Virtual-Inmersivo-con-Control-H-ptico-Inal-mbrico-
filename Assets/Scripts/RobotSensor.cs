using UnityEngine;

public class RobotSensor : MonoBehaviour
{
    public Transform origin;
    public float maxRange = 4f;
    public LayerMask mask = ~0;

    public float Distance { get; private set; }

    RobotController robot;

    void Awake()
    {
        if (origin == null) origin = transform;
        robot = GetComponent<RobotController>();
        Distance = maxRange;
    }

    void FixedUpdate()
    {
        if (Physics.Raycast(origin.position, origin.forward, out RaycastHit hit, maxRange, mask, QueryTriggerInteraction.Ignore))
            Distance = hit.distance;
        else
            Distance = maxRange;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 22), "Distancia frontal: " + Distance.ToString("F2") + " m");
        if (robot != null)
            GUI.Label(new Rect(10, 32, 300, 22), "Velocidad: " + robot.CurrentSpeed.ToString("F2") + " m/s");
    }
}