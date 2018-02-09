using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meche_TreeScript : MonoBehaviour {
	public Color color;
	public string material_name;
	public bool isStable;
	private int enemy_health = 5;
	private int priority = 3;
	public float triggerRadius;
	private float attackRadius;

  	[SerializeField]Transform player;
  	private float playerDist;
  	public GameObject musicObject;
  	private MusicManager musicScript;

  	//Audio Clips
  	public AudioClip alertMusic;
  	public AudioClip fightMusic_1;
  	public AudioClip fightMusic_2;
  	public AudioClip fightMusic_3;
  	public AudioClip transition_1;
  	public AudioClip transition_2;
  	public AudioClip defeatMusic;
  	public int tree_tempo;

	// Use this for initialization
	void Start () {
		attackRadius = triggerRadius / 2.0f;
    	musicScript = musicObject.GetComponent<MusicManager>();
	    //set color (for not white everything)
	    Renderer rend = GetComponent<Renderer>();
	    rend.material.shader = Shader.Find(material_name);
	    rend.material.SetColor("_SpecColor", color);
	}  

	void FollowPlayer() {
    	transform.LookAt(new Vector3(player.position.x, this.transform.position.y, player.position.z));
  	}  

  	void OnCollisionEnter(Collision collider) {
	    if (collider.gameObject.tag == "Player") {
	      if (!isStable) {
	        enemy_health --;
	        musicScript.transitionTrigger = true;
		      if (enemy_health == 0) {
		        isStable = true;
		        this.tag = "Friend";
		        GameManager.GM.Switch("Walk");
		      }
	      }
	    }
	}

	void Attack() {
		//smack a branch or something
	}
	
	// Update is called once per frame
	void Update () {
    	playerDist = Vector3.Distance (player.position, this.transform.position);
		if (isStable) {

		}
		else {
			//Enemy
		      if (playerDist <= triggerRadius) {
		        if (GameManager.GM.fightMode) {
		          if (playerDist >= attackRadius) {
		            Attack();
		          }
		          else {
		            FollowPlayer();
		          }
		        }
		        else {
		          print("player within radius, FIGHT");
		          //Switch to fight mode
		          if (GameManager.GM.fightObject == null)
		            GameManager.GM.fightObject = this.gameObject;
		          else {
		            //priority
		            GameManager.GM.PriorityFightObject(this.gameObject, priority); //set this to fight object if there is higher priority
		          }
		          GameManager.GM.Switch("Fight");
		          musicScript.receiveFightMusic(alertMusic, fightMusic_1, fightMusic_2, fightMusic_3,
		                                        transition_1, transition_2, defeatMusic, tree_tempo);
		        }
		      }
		      else {
		        if (GameManager.GM.fightMode) {
		          if (GameManager.GM.fightObject == this.gameObject) {
		            if (playerDist > triggerRadius) {
			            GameManager.GM.fightObject = null;
			            StartCoroutine(GameManager.GM.CheckForUpdates());
			        }
		          }
		        }
		      }
		}
		
	}
}
