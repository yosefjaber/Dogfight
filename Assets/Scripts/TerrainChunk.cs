using UnityEngine;

public class TerrainChunk {
    const float colliderGenerationDistanceThreshold = 5;

    public event System.Action<TerrainChunk, bool> onVisibilityChanged;
    public event System.Action<TerrainChunk> onChunkReady;

    public Vector2 coord;
    public bool isSpawnChunk { get; private set; }
    public bool forceMaxLOD { get; private set; }

    GameObject meshObject;
    Vector2 sampleCentre;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;

    float maxViewDst;
    float sqrMaxViewDst; // Cached squared distance

    HeightMapSettings heightMapSettings;
    public MeshSettings meshSettings;
    Transform viewer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings,
        LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer,
        Material material, bool isSpawnChunk = false, bool forceMaxLOD = false) {

        this.coord = coord;
        this.isSpawnChunk = isSpawnChunk;
        this.forceMaxLOD = forceMaxLOD;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = colliderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        sampleCentre = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject(isSpawnChunk ? "Spawn Chunk" : "Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;

        SetVisible(isSpawnChunk);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++) {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateTerrainChunk;
            if (i == colliderLODIndex) {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        sqrMaxViewDst = maxViewDst * maxViewDst; // Cache squared distance
    }

    public void Load() {
        ThreadedDataRequester.RequestData(
            () => HeightMapGenerator.GenerateHeightMap(
                meshSettings.numVertsPerLine,
                meshSettings.numVertsPerLine,
                heightMapSettings,
                sampleCentre,
                meshSettings.meshWorldSize),
            OnHeightMapReceived
        );
    }

    void OnHeightMapReceived(object heightMapObject) {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;
        UpdateTerrainChunk();
    }

    Vector2 viewerPosition {
        get { return new Vector2(viewer.position.x, viewer.position.z); }
    }

    public void UpdateTerrainChunk() {
        if (heightMapReceived) {
            // Use squared distance - faster and still circular
            float sqrViewerDstFromNearestEdge = bounds.SqrDistance(viewerPosition);
            bool wasVisible = IsVisible();
            bool visible = isSpawnChunk || sqrViewerDstFromNearestEdge <= sqrMaxViewDst;

            if (visible) {
                int lodIndex = 0;

                if (isSpawnChunk && forceMaxLOD) {
                    lodIndex = 0;
                } else {
                    // Use squared distances for LOD checks too
                    for (int i = 0; i < detailLevels.Length - 1; i++) {
                        if (sqrViewerDstFromNearestEdge > detailLevels[i].sqrVisibleDstThreshold) {
                            lodIndex = i + 1;
                        } else break;
                    }
                }

                if (lodIndex != previousLODIndex) {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh) {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    } else if (!lodMesh.hasRequestedMesh) {
                        lodMesh.RequestMesh(heightMap, meshSettings);
                    }
                }
            }

            if (wasVisible != visible) {
                SetVisible(visible);
                onVisibilityChanged?.Invoke(this, visible);
            }
        }
    }

    public void UpdateCollisionMesh() {
        if (!hasSetCollider) {
            float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);
            bool shouldHaveCollider = isSpawnChunk || sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold;

            if (shouldHaveCollider) {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh) {
                    lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
                }
            }

            bool closeEnoughForCollider = isSpawnChunk || sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold;

            if (closeEnoughForCollider) {
                if (lodMeshes[colliderLODIndex].hasMesh) {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                    onChunkReady?.Invoke(this);
                }
            }
        }
    }

    public void SetVisible(bool visible) {
        meshObject.SetActive(visible);
    }

    public bool IsVisible() {
        return meshObject.activeSelf;
    }

    public Transform GetRootTransform() {
        return meshObject.transform;
    }

    public HeightMap GetHeightMap() { return heightMap; }
}

class LODMesh {
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action updateCallback;

    public LODMesh(int lod) {
        this.lod = lod;
    }

    void OnMeshDataReceived(object meshDataObject) {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        updateCallback();
    }

    public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
        hasRequestedMesh = true;
        ThreadedDataRequester.RequestData(
            () => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod),
            OnMeshDataReceived
        );
    }
}