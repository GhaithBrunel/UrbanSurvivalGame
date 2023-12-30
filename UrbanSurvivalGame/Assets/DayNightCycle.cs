using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Light directionalLight;
    public float dayDuration = 120f; // Duration of a full day in seconds.

    private float time;
    private float intensity;

    void Update()
    {
        // Update time
        time += Time.deltaTime / dayDuration;

        // Calculate the sun's rotation
        float angle = time * 360f;
        directionalLight.transform.localRotation = Quaternion.Euler(new Vector3(angle, -30f, 0));

        // Update light intensity and color
        UpdateLighting(time % 1f);
    }

    void UpdateLighting(float time)
    {
        if (time < 0.25f || time > 0.75f) // Night
        {
            intensity = 0;
        }
        else if (time < 0.5f) // Dawn to Noon
        {
            intensity = Mathf.Lerp(0, 1, (time - 0.25f) * 4);
        }
        else // Noon to Dusk
        {
            intensity = Mathf.Lerp(1, 0, (time - 0.5f) * 4);
        }

        directionalLight.intensity = intensity;

        // Change color if needed
        // directionalLight.color = Color.Lerp(nightColor, dayColor, intensity);
    }
}
