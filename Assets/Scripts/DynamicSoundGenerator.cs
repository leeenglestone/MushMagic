using System.Collections;
using UnityEngine;

public class DynamicSoundGenerator : MonoBehaviour
{
    public AudioSource audioSource; // Assign this in the Unity Editor

    private void Start()
    {
        // Example frequency and duration
        int frequency = 440; // A4
        float duration = 1.0f; // Duration in seconds

        // Generate and play audio
        AudioClip audioClip = GenerateAudioClip(frequency, duration);
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    private AudioClip GenerateAudioClip(int frequency, float duration)
    {
        int sampleRate = 44100; // Standard sample rate for audio
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            float waveValue = Mathf.Sin(2 * Mathf.PI * frequency * time);
            samples[i] = waveValue;
        }

        // Create an AudioClip and fill it with the generated samples
        AudioClip audioClip = AudioClip.Create("DynamicClip", sampleCount, 1, sampleRate, false);
        audioClip.SetData(samples, 0);

        return audioClip;
    }
}
