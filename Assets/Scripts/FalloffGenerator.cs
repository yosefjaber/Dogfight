using UnityEngine;

public static class FalloffGenerator {

    public static float[,] GenerateFalloffMap(int width, int height, Vector2 sampleCentre, Vector2 falloffCenter, float falloffRadius, float meshWorldSize) {
        float[,] map = new float[width, height];

        if (falloffRadius <= 0f) return map;

        // Calculate the top-left corner of the chunk in world space
        // This matches how MeshGenerator calculates vertex positions
        Vector2 topLeft = sampleCentre + new Vector2(-1, 1) * meshWorldSize / 2f;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // Calculate percent position within the chunk (0 to 1)
                Vector2 percent = new Vector2(x - 1, y - 1) / (width - 3);
                
                // Calculate world position using the same method as MeshGenerator
                Vector2 worldPos = topLeft + new Vector2(percent.x, -percent.y) * meshWorldSize;

                float dist = Vector2.Distance(worldPos, falloffCenter);
                float normalized = Mathf.Clamp01(dist / falloffRadius);

                map[x, y] = Evaluate(normalized);
            }
        }

        return map;
    }

    // Alternative method that directly matches your mesh vertex calculation
    public static float[,] GenerateFalloffMapExact(int width, int height, Vector2 sampleCentre, Vector2 falloffCenter, float falloffRadius, float meshWorldSize, int numVertsPerLine) {
        float[,] map = new float[width, height];

        if (falloffRadius <= 0f) return map;

        Vector2 topLeft = new Vector2(-1, 1) * meshWorldSize / 2f;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                // Use the exact same calculation as in MeshGenerator
                Vector2 percent = new Vector2(x - 1, y - 1) / (numVertsPerLine - 3);
                Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshWorldSize;
                
                // Convert to world space by adding the sample centre
                Vector2 worldPos = new Vector2(
                    sampleCentre.x + vertexPosition2D.x,
                    sampleCentre.y + vertexPosition2D.y
                );

                float dist = Vector2.Distance(worldPos, falloffCenter);
                float normalized = Mathf.Clamp01(dist / falloffRadius);

                map[x, y] = Evaluate(normalized);
            }
        }

        return map;
    }

    static float Evaluate(float value) {

        if (value < 0.25f) {
            return -16f * Mathf.Pow(value, 2f) + 1f;
        } else if (0.25f <= value && value <= 0.75f) {
            return 0f;
        } else {
            return 16f * Mathf.Pow(value - 0.75f, 2f) - 1f;
        }
    }

/*
    static float Evaluate(float value) {
        float a = 2;
        float b = 20f;
        float c = 0.1f;

        return -Mathf.Pow(value + c, a) / (Mathf.Pow(value + c, a) + Mathf.Pow(b - b * (value + c), a));
    }
*/
}