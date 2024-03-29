﻿using UnityEngine;

public class TerrainDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public Terrain terrain;

    public void DrawTexture(Texture2D texture) {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    public void DrawTerrain(TerrainData terrainData, TerrainType[] regions)
    {
        terrain.terrainData = terrainData;
        SplatMapScript.AssignSplatMap(terrainData, regions);
    }
}
