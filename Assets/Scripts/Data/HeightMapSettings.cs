using UnityEngine;
using System.Collections;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{
    public ChunkNoiseSettings chunkNoiseSettings;
    public PlantNoiseSettings plantNoiseSettings;
    public float heightMultiplier = 20f;
    public AnimationCurve heightCurve;

    public bool useFalloff;
    
    [Header("Falloff")]
    public Vector2 falloffCenter = Vector2.zero; // world-space (x,z)
    public float falloffRadius = 100f; // world units

    public float minHeight {
        get {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight {
        get {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate() {
        if (chunkNoiseSettings != null) {
            chunkNoiseSettings.ValidateValues();
        }
        base.OnValidate();
    }

#endif
}