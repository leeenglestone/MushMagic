using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnHit : MonoBehaviour
{
    public float scaleDownFactor = 0.5f; // The factor by which to scale down
    public float scaleDuration = 0.2f;   // The duration of the scaling down and up
    public float wiggleAmount = 10f;     // The amount of wiggle (in degrees for rotation or units for position)
    public float wiggleDuration = 0.2f;  // The duration of the wiggle effect
    public int wiggleCycles = 3;         // Number of wiggle cycles

    private Vector3 originalScale;       // To store the original scale of the object
    private Quaternion originalRotation; // To store the original rotation of the object

    void Start()
    {
        // Store the original scale and rotation of the object
        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name.Contains("Hand"))
        {

            // Start the scaling and wiggle coroutine when the object is hit
            StartCoroutine(ScaleAndWiggle());
        }
    }

    private System.Collections.IEnumerator ScaleAndWiggle()
    {
        // Start scaling down
        yield return ScaleTo(originalScale * scaleDownFactor, scaleDuration / 2);

        // Wiggle side to side while scaled down
        yield return Wiggle(wiggleAmount, wiggleDuration, wiggleCycles);

        // Scale back up to the original size
        yield return ScaleTo(originalScale, scaleDuration / 2);
    }

    private System.Collections.IEnumerator ScaleTo(Vector3 targetScale, float duration)
    {
        Vector3 currentScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(currentScale, targetScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Ensure the final scale is set
    }

    private System.Collections.IEnumerator Wiggle(float amount, float duration, int cycles)
    {
        float time = 0f;
        float cycleDuration = duration / (float)cycles;

        while (time < duration)
        {
            float angle = Mathf.Sin((time / cycleDuration) * Mathf.PI * 2) * amount;
            transform.rotation = originalRotation * Quaternion.Euler(0, 0, angle);
            time += Time.deltaTime;
            yield return null;
        }

        // Ensure the final rotation is reset to the original
        transform.rotation = originalRotation;
    }
}
