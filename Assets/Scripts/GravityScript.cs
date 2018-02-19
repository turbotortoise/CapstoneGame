using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	void OnCollisionEnter(Collision collider) {
	    if (collider.gameObject.tag == "Player") {
	    	collider.gameObject.GetComponent<Main_Player>().isOnGround = true;
		}
 	}

	// Update is called once per frame
	void Update () {
		
	}
}
