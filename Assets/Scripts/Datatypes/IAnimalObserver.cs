using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAnimalObserver
{
    void OnAnimalDeath(Animal animal);
}