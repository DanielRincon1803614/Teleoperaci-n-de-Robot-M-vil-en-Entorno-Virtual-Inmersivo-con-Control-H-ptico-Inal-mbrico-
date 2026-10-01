using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class Etapa2RobotBuilder
{
    [MenuItem("Proyecto/Etapa 2/Construir robot")]
    public static void Build()
    {
        GameObject env = GameObject.Find("Environment");
        GameObject robot = GameObject.Find("Environment/Robot");
        if (robot == null)
        {
            robot = new GameObject("Robot");
            if (env != null) robot.transform.SetParent(env.transform, false);
        }
        if (robot.GetComponent<RobotController>() != null)
        {
            Debug.LogWarning("El robot ya existe.");
            return;
        }

        Undo.RegisterCreatedObjectUndo(robot, "Construir robot");
        robot.transform.position = new Vector3(-8f, 0.05f, -5f);
        robot.transform.rotation = Quaternion.identity;

        Material bodyMat = Mat("Robot_Body", new Color(0.15f, 0.65f, 0.25f));
        Material wheelMat = Mat("Robot_Wheel", new Color(0.08f, 0.08f, 0.08f));
        Material markMat = Mat("Robot_Front", new Color(1f, 0.85f, 0.1f));

        Part("Body", PrimitiveType.Cube, robot.transform, new Vector3(0f, 0.25f, 0f), new Vector3(0.6f, 0.3f, 0.8f), Vector3.zero, bodyMat);
        Part("Wheel_L", PrimitiveType.Cylinder, robot.transform, new Vector3(-0.34f, 0.15f, 0f), new Vector3(0.3f, 0.04f, 0.3f), new Vector3(0f, 0f, 90f), wheelMat);
        Part("Wheel_R", PrimitiveType.Cylinder, robot.transform, new Vector3(0.34f, 0.15f, 0f), new Vector3(0.3f, 0.04f, 0.3f), new Vector3(0f, 0f, 90f), wheelMat);
        Part("FrontMarker", PrimitiveType.Cube, robot.transform, new Vector3(0f, 0.3f, 0.4f), new Vector3(0.25f, 0.1f, 0.04f), Vector3.zero, markMat);

        GameObject sensorPoint = new GameObject("SensorPoint");
        sensorPoint.transform.SetParent(robot.transform, false);
        sensorPoint.transform.localPosition = new Vector3(0f, 0.25f, 0.42f);

        Rigidbody rb = robot.AddComponent<Rigidbody>();
        rb.mass = 10f;
        rb.linearDamping = 0f;
        rb.angularDamping = 0.05f;

        BoxCollider col = robot.AddComponent<BoxCollider>();
        col.center = new Vector3(0f, 0.2f, 0f);
        col.size = new Vector3(0.68f, 0.4f, 0.8f);

        robot.AddComponent<RobotController>();
        RobotSensor sensor = robot.AddComponent<RobotSensor>();
        sensor.origin = sensorPoint.transform;

        Selection.activeGameObject = robot;
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    static Material Mat(string name, Color c)
    {
        string path = "Assets/Materials/" + name + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetColor("_BaseColor", c);
        AssetDatabase.CreateAsset(m, path);
        return m;
    }

    static void Part(string name, PrimitiveType type, Transform parent, Vector3 pos, Vector3 scale, Vector3 euler, Material m)
    {
        GameObject g = GameObject.CreatePrimitive(type);
        g.name = name;
        Object.DestroyImmediate(g.GetComponent<Collider>());
        g.transform.SetParent(parent, false);
        g.transform.localPosition = pos;
        g.transform.localScale = scale;
        g.transform.localEulerAngles = euler;
        g.GetComponent<Renderer>().sharedMaterial = m;
    }
}
