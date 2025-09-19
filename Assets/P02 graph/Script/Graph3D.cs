using System;
using System.Diagnostics;
using UnityEngine;

public class Graph3D : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    
    [SerializeField, Range(10, 100)]
    int resolution = 10;
    
    [SerializeField]
    FuncLib3D.FuncName func;

    Transform[] points;

    void Awake()
    {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        
        points = new Transform[resolution * resolution];
        
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            if (x == resolution)
            {
                x = 0;
                z += 1;
            }
            
            position.x = (x + 0.5f) * step - 1f;
            position.z = (z + 0.5f) * step - 1f;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // FuncLib.Func f = FuncLib.GetFunc2(func);
        FuncLib3D.Func f = FuncLib3D.GetFunc(func);
        float time = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            // position.y = position.x * position.x * position.x;
            // position.y = Mathf.Sin(Mathf.PI *  (position.x + time));
            
            position.y = f(position.x, position.z, time);
            point.localPosition = position;
        }
    }
}
