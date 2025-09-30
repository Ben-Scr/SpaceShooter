using System;


[Serializable]
public struct Range
{
    public float Min;
    public float Max;

    public Range(float min, float max)
    {
        Min = min;
        Max = max;
    }
}

