using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTiling : MonoBehaviour
{
    private Renderer rend;
    [SerializeField] private Vector2 tilingVec;

    void Start()
    {
        rend = GetComponent<Renderer>();
        Material _mat = rend.material;
        _mat.SetVector("_TilingVec", tilingVec);
    }
}
