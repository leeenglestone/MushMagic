using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HittableObject : MonoBehaviour
{
    // Assign the sound clip in the Unity Inspector
    public AudioClip hitSound;

    // Reference to the AudioSource component
    private AudioSource audioSource;

    // Set volume scaling factors
    public float minVolume = 0.1f;
    public float maxVolume = 1.0f;
    public float velocityFactor = 0.1f; // Adjust this to scale the velocity impact on volume

    private ColourSoundProcessor colourSoundProcessor;


    // This function is called when another collider enters the trigger collider attached to the game object
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a hand (Meta hand tracking)
        if (collision.gameObject.name.Contains("Hand"))
        {
            // Calculate the collision velocity magnitude
            float collisionVelocity = collision.relativeVelocity.magnitude;

            // Scale the volume based on the collision velocity
            float volume = Mathf.Clamp(collisionVelocity * velocityFactor, minVolume, maxVolume);

            // Play the hit sound with the calculated volume
            //PlayHitSound(volume);

           if (colourSoundProcessor != null)
            {
                // Trigger the sound based on the object's color
                colourSoundProcessor.PlaySoundOnHit();
            }
            else
            {
                PlayHitSound(volume);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if the collider belongs to a hand (Meta hand tracking)
        if (other.CompareTag("Hand"))
        {
            // Optional: Handle continuous contact with the object
            Debug.Log("Hand is still in contact with the object!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider belongs to a hand (Meta hand tracking)
        if (other.CompareTag("Hand"))
        {
            // Handle the event when the hand stops touching the object
            Debug.Log("Hand left the object!");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Add an AudioSource component to the GameObject at runtime
        audioSource = gameObject.AddComponent<AudioSource>();

        // Set up the AudioSource with spatial audio settings
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 10f;

        // Get the ColourSoundProcessor component attached to this GameObject
        //colourSoundProcessor = GetComponent<ColourSoundProcessor>();

        //if (colourSoundProcessor == null)
        //{
        //    Debug.LogWarning("No ColourSoundProcessor found on the object.");
        //}

    }

    private void PlayHitSound(float volume)
    {
        if (hitSound != null && audioSource != null)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = volume;
                audioSource.clip = hitSound;
                audioSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("Hit sound or AudioSource is not assigned.");
        }
    }
}
