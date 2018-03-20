using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Player : MonoBehaviour {
  public Color color;
  private Rigidbody rigidbody;
  [SerializeField]Transform camera_transform;

  public GameObject musicObject;
  private MusicManager musicScript;
  //private EnvironmentScript environment;
  public GameObject companionObject;

  //Camera
  //[SerializeField]Transform camera;
  //Force of Movement variables
  private float rotateSpeed = 1000f;
  private float walkForce = 10.0f;
  private float jumpForce = 200.0f;
  public float velocity = 0.0f;
  public float acceleration = 1.0f;
  //private float rotateMultiplier = 1.0f;
  private float walkMultiplier = 1.0f;
  private float jumpMultiplier = 1.0f;
  private Vector3 offsetX;
  private Vector3 offsetY;
  private Vector3 velocityVector;

  //private float regenSpeed = 2f;
  public bool controlLock = false;
  public bool isHit = false;
  //Walking specific variables
  private float horzMovement;
  private float vertMovement;
  private float jumpMovement;
  //private float horzVelocity = 0.0f;
  //private float vertVelocity = 0.0f;
  //locks
  //private bool walkLock = false;
  private bool jumpLock = false;
  public float AButton; //only accessible to game manager
  public float YButton;
  public bool AisPressed = false;
  public bool AisReleased = true;
  public bool isOnGround = false;
  //Enemy Attacks on Player
  public AudioClip hitEffect;
  //public float damage;
  //Civilian
  private string interactTag = "Friend";
  private string enemyTag = "Enemy";
  public bool waitForSpeechDelay = false;
  public float sphereColliderRadius = 10.0f;
  public float enemyColliderRadius = 20.0f;
  //Health
  private float healthBar = 100f;
  //Attacks
  public AttackDestroyScript groundAttack;

  //Game State
  public bool GameOver = false;

  // Use this for initialization
  void Start () {
    rigidbody = GetComponent<Rigidbody>();
    //rigidbody = GetComponent<RigidbodyScript>();
    musicScript = musicObject.GetComponent<MusicManager>();
    
    //set color
    /*Renderer rend = GetComponent<Renderer>();
    rend.material.shader = Shader.Find("Diffuse");
    rend.material.SetColor("_Color", color);*/
  }

  public void ChangeAttributes(float env_gravity, float env_air_resist) {
    acceleration = env_air_resist;
    jumpMultiplier = env_gravity;
    //rotateMultiplier = env_air_resist;
  }

  public IEnumerator Damaged(float damage) {
    controlLock = true;
    healthBar -= damage;
    if (healthBar <= 0.0f) {
      GameOver = true;
      GameManager.GM.TriggerGameOver();
      healthBar = 100.0f;
    }
    yield return new WaitForSeconds(1.0f);
    isHit = false;
    controlLock = false;
  }

  private void Move (float horizontal, float vertical, float jump) {

    Vector3 nextPosx = (horizontal * camera_transform.right);
    Vector3 forward_pos = (Quaternion.Euler(0, -90, 0) * camera_transform.right);
    Vector3 nextPosy = (vertical * forward_pos);
    Vector3 nextPos = (nextPosx + nextPosy);

    //calculate velocity
    //ramp up
    if (velocity < (Mathf.Abs(horizontal) + Mathf.Abs(vertical))) {
      if (isOnGround) velocity += acceleration * 0.15f;
      else velocity += acceleration * 0.05f;

      if (velocity > 1.0f) velocity = 1.0f;
    }
    //ramp down
    if (velocity > (Mathf.Abs(horizontal) + Mathf.Abs(vertical))) {
      if (isOnGround) velocity -= acceleration * 0.15f;
      else velocity -= acceleration * 0.05f;

      if (velocity < 0.0f) velocity = 0.0f;
    }

    //calculate next position
    if (nextPos == Vector3.zero) {
      nextPos = velocityVector * velocity;
    }
    else {
      nextPos = ((velocityVector + nextPos) / ((velocityVector.magnitude + nextPos.magnitude))) * velocity;
    }
    transform.Translate(nextPos * walkForce * Time.deltaTime);

    //Translate
    velocityVector = nextPos;

    //Rotate
    //float step = rotateSpeed * Time.deltaTime;
    //Vector3 newDir = Vector3.RotateTowards(transform.forward, nextPos, step, 0.0f);
    //Debug.DrawRay(transform.position, newDir, Color.red);
    //transform.rotation = Quaternion.LookRotation(newDir);


    //jump
    if (isOnGround) {
      if (jumpLock) {
        jumpLock = false;
      }
      else {
        if (jump > 0) {
          rigidbody.AddForce(Vector3.up * (jumpForce * jumpMultiplier));
        }
      }
    }

  } //End of Move();

  private GameObject FindClosestObject(Collider[] colliders, string compareTag) {
    //if there's only one game object, pick that object
    if (colliders.Length == 1) {
      if ((colliders[0].gameObject.tag == compareTag) && (colliders[0].gameObject != companionObject))
        return colliders[0].gameObject;
      else
        return null;
    }
    else if (colliders.Length > 1) {
      //else look through all possibilites and find closest object to player
      float minDist = sphereColliderRadius;
      GameObject minObject = null;

      for (int i = 0; i < colliders.Length; i++) {
        if ((colliders[i].gameObject.tag == compareTag) && (colliders[i].gameObject != companionObject)) {
          GameObject currentCollider = colliders[i].gameObject;
          float tempDist = Mathf.Abs(Vector3.Distance (this.transform.position, currentCollider.transform.position));
          if (tempDist < minDist) {
            minDist = tempDist;
            minObject = currentCollider;
          }
        }
      }
      if ((minObject != null) && (minObject.tag != this.gameObject.tag))
        return minObject;
    }
    return null;
  }
  

  void OnCollisionEnter(Collision collider) {

    if (collider.gameObject.tag == "Level") {

    }
    else if (collider.gameObject.tag == "Ground") {
      GameManager.GM.environmentObject = collider.gameObject;
      AttackDestroyScript clone;
      isOnGround = true;
      //environment = collider.gameObject.GetComponent<EnvironmentScript>();
      clone = (AttackDestroyScript)Instantiate(groundAttack, 
        new Vector3(transform.position.x, collider.gameObject.transform.position.y, transform.position.z), 
        transform.rotation);
      clone.tempo = (int)musicScript.tempo;

    }

    if ((collider.gameObject.tag == "Enemy") && (!isHit)) {
      //musicScript.enemyHit = true;
      isHit = true;
      StartCoroutine(Damaged(10.0f));
    }
    if (collider.gameObject.tag == "AudioTrigger") {
      //grab its info and pass it to music manager
    }
  }

  void OnCollisionExit(Collision collider) {
    if ((collider.gameObject.tag == "Ground") && (collider.gameObject == GameManager.GM.environmentObject)) {
      isOnGround = false;
    }
  }
  
  // Update is called once per frame
  void Update () {
    //Button Inputs
    horzMovement = Input.GetAxisRaw("Horizontal");
    vertMovement = Input.GetAxisRaw("Vertical");
    jumpMovement = Input.GetAxisRaw("Jump");
    AButton = Input.GetAxisRaw("Submit");
    YButton = Input.GetAxisRaw("Interact");

    //Gravity();

    //Walk Mode
    if (GameManager.GM.walkMode) {
      if (!controlLock) {
        Move(horzMovement, vertMovement, jumpMovement);
        //All GameObjects within 5 meter radius of player
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereColliderRadius);

        if (hitColliders.Length >= 1) {

          //if there is something the player is close to, give player option to interact
          GameObject enemy = FindClosestObject(hitColliders, enemyTag);
          if (enemy != null) {
            /*GameManager.GM.fightObject = enemy;
            if (GameManager.GM.fightObject != null) {
              GameManager.GM.Switch("Fight");
            }*/
          }
          else {
            //if (!GameManager.GM.walkMode)
            //  GameManager.GM.Switch("Walk"); //switch to walk mode from fight mode
          }

          if ((AButton < 0) || (AButton > 0)) {
            //need to find object with shortest distance to player
            GameObject friend = FindClosestObject(hitColliders, interactTag);
            if (friend != null) {
              if (friend.tag == interactTag) { 
                GameManager.GM.converseObject = friend;
                //switch to talk mode
                if (GameManager.GM.converseObject != null) {
                  GameManager.GM.Switch("Talk");
                  //controlLock = true;
                }
              }
            } 
          }

          if ((YButton < 0) || (YButton > 0)) {
            //find companion
            //if (companionObject == null) {
             // companionObject = FindClosestObject(hitColliders, "Horse");
              //companionObject = FindClosestObject(hitColliders, "Dog");
            //  if (companionObject != null) {
            //    controlLock = true;
                //musicScript.ChangeMasterVolume(0.2f);
            //  }
            //}
            //else
              companionObject = null;
              controlLock = true;
              musicScript.ResetMasterVolume();
            }

        }
      }
    }

    //Talk Mode
    else if (GameManager.GM.converseMode) {
      if (!controlLock) {
        if (AisReleased && ((AButton < 0) || (AButton > 0))) {
          AisReleased = false;
          AisPressed = true;
        }
        if (AisPressed && (AButton == 0)) {
          AisReleased = true;
          AisPressed = false;
          GameManager.GM.textWaitForInput();
        }
      }
    }

    //Fight Mode
    else if (GameManager.GM.fightMode) {
      if (!controlLock) {
        Move(horzMovement, vertMovement, jumpMovement);
      }
    }

  }

}
