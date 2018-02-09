using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyScript : MonoBehaviour {
  public EnvironmentScript environment;
  private float height = 0.0f;
  //public float weight;
  public float x_vel = 0.0f;
  public float y_vel = 0.0f;
  private bool isOnGround = false;

  // Use this for initialization
  void Start () {
    
  }

  public void SetEnvironment(EnvironmentScript new_environment) {
    environment = new_environment;
  }


//Collision Informaion
  void OnCollisionEnter(Collision collider) {
    if (collider.gameObject.tag == "Ground") {
      isOnGround = true;
      height = transform.position.y;
    }
  }
  void OnCollisionExit(Collision collider) {
     if (collider.gameObject.tag == "Ground") {
      isOnGround = false;
      height = 0.0f;
     }
  }
  
//Gravity Information
  void Gravity() {
    if (!isOnGround) {
      float error = 0.001f;
      if (transform.position.y <= (height + error)) {
        y_vel = 0.0f;
      }
      else {
        transform.Translate(new Vector3(0.0f, y_vel, 0.0f));
        y_vel -= environment.gravity;

      }
    }
    else {
      if (y_vel != 0.0f) {
        transform.Translate(new Vector3(0.0f, y_vel, 0.0f));
      }
    }
  }
  
  // Update is called once per frame
  void Update () {
    if (environment == null) {
      height = 0.0f;
      transform.Translate(new Vector3(0.0f, -0.098f, 0.0f));
    }
    else {
      Gravity();
    }
  }
}
