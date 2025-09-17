using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    const float hours2degrees = -30f;
    const float minutes2degrees = -6f;
    const float seconds2degrees = -6f;
    
    [SerializeField]
    Transform hoursPivot, minutesPivot, secondsPivot;
    
    // [SerializeField]
    // Transform minutesPivot;
    
    // [SerializeField]
    // Transform secondsPivot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // var time = DateTime.Now;
        // Debug.Log(DateTime.Now.Hour);
        
        TimeSpan time = DateTime.Now.TimeOfDay;
        
        /*
         hoursPivot.rotation = Quaternion.Euler(0f, 0f, hours2degrees * time.Hour);
        minutesPivot.localRotation = Quaternion.Euler(0f, 0f, minutes2degrees * time.Minute);
        secondsPivot.localRotation = Quaternion.Euler(0f, 0f, seconds2degrees * time.Second);
        */
        
        hoursPivot.rotation = Quaternion.Euler(0f, 0f, hours2degrees * (float)time.TotalHours);
        minutesPivot.localRotation = Quaternion.Euler(0f, 0f, minutes2degrees * (float)time.TotalMinutes);
        secondsPivot.localRotation = Quaternion.Euler(0f, 0f, seconds2degrees * (float)time.TotalSeconds);
    }
}