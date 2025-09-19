using System;
using System.Diagnostics;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField]
    Transform pointPrefab;
    
    [SerializeField, Range(10, 100)]
    int resolution = 10;
    
    [SerializeField]
    FuncLib.FuncName func;

    Transform[] points;

    void Awake()
    {
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        var position = Vector3.zero;
        
        points = new Transform[resolution];
        
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            
            position.x = (i + 0.5f) * step - 1f;
            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform, false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FuncLib.Func f = FuncLib.GetFunc2(func);
        float time = Time.time;
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];
            Vector3 position = point.localPosition;
            // position.y = position.x * position.x * position.x;
            // position.y = Mathf.Sin(Mathf.PI *  (position.x + time));
            
            position.y = f(position.x, time);
            point.localPosition = position;
        }
    }
}
