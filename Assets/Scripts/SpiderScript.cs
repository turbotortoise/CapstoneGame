/* Spider Class
 * Contains both Peaceful and Aggresive scripts for the spider people.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SpiderScript : MonoBehaviour {
  public Color color;
  private Color current_color;
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

  //Jump settings
  private Vector3 jump_direction;
  private float jump_x_height;
  private float jump_y_height;

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
  int currentMeasure = 0;
  private bool onGround = false;
  private string spiderText;
  private float prevWalkTime = 0.0f;
  private float subdivision = 4.0f; //16th notes
  private float walkWaitTime;
  private int followTrigger = 1;
  private int interactivity = 0; //the number of times talked to player

  //Enemy settings
  private float attackRadius = 10.0f;
  private float enemy_health;
  private float max_health = 3.0f;
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
    if (!isStable) enemy_health = max_health;
    startPos = this.transform.position;
    rb = GetComponent<Rigidbody>();
    musicScript = musicObject.GetComponent<MusicManager>();
    walkWaitTime = (60.0f / musicScript.tempo);
    AttackDelay = walkWaitTime * 2.0f;
    //if (isStable)
    //  triggerRadius = 5.0f;

    //set color (for not white everything)
    /*Renderer rend = GetComponent<Renderer>();
    rend.material.shader = Shader.Find(material_name);
    rend.material.SetColor("_Color", color);
    current_color = color;*/

    //grab text
    spiderText = textData.ToString();
    spiderTextList = GameManager.GM.PreparseText(spiderText);
    spiderText = musicData.ToString();
    spiderMusicList = GameManager.GM.PreparseMusic(spiderText);

    StartCoroutine(Metronome());
  }

  /************************************** Coroutines ******************************************/

  IEnumerator Metronome() {
    //Metronome keeps track of the current bpm relative to the current time
    //if (GameManager.GM.fightObject == this.gameObject) print("clip: " + GetComponent<AudioSource>)
    if (Time.time > prevWalkTime) {

      if (music_measure < (spiderMusicList[music_section].Count - 1)) music_measure++;
      else music_measure = 0;


      if (isStable) {

      }

      if ((!isStable) && (GameManager.GM.fightMode) && (GameManager.GM.fightObject == this.gameObject)) {
        if (music_measure == 0) {
          if (((music_section % 2) == 0) && (music_section < (spiderMusicList.Count - 1))) {
            music_section++;
            if (music_section == 1) GetComponent<AudioSource>().clip = fightMusic_1;
            else if (music_section == 3) GetComponent<AudioSource>().clip = fightMusic_2;
            else if (music_section == 5) GetComponent<AudioSource>().clip = fightMusic_3;
          }
        }
      }

      if (onGround) Jump();
    }

    //next metronome call
    currentMeasure = spiderMusicList[music_section][music_measure];
    float nextWalkTime = prevWalkTime + ((currentMeasure * (60.0f / musicScript.tempo)) / subdivision);
    prevWalkTime = nextWalkTime;
    float calctime = nextWalkTime - Time.time;
    yield return new WaitForSeconds(calctime - (Time.deltaTime / 50.0f));
    StartCoroutine(Metronome());

  }


  IEnumerator Attack(Vector3 playerPos) {
    isAttacking = true;
    transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
    //int currentMeasure = spiderMusicList[music_section][music_measure];

    //Jump Backwards
    yield return new WaitForSeconds((prevWalkTime + (walkWaitTime * currentMeasure)) - Time.time);
    jump_direction = (playerPos - transform.position).normalized;
    jump_x_height = (-attack_speed / (currentMeasure * 2.5f));
    jump_y_height = 0.5f;
    transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
    //Jump Forward
    yield return new WaitForSeconds(((prevWalkTime + (walkWaitTime * currentMeasure)) - Time.time) * 2.5f);
    jump_x_height = attack_speed;
    jump_y_height = 3.0f;
    transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
    yield return new WaitForSeconds(((prevWalkTime + (walkWaitTime * currentMeasure)) - Time.time) * 3.0f);
    isAttacking = false;
    //delay for player to attack
    yield return new WaitForSeconds((prevWalkTime + (AttackDelay * currentMeasure)) - Time.time);
    transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
  }

  IEnumerator TakeDamage(float damage) {
    //Check if enemy is defeated
    if (enemy_health <= 0) {
      //defeat
      isStable = true;
      this.tag = "Friend";
      GameManager.GM.Switch("Walk");
      //triggerRadius /= 2.0f;
      GetComponent<AudioSource>().clip = defeatMusic;
    }

    else if ((enemy_health > 0) && (damage > 0.01f)) {

      //Change color
      /*current_color.r += damage * Time.deltaTime;
      if (current_color.r > 1.0f) current_color.r = 1.0f;
      Renderer rend = GetComponent<Renderer>();
      rend.material.shader = Shader.Find(material_name);
      rend.material.SetColor("_Color", current_color);
      //Change stats
      enemy_health -= damage * Time.deltaTime;
      damage /= 2.0f;*/

      //Enemy isn't defeated, check for phase changes
      if ((enemy_health <= 2) && ((GetComponent<AudioSource>().clip != transition_1) || (GetComponent<AudioSource>().clip != fightMusic_2)))
        GetComponent<AudioSource>().clip = transition_1;
      if ((enemy_health <= 1) && ((GetComponent<AudioSource>().clip != transition_2) || (GetComponent<AudioSource>().clip != fightMusic_3)))
        GetComponent<AudioSource>().clip = transition_2;

      yield return new WaitForSeconds(Time.deltaTime);
      StartCoroutine(TakeDamage(damage));
    }
    yield return new WaitForSeconds(Time.deltaTime);

  }

  /***************************************************** Functions ***********************************/


  void Jump() {
    if (onGround) {
      rb.AddForce(jump_direction * (jump_x_height * spider_speed));
      rb.AddForce(transform.up * (jump_y_height * jump_height));
      audioSource.clip = jump_sound;
      audioSource.Play();
    }
  }

  void Pace() {
    //calculate distance from origin position
    Vector3 posToOrigin = startPos - transform.position;
    float distFromOrigin = Vector3.Distance (transform.position, startPos);
    if (distFromOrigin >= triggerRadius) {
      transform.LookAt(startPos);
      //too far from origin
      /*//calculate angle
      float angle = Vector3.Angle(startPos, transform.forward);
      //if angle is great enough, keep rotating
      if (angle <= 90.0f)
        transform.Rotate(Vector3.up, Random.Range(0.2f, 0.3f));
      else if (angle <= 180.0f)
        transform.Rotate(Vector3.up, Random.Range(-0.3f, -0.2f));*/
    }
    jump_direction = transform.forward;
    jump_x_height = 1.0f;
    jump_y_height = 1.0f;
  }


  void FollowPlayer() {
    transform.LookAt(new Vector3(player.position.x, this.transform.position.y, player.position.z));
    //int currentMeasure = spiderMusicList[music_section][music_measure];

    if (onGround) {
      jump_x_height = 2.0f;
      jump_y_height = 0.5f;
      if (followTrigger == 0) {
        //move right
        jump_direction = transform.right;
      }
      else {
        //move left
        jump_direction = -1.0f * transform.right;
      }
      followTrigger = (currentMeasure % 2);
    }
  }

  void OnCollisionEnter(Collision collider) {
    if (collider.gameObject.tag == "Ground") {
      AttackDestroyScript clone;
      onGround = true;
      clone = (AttackDestroyScript)Instantiate(groundAttack, 
        new Vector3(transform.position.x, collider.gameObject.transform.position.y, transform.position.z), transform.rotation);
      clone.gameObject.transform.rotation = collider.gameObject.transform.rotation;
      clone.tempo = spider_tempo;
      clone.approxColor = reverbColor;
      clone.transform.localScale = (2.0f * transform.localScale);
    }
    else if (collider.gameObject.tag == "Player") {
      if ((!isStable) && (enemy_health > 0) && (GameManager.GM.fightMode) && (!isAttacking)) {
        StartCoroutine(TakeDamage(0.3f));
      }
      else if (isAttacking) {
        isAttacking = false;
        //prevWalkTime += (walkWaitTime);
        //player health --
        //StartCoroutine(player.gameObject.GetComponent<Main_Player>().Damaged(attack_power));
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
      else if (GameManager.GM.walkMode) {
        if (playerDist <= triggerRadius) {
          //Switch to fight mode
          if (GameManager.GM.fightObject == null)
            GameManager.GM.fightObject = this.gameObject;
          else {
            GameManager.GM.PriorityFightObject(this.gameObject, priority); //set this to fight object if there is higher priority
          }
          GameManager.GM.Switch("Fight");
          music_measure = 0;
          GetComponent<AudioSource>().clip = alertMusic;
          GetComponent<AudioSource>().Play();
          musicScript.tempo = spider_tempo;
        }
        else {
          Pace();
        }
      }
    }
    
  }
}
