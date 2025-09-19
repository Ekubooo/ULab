using System;
using System.Diagnostics;
using UnityEngine;

public class GraphXD : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    
    [SerializeField, Range(10, 100)]
    int resolution = 10;
    
    [SerializeField]
    FuncLibXD.FuncName func;

    Transform[] points;

    void Awake()
    {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        
        points = new Transform[resolution * resolution];
        
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // FuncLib.Func f = FuncLib.GetFunc2(func);
        FuncLibXD.Func f = FuncLibXD.GetFunc(func);
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) *  step - 1f;
            }
            float u = (x + 0.5f) *  step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }
}
