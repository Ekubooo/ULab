using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField, Range(1, 8)] int depth = 4;

    Fractal FractalChild(Vector3 direction)
    {
        Fractal child = Instantiate(this);
        child.depth = depth - 1;
        child.transform.SetParent(transform, false);
        child.transform.localPosition = direction * 0.75f;
        child.transform.localScale = Vector3.one * 0.5f;
        return child;
    }
    // Start is called before the first frame update
    void Start()
    {
        name = "Fractal" + depth;
        if (depth <= 1) return;
        Fractal A = FractalChild(Vector3.right);
        A.transform.SetParent(transform, false);
        Fractal B = FractalChild(Vector3.up);
        B.transform.SetParent(transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
