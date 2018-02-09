using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogScript : MonoBehaviour {
	
	//Audio
    public GameObject musicObject;
    private MusicManager musicScript;
	private AudioSource audioSource; //only one clip
	public AudioClip audioClip;

	//Text
	public TextAsset bindata;

	public int dog_tempo;
	private Vector3 original_position;
	private bool sentText = false;
	private float walkSpeed = 2.0f;
	private string dogSpeech;// = "hi how you doin?!";
	private Main_Player player;
	public float triggerRadius = 5.0f;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Main_Player> ();
		original_position = this.transform.position;
    	musicScript = musicObject.GetComponent<MusicManager>();
    	dogSpeech = bindata.ToString();
	}

	void Pace() {
		if (((int)Time.time % 2) == 0) {
			transform.Translate(transform.forward * walkSpeed * Time.deltaTime);
		}
		else {
			transform.Translate(-1.0f * transform.forward * walkSpeed * Time.deltaTime);
		}
	}
	

	// Update is called once per frame
	void Update () {
		if (GameManager.GM.walkMode) {
			if (!sentText) sentText = false;
			//Check Player Distance
			if (Vector3.Distance(player.transform.position, transform.position) <= triggerRadius) {
				transform.LookAt(player.transform.position);
			}
			else {
				Pace();
			}

		}
		else if (GameManager.GM.converseMode) {
			//Check Converse Status
			if ((GameManager.GM.converseObject != null) && (GameManager.GM.converseObject == this.gameObject)) {
				if ((!sentText)) {
		          sentText = true;
		          GameManager.GM.isTalking = true;
		          GameManager.GM.receiveText(dogSpeech);
		          musicScript.receiveConverseMusic(audioClip, dog_tempo);
		        }
			}

		}
		else if (GameManager.GM.fightMode) {
			//Check Position Relative to Original Position
			if ((Vector3.Distance (transform.position, original_position)) > triggerRadius) {
				transform.Translate(original_position);
			}

		}
	}
}
