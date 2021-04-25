using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light light;
    public float sinAmp = 1.0f;
    public float sinFreq = 1.0f;
    public float sin2Amp = 1.0f;
    public float sin2Freq = 1.0f;
    public float baseIntensity = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        light.intensity = baseIntensity +
            (Mathf.Sin(Time.realtimeSinceStartup * sinFreq) * sinAmp) +
            (Mathf.Sin(Time.realtimeSinceStartup * sin2Freq) * sin2Amp);      
    }
}
