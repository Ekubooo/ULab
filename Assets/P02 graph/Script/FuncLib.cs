using UnityEngine;
using static UnityEngine.Mathf;
public static class FuncLib
{
    public delegate float Func(float x, float t);
    public enum FuncName{Wave, MultiWave, Ripple}

    static Func[] _funcs = {Wave, MultiWave, Ripple};

    public static Func GetFunc2(FuncName name)
    {
        return _funcs[(int)name];
    }

    public static Func GetFunc(int index)
    {
        switch (index)
        {
            case 0 :
                return Wave;
                break;
            case 1 :
                return MultiWave;
                break;
            case 2 :
                return Ripple;
                break;
            default:
                break;
        }
        return null;
    }
    
    // not for object template, though useing static
    public static float Wave(float x, float t)
    {
        return Sin(PI * (x + t));
    }
    
    public static float MultiWave(float x, float t)
    {
        float y =  Sin(PI * (x + 0.5f * t));
        y += 0.5f * Sin(2f * PI * (x + t));
        return y * (2f / 3f);
    }

    public static float Ripple(float x, float t)
    {
        float d = Abs(x);
        float y = Sin(PI * (4f * d - t));
        return y / (1f + 10f * d);
    }
}

