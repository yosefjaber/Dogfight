using UnityEngine;
using System.Collections.Generic;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    [Header("Spawn Zone Settings")]
    public Vector2 spawnCenter = Vector2.zero;
    public float spawnRadius = 200f;
    public bool forceMaxLODInSpawn = true;

    [Header("Terrain Settings")]
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int colliderLODIndex;
    [Space(10)]

    public LODInfo[] detailLevels;
    [Space(10)]
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;
    public ObjectPlacementData objectPlacementData;
    public Material mapMaterial;
    public Transform viewer;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    float meshWorldSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
    HashSet<Vector2> spawnChunks = new HashSet<Vector2>();
    
    bool isInitialized = false;

    void Start()
    {
        // Don't initialize until viewer is assigned by RoomManager
    }
    
    public void InitializeTerrain()
    {
        Debug.Log("InitializeTerrain called");
        
        if (isInitialized)
        {
            Debug.Log("Terrain already initialized");
            return;
        }
        
        if (viewer == null)
        {
            Debug.LogError("Viewer must be assigned before initializing terrain!");
            return;
        }
        
        Debug.Log($"Viewer assigned: {viewer.name}");

        if (textureSettings != null && mapMaterial != null)
        {
            textureSettings.ApplyToMaterial(mapMaterial);
            textureSettings.UpdateMeshHeights(mapMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);
            Debug.Log("Material settings applied");
        }

        float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

        Debug.Log("Calculating spawn chunks...");
        CalculateSpawnChunks();
        Debug.Log($"Spawn chunks: {spawnChunks.Count}");
        
        Debug.Log("Updating visible chunks...");
        UpdateVisibleChunks();
        Debug.Log($"Terrain chunks created: {terrainChunkDictionary.Count}");
        
        Debug.Log("Placing objects on spawn chunks...");
        PlaceObjectsOnSpawnChunks();
        
        isInitialized = true;
        Debug.Log("Terrain initialization complete!");
    }

    void CalculateSpawnChunks()
    {
        spawnChunks.Clear();

        int spawnChunksRadius = Mathf.CeilToInt(spawnRadius / meshWorldSize);
        int spawnCenterX = Mathf.RoundToInt(spawnCenter.x / meshWorldSize);
        int spawnCenterY = Mathf.RoundToInt(spawnCenter.y / meshWorldSize);

        for (int yOffset = -spawnChunksRadius; yOffset <= spawnChunksRadius; yOffset++)
        {
            for (int xOffset = -spawnChunksRadius; xOffset <= spawnChunksRadius; xOffset++)
            {
                Vector2 chunkCoord = new Vector2(spawnCenterX + xOffset, spawnCenterY + yOffset);
                Vector2 chunkWorldPos = chunkCoord * meshWorldSize;
                float distanceToSpawn = Vector2.Distance(chunkWorldPos, spawnCenter);

                if (distanceToSpawn <= spawnRadius)
                {
                    spawnChunks.Add(chunkCoord);
                }
            }
        }
    }

    public HashSet<Vector2> GetSpawnChunks()
    {
        return spawnChunks;
    }

    void Update()
    {
        if (viewer == null || !isInitialized) return;

        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();

        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        foreach (Vector2 spawnChunkCoord in spawnChunks)
        {
            if (!alreadyUpdatedChunkCoords.Contains(spawnChunkCoord))
            {
                LoadOrUpdateChunk(spawnChunkCoord, true);
                alreadyUpdatedChunkCoords.Add(spawnChunkCoord);
            }
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    bool isSpawnChunk = spawnChunks.Contains(viewedChunkCoord);
                    LoadOrUpdateChunk(viewedChunkCoord, isSpawnChunk);
                }
            }
        }
    }

    void LoadOrUpdateChunk(Vector2 chunkCoord, bool isSpawnChunk)
    {
        if (terrainChunkDictionary.ContainsKey(chunkCoord))
        {
            terrainChunkDictionary[chunkCoord].UpdateTerrainChunk();
        }
        else
        {
            TerrainChunk newChunk = new TerrainChunk(
                chunkCoord,
                heightMapSettings,
                meshSettings,
                detailLevels,
                colliderLODIndex,
                transform,
                viewer,
                mapMaterial,
                isSpawnChunk,
                forceMaxLODInSpawn
            );

            terrainChunkDictionary.Add(chunkCoord, newChunk);
            newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
            newChunk.Load();
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            if (!visibleTerrainChunks.Contains(chunk)) 
                visibleTerrainChunks.Add(chunk);
        }
        else
        {
            if (!chunk.isSpawnChunk)
            {
                visibleTerrainChunks.Remove(chunk);
            }
        }
    }

    public Dictionary<Vector2, TerrainChunk> GetTerrainChunkDictionary()
    {
        return terrainChunkDictionary;
    }

    public TerrainChunk GetChunkAtWorldPos(Vector2 worldPos)
    {
        Vector2 chunkCoord = new Vector2(
            Mathf.RoundToInt(worldPos.x / meshWorldSize),
            Mathf.RoundToInt(worldPos.y / meshWorldSize)
        );

        terrainChunkDictionary.TryGetValue(chunkCoord, out TerrainChunk chunk);
        return chunk;
    }

    public Vector3 GetSpawnPosition()
    {
        Vector2 spawnPos2D = spawnCenter;
        TerrainChunk spawnChunk = GetChunkAtWorldPos(spawnPos2D);
        
        if (spawnChunk != null)
        {
            HeightMap heightMap = spawnChunk.GetHeightMap();
            if (heightMap.values != null)
            {
                float height = SampleHeightAtWorldPos(spawnPos2D, spawnChunk, heightMap);
                return new Vector3(spawnPos2D.x, height + 2f, spawnPos2D.y);
            }
        }
        
        return new Vector3(spawnPos2D.x, 10f, spawnPos2D.y);
    }

    private float SampleHeightAtWorldPos(Vector2 worldPos, TerrainChunk chunk, HeightMap heightMap)
    {
        Vector3 chunkWorldPos = chunk.GetRootTransform().position;
        float chunkSize = meshSettings.meshWorldSize;
        
        Vector2 localPos = worldPos - new Vector2(chunkWorldPos.x, chunkWorldPos.z);
        
        float percentX = (localPos.x + chunkSize / 2f) / chunkSize;
        float percentZ = (localPos.y + chunkSize / 2f) / chunkSize;
        
        return SampleHeightBilinear(heightMap.values, percentX, percentZ);
    }

    public void PlaceObjectsOnActiveChunks()
    {
        if (!ValidateObjectPlacementData()) return;

        Transform parent = GetOrCreateParent("Placed Objects");

        foreach (var kvp in terrainChunkDictionary)
        {
            TerrainChunk chunk = kvp.Value;
            if (chunk.IsVisible())
            {
                PlaceObjectsOnChunk(chunk, parent);
            }
        }

        Debug.Log($"Placed objects on active chunks");
    }

    public void PlaceObjectsOnSpawnChunks()
    {
        if (!ValidateObjectPlacementData()) return;

        Transform parent = GetOrCreateParent("Placed Objects");
        int placedCount = 0;

        foreach (Vector2 spawnCoord in spawnChunks)
        {
            if (terrainChunkDictionary.TryGetValue(spawnCoord, out TerrainChunk chunk))
            {
                PlaceObjectsOnChunk(chunk, parent);
                placedCount++;
            }
        }

        Debug.Log($"Placed objects on {placedCount} spawn chunks");
    }

    private void PlaceObjectsOnChunk(TerrainChunk chunk, Transform globalParent)
    {
        HeightMap chunkHeightMap = chunk.GetHeightMap();
        if (chunkHeightMap.values == null)
        {
            Debug.LogWarning($"Chunk {chunk.coord} heightmap not ready yet");
            return;
        }

        Vector2 chunkCoord = chunk.coord;
        Vector2 sampleCentre = chunkCoord * meshSettings.meshWorldSize / meshSettings.meshScale;
        
        float[,] chunkNoise = Noise.GeneratePlantNoiseMap(
            meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine,
            heightMapSettings.plantNoiseSettings,
            sampleCentre
        );

        Transform chunkParent = GetOrCreateChunkParent(chunkCoord, globalParent);
        ClearChunkObjects(chunkParent);

        int width = chunkNoise.GetLength(0);
        int height = chunkNoise.GetLength(1);
        Vector3 chunkWorldPos = chunk.GetRootTransform().position;

        foreach (var settings in objectPlacementData.objectSettingsList)
        {
            if (settings?.placementPrefab == null) continue;

            PlaceObjectsForSettings(settings, chunkNoise, chunkHeightMap.values, chunkWorldPos, chunkParent);
        }
    }

    private void PlaceObjectsForSettings(ObjectPlacementData.PlacementSettings settings, float[,] chunkNoise, 
        float[,] heightData, Vector3 chunkWorldPos, Transform parent)
    {
        int width = chunkNoise.GetLength(0);
        int height = chunkNoise.GetLength(1);
        int step = Mathf.Max(1, settings.placementResolution);
        float chunkSize = meshSettings.meshWorldSize;

        for (int x = 1; x < width - 1; x += step)
        {
            for (int z = 1; z < height - 1; z += step)
            {
                float noiseValue = chunkNoise[x, z];
                float fitness = GetFitness(noiseValue, heightData, x, z, settings);

                if (fitness <= 1f - settings.placementDensity) continue;

                Vector3 worldPos = CalculateWorldPosition(x, z, step, width, height, heightData, chunkWorldPos, chunkSize);
                CreatePlacedObject(settings, worldPos, heightData, x, z, parent);
            }
        }
    }

    private Vector3 CalculateWorldPosition(int x, int z, int step, int width, int height, 
        float[,] heightData, Vector3 chunkWorldPos, float chunkSize)
    {
        float jitterX = Random.Range(-0.5f, 0.5f) * step;
        float jitterZ = Random.Range(-0.5f, 0.5f) * step;

        float percentX = Mathf.Clamp01((x - 1f + jitterX) / (width - 3));
        float percentZ = Mathf.Clamp01((z - 1f + jitterZ) / (height - 3));

        Vector2 topLeft = new Vector2(-1, 1) * chunkSize / 2f;
        Vector2 vertexPosition2D = topLeft + new Vector2(percentX, -percentZ) * chunkSize;

        float terrainHeight = SampleHeightBilinear(heightData, percentX, percentZ);

        return chunkWorldPos + new Vector3(vertexPosition2D.x, terrainHeight, vertexPosition2D.y);
    }

    private void CreatePlacedObject(ObjectPlacementData.PlacementSettings settings, Vector3 worldPos, 
        float[,] heightData, int x, int z, Transform parent)
    {
        GameObject obj = Instantiate(settings.placementPrefab, worldPos, Quaternion.identity, parent);

        if (settings.randomRotationY)
            obj.transform.Rotate(0, Random.Range(0, 360), 0);

        if (settings.useSteepnessFilter)
        {
            Vector3 surfaceNormal = CalculateNormalFromHeightMap(heightData, x, z);
            obj.transform.up = Vector3.Slerp(Vector3.up, surfaceNormal, 0.5f);
        }

        Vector3 finalScale = Vector3.one;

        if (settings.uniformScale)
        {
            float scale = Random.Range(settings.uniformScaleValue.x, settings.uniformScaleValue.y);
            finalScale = Vector3.one * scale;
        }
        else
        {
            if (settings.randomScaleX)
                finalScale.x = Random.Range(settings.scaleXRange.x, settings.scaleXRange.y);
            if (settings.randomScaleY)
                finalScale.y = Random.Range(settings.scaleYRange.x, settings.scaleYRange.y);
            if (settings.randomScaleZ)
                finalScale.z = Random.Range(settings.scaleZRange.x, settings.scaleZRange.y);
        }

        obj.transform.localScale = finalScale;

        Vector3 rotation = Vector3.zero;
        if (settings.randomRotationX)
            rotation.x = Random.Range(settings.rotationXRange.x, settings.rotationXRange.y);
        if (settings.randomRotationY)
            rotation.y = Random.Range(settings.rotationYRange.x, settings.rotationYRange.y);
        if (settings.randomRotationZ)
            rotation.z = Random.Range(settings.rotationZRange.x, settings.rotationZRange.y);

        obj.transform.rotation = Quaternion.Euler(rotation);
    }

    public void ClearAllPlacedObjects()
    {
        ClearObjectsByName("Placed Objects");
    }

    public void PreviewObjectPlacement()
    {
        if (!ValidateObjectPlacementData()) return;

        ClearObjectsByName("Preview Objects");
        GameObject previewRoot = new GameObject("Preview Objects");

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(
            meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine,
            heightMapSettings,
            Vector2.zero,
            meshSettings.meshWorldSize
        );

        float[,] plantNoise = Noise.GeneratePlantNoiseMap(
            meshSettings.numVertsPerLine,
            meshSettings.numVertsPerLine,
            heightMapSettings.plantNoiseSettings,
            Vector2.zero
        );

        CreatePreviewObjects(heightMap, plantNoise, previewRoot.transform);
    }

    private void CreatePreviewObjects(HeightMap heightMap, float[,] plantNoise, Transform previewRoot)
    {
        int width = plantNoise.GetLength(0);
        int height = plantNoise.GetLength(1);
        float chunkSize = meshSettings.meshWorldSize;

        foreach (var settings in objectPlacementData.objectSettingsList)
        {
            if (settings?.placementPrefab == null || !settings.showPlacementPreview) continue;

            Transform previewParent = new GameObject($"Preview_{settings.placementPrefab.name}").transform;
            previewParent.SetParent(previewRoot);

            CreatePreviewForSettings(settings, heightMap, plantNoise, previewParent, width, height, chunkSize);
        }
    }

    private void CreatePreviewForSettings(ObjectPlacementData.PlacementSettings settings, HeightMap heightMap, 
        float[,] plantNoise, Transform previewParent, int width, int height, float chunkSize)
    {
        int previewStep = Mathf.Max(1, settings.placementResolution * 2);

        for (int x = 1; x < width - 1; x += previewStep)
        {
            for (int z = 1; z < height - 1; z += previewStep)
            {
                float noiseValue = plantNoise[x, z];
                float fitness = GetFitness(noiseValue, heightMap.values, x, z, settings);

                if (fitness <= 1f - settings.placementDensity) continue;

                Vector2 percent = new Vector2(x - 1, z - 1) / (float)(width - 3);
                Vector2 topLeft = new Vector2(-1, 1) * chunkSize / 2f;
                Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * chunkSize;

                float terrainHeight = heightMap.values[x, z];
                Vector3 worldPos = new Vector3(vertexPosition2D.x, terrainHeight, vertexPosition2D.y);

                GameObject obj = Instantiate(settings.placementPrefab, worldPos, Quaternion.identity, previewParent);
                
                if (settings.randomRotationY)
                    obj.transform.Rotate(0, Random.Range(0, 360), 0);

                MakeObjectTransparent(obj);
            }
        }
    }

    private bool ValidateObjectPlacementData()
    {
        if (objectPlacementData?.objectSettingsList == null || objectPlacementData.objectSettingsList.Count == 0)
        {
            Debug.LogWarning("No object placement settings assigned!");
            return false;
        }
        return true;
    }

    private Transform GetOrCreateParent(string name)
    {
        GameObject parent = GameObject.Find(name);
        if (parent == null)
        {
            parent = new GameObject(name);
        }
        return parent.transform;
    }

    private Transform GetOrCreateChunkParent(Vector2 chunkCoord, Transform globalParent)
    {
        string parentName = $"Objects_Chunk_({chunkCoord.x}_{chunkCoord.y})";
        Transform chunkParent = globalParent.Find(parentName);
        
        if (chunkParent == null)
        {
            chunkParent = new GameObject(parentName).transform;
            chunkParent.SetParent(globalParent);
        }
        
        return chunkParent;
    }

    private void ClearChunkObjects(Transform chunkParent)
    {
        for (int i = chunkParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(chunkParent.GetChild(i).gameObject);
        }
    }

    private void ClearObjectsByName(string objectName)
    {
        Transform parent = GameObject.Find(objectName)?.transform;
        if (parent != null)
        {
            DestroyImmediate(parent.gameObject);
        }
    }

    private void MakeObjectTransparent(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer?.sharedMaterial != null)
        {
            Material previewMat = new Material(renderer.sharedMaterial);
            Color color = previewMat.color;
            color.a = 0.7f;
            previewMat.color = color;
            renderer.material = previewMat;
        }
    }

    private float GetFitness(float noiseValue, float[,] heightData, int x, int z, ObjectPlacementData.PlacementSettings settings)
    {
        float fitness = noiseValue + Random.Range(-0.6f, 0.6f);
        float currentHeight = heightData[x, z];

        if (settings.useSteepnessFilter)
        {
            float steepness = CalculateSteepness(heightData, x, z);
            if (steepness > settings.maxSlopeAngle) return 0f;
            
            fitness *= 1f - (steepness / settings.maxSlopeAngle);
        }

        if (settings.useHeightFilter)
        {
            if (currentHeight < settings.minHeight || currentHeight > settings.maxHeight) return 0f;
            
            float normalizedHeight = Mathf.InverseLerp(settings.minHeight, settings.maxHeight, currentHeight);
            fitness *= settings.heightFitnessCurve.Evaluate(normalizedHeight);
        }

        return fitness;
    }

    private float SampleHeightBilinear(float[,] heightData, float percentX, float percentZ)
    {
        int w = heightData.GetLength(0);
        int h = heightData.GetLength(1);

        float fx = Mathf.Clamp(percentX * (w - 1), 0f, w - 1f);
        float fz = Mathf.Clamp(percentZ * (h - 1), 0f, h - 1f);

        int x0 = Mathf.FloorToInt(fx);
        int z0 = Mathf.FloorToInt(fz);
        int x1 = Mathf.Min(x0 + 1, w - 1);
        int z1 = Mathf.Min(z0 + 1, h - 1);

        float tx = fx - x0;
        float tz = fz - z0;

        float h00 = heightData[x0, z0];
        float h10 = heightData[x1, z0];
        float h01 = heightData[x0, z1];
        float h11 = heightData[x1, z1];

        float h0 = Mathf.Lerp(h00, h10, tx);
        float h1 = Mathf.Lerp(h01, h11, tx);
        return Mathf.Lerp(h0, h1, tz);
    }

    private float CalculateSteepness(float[,] heightData, int x, int z)
    {
        int width = heightData.GetLength(0);
        int height = heightData.GetLength(1);

        float heightL = (x > 0) ? heightData[x - 1, z] : heightData[x, z];
        float heightR = (x < width - 1) ? heightData[x + 1, z] : heightData[x, z];
        float heightD = (z > 0) ? heightData[x, z - 1] : heightData[x, z];
        float heightU = (z < height - 1) ? heightData[x, z + 1] : heightData[x, z];

        float gradientX = (heightR - heightL) / (2f * meshSettings.meshScale);
        float gradientZ = (heightU - heightD) / (2f * meshSettings.meshScale);

        return Mathf.Atan(Mathf.Sqrt(gradientX * gradientX + gradientZ * gradientZ)) * Mathf.Rad2Deg;
    }

    private Vector3 CalculateNormalFromHeightMap(float[,] heightData, int x, int z)
    {
        int width = heightData.GetLength(0);
        int height = heightData.GetLength(1);

        float heightL = (x > 0) ? heightData[x - 1, z] : heightData[x, z];
        float heightR = (x < width - 1) ? heightData[x + 1, z] : heightData[x, z];
        float heightD = (z > 0) ? heightData[x, z - 1] : heightData[x, z];
        float heightU = (z < height - 1) ? heightData[x, z + 1] : heightData[x, z];

        Vector3 tangentX = new Vector3(2f * meshSettings.meshScale, heightR - heightL, 0);
        Vector3 tangentZ = new Vector3(0, heightU - heightD, 2f * meshSettings.meshScale);

        return Vector3.Cross(tangentZ, tangentX).normalized;
    }
}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numSupportedLODs - 1)]
    public int lod;
    public float visibleDstThreshold;

    public float sqrVisibleDstThreshold
    {
        get { return visibleDstThreshold * visibleDstThreshold; }
    }
}