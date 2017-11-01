using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour {
	private GameObject buttonUI;
	private bool isSelected = false;
	//private bool isCorrectlyPlaced = false;
	//Transform tileObject = null;
	// Use this for initialization
	void Start () {
		buttonUI = gameObject;
		Button button = buttonUI.GetComponent<Button> ();
		button.onClick.AddListener (sendBuildMassage);
	}
	
	// Update is called once per frame
	/*void Update () {
		if (isSelected) {
			moveBuilding ();
			colorBuilding ();
		}
	}*/

	private void sendBuildMassage() {
		MouseControll.isBuilding = true;
		string buildingName = buttonUI.GetComponentInChildren<Text> ().text;
		string path = "Prefabs/Buildings/" + buildingName + "/" + buildingName;
		MouseControll.loadBuilding (path);
	}
}
