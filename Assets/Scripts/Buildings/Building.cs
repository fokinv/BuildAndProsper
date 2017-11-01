using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Building {
	int healthPoint { get; set; }
	bool isSelected {get; set; }
	int timeRequirement { get; set; }

	void placeBuilding ();
	//TODO: resources
}
