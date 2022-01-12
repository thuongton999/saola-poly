using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaoLa : Animal {
    public AnimalStats SaoLaStats;

    public override void Init(Coord coord)
    {
        base.Init(coord);
        RegisterObserver (SaoLaStats);
    }

    public override void Init(Coord coord, Genes genes)
    {
        base.Init(coord, genes);
        RegisterObserver (SaoLaStats);
    }
}