using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSystem : MonoBehaviour
{
    public Light directionalLight;
    public float dayDurationInSeconds = 24.0f; //adjust the duration of a full day in seconds
    public int currentHour;
    public float currentTimeOfDay = 0.0f;

    float blendedValue = 0.00f;
    public List<SkyboxTimeMapping> timeMappings;

    // Update is called once per frame
    void Update()
    {
        // Calculate current time of the day based on game time and multiplier
        currentTimeOfDay += (Time.deltaTime / dayDurationInSeconds);
        currentTimeOfDay %= 1; // Ensure it stays between 0 & 1
        currentHour = Mathf.FloorToInt(currentTimeOfDay * 24);

        //update directional light rotation
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((currentTimeOfDay*360) - 90 , 170, 0));
        //update skybox according to tiem
        UpdateSkybox();
    }
    private void UpdateSkybox()
    {
        Material currentSkybox = null;
        foreach(SkyboxTimeMapping mapping in timeMappings)
        {
            if(currentHour == mapping.hour)
            {
                currentSkybox = mapping.skyboxMaterial;

                if(currentSkybox.shader != null)
                {
                    if(currentSkybox.shader.name == "Custom/SkyboxTransition")
                    {
                        // Increment blendedValue over time
                        blendedValue += Time.deltaTime;
                        blendedValue = Mathf.Clamp01(blendedValue);
                        // Update the _TransitionFactor property
                        currentSkybox.SetFloat("_TransitionFactor",blendedValue);
                    }
                    else
                    {
                        blendedValue =0;
                    }
                    
                }
                break;
            }
        }

        if(currentSkybox != null)
        {
            RenderSettings.skybox = currentSkybox;
        }
    }
    public void setTime(int hour)
    {
        // Ensure the hour is within valid bounds (0-23)
        hour = Mathf.Clamp(hour, 0, 23);

        // Convert the hour to the time of day (0.0 to 1.0 range)
        currentTimeOfDay = (float)hour / 24.0f;

        // Update the current hour directly
        currentHour = hour;

        // Optional: Update light and skybox immediately after setting the time
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3((currentTimeOfDay * 360) - 90, 170, 0));
        UpdateSkybox();

        Debug.Log($"Time set to: {hour}:00, currentTimeOfDay: {currentTimeOfDay}");
    }

}
[System.Serializable]
public class SkyboxTimeMapping
{
    public string phaseName;
    public int hour; //the hour of the day (0-23)
    public Material skyboxMaterial;
}
