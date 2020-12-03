using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    Random rng = new Random();

    private Light myLight;

    [Header("Timer")]
    [SerializeField] private float timer;

    [Header("Flicker Status")]
    [SerializeField] private bool FlickerActive;

    [Header("Randomising Timings")]
    [SerializeField] private float[] randomTimesToStartFlickering;
    [SerializeField] private float[] randomFlickerDurations;

    [Header("Current Timings")]
    [SerializeField] private float currentTimeToStartFlickering;
    [SerializeField] private float currentFlickerDuration;


    private float defaultLightIntensity;
    [SerializeField] private float maxIntensity;
    private float flickerTimer;

    float test = 0;

    private void Awake() => myLight = GetComponent<Light>();
    private void Start()
    {
        ChooseRandomFlickerTime();

        defaultLightIntensity = myLight.intensity;
    }

    private void ChooseRandomFlickerDuration() => currentFlickerDuration = randomFlickerDurations[Random.Range(0, randomFlickerDurations.Length - 1)];
    private void ChooseRandomFlickerTime() => currentTimeToStartFlickering = randomTimesToStartFlickering[Random.Range(0, randomTimesToStartFlickering.Length - 1)];
    private void ResetTimer() => timer = 0;
    private void ResetLightIntensity() => myLight.intensity = defaultLightIntensity;

    private void Update()
    {
        //timer += Time.deltaTime;

        //if (FlickerActive == false) //Countdown to flicker.
        //{
        //    if (timer >= currentTimeToStartFlickering)
        //    {
        //        ResetTimer();
        //        FlickerActive = true;
        //        ChooseRandomFlickerDuration();
        //    }
        //}
        //else //Flicker is active.
        //{
        //    //Flicker code.

        //    flickerTimer += Time.deltaTime * 5;
        //    myLight.intensity = Mathf.PingPong(flickerTimer, defaultLightIntensity);




        //    if (timer >= currentFlickerDuration)
        //    {
        //        ResetLightIntensity();
        //        FlickerActive = false;
        //        ResetTimer();
        //        ChooseRandomFlickerTime();
        //    }
        //}
    }
}
