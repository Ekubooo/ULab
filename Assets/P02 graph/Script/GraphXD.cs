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
    
    public enum TransitionMode { Cycle, Random }

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField, Min(0f)] 
    float funcDuration = 1f, transitionDuration = 1f;

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

    float duration;
    bool transitioning;
    private FuncLibXD.FuncName transFunc;
    
    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration) 
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }

        else if (duration >= funcDuration)
        {
            duration -= funcDuration;
            transitioning = true;
            transFunc = func;
            PickNextFunction();
        }
        
        if (transitioning) 
            UpdateFunctionTransition();
        else 
            UpdateFunction();
    }
    void UpdateFunction ()
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
    
    void UpdateFunctionTransition  ()
    {
        // FuncLib.Func f = FuncLib.GetFunc2(func);
        FuncLibXD.Func
            from = FuncLibXD.GetFunc(transFunc),
            to = FuncLibXD.GetFunc(func);
        float progress = duration / transitionDuration;
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
            points[i].localPosition 
                = FuncLibXD.Morph(u, v, time, from, to, progress);
        }
    }
    
    void PickNextFunction () 
    {
        func = transitionMode == TransitionMode.Cycle ?
            FuncLibXD.GetNextFuncName(func) :
            FuncLibXD.GetRandomFunctionNameOtherThan(func);
    }
    
}
