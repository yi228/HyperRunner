using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineThickness : MonoBehaviour
{
    private Renderer rend;
    [SerializeField] private float thickness;

    void Start()
    {
        rend = GetComponent<Renderer>();
        Material _mat;
        _mat = rend.materials[1];
        _mat.SetFloat("_Thickness", thickness);
    }
}
