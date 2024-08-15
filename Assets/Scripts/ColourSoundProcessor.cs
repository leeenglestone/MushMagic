using System.Collections.Generic;
using UnityEngine;

public class ColourSoundProcessor : MonoBehaviour
{
    public static readonly Dictionary<Color, int> ColorSoundMap = new Dictionary<Color, int>
    {
        { Color.red, 440 },   // Example mapping
        { Color.blue, 494 },
        { Color.green, 523 },
        // Add more colors and their associated frequencies
    };

    public void ProcessColorAndPlaySound()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Texture2D texture = renderer.material.mainTexture as Texture2D;
            if (texture != null)
            {
                // Get the dominant color from the texture
                Color dominantColor = GetDominantColor(texture);

                // Find the closest color and its frequency
                var (closestColor, _) = GetClosestColor(dominantColor);
                if (ColorSoundMap.TryGetValue(closestColor, out int frequency))
                {
                    // Process the color and play the sound
                    ProcessColor(dominantColor, frequency);
                    PlayColorSound();
                }
                else
                {
                    Debug.LogError($"No frequency mapping exists for color {closestColor}.");
                }
            }
            else
            {
                Debug.LogError("No texture found on the material.");
            }
        }
        else
        {
            Debug.LogError("No Renderer component found.");
        }
    }

    public Color GetDominantColor(Texture2D tex)
    {
        // Simplistic implementation: sample the color at the center of the texture
        // This can be enhanced with a more complex algorithm if needed
        Color[] texColors = tex.GetPixels();
        Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();
        foreach (Color col in texColors)
        {
            if (colorCounts.ContainsKey(col))
            {
                colorCounts[col]++;
            }
            else
            {
                colorCounts[col] = 1;
            }
        }

        Color dominantColor = Color.black;
        int maxCount = 0;
        foreach (var kvp in colorCounts)
        {
            if (kvp.Value > maxCount)
            {
                dominantColor = kvp.Key;
                maxCount = kvp.Value;
            }
        }

        return dominantColor;
    }

    public (Color, string) GetClosestColor(Color targetColor)
    {
        Color closestColor = Color.black;
        float minDistance = float.MaxValue;
        string colorName = "";

        foreach (var colorPair in ColorSoundMap)
        {
            Color color = colorPair.Key;
            float distance = Vector3.Distance(new Vector3(color.r, color.g, color.b), new Vector3(targetColor.r, targetColor.g, targetColor.b));
            if (distance < minDistance)
            {
                closestColor = color;
                minDistance = distance;
                colorName = color.ToString();
            }
        }

        return (closestColor, colorName);
    }

    public void ProcessColor(Color color, int frequency)
    {
        // Set frequency and do any other processing needed
        // This is where you would prepare the frequency for sound generation
        Debug.Log($"Processed color {color} with frequency {frequency} Hz.");
    }

    public void PlayColorSound()
    {
        // Logic to play the sound using the processed frequency
        // This is a placeholder and should be replaced with actual sound playing logic
        Debug.Log("Playing sound at the processed frequency.");
    }
}
