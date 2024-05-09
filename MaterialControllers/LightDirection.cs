using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightDirection : MonoBehaviour
{
    void Update()
    {
        if (gameObject.transform.hasChanged)
        {
            Renderer[] renderers = FindObjectsOfType<Renderer>();
            foreach(var r  in renderers)
            {
                Material _mat;
#if UNITY_EDITOR
                _mat = r.sharedMaterial;
#else
                _mat = r.material;
#endif
                if (string.Compare(_mat.shader.name, "Shader Graphs/ToonRampShader") == 0)
                    _mat.SetVector("_LightDir", transform.forward);
            }
        }
    }
}
