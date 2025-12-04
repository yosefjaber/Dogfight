using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapPreview : MonoBehaviour {

	public Renderer textureRender;
	public MeshFilter meshFilter;
	public MeshRenderer meshRenderer;

	public enum DrawMode {NoiseMap, Mesh, FalloffMap, PlantNoise};
	public DrawMode drawMode;

	public MeshSettings meshSettings;
	public HeightMapSettings heightMapSettings;
	public TextureData textureData;

	public Material terrainMaterial;

	[Range(0,MeshSettings.numSupportedLODs-1)]
	public int editorPreviewLOD;
	public bool autoUpdate;
	
	private TerrainGenerator terrainGenerator;

	public void DrawMapInEditor() {
		textureData.ApplyToMaterial (terrainMaterial);
		textureData.UpdateMeshHeights (terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
		HeightMap heightMap = HeightMapGenerator.GenerateHeightMap (meshSettings.numVertsPerLine, meshSettings.numVertsPerLine, 
																		heightMapSettings, Vector2.zero, meshSettings.meshWorldSize);

		if (drawMode == DrawMode.NoiseMap) {
			DrawTexture (TextureGenerator.TextureFromHeightMap (heightMap));
		} else if (drawMode == DrawMode.Mesh) {
			DrawMesh (MeshGenerator.GenerateTerrainMesh (heightMap.values,meshSettings, editorPreviewLOD));
		} else if (drawMode == DrawMode.FalloffMap) {
			Vector2 chunkOrigin = Vector2.zero - new Vector2(meshSettings.meshWorldSize / 2f, meshSettings.meshWorldSize / 2f);
			float[,] falloff = FalloffGenerator.GenerateFalloffMapExact(
				meshSettings.numVertsPerLine, 
				meshSettings.numVertsPerLine, 
				Vector2.zero,  // sampleCentre for preview
				heightMapSettings.falloffCenter, 
				heightMapSettings.falloffRadius, 
				meshSettings.meshWorldSize,
				meshSettings.numVertsPerLine
			);
			DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(falloff, 0, 1)));
		} else if (drawMode == DrawMode.PlantNoise) {
			float[,] plantNoise = Noise.GeneratePlantNoiseMap(
				meshSettings.numVertsPerLine,
				meshSettings.numVertsPerLine,
				heightMapSettings.plantNoiseSettings,
				Vector2.zero
			);
			DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(plantNoise, 0, 1)));
		}
	}

	public Vector3 CalculateNormalFromHeightMap(float[,] heightData, int x, int z) {
		int width = heightData.GetLength(0);
		int height = heightData.GetLength(1);
		
		// Get neighboring height values for normal calculation
		float heightL = (x > 0) ? heightData[x - 1, z] : heightData[x, z];
		float heightR = (x < width - 1) ? heightData[x + 1, z] : heightData[x, z];
		float heightD = (z > 0) ? heightData[x, z - 1] : heightData[x, z];
		float heightU = (z < height - 1) ? heightData[x, z + 1] : heightData[x, z];
		
		// Calculate normal using cross product of tangent vectors
		Vector3 tangentX = new Vector3(2f * meshSettings.meshScale, heightR - heightL, 0);
		Vector3 tangentZ = new Vector3(0, heightU - heightD, 2f * meshSettings.meshScale);
		
		return Vector3.Cross(tangentZ, tangentX).normalized;
	}

	public void DrawTexture(Texture2D texture) {
		textureRender.sharedMaterial.mainTexture = texture;
		textureRender.transform.localScale = new Vector3 (texture.width, 1, texture.height) /10f;

		textureRender.gameObject.SetActive (true);
		meshFilter.gameObject.SetActive (false);
	}

	public void DrawMesh(MeshData meshData) {
		meshFilter.sharedMesh = meshData.CreateMesh ();

		textureRender.gameObject.SetActive (false);
		meshFilter.gameObject.SetActive (true);
	}

	void OnValuesUpdated() {
		if (!Application.isPlaying) {
			DrawMapInEditor ();
		}
	}

	void OnTextureValuesUpdated() {
		textureData.ApplyToMaterial (terrainMaterial);
	}

	void OnValidate() {

		if (meshSettings != null) {
			meshSettings.OnValuesUpdated -= OnValuesUpdated;
			meshSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (heightMapSettings != null) {
			heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
			heightMapSettings.OnValuesUpdated += OnValuesUpdated;
		}
		if (textureData != null) {
			textureData.OnValuesUpdated -= OnTextureValuesUpdated;
			textureData.OnValuesUpdated += OnTextureValuesUpdated;
		}
	}
}