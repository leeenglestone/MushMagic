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

    private GameObject _debugCube;

    // Reference to the Rigidbody of the object this script is attached to
    //private Rigidbody rb;

    // Store the previous velocity of the object
    //private Vector3 previousVelocity;

    // This function is called when another collider enters the trigger collider attached to the game object
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with a hand (Meta hand tracking)
        //if (collision.gameObject.CompareTag("Hand") || collision.gameObject.CompareTag("Hand"))
        if (collision.gameObject.name.Contains("Hand"))
        {
            // Calculate the collision velocity magnitude
            float collisionVelocity = collision.relativeVelocity.magnitude;

            // Scale the volume based on the collision velocity
            float volume = Mathf.Clamp(collisionVelocity * velocityFactor, minVolume, maxVolume);

            // Play the hit sound with the calculated volume
            PlayHitSound(volume);
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    // If the other object has a Rigidbody, calculate the relative velocity
    //    Rigidbody otherRb = other.attachedRigidbody;
    //    if (otherRb != null)
    //    {
    //        Vector3 relativeVelocity = otherRb.velocity - previousVelocity;
    //        float magnitude = relativeVelocity.magnitude;

    //        Debug.Log("Trigger entered by: " + other.gameObject.name);
    //        Debug.Log("Relative velocity magnitude: " + magnitude);

    //        // Play sound or perform other actions based on the relative velocity
    //        PlayHitSoundBasedOnVelocity(magnitude);
    //    }
    //    else
    //    {
    //        Debug.Log("The other object does not have a Rigidbody.");
    //    }
    //}



    // This function is called when another collider stays within the trigger collider attached to the game object
    private void OnTriggerStay(Collider other)
    {
        // Check if the collider belongs to a hand (Meta hand tracking)
        if (other.CompareTag("Hand"))
        {
            // Optional: Handle continuous contact with the object
            Debug.Log("Hand is still in contact with the object!");
        }
    }

    // This function is called when another collider exits the trigger collider attached to the game object
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
        // Get the Rigidbody component attached to this GameObject
        //rb = GetComponent<Rigidbody>();

        // Add an AudioSource component to the GameObject at runtime
        audioSource = gameObject.AddComponent<AudioSource>();

        // Set up the AudioSource with spatial audio settings
        audioSource.spatialBlend = 1.0f; // 3D sound
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1f;
        audioSource.maxDistance = 10f;
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

    //private void PlayHitSoundBasedOnVelocity(float velocityMagnitude)
    //{
    //    // Assuming you have an AudioSource and AudioClip attached and set up
    //    AudioSource audioSource = GetComponent<AudioSource>();

    //    if (audioSource != null)
    //    {
    //        // Scale volume based on velocity
    //        float volume = Mathf.Clamp(velocityMagnitude / 10.0f, 0.1f, 1.0f); // Adjust scaling as needed
    //        audioSource.volume = volume;
    //        audioSource.Play();
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    //void FixedUpdate()
    //{
    //    // Update the previous velocity to the current velocity each physics update
    //    if (rb != null)
    //    {
    //        previousVelocity = rb.velocity;
    //    }
    //}
}
