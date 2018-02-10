/* Spider Class
 * Contains both Peaceful and Aggresive scripts for the spider people.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LizardScript : MonoBehaviour {
  public Color color;
  public Color reverbColor;
  public string material_name;
  //3D settings
  private Rigidbody rb;
  private Vector3 startPos;
  public bool isStable;
  public float triggerRadius;
  public float lizard_speed;
  public float attack_speed;

  [SerializeField]Transform player;
  public GameObject musicObject;
  private MusicManager musicScript;
  //private Main_Player playerScript;
  private float playerDist;

  //Civilian Settings
  //private float walkSpeed = 1.0f;
  //private float rotateSpeed = 1.0f;
  //Text
  public TextAsset bindata;
  private bool sentText = false;
  public List<List<string>> lizardTextList = new List<List<string>>();
  private bool onGround = false;
  private string lizardText = "";
  private int interactivity = 0; //the number of times talked to player

  //Enemy settings
  private float attackRadius = 10.0f;
  private int enemy_health = 3;
  public AudioClip alertMusic; //clip triggered when player is discovered
  public AudioClip fightMusic_1; //stage 1 soundtrack
  public AudioClip fightMusic_2; //stage 2 soundtrack
  public AudioClip fightMusic_3; //stage 3 soundtrack
  public AudioClip transition_1; //transition to second soundtrack
  public AudioClip transition_2; //transition to third soundtrack
  public AudioClip defeatMusic; //clip triggered when health reaches 0
  public int lizard_tempo;
  //Type of attack varies on species
  public AttackDestroyScript groundAttack;
  //priority varies on species
  public int priority;
  private bool isAttacking = false;
  //private float attackPower;

  //Audio settings
  public AudioClip talkMusic;

  // Use this for initialization
  void Start () {
    startPos= this.transform.position;
    rb = GetComponent<Rigidbody>();
    musicScript = musicObject.GetComponent<MusicManager>();
    //playerScript = GetComponent<Main_Player>();
    if (isStable)
      triggerRadius = 5.0f;

    //set color (for not white everything)
    Renderer rend = GetComponent<Renderer>();
    rend.material.shader = Shader.Find(material_name);
    rend.material.SetColor("_Color", color);

    //grab text
    //bindata = Resources.Load("DogText") as TextAsset;
    lizardText = bindata.ToString();
    lizardTextList = GameManager.GM.PreparseText(lizardText);
  }

  void FollowPlayer() {
    transform.LookAt(new Vector3(player.position.x, this.transform.position.y, player.position.z));
  }


  void Pace() {
  }

  void Roaming() {
    transform.Translate(new Vector3(Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f), Random.Range(-0.02f, 0.02f)));
  }

  void Attack() {
    transform.LookAt(new Vector3(player.position.x, this.transform.position.y, player.position.z));
  }

  void OnCollisionEnter(Collision collider) {
    if (collider.gameObject.tag == "Ground") {
      AttackDestroyScript clone;
      onGround = true;
      if (isAttacking) isAttacking = false;
      clone = (AttackDestroyScript)Instantiate(groundAttack, 
        new Vector3(transform.position.x, collider.gameObject.transform.position.y, transform.position.z), transform.rotation);
      clone.gameObject.transform.rotation = collider.gameObject.transform.rotation;
      clone.tempo = lizard_tempo;
    }
    else if (collider.gameObject.tag == "Player") {
      if ((!isStable) && (enemy_health > 0) && (GameManager.GM.fightMode) && (GameManager.GM.fightObject == this.gameObject) && (!isAttacking)) {
        enemy_health --;
        musicScript.transitionTrigger = true;
        if (enemy_health == 0) {
          isStable = true;
          this.tag = "Friend";
          GameManager.GM.Switch("Walk");
          triggerRadius = 5.0f;
        }
      }
      else if (isAttacking) {
        isAttacking = false;
        //player health --
      }
    }
  }

  void OnCollisionExit(Collision collider) {
    if (collider.gameObject.tag == "Ground") {
      onGround = false;
    }
  }

  List<string> FindText(List<List<string>> textList) {
    List<string> list = new List<string>();

    if (textList.Count == 0)
      list = null;

    if (interactivity >= textList.Count)
      list =  new List<string>(textList[textList.Count - 1]);
    else
      list = new List<string>(textList[interactivity]);

    interactivity++;
    return list;
  }
  
  // Update is called once per frame
  void Update () {
    playerDist = Vector3.Distance (player.position, this.transform.position);
    //split based on nature
    //Civilian
    if (isStable) {
      if (GameManager.GM.walkMode) {
        if (sentText) sentText = false;
        if (playerDist <= triggerRadius) {
          //Always turn to player while within their radius
          FollowPlayer();
        }
        else {
          Pace();
        }
      }
      else if (GameManager.GM.converseMode) {
        //If player is not conversing with this, pause
        if ((GameManager.GM.converseObject == this.gameObject) && (!sentText)) {
          sentText = true;
          GameManager.GM.isTalking = true;
          GameManager.GM.receiveText(FindText(lizardTextList));
          musicScript.receiveConverseMusic(talkMusic, lizard_tempo);
        }
      }
    }


    //Enemy
    else if (!isStable) {
      if (GameManager.GM.fightMode) {
        if (playerDist <= (triggerRadius)) {
          if (playerDist <= attackRadius) {
            Attack();
            if (onGround) isAttacking = false;
          }
          else {
            FollowPlayer();
          }
        }
        else if ((playerDist > triggerRadius) && (GameManager.GM.fightObject == this.gameObject)) {
          print("Player ran away");
          GameManager.GM.fightObject = null;
          StartCoroutine(GameManager.GM.CheckForUpdates());
        }
      }
      else if (GameManager.GM.walkMode) {
        if (!isStable) {
          if (playerDist <= triggerRadius) {
            //Switch to fight mode
            if (GameManager.GM.fightObject == null)
              GameManager.GM.fightObject = this.gameObject;
            else {
              //priority
              GameManager.GM.PriorityFightObject(this.gameObject, priority); //set this to fight object if there is higher priority
            }
            GameManager.GM.Switch("Fight");
            musicScript.receiveFightMusic(alertMusic, fightMusic_1, fightMusic_2, fightMusic_3,
                                          transition_1, transition_2, defeatMusic, lizard_tempo);
          }
          else {
            Pace();
          }
        }
        else {
        }
      }
    }
    
  }
}
