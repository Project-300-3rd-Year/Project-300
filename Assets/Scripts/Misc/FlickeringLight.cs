using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    private Light myLight;

    [Header("Timer")]
    [SerializeField] private float timer;

    [Header("Flicker Status")]
    [SerializeField] private bool FlickerActive;

    [Header("Randomising Elements")]
    [SerializeField] private float[] randomTimesToStartFlickering;
    [SerializeField] private float[] randomFlickerDurations;
    [SerializeField] private float[] randomFlickerSpeeds;

    [Header("Current Timings")]
    [SerializeField] private float currentTimeToStartFlickering;
    [SerializeField] private float currentFlickerDuration;
    [SerializeField] private float currentFlickerSpeed;

    [Header("Actual Info")]
    [Tooltip("Specifies a minimum percentage that the light intensity can go down to when flickering")]
    [Range(0,1)]
    [SerializeField] float minPercentage = 0.7f;
    [SerializeField] float currentMinFlickerIntensity;

    private float flickerMinIntensity;
    private float flickerIntensity;
    private bool up = false;
    private float defaultLightIntensity;

    //Start.
    private void Awake() => myLight = GetComponent<Light>();
    private void Start()
    {
        ChooseRandomFlickerTime();

        defaultLightIntensity = myLight.intensity;
        flickerMinIntensity = defaultLightIntensity - (defaultLightIntensity * minPercentage);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (FlickerActive == false) //Countdown to flicker.
        {
            if (timer >= currentTimeToStartFlickering)
            {
                ResetTimer();
                FlickerActive = true;
                ChooseRandomFlickerDuration();
                ChooseRandomMinimumFlickerIntensity();

            }
        }
        else //Flicker process is active.
        {
            myLight.intensity = flickerIntensity;

            if (up)
                flickerIntensity += Time.deltaTime * currentFlickerSpeed;
            else
                flickerIntensity -= Time.deltaTime * currentFlickerSpeed;


            if (flickerIntensity >= defaultLightIntensity) //Only when the intensity reaches default intensity does it check if the timer has gone over 
            {
                if (timer >= currentFlickerDuration)
                {

                    ResetLightIntensity();
                    FlickerActive = false;
                    ResetTimer();
                    ChooseRandomFlickerTime();
                }

                flickerIntensity = defaultLightIntensity;
                up = false;
                ChooseRandomFlickerSpeed();
                ChooseRandomMinimumFlickerIntensity();
            }
            else if (flickerIntensity <= currentMinFlickerIntensity)
            {
                flickerIntensity = currentMinFlickerIntensity;
                up = true;
                ChooseRandomFlickerSpeed();
            }
        }
    }


    private void ChooseRandomFlickerDuration() => currentFlickerDuration = randomFlickerDurations[Random.Range(0, randomFlickerDurations.Length - 1)];
    private void ChooseRandomFlickerTime() => currentTimeToStartFlickering = randomTimesToStartFlickering[Random.Range(0, randomTimesToStartFlickering.Length - 1)];
    private void ChooseRandomFlickerSpeed() => currentFlickerSpeed = randomFlickerSpeeds[Random.Range(0, randomFlickerSpeeds.Length - 1)];
    private void ChooseRandomMinimumFlickerIntensity()
    {
        currentMinFlickerIntensity = Random.Range(flickerMinIntensity, defaultLightIntensity);
        flickerIntensity = defaultLightIntensity;
    }
    private void ResetTimer() => timer = 0;
    private void ResetLightIntensity() => myLight.intensity = defaultLightIntensity;

}
