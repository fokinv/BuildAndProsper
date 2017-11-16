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
		button.onClick.AddListener (buttonClicked);
	}

	private void enableButton(Transform tile) {
		tileObject = tile;
	}

	private void disableButton() {
		tileObject = null;
		gameObject.SetActive (false);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		Camera.main.SendMessage ("setIsOverGUI", true);
	}

	public void OnPointerExit(PointerEventData eventData) {
		Camera.main.SendMessage ("setIsOverGUI", false);
	}

	private void buttonClicked() {
		string prefabName = GetComponentInChildren<Text> ().text;
		Camera.main.SendMessage ("handleButtonClick", prefabName);
	}
}
