using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummySurface : ISurface
{
    public float HeightAt(Vector2 xy)
    {
        return 0;
    }

    public float SpeedReductionCoefAt(Vector2 xy)
    {
        return 0.5F;
    }
}
