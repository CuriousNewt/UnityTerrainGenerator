using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.Rendering;
using System;
// modified from https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/

public static class SplatMapScript
{

    public static void AssignSplatMap(TerrainData terrainData, TerrainType[] regions)
    {
        terrainData.terrainLayers = new TerrainLayer[0];
        float unit = 1f / (terrainData.size.x - 1);
        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, regions.Length];

        terrainData.terrainLayers = GenerateTerrainLayers(regions);

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.terrainLayers.Length];

                // get height and slope at corresponding point
                float height = GetHeightAtPoint(x_01 * terrainData.size.x, y_01 * terrainData.size.z, terrainData, unit);
                float slope = GetSlopeAtPoint(x_01 * terrainData.size.x, y_01 * terrainData.size.z, terrainData, unit);

                foreach (TerrainType region in regions)
                {
                    float targetHeight = region.height * terrainData.size.y;
                    if (height > targetHeight)
                    {
                        splatWeights[0] = 0;
                        splatWeights[1] = 0;
                        splatWeights[2] = 0;
                        splatWeights[3] = 0;
                        splatWeights[4] = 0;
                        splatWeights[5] = 1;
                    }
                    //else if (height < targetHeight && height < beachHeight + textureBorderModifier)
                    //{
                    //    splatWeights[0] = 1 - (height - beachHeight);
                    //    splatWeights[1] = height - beachHeight;
                    //    splatWeights[2] = 0;
                    //}
                    //else if (height > lowlandsHeight - textureBorderModifier)
                    //{
                    //    splatWeights[0] = 0;
                    //    splatWeights[1] = 1 - (height - lowlandsHeight);
                    //    splatWeights[2] = height - lowlandsHeight;
                    //}
                    else if (height < targetHeight)
                    {
                        splatWeights[region.index] = 1;
                    }
                }

                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[y, x, i] = splatWeights[i];
                    // NOTE: Alpha map array dimensions are shifted in relation to heightmap and world space (y is x and x is y or z)
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    private static TerrainLayer[] GenerateTerrainLayers(TerrainType[] regions)
    {
        TerrainLayer[] layers = new TerrainLayer[regions.Length];
        for (int i = 0; i < layers.Length-1; i++) {
            layers[i] = new TerrainLayer();
            Texture2D texture = new Texture2D(1, 1);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Repeat;
            texture.SetPixel(0, 0, regions[i].color);
            texture.Apply();

            layers[i].diffuseTexture = texture;
        }

        return layers;
    }

    static float GetSlopeAtPoint(float pointX, float pointZ, TerrainData terrainData, float unit, bool scaleToRatio = true)
    {
        float factor = (scaleToRatio) ? 90f : 1f;
        return terrainData.GetSteepness(unit * pointX, unit * pointZ) / 90f; // x and z coordinates must be scaled
    }

    static float GetHeightAtPoint(float pointX, float pointZ, TerrainData terrainData, float unit, bool scaleToTerrain = false)
    {
        float height = terrainData.GetInterpolatedHeight(unit * pointX, unit * pointZ);

        // x and z coordinates must be scaled with "unit"
        if (scaleToTerrain)
            return height / terrainData.size.y;
        else
            return height;
    }
}
