/* Fire Glob Class
 * Contains both Peaceful and Aggresive scripts for the fire people.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GlobScript : MonoBehaviour {
  public Color color;
  public Color reverbColor;
  public string material_name;
  //3D settings
  private Rigidbody rb;
  private Vector3 startPos;
  public bool isStable;
  public bool random_text;
  public float triggerRadius;
  public float spider_speed;
  public float jump_height;
  public float attack_speed;
  public float attack_power;

  [SerializeField]Transform player;
  public GameObject musicObject;
  private MusicManager musicScript;
  //private Main_Player playerScript;
  private float playerDist;

  //Civilian Settings
  //private float walkSpeed = 1.0f;
  //private float rotateSpeed = 1.0f;
  //Text
  public TextAsset textData;
  public TextAsset musicData;
  private bool sentText = false;
  private List<List<string>> spiderTextList = new List<List<string>>();
  private List<List<int>> spiderMusicList = new List<List<int>>();
  int music_section = 0;
  int music_measure = 0;
  private bool onGround = false;
  private string spiderText;
  private float prevWalkTime = 0.0f;
  private float walkWaitTime;
  private int followTrigger = 1;
  private int interactivity = 0; //the number of times talked to player

  //Enemy settings
  private float attackRadius = 10.0f;
  private int enemy_health = 3;
  public AudioSource audioSource;
  public AudioClip talkMusic;
  public AudioClip alertMusic; //clip triggered when player is discovered
  public AudioClip fightMusic_1; //stage 1 soundtrack
  public AudioClip fightMusic_2; //stage 2 soundtrack
  public AudioClip fightMusic_3; //stage 3 soundtrack
  public AudioClip transition_1; //transition to second soundtrack
  public AudioClip transition_2; //transition to third soundtrack
  public AudioClip defeatMusic; //clip triggered when health reaches 0
  public AudioClip jump_sound; //clip triggered when spider jumps
  public AudioClip attack_sound; //clip triggered when spider attacks
  public int spider_tempo;
  //Type of attack varies on species
  public AttackDestroyScript groundAttack;
  //priority varies on species
  public int priority;
  private bool isAttacking = false;
  private float AttackDelay;

  void Start () {
    //audioSource = this.AudioSource;
    startPos = this.transform.position;
    rb = GetComponent<Rigidbody>();
    musicScript = musicObject.GetComponent<MusicManager>();
    walkWaitTime = ((60.0f / spider_tempo) / 4.0f);
    AttackDelay = walkWaitTime * 2.0f;
    //playerScript = GetComponent<Main_Player>();
    //if (isStable)
    //  triggerRadius = 5.0f;

    //set color (for not white everything)
    Renderer rend = GetComponent<Renderer>();
    rend.material.shader = Shader.Find(material_name);
    rend.material.SetColor("_Color", color);

    //grab text
    //textData = Resources.Load("DogText") as TextAsset;
    spiderText = textData.ToString();
    spiderTextList = GameManager.GM.PreparseText(spiderText);
    spiderText = musicData.ToString();
    spiderMusicList = GameManager.GM.PreparseMusic(spiderText);

  }

  void Jump(float x, float y, Vector3 dir) {
    if (onGround) {
      rb.AddForce(dir * (x * spider_speed));
      rb.AddForce(transform.up * (y * jump_height));
      //musicScript.receiveSoundEffect(jump_sound);
      audioSource.clip = jump_sound;
      audioSource.Play();
    }
  }

  void FollowPlayer() {
    transform.LookAt(new Vector3(player.position.x, this.transform.position.y, player.position.z));
    int currentMeasure = spiderMusicList[music_section][music_measure];

    if ((prevWalkTime + (walkWaitTime * currentMeasure) <= Time.time)) {
      prevWalkTime = Time.time;

      if (music_measure < (spiderMusicList[music_section].Count - 1)) 
        music_measure++;
      else 
        music_measure = 0;
    }
  }


  void Pace() {

  }

  IEnumerator Attack(Vector3 playerPos) {
    isAttacking = true;
    transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
    int currentMeasure = spiderMusicList[music_section][music_measure];
    isAttacking = false;
    //delay for player to attack
    yield return new WaitForSeconds((prevWalkTime + (AttackDelay * currentMeasure)) - Time.time);
    prevWalkTime = Time.time;
  }

  void OnCollisionEnter(Collision collider) {
    if (collider.gameObject.tag == "Ground") {
      AttackDestroyScript clone;
      onGround = true;
      clone = (AttackDestroyScript)Instantiate(groundAttack, 
        new Vector3(transform.position.x, collider.gameObject.transform.position.y, transform.position.z), transform.rotation);
      clone.gameObject.transform.rotation = collider.gameObject.transform.rotation;
      clone.tempo = spider_tempo;
    }
    else if (collider.gameObject.tag == "Player") {
      if ((!isStable) && (enemy_health > 0) && (GameManager.GM.fightMode) && (!isAttacking)) {
        enemy_health --;
        if (music_section < (spiderMusicList.Count - 1)) music_section++;
        musicScript.transitionTrigger = true;
        if (enemy_health == 0) {
          isStable = true;
          this.tag = "Friend";
          GameManager.GM.Switch("Walk");
          triggerRadius /= 2.0f;
        }
      }
      else if (isAttacking) {
        isAttacking = false;
        StartCoroutine(player.gameObject.GetComponent<Main_Player>().Damaged(attack_power));
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

    if (random_text) {
      list = new List<string>(textList[(Random.Range(0, textList.Count - 1))]);
    }
    else {
      if (interactivity >= textList.Count)
        list =  new List<string>(textList[textList.Count - 1]);
      else
        list = new List<string>(textList[interactivity]);
    }

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
          GameManager.GM.receiveText(FindText(spiderTextList));
          musicScript.receiveConverseMusic(talkMusic, spider_tempo);
        }
      }
    }


    //Enemy
    else if (!isStable) {
      if (GameManager.GM.fightMode) {
        if (musicScript.transitionTrigger && (music_section < (spiderMusicList.Count - 1)) && (music_section % 2 == 0)) music_section++;
        if (!isAttacking) {
          if (playerDist <= (triggerRadius)) {
            if ((playerDist <= attackRadius) && (!isAttacking)) {
              StartCoroutine(Attack(player.position));
            }
            else {
              FollowPlayer();
            }
          }
          else if ((playerDist > (triggerRadius * 3.0f)) && (GameManager.GM.fightObject == this.gameObject)) {
            print("Player ran away");
            GameManager.GM.fightObject = null;
            StartCoroutine(GameManager.GM.CheckForUpdates());
          }
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
            prevWalkTime = Time.time;
            music_measure = 0;
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
