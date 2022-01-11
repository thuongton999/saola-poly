using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : Animal {
    public override void Eat()
    {
        float eatAmount = Mathf.Min (hunger, Time.deltaTime * 1 / eatDuration);
        foodTarget.KilledBy(this);
        hunger -= eatAmount;
    }
}