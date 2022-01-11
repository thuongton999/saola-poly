using System;

public interface IAnimalStateObserver
{
    void UpdateState(IAnimalStateSubject subject);
}