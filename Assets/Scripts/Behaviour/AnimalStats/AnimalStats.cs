using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalStats : MonoBehaviour, IAnimalStateObserver {
	public StatBar hungerBar;
	public StatBar thirstBar;
	public StatBar reproductiveUrgeBar;
	public TextMeshProUGUI actionText;	

	void OnValidate() {
		if (hungerBar == null)
			Debug.Log("Hunger bar not set");
		if (thirstBar == null)
			Debug.Log("Thirst bar not set");
		if (reproductiveUrgeBar == null)
			Debug.Log("Reproductive urge bar not set");
		if (actionText == null)
			Debug.Log("Action text not set");
		if (GetComponent<Canvas>() == null) {
			gameObject.AddComponent<Canvas> ();
			gameObject.AddComponent<CanvasScaler> ();
			gameObject.AddComponent<GraphicRaycaster> ();
		}
		if (GetComponent<Canvas>().worldCamera == null)
			GetComponent<Canvas>().worldCamera = Camera.main;
		if (GetComponent<Billboard>() == null)
			gameObject.AddComponent<Billboard> ();
	}

	public void UpdateState(IAnimalStateSubject subject) {
		if (subject == null) return;
		if (subject is Animal) {
			Animal animal = (Animal)subject;
			hungerBar.SetValue(animal.hunger);
			thirstBar.SetValue(animal.thirst);
			reproductiveUrgeBar.SetValue(animal.reproductiveUrge);
			actionText.text = animal.currentAction.ToString();
		}
	}
}