using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private Transform tileObject = null;

	void Start () {
		Button button = GetComponent<Button> ();
		gameObject.SetActive (false);
		button.onClick.AddListener (ButtonClicked);
	}

	private void EnableButton(Transform tile) {
		tileObject = tile;
	}

	private void DisableButton() {
		tileObject = null;
		gameObject.SetActive (false);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		Camera.main.SendMessage ("SetIsOverGUI", true);
	}

	public void OnPointerExit(PointerEventData eventData) {
		Camera.main.SendMessage ("SetIsOverGUI", false);
	}

	private void ButtonClicked() {
		string prefabName = GetComponentInChildren<Text> ().text;
		Camera.main.SendMessage ("HandleButtonClick", prefabName);
	}
}
