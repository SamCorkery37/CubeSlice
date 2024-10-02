using UnityEngine;

public class FogAnimator : MonoBehaviour
{
    public float fogMinDensity = 0.01f;
    public float fogMaxDensity = 0.05f;
    public float fogChangeSpeed = 1.0f;

    private bool increasing = true;

    void Update()
    {
        if (increasing)
        {
            RenderSettings.fogDensity += Time.deltaTime * fogChangeSpeed * 0.001f;
            if (RenderSettings.fogDensity >= fogMaxDensity)
            {
                increasing = false;
            }
        }
        else
        {
            RenderSettings.fogDensity -= Time.deltaTime * fogChangeSpeed * 0.001f;
            if (RenderSettings.fogDensity <= fogMinDensity)
            {
                increasing = true;
            }
        }
    }
}
