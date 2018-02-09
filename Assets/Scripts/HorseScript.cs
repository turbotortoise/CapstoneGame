using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseScript : MonoBehaviour {
	//sounds for movement
	public AudioSource audioSource_one;
	public AudioSource audioSource_two;
	public AudioClip gallopLoop;
	public AudioClip ascendingLoop;
	public AudioClip descendingLoop;
	public AudioClip curTrack;
	//variables for movement
	private Main_Player player;
	public float walkSpeed;
	public float triggerRadius = 3.5f;
	private Vector3 prevPos;

	// Use this for initialization
	void Start () {
		prevPos = transform.position;
		player = GameObject.Find("Player").GetComponent<Main_Player> ();
		audioSource_one.clip = gallopLoop;
		audioSource_one.Play();
	}

	void TransitionTrack() {
		if (audioSource_one.clip != curTrack) {
			if (audioSource_two.clip != curTrack) {
				audioSource_two.clip = curTrack;
				audioSource_two.Play();
				audioSource_one.Stop();
				audioSource_one.clip = null;
			}
		}
		else {
			if (!audioSource_one.isPlaying) {
				audioSource_one.Play();
			}
		}

		if (audioSource_two.clip != curTrack) {
			if (audioSource_one.clip != curTrack) {
				audioSource_one.clip = curTrack;
				audioSource_one.Play();
				audioSource_two.Stop();
				audioSource_two.clip = null;
			}
		}
		else {
			if (!audioSource_two.isPlaying) {
				audioSource_two.Play();
			}
		}
	}

	public void Metronome() {
		if (transform.position.y > prevPos.y) {
			curTrack = ascendingLoop;
			TransitionTrack();
		}
		else if (transform.position.y == prevPos.y) {
			curTrack = gallopLoop;
			TransitionTrack();
		}
		else {
			curTrack = descendingLoop;
			TransitionTrack();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (GameManager.GM.walkMode && (player.companionObject != null) && (player.companionObject == this.gameObject)) {
			//print(this.gameObject + ": companion object");
			float playerDist = Vector3.Distance (player.transform.position, this.transform.position);
			if (playerDist > triggerRadius) {
				transform.LookAt(player.transform.position);
				this.GetComponent<Rigidbody>().MovePosition(transform.position + transform.forward * 
					(walkSpeed * (playerDist - triggerRadius)) * Time.deltaTime);
			}
		}

		if (player.companionObject != this.gameObject) {
			curTrack = gallopLoop;
			TransitionTrack();
		}
	}
}
