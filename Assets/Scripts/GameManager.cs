using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null; 
	private static GameManager _GM;

	public static GameManager GM {
		get { return _GM; }
	}

	//Modes
	public bool walkMode;
	public bool converseMode;
	public bool fightMode;

	//WalkMode variables

	//TalkMode variables
	public GameObject converseObject;
	public GameObject fightObject;
	public GameObject environmentObject;
	private float fightObjectPriority = 0;

	public bool isTalking = false;
	//text
	public Text displayText;
	private float spamDelay = 0.5f;
	public List<string> dialogueList = new List<string>();

	//FightMode variables

	private Main_Player player;

	void Awake () {
		_GM = this;
	}

	void Start () {
		walkMode = true;
		converseMode = false;
		fightMode = false;
		player = GameObject.Find("Player").GetComponent<Main_Player> ();
		displayText.text = "";
	}

	//Coroutines

	IEnumerator TriggerSwitch(string switch_text) {
		//print("Trigger Switch to: " + switch_text);
		player.controlLock = true;
		yield return new WaitForSeconds(spamDelay);
		Switch(switch_text);
	}

	IEnumerator ReleaseLock() {
		yield return new WaitForSeconds(spamDelay);
		player.controlLock = false;
	}

	public IEnumerator CheckForUpdates() {
		if (converseMode) {
			player.controlLock = true;
			//print("checking if isTalking updated at: " + Time.time);
			yield return new WaitForSeconds(1.0f);
			if (!isTalking) {
					//print("nothing found, return to walkmode at: " + Time.time);
					TriggerSwitch("Walk");
				}
		}
		else if (fightMode) {
			//print("fight Object; " + fightObject);
			if (fightObject == null)
				Switch("Walk");
		}
		StartCoroutine(ReleaseLock());
	}

	//private functions
	void DisplayText(string cur_text) {
		displayText.text = cur_text;
	}

	public List<List<string>> PreparseText (string speech) {
		string section = "";
		List<string> sections = new List<string>();
		List<List<string>> listSections = new List<List<string>>();

		foreach (char c in speech) {
			print("char: " + c);
			if (c.Equals("-")) {
				sections.Add(section);
				section = "";
			}
			else {
				section += c.ToString();
			}
		}
		return listSections;
	}
	/*void SegmentString(string text) {
		//called by other scripts. Will parse string into string lists
		//List <string> temp = new List <string> ();
		dialogueList.Add(text);
	}*/

	//public functions
	
	public void Switch (string mode) {
		if (mode == "Walk") {
			walkMode = true;
			converseMode = false;
			fightMode = false;
			fightObjectPriority = 0;
			player.controlLock = false;
		}
		else if (mode == "Talk") {
			walkMode = false;
			converseMode = true;
			fightMode = false;
			fightObjectPriority = 0;
			player.controlLock = true;
			StartCoroutine(ReleaseLock());
		}
		else if (mode == "Fight") {
			walkMode = false;
			converseMode = false;
			fightMode = true;
			player.controlLock = false;
		}
		//player.controlLock = false;
	}

	public void PriorityFightObject(GameObject enemy, int priority) {
		/*Priority based on difficulty of opponent
		  1 - Small
		  2 - Medium
		  3 - Large
		  4 - Boss
		*/
		if (priority > fightObjectPriority) {
			fightObject = enemy;
			fightObjectPriority = priority;
		}
	}


	public void receiveText(string text) {
		//SegmentString(text);
		dialogueList.Add(text);
		textWaitForInput();
	}

	public void textWaitForInput() {
		/*isTalking = true;
		if (speechList.Count > 0) {
			int i = 0;
			while (i < speechList.Count) {
				if (waitForRelease || (((player.AButton > 0) || (player.AButton < 0)) && (!player.controlLock))) {
					//if ((AisPressed) && (player.AButton == 0)) {
					//	AisPressed = false;
					waitForRelease = true;
					if (waitForRelease && (player.AButton == 0)) {
						waitForRelease = false;
						player.controlLock = true;
						StartCoroutine(ReleaseLock());
						DisplayText(speechList[i]);
						i++;
					}
				}
			}
		}
		converseObject = null;
		player.controlLock = true;
		StartCoroutine(TriggerSwitch("Walk"));
		isTalking = false;
		displayText.text = "";*/
		if (dialogueList.Count == 0) {
			//print("dialogue is blank, switching to walk");
			player.controlLock = true;
			StartCoroutine(TriggerSwitch("Walk"));
			displayText.text = "";
		} else {
			//print("showing dialogue");
			player.controlLock = true;
			DisplayText(dialogueList[0]);
			//mutate the list
			dialogueList.RemoveAt(0);
		}

	}

	// Update is called once per frame
	void Update () {
		if (walkMode) {
			if (player.controlLock)
				StartCoroutine(ReleaseLock());
		}
		else if (converseMode) {
			if (converseObject == player.gameObject) {
				converseObject = null;
				Switch("Walk");
			}
			if (!isTalking)
				StartCoroutine(CheckForUpdates());
		}
		else if (fightMode) {
			if (fightObject == null){
				StartCoroutine(CheckForUpdates());
			}
			if (fightObject == player.gameObject) {
				fightObject = null;
				Switch("Walk");
			}
		}
	}


} //end of script
