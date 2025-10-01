using Unity.VisualScripting;
using UnityEngine;

public class Fractal2 : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int depth = 4;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    static Vector3[] _directions = 
        {Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back };

    static Quaternion[] _rotations =
        {
            Quaternion.identity, 
            Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f), 
            Quaternion.Euler(90f, 0f, 0f),  Quaternion.Euler(-90f, 0f, 0f),
        };

    struct FractalPart
    {
        public Vector3 direction;
        public Quaternion rotation;
        public Transform transform;
    }

    FractalPart[][] parts;
    
    FractalPart CreatePart(int levelIndex, int childIndex, float fScale)
    {
        var GO =  new GameObject("Fractal Part" + levelIndex + "C" + childIndex);
        GO.transform.localScale = fScale * Vector3.one;
        GO.transform.SetParent(transform, false);
        
        GO.AddComponent<MeshFilter>().mesh = mesh;
        GO.AddComponent<MeshRenderer>().material = material;
        return new FractalPart
        {
            direction = _directions[childIndex],
            rotation = _rotations[childIndex],
            transform = GO.transform
        };
    }
    void Awake()
    {
        parts = new FractalPart[depth][];
        for (int i = 0, length = 1; i < parts.Length; i++,  length *= 5)
        {
            parts[i] = new FractalPart[length]; 
        }

        float scales = 1f;
        parts[0][0] =  CreatePart(0,0, scales);
        for (int li = 1; li < parts.Length; li++)
        {
            scales *= 0.5f;
            FractalPart[] levelParts = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi+=5)
                for (int ci = 0; ci < 5; ci++)
                    levelParts[fpi + ci] =  CreatePart(li,ci, scales);
        }
    }

    void Update()
    {
        Quaternion deltaRotation = Quaternion.Euler(0f, 22.5f * Time.deltaTime, 0f);
        FractalPart rootPart = parts[0][0];
        rootPart.rotation *= deltaRotation;
        rootPart.transform.localRotation = rootPart.rotation;
        parts[0][0] = rootPart;
        
        for (int li = 1; li < parts.Length; li++)
        {
            FractalPart[] parentParts = parts[li - 1];
            FractalPart[] levelParts  = parts[li];
            for (int fpi = 0; fpi < levelParts.Length; fpi++)
            {
                Transform parentTransform = parentParts[fpi / 5].transform;
                FractalPart part = levelParts[fpi];
                
                part.rotation *= deltaRotation;
                // careful with the sequence of the Quaternion:
                // Q1 * Q1 means Q2 Rotation first and Q1 after.
                // child rotation first and parent after.
                part.transform.localRotation = 
                    parentTransform.localRotation *  part.rotation;
                part.transform.localPosition =
                    parentTransform.localPosition +
                    parentTransform.localRotation *
                    (1.25f * part.transform.localScale.x * part.direction);
                levelParts[fpi] = part;
            }
        }
    }
}
