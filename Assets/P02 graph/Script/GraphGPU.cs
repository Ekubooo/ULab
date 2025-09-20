using System;
using System.Diagnostics;
using UnityEngine;
public class GraphGPU : MonoBehaviour
{
    const int maxResolution = 1000;
    
    [SerializeField, Range(10, maxResolution)]
    int resolution = 200;
    
    [SerializeField]
    FuncLibXD.FuncName func;
    
    [SerializeField]  
    ComputeShader _cs;
    
    [SerializeField]
    Material _material;
    
    [SerializeField]
    Mesh _mesh;
    
    ComputeBuffer posBuffer;

    static readonly int
        posID = Shader.PropertyToID("_Pos"),
        resID = Shader.PropertyToID("_Res"),
        setpID = Shader.PropertyToID("_Step"),
        timeID = Shader.PropertyToID("_Time");
    
    void OnEnable()
    {
        posBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);    
    }

    void OnDisable()
    {
        posBuffer.Release();
        posBuffer = null;
    }
    void Update()
    {
        updateFuncOnGPU();
    }
    
    void updateFuncOnGPU()
    {
        float step = 2f / resolution;
        _cs.SetInt(resID, resolution);
        _cs.SetFloat(setpID, step);
        _cs.SetFloat(timeID, Time.time);
        
        _cs.SetBuffer(0, posID, posBuffer);
        
        int groups = Mathf.CeilToInt(resolution / 8f);
        _cs.Dispatch(0, groups, groups, 1);
        
        _material.SetBuffer(posID, posBuffer);
        _material.SetFloat(setpID, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f /resolution));
        Graphics.DrawMeshInstancedProcedural
            (_mesh, 0, _material, bounds, resolution * resolution);
    }

}
