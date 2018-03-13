using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	protected GameObject structures;
	protected GameObject characters;
	protected BuildingController buildingController;

	public int wood { get; set; }
	public int stone { get; set; }
	public int food { get; set; }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void IncreaseResource(string resource, int amount) {
		if (resource.Contains ("Tree")) {
			wood += amount;
		} else if (resource.Contains ("Stone")) {
			stone += amount;
		} else if (resource.Contains ("Food")) {
			food += amount;
		}
	}
}
