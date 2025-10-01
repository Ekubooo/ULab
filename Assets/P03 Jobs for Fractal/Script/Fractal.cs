using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int depth = 4;

    Fractal CreateChild(Vector3 direction, Quaternion rotation)
    {
        Fractal child = Instantiate(this);
        child.depth = depth - 1;
        
        child.transform.localPosition = 0.75f * direction;
        child.transform.localRotation = rotation;
        child.transform.localScale = 0.5f * Vector3.one;
        return child;
    }
    // Start is called before the first frame update
    void Start()
    {
        name = "Fractal" + depth;
        if (depth <= 1) return;
        
        Fractal A = CreateChild(Vector3.up, Quaternion.identity);
        Fractal B = CreateChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
        Fractal C = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
        Fractal D = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
        Fractal E = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));
        
        A.transform.SetParent(transform, false);
        B.transform.SetParent(transform, false);
        C.transform.SetParent(transform, false);
        D.transform.SetParent(transform, false);
        E.transform.SetParent(transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        // transform.Rotate(0f, 22.5f * Time.deltaTime, 0f);
    }
}
