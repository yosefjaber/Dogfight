using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator {
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre, float meshWorldSize) {
        float[,] values = Noise.GenerateChunkNoiseMap(width, height, settings.chunkNoiseSettings, sampleCentre);

        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        // Generate falloff map if enabled - using the corrected method
        float[,] falloffMap = null;
        if (settings.useFalloff) {
            // Use the exact method that matches mesh generation
            falloffMap = FalloffGenerator.GenerateFalloffMapExact(
                width, 
                height, 
                sampleCentre, 
                settings.falloffCenter, 
                settings.falloffRadius, 
                meshWorldSize,
                width  // assuming width == numVertsPerLine
            );
        }

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                float value = values[x, y];

                // Apply falloff if enabled
                if (settings.useFalloff && falloffMap != null) {
                    value = Mathf.Clamp01(value - falloffMap[x, y]);
                }

                // Apply height curve + multiplier
                value *= heightCurve_threadsafe.Evaluate(value) * settings.heightMultiplier;
                values[x, y] = value;

                // Track min/max
                if (value > maxValue) maxValue = value;
                if (value < minValue) minValue = value;
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap {
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue) {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}