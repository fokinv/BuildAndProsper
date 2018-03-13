using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour {
	private Transform resources;
	private GameObject ui;
	public Player player { get; set; }

	void Start () {
		player = Camera.main.GetComponent (typeof(Player)) as Player;
		ui = GameObject.Find ("UI");
		string path = "Prefabs/UI/Resources";
		Transform resourcesPrefab = Resources.Load <Transform> (path);
		Vector3 pointToPlace = new Vector3 (Screen.width - resourcesPrefab.GetComponent<RectTransform>().sizeDelta.x / 2,
											resourcesPrefab.GetComponent<RectTransform>().sizeDelta.y / 2,
											0.0f);
		resources = Instantiate (resourcesPrefab, pointToPlace, Quaternion.identity) as Transform;
		resources.transform.position = pointToPlace;
		char[] delimiter = { '(' };
		resources.name = resources.name.Split (delimiter) [0];
		resources.SetParent(ui.transform);
	}
	
	void Update () {
		UpdateResourcesAmount ();
	}

	private void UpdateResourcesAmount() {
		foreach (Transform resource in resources) {
			string resourceName = resource.GetComponentInChildren<Text> ().text;
			char[] delimiter = { ':' };
			resourceName = resourceName.Split (delimiter) [0];
			string amount = "";
			if (resourceName == "Wood") {
				amount = resourceName + ": " + player.wood;
			}
			if (resourceName == "Stone") {
				amount = resourceName + ": " + player.stone;
			}
			if (resourceName == "Food") {
				amount = resourceName + ": " + player.food;
			}
			resource.GetComponentInChildren<Text> ().text = amount;
		}
	}
}
