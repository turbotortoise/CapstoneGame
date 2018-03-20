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
	private float walkSpeed = 1.5f;
	private string dogSpeech;
	private List<List<string>> dogSpeechList = new List<List<string>>();
	private Main_Player player;
	private float triggerRadius = 2.0f;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("Player").GetComponent<Main_Player> ();
		original_position = this.transform.position;
    	musicScript = musicObject.GetComponent<MusicManager>();
    	dogSpeech = bindata.ToString();
    	dogSpeechList = GameManager.GM.PreparseText(dogSpeech);
	}

	void Pace() {
		if (((int)Time.time % 2) == 0) {
			transform.Translate(transform.forward * walkSpeed * Time.deltaTime);
		}
		else {
			transform.Translate(-1.0f * transform.forward * walkSpeed * Time.deltaTime);
		}
	}

	void FollowPlayer() {
		float playerDist = Vector3.Distance (player.transform.position, this.transform.position);
		transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
		transform.Translate(transform.forward * (walkSpeed + playerDist) * Time.deltaTime);
	}
	

	// Update is called once per frame
	void Update () {
		if (GameManager.GM.walkMode) {
			float playerDist = Vector3.Distance (player.transform.position, this.transform.position);
			if (sentText) sentText = false;
			//Check Player Distance
			if ((player.companionObject == this.gameObject) && (playerDist > triggerRadius)) FollowPlayer();
			else if ((player.companionObject != this.gameObject) && (playerDist > triggerRadius)) Pace();
		}
		else if (GameManager.GM.converseMode) {
			//Check Converse Status
			if ((GameManager.GM.converseObject != null) && (GameManager.GM.converseObject == this.gameObject)) {
				if ((!sentText)) {
				  player.companionObject = this.gameObject;
				  player.ChangeAttributes(walkSpeed, 2.0f);
		          sentText = true;
		          GameManager.GM.isTalking = true;
		          GameManager.GM.receiveText(dogSpeechList[Random.Range(0, dogSpeechList.Count - 1)]);
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
