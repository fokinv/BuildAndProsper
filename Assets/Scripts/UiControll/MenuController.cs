using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void NothingSelected() {
		foreach (Transform child in transform) {
			foreach (Transform grandChild in child) {
				if (grandChild.gameObject.activeInHierarchy) {
					grandChild.SendMessage ("DisableButton");
				}
			}
		}
	}

	private void Selected(Transform selected) {
		Transform menu = transform.Find (selected.name);
		Vector3 position = new Vector3(32.0f, 32.0f, 0.0f);
		ActivateMenu (menu, selected, position);
		position = new Vector3(32.0f, 96.0f, 0.0f);
		bool menuChanged = false;
		if (MouseControll.availableCharacters.Contains(selected.name)) {
			menu = transform.Find ("CharactersCommon");
			menuChanged = true;
		} else if (MouseControll.availableStructures.Contains(selected.name)) {
			menu = transform.Find ("StructuresCommon");
			menuChanged = true;
		}

		if (menuChanged) {
			ActivateMenu (menu, selected, position);
		}
	}

	private void ActivateMenu(Transform menu, Transform tile, Vector3 position) {
		foreach (Transform child in menu) {
			if (!child.gameObject.activeInHierarchy) {
				child.gameObject.SetActive(true);
			}
			child.SendMessage("EnableButton", tile);
			child.GetComponent<RectTransform>().position = position;
			position.x += child.GetComponent<RectTransform>().sizeDelta.x;
		}
	}
}
