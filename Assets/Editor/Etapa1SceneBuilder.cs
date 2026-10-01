using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class Etapa1SceneBuilder
{
    // ============================================================
    // ETAPA 1 - CONSTRUCTOR DEL ENTORNO VIRTUAL
    // Proyecto: VR + Teleoperación + Robot móvil + Control háptico
    //
    // Uso:
    // 1. Crear carpeta Assets/Editor
    // 2. Copiar este archivo allí
    // 3. Esperar a que Unity compile
    // 4. Menú: Proyecto > Etapa 1 > Construir entorno
    // ============================================================

    private static Transform environmentRoot;
    private static Transform obstaclesRoot;
    private static Transform manipulablesRoot;
    private static Transform playerRoot;
    private static Transform lightingRoot;

    private static Material floorMat;
    private static Material wallMat;
    private static Material obstacleMat;
    private static Material obstacleAltMat;
    private static Material manipMat;
    private static Material lineMat;

    [MenuItem("Proyecto/Etapa 1/Construir entorno")]
    public static void BuildScene()
    {
        if (!EditorUtility.DisplayDialog(
            "Construir entorno - Etapa 1",
            "Se creará una escena nueva y se reemplazará la escena actual no guardada. ¿Continuar?",
            "Construir",
            "Cancelar"))
            return;

        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateMaterials();
        CreateRoots();
        CreateEnvironment();
        CreateObstacles();
        CreateManipulableObjects();
        CreatePlayer();
        CreateLighting();
        CreateSceneSettings();

        Scene scene = SceneManager.GetActiveScene();
        scene.name = "MainScene";
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/MainScene.unity");

        Selection.activeGameObject = environmentRoot.gameObject;

        Debug.Log("ETAPA 1 completada: entorno virtual creado en Assets/Scenes/MainScene.unity");
        EditorUtility.DisplayDialog(
            "ETAPA 1 completada",
            "El entorno virtual fue creado correctamente.\n\n" +
            "Incluye:\n" +
            "• Piso y límites físicos\n" +
            "• Obstáculos\n" +
            "• Objetos manipulables\n" +
            "• Player + cámara en primera persona\n" +
            "• Iluminación\n" +
            "• Materiales\n\n" +
            "Siguiente etapa: movimiento del avatar.",
            "Aceptar");
    }

    private static void CreateRoots()
    {
        environmentRoot = new GameObject("Environment").transform;
        obstaclesRoot = new GameObject("Obstacles").transform;
        obstaclesRoot.SetParent(environmentRoot);

        manipulablesRoot = new GameObject("ManipulableObjects").transform;
        manipulablesRoot.SetParent(environmentRoot);

        playerRoot = new GameObject("Player").transform;
        playerRoot.SetParent(environmentRoot);

        lightingRoot = new GameObject("Lighting").transform;
        lightingRoot.SetParent(environmentRoot);

        CreateEmpty("Robot", environmentRoot);
        CreateEmpty("Systems", environmentRoot);
    }

    private static GameObject CreateEmpty(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        return go;
    }

    private static void CreateMaterials()
    {
        floorMat = CreateMaterial("MAT_Floor", new Color(0.16f, 0.18f, 0.21f));
        wallMat = CreateMaterial("MAT_Wall", new Color(0.28f, 0.31f, 0.35f));
        obstacleMat = CreateMaterial("MAT_Obstacle_Blue", new Color(0.04f, 0.16f, 0.42f));
        obstacleAltMat = CreateMaterial("MAT_Obstacle_DarkBlue", new Color(0.02f, 0.08f, 0.22f));
        manipMat = CreateMaterial("MAT_Manipulable_Red", new Color(0.75f, 0.04f, 0.03f));
        lineMat = CreateMaterial("MAT_GuideLine", new Color(0.05f, 0.8f, 1.0f), 1.0f);
    }

    private static Material CreateMaterial(string name, Color color, float metallic = 0.0f)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");

        Material mat = new Material(shader);
        mat.name = name;
        mat.color = color;

        if (mat.HasProperty("_Metallic"))
            mat.SetFloat("_Metallic", metallic);

        if (mat.HasProperty("_Smoothness"))
            mat.SetFloat("_Smoothness", 0.55f);

        EnsureFolder("Assets", "Materials");
        AssetDatabase.CreateAsset(mat, $"Assets/Materials/{name}.mat");
        return mat;
    }

    private static void CreateEnvironment()
    {
        // Dimensiones principales:
        // 20 m x 14 m de área útil.
        // Paredes de 0.4 m de espesor y 3.5 m de altura.

        CreateCube(
            "Floor",
            new Vector3(0f, -0.15f, 0f),
            new Vector3(20f, 0.30f, 14f),
            floorMat,
            environmentRoot,
            true,
            false);

        CreateWall("Wall_North", new Vector3(0f, 1.75f, 7f), new Vector3(20.4f, 3.5f, 0.4f));
        CreateWall("Wall_South", new Vector3(0f, 1.75f, -7f), new Vector3(20.4f, 3.5f, 0.4f));
        CreateWall("Wall_East", new Vector3(10f, 1.75f, 0f), new Vector3(0.4f, 3.5f, 14f));
        CreateWall("Wall_West", new Vector3(-10f, 1.75f, 0f), new Vector3(0.4f, 3.5f, 14f));

        // Líneas de referencia visual del piso.
        for (int x = -9; x <= 9; x += 2)
            CreateGuideLine($"Guide_X_{x}", new Vector3(x, 0.012f, 0f), new Vector3(0.025f, 0.025f, 13.7f));

        for (int z = -6; z <= 6; z += 2)
            CreateGuideLine($"Guide_Z_{z}", new Vector3(0f, 0.014f, z), new Vector3(19.7f, 0.025f, 0.025f));
    }

    private static void CreateWall(string name, Vector3 position, Vector3 scale)
    {
        CreateCube(name, position, scale, wallMat, environmentRoot, true, false);
    }

    private static void CreateObstacles()
    {
        // Obstáculos verticales tipo columnas.
        CreateCube("Obstacle_01", new Vector3(-5.5f, 1.0f, 3.5f), new Vector3(1.4f, 2.0f, 1.4f), obstacleMat, obstaclesRoot, true, false);
        CreateCube("Obstacle_02", new Vector3(-2.0f, 1.35f, 3.8f), new Vector3(1.1f, 2.7f, 1.1f), obstacleAltMat, obstaclesRoot, true, false);
        CreateCube("Obstacle_03", new Vector3(2.0f, 1.1f, 3.6f), new Vector3(1.2f, 2.2f, 1.2f), obstacleMat, obstaclesRoot, true, false);
        CreateCube("Obstacle_04", new Vector3(5.0f, 1.5f, 3.0f), new Vector3(1.5f, 3.0f, 1.5f), obstacleAltMat, obstaclesRoot, true, false);

        CreateCube("Obstacle_05", new Vector3(-4.5f, 1.1f, -2.7f), new Vector3(1.2f, 2.2f, 1.2f), obstacleAltMat, obstaclesRoot, true, false);
        CreateCube("Obstacle_06", new Vector3(0.0f, 0.9f, -3.2f), new Vector3(1.5f, 1.8f, 1.5f), obstacleMat, obstaclesRoot, true, false);
        CreateCube("Obstacle_07", new Vector3(4.5f, 1.25f, -2.8f), new Vector3(1.3f, 2.5f, 1.3f), obstacleAltMat, obstaclesRoot, true, false);
    }

    private static void CreateManipulableObjects()
    {
        CreateManipulable("Manipulable_01", new Vector3(-7.2f, 0.8f, 4.8f), new Vector3(1.6f, 1.6f, 1.6f));
        CreateManipulable("Manipulable_02", new Vector3(7.2f, 0.8f, 4.8f), new Vector3(1.6f, 1.6f, 1.6f));
        CreateManipulable("Manipulable_03", new Vector3(-7.2f, 0.8f, -4.8f), new Vector3(1.6f, 1.6f, 1.6f));
        CreateManipulable("Manipulable_04", new Vector3(7.2f, 0.8f, -4.8f), new Vector3(1.6f, 1.6f, 1.6f));
    }

    private static void CreateManipulable(string name, Vector3 position, Vector3 scale)
    {
        GameObject go = CreateCube(name, position, scale, manipMat, manipulablesRoot, true, true);

        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.mass = 2.0f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Etiqueta útil para el sistema de agarre de etapas posteriores.
        go.tag = "Untagged";
    }

    private static GameObject CreateCube(
        string name,
        Vector3 position,
        Vector3 scale,
        Material material,
        Transform parent,
        bool addCollider,
        bool beveledVisual)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent);
        go.transform.position = position;
        go.transform.localScale = scale;

        Renderer renderer = go.GetComponent<Renderer>();
        renderer.sharedMaterial = material;

        if (!addCollider)
        {
            Collider col = go.GetComponent<Collider>();
            if (col != null)
                Object.DestroyImmediate(col);
        }

        return go;
    }

    private static void CreateGuideLine(string name, Vector3 position, Vector3 scale)
    {
        GameObject line = CreateCube(name, position, scale, lineMat, environmentRoot, false, false);
        line.GetComponent<Renderer>().sharedMaterial = lineMat;
    }

    private static void CreatePlayer()
    {
        GameObject player = playerRoot.gameObject;
        player.transform.position = new Vector3(0f, 0f, -5.5f);

        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 1.8f;
        controller.radius = 0.3f;
        controller.center = new Vector3(0f, 0.9f, 0f);
        controller.stepOffset = 0.25f;

        GameObject cameraObject = new GameObject("MainCamera");
        cameraObject.transform.SetParent(player.transform);
        cameraObject.transform.localPosition = new Vector3(0f, 1.62f, 0f);
        cameraObject.transform.localRotation = Quaternion.identity;

        Camera cam = cameraObject.AddComponent<Camera>();
        cam.fieldOfView = 75f;
        cam.nearClipPlane = 0.03f;
        cam.farClipPlane = 100f;
        cam.clearFlags = CameraClearFlags.Skybox;
        cameraObject.tag = "MainCamera";

        // Punto de referencia para futuras etapas VR.
        GameObject head = new GameObject("Head");
        head.transform.SetParent(player.transform);
        head.transform.localPosition = new Vector3(0f, 1.62f, 0f);

        // Marcador visual en escena para recordar la posición inicial.
        CreateCube(
            "PlayerStartMarker",
            new Vector3(0f, 0.025f, -5.5f),
            new Vector3(0.8f, 0.05f, 0.8f),
            lineMat,
            playerRoot,
            false,
            false);
    }

    private static void CreateLighting()
    {
        GameObject sun = new GameObject("DirectionalLight");
        sun.transform.SetParent(lightingRoot);
        sun.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

        Light dir = sun.AddComponent<Light>();
        dir.type = LightType.Directional;
        dir.intensity = 1.15f;
        dir.shadows = LightShadows.Soft;

        // Luces puntuales para dar sensación de laboratorio.
        Vector3[] positions =
        {
            new Vector3(-6f, 3.0f, -4f),
            new Vector3(0f, 3.0f, -4f),
            new Vector3(6f, 3.0f, -4f),
            new Vector3(-6f, 3.0f, 3f),
            new Vector3(0f, 3.0f, 3f),
            new Vector3(6f, 3.0f, 3f)
        };

        for (int i = 0; i < positions.Length; i++)
        {
            GameObject lightObject = new GameObject($"CeilingLight_{i + 1:00}");
            lightObject.transform.SetParent(lightingRoot);
            lightObject.transform.position = positions[i];

            Light point = lightObject.AddComponent<Light>();
            point.type = LightType.Point;
            point.range = 7f;
            point.intensity = 2.2f;
            point.shadows = LightShadows.Soft;
        }
    }

    private static void CreateSceneSettings()
    {
        // Asegura que haya una cámara activa.
        Camera main = Camera.main;
        if (main != null)
        {
            main.backgroundColor = new Color(0.025f, 0.035f, 0.05f);
        }

        // Crea las carpetas si no existen.
        EnsureFolder("Assets", "Scenes");
        EnsureFolder("Assets", "Scripts");
        EnsureFolder("Assets", "Prefabs");
    }

    private static void EnsureFolder(string parent, string child)
    {
        string path = parent + "/" + child;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder(parent, child);
    }
}
