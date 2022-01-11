using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalStats : MonoBehaviour, IAnimalStateObserver {
	public StatBar hungerBar;
	public StatBar thirstBar;
	public TextMeshProUGUI actionText;	

	void Start() {
		// create canvas if not exists
		if (GetComponent<Canvas>() == null) {
			gameObject.AddComponent<Canvas> ();
			gameObject.AddComponent<CanvasScaler> ();
			gameObject.AddComponent<GraphicRaycaster> ();
		}
		if (GetComponent<Billboard>() == null) {
			gameObject.AddComponent<Billboard> ();
			Billboard billboard = GetComponent<Billboard> ();
			billboard.target = Camera.main.transform;
		}
	}

	void OnValidate() {
		if (hungerBar == null)
			Debug.Log("Hunger bar not set");
		if (thirstBar == null)
			Debug.Log("Thirst bar not set");
		if (actionText == null)
			Debug.Log("Action text not set");
	}

	public void UpdateState(IAnimalStateSubject subject) {
		if (subject == null) return;
		if (subject is Animal) {
			Animal animal = (Animal)subject;
			hungerBar.SetValue(animal.hunger);
			thirstBar.SetValue(animal.thirst);
			actionText.text = animal.currentAction.ToString();
		}
	}
}