using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class xsize : MonoBehaviour {

	public void SetSizeText(float value) {
		Text text = GetComponent<Text>();
		text.text = "" + value;
	}
}
