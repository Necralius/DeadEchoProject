using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightSwitch : MonoBehaviour
{
    private Light _light => GetComponent<Light>();

    public void SwitchState() => _light.enabled = !_light.enabled;
}