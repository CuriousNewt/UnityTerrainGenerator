using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public enum DrawMode { NoiseMap, ColorMap, Mesh, Terrain};
    public DrawMode drawMode;

    public TerrainData terrainData;
    public bool autoUpdate = true;
    public int mapWidth, mapHeight, octaves, seed;
    public float noiseScale, persistance, lacunarity;
    public Vector2 offset;
    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;
    public TerrainType[] regions;
    


    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoise(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height) {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        TerrainDisplay display = FindObjectOfType<TerrainDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(GenerateTexture(colorMap));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), GenerateTexture(colorMap));
        }
        else if (drawMode == DrawMode.Terrain) {
            display.DrawTerrain(TerrainGeneration.GenerateTerrain(terrainData, noiseMap), regions);
        }

    }

    private Texture2D GenerateTexture(Color[] colorMap)
    {
        return TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight);
    }

    void OnValidate()
    {
        if (mapWidth < 1) mapWidth = 1;

        if (mapHeight < 1) mapHeight = 1;
        
        if (lacunarity < 1) lacunarity = 1;
      
        if (octaves < 0) octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType{
    public string name;
    public int index;
    public float height;
    public Color color;
    }
