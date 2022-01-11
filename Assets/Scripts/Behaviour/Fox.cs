using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Animal {
    AnimalStats foxStats;

    public override void Init(Coord coord)
    {
        base.Init(coord);
        RegisterObserver (foxStats);
    }

    public override void Eat()
    {
        float eatAmount = Mathf.Min (hunger, Time.deltaTime * 1 / eatDuration);
        foodTarget.KilledBy(this);
        hunger -= eatAmount;
    }
}