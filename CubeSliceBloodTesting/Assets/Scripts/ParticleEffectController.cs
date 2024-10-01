using UnityEngine;
using System.Collections;

public class ParticleEffectController : MonoBehaviour
{
    public ParticleSystem particleEffect;  // Assign the particle system in the Inspector

    // This function will be called from an animation event
    public void PlayParticleEffect()
    {
        if (particleEffect != null)
        {
            StartCoroutine(PlayEffectForDuration(0.7f));  // Play the particle effect for 1 second
        }
        else
        {
            Debug.LogWarning("No particle system assigned!");
        }
    }

    // Coroutine to play particle effect for a set duration
    private IEnumerator PlayEffectForDuration(float duration)
    {
        particleEffect.Play();
        yield return new WaitForSeconds(duration);  // Wait for the specified time (1 second in this case)
        particleEffect.Stop();
    }
}
