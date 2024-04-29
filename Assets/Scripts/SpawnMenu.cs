using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class SpawnMenu : EditorWindow
{
    private Texture2D _noiseTexture;
    private float density = 0.5f;
    private GameObject prefab;
    private Vector3 _eulerAngleVariance = new Vector3(10, 180, 10);

    [MenuItem("Tools/Spawn Menu")]
    public static void ShowWindow()
    {
        GetWindow<SpawnMenu>("Spawn Menu");
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        _noiseTexture = (Texture2D)EditorGUILayout.ObjectField("Noise Map Texture", _noiseTexture, typeof(Texture2D), false);
        if (GUILayout.Button("Generate Noise"))
        {
            int width = (int)Terrain.activeTerrain.terrainData.size.x;
            int height = (int)Terrain.activeTerrain.terrainData.size.y;
            float scale = 5;
            _noiseTexture = Noise.GetNoiseMap(width, height, scale);
        }
        EditorGUILayout.EndHorizontal();

        density = EditorGUILayout.Slider("Density", density, 0, 1);

        prefab = (GameObject)EditorGUILayout.ObjectField("Object Prefab", prefab, typeof(GameObject), false);

        _eulerAngleVariance = (Vector3)EditorGUILayout.Vector3Field("Euler Angle Variance", _eulerAngleVariance);

        if (GUILayout.Button("Place Objects"))
        {
            PlaceObjects(Terrain.activeTerrain, _noiseTexture, density, prefab, _eulerAngleVariance);
        }
    }

    public static void PlaceObjects(Terrain terrain, Texture2D noiseTex, float density, GameObject prefab, Vector3 eulerVariance)
    {
        Transform parent = new GameObject("SpawnedObjects").transform;

        for (int x = 0; x < terrain.terrainData.size.x; x++)
        {
            for (int z = 0; z < terrain.terrainData.size.z; z++)
            {
                float noiseValue = noiseTex.GetPixel(x, z).g;

                if (noiseValue > 1 - density)
                {
                    Vector3 pos = new Vector3(x, 0, z);
                    pos.y = terrain.terrainData.GetInterpolatedHeight(x / terrain.terrainData.size.x, z / (float)terrain.terrainData.size.y);

                    GameObject spawnedObj = Instantiate(prefab, pos, Quaternion.identity, parent);
                    Vector3 rot = new Vector3(
                        Random.Range(-eulerVariance.x, eulerVariance.x),
                        Random.Range(-eulerVariance.y, eulerVariance.y),
                        Random.Range(-eulerVariance.z, eulerVariance.z)
                        );
                    spawnedObj.transform.Rotate(rot, Space.Self);
                }
            }
        }
    }
}
#endif

public class Noise
{
    public static Texture2D GetNoiseMap(int width, int height, float scale)
    {
        Texture2D noiseMapTexture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noiseValue = Mathf.PerlinNoise((float)x / width * scale, (float)y / height * scale);
                noiseMapTexture.SetPixel(x, y, new Color(0, noiseValue, 0));
            }
        }

        noiseMapTexture.Apply();
        return noiseMapTexture;
    }
}
