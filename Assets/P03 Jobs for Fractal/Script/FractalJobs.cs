using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using Unity.VisualScripting;
using UnityEngine;

using static Unity.Mathematics.math;
using float4x4 = Unity.Mathematics.float4x4;
using quaternion = Unity.Mathematics.quaternion;

public class FractalJobs : MonoBehaviour
{
    // Interface of "Job", using in the FOR loop
    [BurstCompile(CompileSynchronously = true)] 
    struct UpdateFractalLevelJob : IJobFor
    {   
        public float spinAngleDelta;
        public float scale;
        
        public NativeArray<FractalPart> parts;
        [ReadOnly] public NativeArray<FractalPart>  parents;
        [WriteOnly]public NativeArray<float4x4>    matrices;
        public void Execute(int i)
        {   // replace the innermost loop of update()
            FractalPart parent = parents[i / 5];
            FractalPart part = parts[i];
                
            part.spinAngle += spinAngleDelta;
            part.worldRotation = 
                mul(parent.worldRotation, mul(part.rotation, quaternion.RotateY(part.spinAngle)));
            part.worldPosition = parent.worldPosition +
                mul(parent.worldRotation , 1.5f * scale * part.direction);
                
            parts[i] = part;
            matrices[i] = float4x4.TRS
                (part.worldPosition, part.worldRotation, float3(scale));
        }
    }

    [SerializeField, Range(1, 9)] int depth = 6;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    static float3[] _directions = {up(), right(), left(), forward(), back()};

    static quaternion[] _rotations =
        {
            quaternion.identity, 
            quaternion.RotateZ(-0.5f * PI), quaternion.RotateZ(0.5f * PI), 
            quaternion.RotateX(0.5f * PI),  quaternion.RotateX(-0.5f * PI),
        };

    struct FractalPart
    {
        public float3 direction, worldPosition;
        public quaternion rotation, worldRotation;
        public float spinAngle;
    }
    
    static readonly int matricesId = Shader.PropertyToID("_Matrices");
    static MaterialPropertyBlock propertyBlock;
    
    NativeArray<FractalPart>[] parts;
    NativeArray<float4x4>[] matrices;
    
    FractalPart CreatePart(int childIndex) => new FractalPart
        { direction = _directions[childIndex], rotation = _rotations[childIndex]};
    
    ComputeBuffer[] matricesBuffers;
    
    void OnEnable()
    {
        parts = new NativeArray<FractalPart>[depth];
        matrices = new NativeArray<float4x4>[depth];
        matricesBuffers = new ComputeBuffer[depth];

        int stride = 16 * 4; 
        for (int i = 0, length = 1; i < parts.Length; i++,  length *= 5)
        {
            parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
            matrices[i] = new NativeArray<float4x4>(length, Allocator.Persistent);
            matricesBuffers[i] = new ComputeBuffer(length, stride);
        }

        parts[0][0] =  CreatePart(0);
        for (int li = 1; li < parts.Length; li++)
        {
            NativeArray<FractalPart> levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi+=5)
                for (int ci = 0; ci < 5; ci++)
                    levelParts[fpi + ci] =  CreatePart(ci);
        }
        propertyBlock ??= new MaterialPropertyBlock();
    }

    void OnDisable()
    {
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            matricesBuffers[i].Release();
            parts[i].Dispose();
            matrices[i].Dispose();
        }
        parts = null;
        matrices = null;
        matricesBuffers = null;
    }

    void OnValidate()
    {
        if (parts != null && enabled)
        {
            OnDisable();
            OnEnable();
        }
    }
    
    void Update()
    {
        float spinAngleDelta = 0.125f * PI * Time.deltaTime;
        FractalPart rootPart = parts[0][0];
        rootPart.spinAngle += spinAngleDelta;
        rootPart.worldRotation = mul(rootPart.rotation ,quaternion.RotateY(rootPart.spinAngle));
        
        parts[0][0] = rootPart;
        matrices[0][0] = float4x4.TRS
            (rootPart.worldPosition, rootPart.worldRotation, float3(1f));

        float scale = 1f;
        JobHandle jHandle = default;
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            
            jHandle = new UpdateFractalLevelJob
            {   
                spinAngleDelta = spinAngleDelta,
                scale = scale,
                parents = parts[li - 1],
                parts = parts[li],
                matrices = matrices[li]
            }.Schedule(parts[li].Length, jHandle);
            // Schedule(iteration count, sequence pattern)
            // 1.innermost FOR loop count 2.sequential dependency between jobs
        }
        // schedule first, invoke and execute after
        jHandle.Complete();

        var bounds = new Bounds(Vector3.zero, 3f * Vector3.one);
        for (int i = 0; i < matricesBuffers.Length; i++)
        {
            ComputeBuffer buffer = matricesBuffers[i];
            buffer.SetData(matrices[i]);
            propertyBlock.SetBuffer(matricesId, buffer);
            Graphics.DrawMeshInstancedProcedural
                (mesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }
}
