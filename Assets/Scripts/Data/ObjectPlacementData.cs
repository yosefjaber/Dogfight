using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ObjectPlacementData", menuName = "Terrain/Object Placement Data")]
public class ObjectPlacementData : UpdatableData
{
    [System.Serializable]
    public class PlacementSettings
    {
        [Header("Prefab")]
        public GameObject placementPrefab;

        [Header("Placement Controls")]
        [Range(1, 20)] public int placementResolution = 5;
        [Range(0f, 1f)] public float placementDensity = 0.5f;
        public bool showPlacementPreview = true;

        [Header("Rotation Randomization")]
        public bool randomRotationY = true;
        public Vector2 rotationYRange = new Vector2(0f, 360f);

        public bool randomRotationX = false;
        public Vector2 rotationXRange = new Vector2(-10f, 10f);

        public bool randomRotationZ = false;
        public Vector2 rotationZRange = new Vector2(-10f, 10f);

        [Header("Scale Randomization")]
        public bool uniformScale = true;
        public Vector2 uniformScaleValue = new Vector2(1f, 1f);

        public bool randomScaleY = true;
        public Vector2 scaleYRange = new Vector2(1f, 1f);

        public bool randomScaleX = false;
        public Vector2 scaleXRange = new Vector2(1f, 1f);

        public bool randomScaleZ = false;
        public Vector2 scaleZRange = new Vector2(1f, 1f);

        [Header("Slope Filter")]
        public bool useSteepnessFilter = false;
        [Range(0f, 90f)] public float maxSlopeAngle = 30f;

        [Header("Height Filter")]
        public bool useHeightFilter = false;
        public float minHeight = 0f;
        public float maxHeight = 1f;
        public AnimationCurve heightFitnessCurve = AnimationCurve.Linear(0, 1, 1, 1);
    }

    [Tooltip("List of different objects and their placement settings")]
    public List<PlacementSettings> objectSettingsList = new List<PlacementSettings>();
}