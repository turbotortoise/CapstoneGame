using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDetection : MonoBehaviour {

  // Use this for initialization
  void Start () {
    
  }
  
  void OnCollisionEnter(Collision collider) {
      print ("sphere collision with: " + collider.gameObject.name);
  }

  void OnTriggerEnter(Collider collider) {
    print ("sphere triggered by: " + collider.gameObject.name);
  }
  // Update is called once per frame
  void Update () {
    
  }
}