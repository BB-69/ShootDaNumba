using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class Meter : MonoBehaviour
{
    public Slider meter;
    private float meterSpeed = 10f, snapDelta = 0.003f;


    void Update()
    {
        SetMeter();
    }

    private void SetMeter()
    {
        if (meter.value == Glock.magCount/(float)Glock.maxMag) return;
        else if (Mathf.Abs( Glock.magCount/(float)Glock.maxMag - meter.value ) < snapDelta) {
            meter.value = Glock.magCount/(float)Glock.maxMag;
        } else meter.value = Mathf.Lerp(meter.value, Glock.magCount/(float)Glock.maxMag, meterSpeed * Time.deltaTime);
    }
}