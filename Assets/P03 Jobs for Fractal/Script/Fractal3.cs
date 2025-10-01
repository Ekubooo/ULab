using System;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

public class Fractal3 : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int depth = 4;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    
    static readonly int matrixsID = Shader.PropertyToID("_Matrixs");
    static MaterialPropertyBlock propertyBlock;
    static Vector3[] _directions = 
        {Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back};

    static Quaternion[] _rotations =
        {
            Quaternion.identity, 
            Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f), 
            Quaternion.Euler(90f, 0f, 0f),  Quaternion.Euler(-90f, 0f, 0f),
        };

    struct FractalPart
    {
        public Vector3 direction, worldPosition;
        public Quaternion rotation,  worldRotation;
        public float spinAngle;
    }
    FractalPart CreatePart(int childIndex) => new FractalPart
        { direction = _directions[childIndex], rotation = _rotations[childIndex]};
    
    FractalPart[][] parts;
    Matrix4x4[][] matrixs;

    ComputeBuffer[] MatrixBuffers;
    
    void OnEnable()
    {
        parts = new FractalPart[depth][];
        matrixs = new Matrix4x4[depth][];
        MatrixBuffers = new ComputeBuffer[depth];
        int stride = 16 * 4;
        for (int i = 0, length = 1; i < parts.Length; i++,  length *= 5)
        {
            parts[i] = new FractalPart[length]; 
            matrixs[i] = new Matrix4x4[length];
            MatrixBuffers[i] = new ComputeBuffer(length, stride);
        }

        parts[0][0] =  CreatePart(0);
        for (int li = 1; li < parts.Length; li++)
        {
            FractalPart[] levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi+=5)
            for (int ci = 0; ci < 5; ci++)
                levelParts[fpi + ci] =  CreatePart(ci);
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < MatrixBuffers.Length; i++)
        {
            MatrixBuffers[i].Release();
            parts = null;
            matrixs = null;
            MatrixBuffers = null;
            propertyBlock ??= new MaterialPropertyBlock();
        }
    }

    void OnValidate()
    {   // hot reloads change supported
        if (parts!= null&enabled)
        {
            OnDisable();
            OnEnable();
        }
    }

    void Update()
    {
        // Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
        float spinAngleDelta = 22.5f * Time.deltaTime;
        FractalPart rootPart = parts[0][0];
        rootPart.spinAngle += spinAngleDelta;
        rootPart.worldRotation = transform.rotation * 
            (rootPart.rotation * Quaternion.Euler(0f, rootPart.spinAngle, 0f));
        rootPart.worldPosition = transform.position;
        
        float objScale = transform.lossyScale.x;
        parts[0][0] = rootPart;
        matrixs[0][0] = Matrix4x4.TRS
            (rootPart.worldPosition, rootPart.worldRotation, objScale * Vector3.one);

        float scale = objScale;
        for (int li = 1; li < parts.Length; li++)
        {
            scale *= 0.5f;
            FractalPart[] parentParts = parts[li - 1];
            FractalPart[] levelParts  = parts[li];
            Matrix4x4[] levelMatrixs = matrixs[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                FractalPart parent = parentParts[fpi / 5];
                FractalPart part = levelParts[fpi];

                part.spinAngle += spinAngleDelta;
                // careful with the sequence of the Quaternion:
                // Q1 * Q1 means Q2 Rotation first and Q1 after.
                // child rotation first and parent after.
                part.worldRotation = parent.worldRotation 
                    * (part.rotation * Quaternion.Euler(0f, part.spinAngle, 0f));
                part.worldPosition = parent.worldPosition + 
                    parent.worldRotation * (1.25f * scale * part.direction);
                levelParts[fpi] = part;
                levelMatrixs[fpi] = Matrix4x4.TRS
                    (rootPart.worldPosition, rootPart.worldRotation, scale * Vector3.one);
            }
        }
        var bounds = new Bounds(rootPart.worldPosition, 3f * objScale * Vector3.one);
        for (int i = 0; i < MatrixBuffers.Length; i++)
        {
            ComputeBuffer buffer = MatrixBuffers[i];
            buffer.SetData(matrixs[i]);
            propertyBlock.SetBuffer(matrixsID, buffer);
            Graphics.DrawMeshInstancedProcedural
                (mesh, 0, material, bounds, buffer.count, propertyBlock);
        }
    }
}
