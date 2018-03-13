using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour {
	public int amountLeft { get; set; }
	protected float minedAmount = 0.0f;
	protected float combinedMiningSpeed = 0.0f;

	protected bool isUnderMining = false;

	protected int maxMiners;
	protected List<Transform> miners = new List<Transform> ();
	protected List<Transform> currentMiners = new List<Transform> ();
	//protected Builder minerScript = null;

	protected void Start () {
		
	}

	protected void Update () {
		
	}

	public void StartMining(Transform miner) {
		if (miners.Count == maxMiners) {
			//miner.SendMessage ("SendToNearestResource");
			return;
		}
		if (!miners.Contains (miner)) { 
			miners.Add (miner);
		}
		currentMiners.Add (miner);
		Builder minerScript = miner.GetComponent<Builder> ();
		combinedMiningSpeed += minerScript.actionSpeed;
		isUnderMining = true;
	}

	public void PauseMining(Transform miner) {
		currentMiners.Remove (miner);
		Builder minerScript = miner.GetComponent<Builder> ();
		combinedMiningSpeed -= minerScript.actionSpeed;
	}

	public void ResumeMining(Transform miner) {
		currentMiners.Add (miner);
		Builder minerScript = miner.GetComponent<Builder> ();
		combinedMiningSpeed += minerScript.actionSpeed;
	}

	protected void Mine() {
		minedAmount += combinedMiningSpeed * Time.deltaTime;
		int amount = (int)System.Math.Floor (minedAmount);
		if (amount > 0) {
			minedAmount -= amount;
			amountLeft -= amount;
			foreach (Transform miner in miners) {
				int resourceToAdd = amount / currentMiners.Count;
				if (currentMiners.Contains (miner)) {
					miner.SendMessage ("IncreaseCarriedResources", resourceToAdd);
				}
			}
		}
	}
}
