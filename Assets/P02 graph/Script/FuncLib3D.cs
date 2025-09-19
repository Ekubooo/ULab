using UnityEngine;
using static UnityEngine.Mathf;
public static class FuncLib3D
{
    public delegate float Func(float x, float z, float t);
    public enum FuncName{Wave, MultiWave, CrossWave, Ripple}

    static Func[] _funcs = {Wave, MultiWave, CrossWave, Ripple};

    public static Func GetFunc(FuncName name)
    {
        return _funcs[(int)name];
    }
    
    // not for object template, though useing static
    public static float Wave(float x, float z, float t)
    {
        return Sin(PI * (x + z + t));
    }
    
    public static float MultiWave(float x, float z, float t)
    {
        float y =  Sin(PI * (x + 0.5f * t));
        y += 0.5f * Sin(2f * PI * (z + t));
        return y * (2f / 3f);
    }

    public static float CrossWave(float x, float z, float t)
    {
        float y =  Sin(PI * (x + 0.5f * t));
        y += 0.5f * Sin(2f * PI * (z + t));
        y += Sin(PI * (x + z + 0.25f * t));
        return y * (1f / 2.5f);
    }
    
    public static float Ripple(float x, float z, float t)
    {
        float d = Sqrt(x * x + z * z);
        float y = Sin(PI * (4f * d - t));
        return y / (1f + 10f * d);
    }
}

