using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScript : MonoBehaviour {
	private int stage; //state of the environment, ranges from 1 to 7
	private List<GameObject> worldEnemies = new List<GameObject>();
	public int numEnemies;
	public int tempo = 60;
	public AudioClip walkLoop; //loops every measure when walking
	public AudioClip idleLoop; //loops every measure when idle
	public AudioClip soundStage1; //most aggresive soundtrack
	public AudioClip soundStage2;
	public AudioClip soundStage3;
	public AudioClip soundStage4;
	public AudioClip soundStage5;
	public AudioClip soundStage6;
	public AudioClip finalStage; //least aggresive soundtrack
	public AudioClip environmentMusic; //soundtrack for area, will have multiple for stages
	public AudioClip transitionMusic; //quick track for transitioning

	//Environment qualities
	public float gravity;
	public float acceleration;
	public float ground_hardiness;
	public Vector3 PlayerSpawnPoint;

	//public GameObject reverbReaction;

    public GameObject musicObject;
    private MusicManager musicScript;

	// Use this for initialization
	void Start () {
    	musicScript = musicObject.GetComponent<MusicManager>();
    	environmentMusic = soundStage1;
	}

	void SetGravityAttributes(Main_Player player) {
		//changes how the collider moves in this space.
		player.ChangeAttributes(gravity, acceleration);
		//player.isOnGround = true;

	}

	void OnTriggerEnter(Collider collider) {
	    if (collider.gameObject.tag == "Player") {
	    	print("collided with player");
	    	if ((GameManager.GM.environmentObject != this.gameObject) || (musicScript.walkMusic == null)) {
		    	GameManager.GM.environmentObject = this.gameObject;
		    	musicScript.receiveWalkMusic(environmentMusic, transitionMusic, tempo);
		    	musicScript.receiveWalkLoop(walkLoop, idleLoop);
		    	SetGravityAttributes(collider.gameObject.GetComponent<Main_Player>());
	    	}
		}
		else if (collider.gameObject.tag == "Enemy") {
			worldEnemies.Add(collider.gameObject); //should add all 5 spiders
			numEnemies++;
		}
 	}

 	bool CheckStage() {
 		//print("environment: " + this.gameObject.name + "stage: " + stage + "\nenemies: " + worldEnemies.Count);
 		int prevCount = worldEnemies.Count;
 		int removeAt = -1;
 		for (int i = 0; i < worldEnemies.Count; i++) {
 			if ((worldEnemies[i] == null) || (worldEnemies[i].tag == "Friend"))
 				removeAt = i;
 		}
 		if (removeAt >= 0) {
 			worldEnemies.RemoveAt(removeAt);
 			removeAt = -1;
 		}
 		return (!(prevCount == worldEnemies.Count));
 	}

 	void CheckEnemies() {
		AudioClip prevEnvMusic = environmentMusic;
		if (numEnemies == 0) {
			if (environmentMusic != finalStage)
				environmentMusic = finalStage;
		}
		else {
			stage = 7 - worldEnemies.Count;
			if (stage == 0) {
				if (environmentMusic != soundStage1)
					environmentMusic = soundStage1;
			}
			else if (stage == 1) {
				if (environmentMusic != soundStage2)
					environmentMusic = soundStage2;

			}
			else if (stage == 2) {
				if (environmentMusic != soundStage3)
					environmentMusic = soundStage3;
			}
			else if (stage == 3) {
				if (environmentMusic != soundStage4)
					environmentMusic = soundStage4;

			}
			else if (stage == 4) {
				if (environmentMusic != soundStage5)
					environmentMusic = soundStage5;

			}
			else if (stage == 5) {
				if (environmentMusic != soundStage6)
					environmentMusic = soundStage6;

			}
		}

		if ((environmentMusic != null) && ((environmentMusic != prevEnvMusic) || 
			((this.gameObject == GameManager.GM.environmentObject) && (musicScript.walkMusic != environmentMusic)))) {
			musicScript.receiveWalkMusic(environmentMusic, transitionMusic, tempo);
			musicScript.receiveWalkLoop(walkLoop, idleLoop);
		}

 	}
	
	// Update is called once per frame
	void Update () {
		if (CheckStage())
			CheckEnemies();
	}
}
