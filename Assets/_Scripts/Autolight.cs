using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Autolight : MonoBehaviour
{
    [SerializeField] float _intensity;
    [SerializeField] Light2D _light;

    private void Update()
    {
        _light.intensity = Mathf.Max(0, _intensity - GameManager.Instance.GlobalLight.intensity);
    }
}
