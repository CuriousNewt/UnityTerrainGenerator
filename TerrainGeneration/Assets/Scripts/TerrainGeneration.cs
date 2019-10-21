using UnityEngine;

public static class TerrainGeneration
{
    public static TerrainData GenerateTerrain(TerrainData tData, float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        int depth = 20;

        tData.heightmapResolution = width;
        tData.size = new Vector3(width, depth, height);
        tData.SetHeights(0, 0, heightMap);

        return tData;
    }
}