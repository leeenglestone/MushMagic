using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ColourSoundProcessor : MonoBehaviour
{
    private static readonly List<Color> BasicColors = new List<Color>
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta,
        Color.black,
        Color.white,
        Color.gray
    };

    private static readonly Dictionary<Color, int> ColorSoundMap = new Dictionary<Color, int>
    {
        { Color.red, 329 },     // E4
        { Color.green, 349 },   // F4
        { Color.blue, 391 },    // G4
        { Color.yellow, 440 },  // A4
        { Color.cyan, 493 },    // B4
        { Color.magenta, 523 }, // C5
        { Color.black, 261 },   // C4
        { Color.white, 293 },   // D4
        { Color.gray, 587 },    // D5
    };

    private static readonly Dictionary<Color, string> ColorNames = new Dictionary<Color, string>
    {
        { Color.red, "Red" },
        { Color.green, "Green" },
        { Color.blue, "Blue" },
        { Color.yellow, "Yellow" },
        { Color.cyan, "Cyan" },
        { Color.magenta, "Magenta" },
        { Color.black, "Black" },
        { Color.white, "White" },
        { Color.gray, "Gray" },
    };

    public AudioSource audioSource; // Assign this in the Unity Editor
    public GameObject targetObject; // Assign the target GameObject (Prefab or instance) in the Unity Editor
    public float sineWaveDuration = 1.0f; // Duration of each sine wave sound
    public int numberOfWaves = 3; // Number of separate waves to play in succession
    public float waveInterval = 0.0f; // Time interval between consecutive waves (set to 0 for no gap)
    public int numberOfClusters = 5; // Number of color clusters for KMeans

    private Color dominantColor;
    private int targetFrequency;

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!");
            return;
        }

        if (targetObject == null)
        {
            Debug.LogError("Target GameObject is not assigned!");
            return;
        }

        Renderer renderer = targetObject.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Target GameObject does not have a Renderer component!");
            return;
        }

        // Get the texture from the material
        Texture2D texture = renderer.material.mainTexture as Texture2D;

        if (texture == null)
        {
            Debug.LogError("No texture found on the material.");
            return;
        }

        // Find the dominant color from the texture using clustering
        dominantColor = GetDominantColor(texture);

        // Find the closest basic color to the dominant color
        var (closestColor, closestColorName) = GetClosestColor(dominantColor);
        if (ColorSoundMap.TryGetValue(closestColor, out int frequency))
        {
            targetFrequency = frequency;
            Debug.Log($"Dominant Color RGB ({dominantColor.r * 255}, {dominantColor.g * 255}, {dominantColor.b * 255}) is closest to {closestColorName} with RGB ({closestColor.r * 255}, {closestColor.g * 255}, {closestColor.b * 255}) and Base Frequency: {frequency} Hz");
        }
        else
        {
            Debug.Log($"No frequency mapping exists for color {closestColorName} ({ColorToString(closestColor)})");
            targetFrequency = -1; // Invalid frequency
        }
    }

    public void PlaySoundOnHit()
    {
        // Ensure the target frequency has been set
        if (targetFrequency != -1)
        {
            StartCoroutine(ProcessColor(dominantColor, targetFrequency));
        }
        else
        {
            Debug.LogError("No valid frequency found for the object's color.");
        }
    }

    private (Color, string) GetClosestColor(Color targetColor)
    {
        Color closestColor = Color.black;
        string closestColorName = "Unknown";
        float minDistance = float.MaxValue;

        // Convert target color to CIE-LAB color space
        Vector3 targetLab = RGBToLab(targetColor);

        foreach (var color in BasicColors)
        {
            // Convert each basic color to CIE-LAB color space
            Vector3 colorLab = RGBToLab(color);

            // Compute the color distance with adjusted weights
            float distance = ColorDistance(targetLab, colorLab, color);
            Debug.Log($"Comparing target color ({ColorToString(targetColor)}) with {ColorNames[color]}: Distance = {distance}");

            if (distance < minDistance)
            {
                minDistance = distance;
                closestColor = color;
                closestColorName = ColorNames[color];
            }
        }

        Debug.Log($"Closest Color: {closestColorName} with Distance = {minDistance}");

        return (closestColor, closestColorName);
    }

    private float ColorDistance(Vector3 lab1, Vector3 lab2, Color color)
    {
        // CIE76 color difference formula
        float lDiff = lab1.x - lab2.x;
        float aDiff = lab1.y - lab2.y;
        float bDiff = lab1.z - lab2.z;
        float distance = Mathf.Sqrt(lDiff * lDiff + aDiff * aDiff + bDiff * bDiff);

        // Increase the distance for black, gray, and white
        if (color == Color.black || color == Color.white || color == Color.gray)
        {
            distance *= 1.25f; // Increase the distance by 25%
        }

        return distance;
    }

    private Vector3 RGBToLab(Color color)
    {
        // Convert RGB to XYZ color space
        Vector3 xyz = RGBToXYZ(color);

        // Convert XYZ to Lab color space
        float x = xyz.x / 95.047f;
        float y = xyz.y / 100.000f;
        float z = xyz.z / 108.883f;

        x = x > 0.008856 ? Mathf.Pow(x, 1f / 3f) : (x * 7.787f) + (16f / 116f);
        y = y > 0.008856 ? Mathf.Pow(y, 1f / 3f) : (y * 7.787f) + (16f / 116f);
        z = z > 0.008856 ? Mathf.Pow(z, 1f / 3f) : (z * 7.787f) + (16f / 116f);

        float l = (116f * y) - 16f;
        float a = (x - y) * 500f;
        float b = (y - z) * 200f;

        return new Vector3(l, a, b);
    }

    private Vector3 RGBToXYZ(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        r = r <= 0.04045f ? r / 12.92f : Mathf.Pow((r + 0.055f) / 1.055f, 2.4f);
        g = g <= 0.04045f ? g / 12.92f : Mathf.Pow((g + 0.055f) / 1.055f, 2.4f);
        b = b <= 0.04045f ? b / 12.92f : Mathf.Pow((b + 0.055f) / 1.055f, 2.4f);

        r *= 100f;
        g *= 100f;
        b *= 100f;

        float x = r * 0.4124564f + g * 0.3575761f + b * 0.1804375f;
        float y = r * 0.2126729f + g * 0.7151522f + b * 0.0721750f;
        float z = r * 0.0193339f + g * 0.1191920f + b * 0.9503041f;

        return new Vector3(x, y, z);
    }

    private Color GetDominantColor(Texture2D tex)
    {
        // Ensure the texture is readable
        if (!tex.isReadable)
        {
            Debug.LogError("Texture is not readable. Please mark the texture as readable in the import settings.");
            return Color.clear;
        }

        // Retrieve all pixel colors from the texture
        Color[] pixels = tex.GetPixels();

        // Convert colors to float arrays for clustering
        List<Vector3> colorVectors = pixels.Select(p => new Vector3(p.r, p.g, p.b)).ToList();

        // Perform KMeans clustering
        KMeans kmeans = new KMeans(numberOfClusters);
        List<Vector3> clusterCenters = kmeans.Cluster(colorVectors);

        // Determine the most frequent cluster
        List<int> clusterSizes = new List<int>(new int[numberOfClusters]);
        foreach (var colorVector in colorVectors)
        {
            int clusterIndex = kmeans.GetNearestClusterIndex(colorVector);
            clusterSizes[clusterIndex]++;
        }

        int dominantClusterIndex = clusterSizes.IndexOf(clusterSizes.Max());
        Vector3 dominantColorVector = clusterCenters[dominantClusterIndex];

        return new Color(dominantColorVector.x, dominantColorVector.y, dominantColorVector.z);
    }

    private string ColorToString(Color color)
    {
        return $"({color.r * 255}, {color.g * 255}, {color.b * 255})";
    }

    private IEnumerator ProcessColor(Color color, float frequency)
    {
        for (int i = 0; i < numberOfWaves; i++)
        {
            PlaySineWave(frequency, sineWaveDuration);
            yield return new WaitForSeconds(sineWaveDuration + waveInterval);
        }
    }

    private void PlaySineWave(float frequency, float duration)
    {
        // Generate and play a sine wave
        int sampleRate = AudioSettings.outputSampleRate;
        int samples = Mathf.FloorToInt(sampleRate * duration);
        float[] buffer = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            buffer[i] = Mathf.Sin(2 * Mathf.PI * frequency * i / sampleRate);
        }

        AudioClip clip = AudioClip.Create("SineWave", samples, 1, sampleRate, false);
        clip.SetData(buffer, 0);
        audioSource.clip = clip;
        audioSource.Play();
    }
}
