using System.Collections.Generic;
using UnityEngine;

public class SimpleKMeans
{
    public int NumClusters { get; private set; }
    public List<Color> ClusterCenters { get; private set; }

    public SimpleKMeans(int numClusters)
    {
        NumClusters = numClusters;
        ClusterCenters = new List<Color>(numClusters);
    }

    public void Fit(Color[] data)
    {
        // Initialize cluster centers (randomly choose initial centers)
        InitializeCenters(data);

        bool hasChanged;
        do
        {
            hasChanged = false;

            // Create clusters
            List<Color>[] clusters = new List<Color>[NumClusters];
            for (int i = 0; i < NumClusters; i++)
            {
                clusters[i] = new List<Color>();
            }

            // Assign each data point to the closest cluster center
            foreach (var color in data)
            {
                int closestCluster = FindClosestCluster(color);
                clusters[closestCluster].Add(color);
            }

            // Recompute cluster centers
            for (int i = 0; i < NumClusters; i++)
            {
                if (clusters[i].Count > 0)
                {
                    Color newCenter = ComputeMeanColor(clusters[i]);
                    if (ClusterCenters[i] != newCenter)
                    {
                        ClusterCenters[i] = newCenter;
                        hasChanged = true;
                    }
                }
            }
        }
        while (hasChanged);
    }

    private void InitializeCenters(Color[] data)
    {
        HashSet<int> chosenIndices = new HashSet<int>();
        System.Random rand = new System.Random();

        for (int i = 0; i < NumClusters; i++)
        {
            int index;
            do
            {
                index = rand.Next(data.Length);
            } while (chosenIndices.Contains(index));

            chosenIndices.Add(index);
            ClusterCenters.Add(data[index]);
        }
    }

    private int FindClosestCluster(Color color)
    {
        int closestCluster = 0;
        float minDistance = ColorDistance(color, ClusterCenters[0]);

        for (int i = 1; i < ClusterCenters.Count; i++)
        {
            float distance = ColorDistance(color, ClusterCenters[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestCluster = i;
            }
        }

        return closestCluster;
    }

    private Color ComputeMeanColor(List<Color> colors)
    {
        float r = 0, g = 0, b = 0;
        foreach (var color in colors)
        {
            r += color.r;
            g += color.g;
            b += color.b;
        }

        int count = colors.Count;
        return new Color(r / count, g / count, b / count);
    }

    private float ColorDistance(Color c1, Color c2)
    {
        float rDiff = c1.r - c2.r;
        float gDiff = c1.g - c2.g;
        float bDiff = c1.b - c2.b;
        return Mathf.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff);
    }
}
