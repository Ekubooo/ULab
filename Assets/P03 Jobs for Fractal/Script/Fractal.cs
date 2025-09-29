using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int depth = 4;

    Fractal FractalChild(Vector3 direction, Quaternion rotation)
    {
        Fractal child = Instantiate(this);
        child.depth = depth - 1;
        child.transform.SetParent(transform, false);
        child.transform.localPosition = direction * 0.75f;
        child.transform.localRotation = rotation;
        child.transform.localScale = Vector3.one * 0.5f;
        return child;
    }
    // Start is called before the first frame update
    void Start()
    {
        name = "Fractal" + depth;
        if (depth <= 1) return;
        
        Fractal A = FractalChild(Vector3.up, Quaternion.identity);
        Fractal B = FractalChild(Vector3.right, Quaternion.Euler(0f, 0f, -90f));
        Fractal C = FractalChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
        Fractal D = FractalChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
        Fractal E = FractalChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));
        
        A.transform.SetParent(transform, false);
        B.transform.SetParent(transform, false);
        C.transform.SetParent(transform, false);
        D.transform.SetParent(transform, false);
        E.transform.SetParent(transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 22.5f * Time.deltaTime, 0f);
    }
}
