using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour {

	public void LoadSceneAt(int index) {
		int xSize = int.Parse(GameObject.Find ("XSizeText").GetComponent<Text>().text);
		int ySize = int.Parse(GameObject.Find ("YSizeText").GetComponent<Text>().text);
		Map.InitMapSize (xSize, ySize);
		Map.numberOfForests = int.Parse(GameObject.Find ("ForestNumber").GetComponent<Text>().text);
		Map.numberOfStones = int.Parse(GameObject.Find ("StoneNumber").GetComponent<Text>().text);
		UnityEngine.SceneManagement.SceneManager.LoadScene (index);
	}
}
