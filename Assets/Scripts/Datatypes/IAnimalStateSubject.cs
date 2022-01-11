using System;

public interface IAnimalStateSubject {
    void RegisterObserver(IAnimalStateObserver observer);
    void RemoveObserver(IAnimalStateObserver observer);
    void NotifyObservers();
}