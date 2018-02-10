using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionScript : MonoBehaviour {
	public string SceneName;


	// Use this for initialization
	void Start () {
		
	}
	
	void OnTriggerEnter(Collider collider) {
		if (collider.tag == "Player") {
			SceneManager.LoadScene(SceneName, LoadSceneMode.Single);
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
