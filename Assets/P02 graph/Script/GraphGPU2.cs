using System;
using System.Diagnostics;
using UnityEngine;

public class GraphGPU2 : MonoBehaviour
{
    // const int maxResolution = 1000;
    [SerializeField, Range(10, 500)] int resolution = 200;
    
    [SerializeField] FuncLibXD.FuncName func;
    
    public enum TransitionMode { Cycle, Random }
    [SerializeField] TransitionMode transitionMode;

    [SerializeField, Min(0f)] 
    float funcDuration = 1f, transitionDuration = 1f;
    
    float duration;
    bool transitioning;
    FuncLibXD.FuncName transFunc;
    
    ComputeBuffer posBuffer;
    [SerializeField] ComputeShader _cs;
    
    static readonly int
        positionsId = Shader.PropertyToID("_Pos"),
        resolutionId = Shader.PropertyToID("_Res"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time");
    
    [SerializeField] Material material;
    [SerializeField] Mesh mesh;
    
    void OnEnable()
    {
        posBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);    
    }

    void OnDisable()
    {
        posBuffer.Release();
        posBuffer = null;
    }
    
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
        UpdateFunctionOnGPU();
    }
    void PickNextFunction () 
    {
        func = transitionMode == TransitionMode.Cycle ?
            FuncLibXD.GetNextFuncName(func) :
            FuncLibXD.GetRandomFunctionNameOtherThan(func);
    }
    
    void UpdateFunctionOnGPU () 
    {
        float step = 2f / resolution;
        _cs.SetInt(resolutionId, resolution);
        _cs.SetFloat(stepId, step);
        _cs.SetFloat(timeId, Time.time);
        
        _cs.SetBuffer(0, positionsId, posBuffer);
        
        int groups = Mathf.CeilToInt(resolution / 8f);
        _cs.Dispatch(0, groups, groups, 1);
        
        material.SetBuffer(positionsId, posBuffer);
        material.SetFloat(stepId, step);
        
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural
            (mesh, 0, material, bounds, posBuffer.count);
        
    }
    
}
