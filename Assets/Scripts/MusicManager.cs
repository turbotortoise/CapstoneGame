using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {
	//Audio Sources
	List<AudioSource> AudioSourceList = new List<AudioSource>();
	List<AudioClip> AudioQueue = new List<AudioClip>();
	//for soundtracks
	public AudioSource audioSource_one;
	public AudioSource audioSource_two;
	public AudioSource audioSource_three;
	public AudioSource audioSource_four;
	public AudioSource audioSource_five;
	public AudioSource audioSource_six;
	//for sound effects

	//Add up to 32 audio sources for misc effects
	public AudioSource audioSource_32; //for metronome
	//Add extra audio sources to list
	//List<AudioSource> MiscAudioList = new List<AudioSource>();

	//Audio Clips for Modes, set by objects
	public AudioClip walkMusic;
	public AudioClip converseMusic;
	public AudioClip fightMusic;
	private List<AudioClip> enemy_Music;
	private int enemyProgressIndex = 0;
	public AudioClip transitionMusic; //measure switching between modes, set by environment
	public AudioClip alertMusic; //measure indicating player is discovered
	public AudioClip walkLoop; //measure loop indicating walking
	public AudioClip idleLoop; //measure loop indicating player is idle
	public int walkMusicIndex = -1;
	public int converseMusicIndex = -1;
	public int fightMusicIndex = -1;
	public int walkLoopIndex = -1;
	private int walkMusicTempo = 0;
	//Sound effects
	public AudioClip enemyHitSound;
	public float enemyHitLength = 0.5f;
	public bool enemyHit = false;
	public bool playerHit = false;
	private bool transitionPlayed = false;
	public bool transitionTrigger = false;
	//Tempo
	private float current_tempo = 60; //keep a separate tempo for race conditions
	public float tempo = 60;
	private float prevBeatTime = 0.0f;
	private float nextBeatTime = 1.0f;


	//Interactables
	private float horzMovement;
	private float vertMovement;
	private Main_Player player;


	// Use this for initialization
	void Start () {
		//Add audio sources to quickly asseccible list
		//audioSource_four.loop = false;
		enemy_Music = new List<AudioClip>();
		AudioSourceList.Add(audioSource_one);
		AudioSourceList.Add(audioSource_two);
		AudioSourceList.Add(audioSource_three);
		AudioSourceList.Add(audioSource_four);
		AudioSourceList.Add(audioSource_five);
		AudioSourceList.Add(audioSource_six);
		StartCoroutine(Metronome());
		player = GameObject.Find("Player").GetComponent<Main_Player> ();

		for (int i = 0; i < AudioSourceList.Count; i++) {
			AudioSourceList[i].loop = false; //don't need these in my life
		}

	}

	public int SetAudioSource(AudioClip currentClip) {
		int i;
		for (i = 0; i < AudioSourceList.Count; i++) {
			if (AudioSourceList[i].clip == null) {
				//index = i;
				AudioSourceList[i].clip = currentClip;
				break;
			}
		}
		if (i >= AudioSourceList.Count) {
			print("set audio failed, not enough room");
		}
		return i;
	}

	public int SetLoop(AudioClip walkClip) {
		int i;
		for (i = 0; i < AudioSourceList.Count; i++) {
			if (AudioSourceList[i].clip == null) {
				AudioSourceList[i].clip = walkClip;
				break;
			}
		}
		return i;
	}

	//Either place a function here to find the correct town music or
	//Have another script deal with this
	// - Attach a script to a collider which surrounds entire town

	IEnumerator Metronome() {
		//Metronome keeps track of the current bpm relative to the current time
		if (Time.time > nextBeatTime) {
			//Start the next beat
			prevBeatTime = nextBeatTime;
			nextBeatTime = prevBeatTime + (60.0f / (float)current_tempo); //eighth notes
		}
		if (current_tempo == tempo) {
			//check equality for race conditions

			//Tempo is correct
			if (GameManager.GM.walkMode && (walkMusicIndex >= 0)) {
				if (!AudioSourceList[walkMusicIndex].isPlaying) {
					if (transitionPlayed && (AudioSourceList[walkMusicIndex].clip == transitionMusic)) {
						AudioSourceList[walkMusicIndex].clip = walkMusic;
					}
					else if (transitionPlayed && (AudioSourceList[walkMusicIndex].clip == walkMusic)) {
						transitionPlayed = false;
					}
					transitionPlayed = true;
					AudioSourceList[walkMusicIndex].Play();
				}
			}
			//audioSource_32.Play();
			if (AudioQueue.Count > 0) {
				//assign clips in queue to free audio sources
				//play each audio source
				//make sure audio sources are null after track has finished

				//Delete later, for temp testing
				int removeati = SetAudioSource(AudioQueue[0]);
				AudioSourceList[removeati].Play();
				AudioQueue.RemoveAt(0);
			}


			if (GameManager.GM.walkMode) {
				if (converseMusicIndex >= 0) {
					AudioSourceList[converseMusicIndex].clip = null;
					converseMusicIndex = -1;
				}
				if (walkLoopIndex >= 0) {
					if (tempo != walkMusicTempo) tempo = walkMusicTempo;
					if (((horzMovement > 0.0f) || (horzMovement < 0.0f)) || 
						((vertMovement > 0.0f) || (vertMovement < 0.0f))) {
					//play walking loop
						if (AudioSourceList[walkLoopIndex].clip != walkLoop) {
							AudioSourceList[walkLoopIndex].Stop();
							AudioSourceList[walkLoopIndex].clip = walkLoop;
							AudioSourceList[walkLoopIndex].Play();
						}
						else {
							if (!AudioSourceList[walkLoopIndex].isPlaying)
								AudioSourceList[walkLoopIndex].Play();
						}
					}
					else {
						if (AudioSourceList[walkLoopIndex].clip != idleLoop) {
							AudioSourceList[walkLoopIndex].Stop();
							AudioSourceList[walkLoopIndex].clip = idleLoop;
							AudioSourceList[walkLoopIndex].Play();
						}
						else {
							if (!AudioSourceList[walkLoopIndex].isPlaying)
								AudioSourceList[walkLoopIndex].Play();
						}
					}
				}

			}
			else if (GameManager.GM.converseMode) {
				if (walkMusicIndex >= 0)
					AudioSourceList[walkMusicIndex].Pause();
				if (converseMusicIndex >= 0) {
					if (!AudioSourceList[converseMusicIndex].isPlaying) {
						if (transitionPlayed && (AudioSourceList[converseMusicIndex].clip == transitionMusic)) {
							AudioSourceList[converseMusicIndex].clip = converseMusic;
						}
						transitionPlayed = true;
						AudioSourceList[converseMusicIndex].Play();
					}
				}
			}
			else if (GameManager.GM.fightMode) {
				if ((fightMusicIndex >= 0) && (!AudioSourceList[fightMusicIndex].isPlaying)) {
					AudioSourceList[fightMusicIndex].Play();
				}
				if ((walkMusicIndex >= 0) && (AudioSourceList[walkMusicIndex].isPlaying))
					AudioSourceList[walkMusicIndex].Pause();

				if (GameManager.GM.fightObject.GetComponent<AudioSource>().clip != fightMusic) {
					fightMusic = GameManager.GM.fightObject.GetComponent<AudioSource>().clip;
					StartCoroutine(FadeOut(fightMusicIndex));
					fightMusicIndex = SetAudioSource(GameManager.GM.fightObject.GetComponent<AudioSource>().clip);
					AudioSourceList[fightMusicIndex].Play();
				}
			}
		}
		else {
			//tempo changed, next beat begins new tempo
			print("tempo change");
			current_tempo = tempo;
		}

		float waitTime;
		if (Time.time > nextBeatTime)
			waitTime = 0.0f;
		else
			waitTime = nextBeatTime - Time.time;
		yield return new WaitForSeconds(waitTime);
		StartCoroutine(Metronome());

	} //end of metronome


	//Receiver functions receive soundtracks from interactables
	public void receiveSoundEffect(AudioClip sound) {
		AudioQueue.Add(sound);
	}

	public void receiveWalkLoop(AudioClip wloop, AudioClip iloop) {
		walkLoop = wloop;
		idleLoop = iloop;
		if (walkLoopIndex != -1)
			AudioSourceList[walkLoopIndex].clip = null;
		int i = SetAudioSource(wloop);
		walkLoopIndex = i;
	}

	public void receiveConverseMusic(AudioClip music, int newtempo) {
		tempo = newtempo;
		converseMusic = music;
	}
	public void receiveWalkMusic(AudioClip music, AudioClip transition, int newtempo) {
		//print("audioname: " + music.name);
		if (walkMusicIndex < 0) {
			int i = SetAudioSource(transition);
			walkMusicIndex = i;
			walkMusicTempo = newtempo;
			tempo = newtempo;
			walkMusic = music;
			transitionMusic = transition;
			AudioSourceList[i].Play();

		}
		else if ((walkMusicIndex >= 0) && (music != AudioSourceList[walkMusicIndex].clip)) {
			print("reset walk music");
			int prevIndex = walkMusicIndex;
			int i = SetAudioSource(music);
			walkMusicIndex = i;
			walkMusicTempo = newtempo;
			tempo = newtempo;
			walkMusic = AudioSourceList[walkMusicIndex].clip;
			if ((prevIndex >= 0) && (prevIndex != walkMusicIndex)) {
				StartCoroutine(FadeOut(prevIndex));
			}
		}
	}
	//public void receiveTransitions(AudioClip transition) {
	//	int i = SetAudioSource(transition);
	//	transition
	//}


	public void ChangeMasterVolume(float new_volume) {
		for (int i = 0; i < AudioSourceList.Count; i++) {
			AudioSourceList[i].volume = new_volume;
		}
	}

	public void ResetMasterVolume() {
		for (int i = 0; i < AudioSourceList.Count; i++) {
			AudioSourceList[i].volume = 1.0f;
		}
	}

	//Pause other sound sources for scenes and things
	void PauseOtherSources(AudioSource currentSource) {
		for (int i = 0; i < AudioSourceList.Count; i++) {
			if (AudioSourceList[i] != null) {
				if (AudioSourceList[i] != currentSource)
					AudioSourceList[i].Pause();
			}
		}
	}

	void ClearFinishedSources() {
		for (int i = 0; i < AudioSourceList.Count; i++) {
			if ((i != walkMusicIndex) && (i != converseMusicIndex) && (i != walkLoopIndex) && (i != fightMusicIndex)) {
				if ((AudioSourceList[i].clip != null) && (!AudioSourceList[i].isPlaying))
					AudioSourceList[i].clip = null;
			}
		}
	}

	IEnumerator FadeOut(int audioSourceIndex) {
		if (AudioSourceList[audioSourceIndex].clip != null) {
			if (AudioSourceList[audioSourceIndex].volume > 0.05f)
				AudioSourceList[audioSourceIndex].volume -= (AudioSourceList[audioSourceIndex].volume * Time.deltaTime);
			else {
				AudioSourceList[audioSourceIndex].clip = null;
				AudioSourceList[audioSourceIndex].volume = 1.0f;
			}
		}
		yield return new WaitForSeconds(Time.deltaTime);
		if (AudioSourceList[audioSourceIndex].clip != null) StartCoroutine(FadeOut(audioSourceIndex));
	}

	IEnumerator QuickFadeOut_Indestructive(int audioSourceIndex) {
		if (AudioSourceList[audioSourceIndex].volume > 0.05f) {
			AudioSourceList[audioSourceIndex].volume -= (5.0f * AudioSourceList[audioSourceIndex].volume * Time.deltaTime);
			yield return new WaitForSeconds(Time.deltaTime);
			StartCoroutine(QuickFadeOut_Indestructive(audioSourceIndex));
		}
		else {
			AudioSourceList[audioSourceIndex].volume = 0.0f;
			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
	
	// Update is called once per frame
	void Update () {
		//grab inputs
	    horzMovement = Input.GetAxisRaw("Horizontal");
	    vertMovement = Input.GetAxisRaw("Vertical");

		//grab soundtrack info
		if (current_tempo == 0) {
			if (tempo != 0)
				current_tempo = tempo;
				//StartCoroutine(Metronome());
		}
		if (GameManager.GM.walkMode) {
			//find if we have walking music
			if (walkMusic != null) {
				//have walking music, check if it was set
				if (walkMusicIndex == -1) {
					//not set, set it
					walkMusicIndex = SetAudioSource(transitionMusic);
				}
			}
			//add condition that we switched from fight to walk mode
			if (fightMusic != null) {
				fightMusic = null;
				StartCoroutine(FadeOut(fightMusicIndex));
			}
			if ((converseMusicIndex >= 0) && (converseMusic != null)) {
				converseMusic = null;
				StartCoroutine(FadeOut(converseMusicIndex));
			}
		}
		else if (GameManager.GM.converseMode) {
			if (converseMusic != null) {
				if (converseMusicIndex == -1) {
					converseMusicIndex = SetAudioSource(transitionMusic);
				}
			}
		}
		else if (GameManager.GM.fightMode) {
			//if (player.isHit) {
			//	player.isHit = false;
			//	AudioQueue.Add(player.hitEffect);
			//}

			if (fightMusic != GameManager.GM.fightObject.GetComponent<AudioSource>().clip) {
				fightMusic = GameManager.GM.fightObject.GetComponent<AudioSource>().clip;
				fightMusicIndex = SetAudioSource(fightMusic);
			}

			if ((converseMusic != null) && (converseMusicIndex >= 0)) {
				converseMusic = null;
				StartCoroutine(FadeOut(converseMusicIndex));
			}

		}

		ClearFinishedSources();
		
	}
}//end of script