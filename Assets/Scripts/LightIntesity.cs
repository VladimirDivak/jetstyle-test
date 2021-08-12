using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[RequireComponent(typeof(AnimationCurve))]
public class LightIntesity : MonoBehaviour
{
    private HDAdditionalLightData _light;
    [SerializeField]
    private AnimationCurve lightIntensityCurve;

    private float _curveTime;
    private float _curveTotalTime;

    [ExecuteAlways]
    void Start()
    {
        _light = GetComponent<HDAdditionalLightData>();
        _curveTotalTime = lightIntensityCurve.keys.Last().time;
        
    }
    [ExecuteAlways]
    void Update()
    {
        _light.intensity = lightIntensityCurve.Evaluate(_curveTime);
        _curveTime += Time.deltaTime;

        if(_curveTime > _curveTotalTime) _curveTime = 0;
    }
}
