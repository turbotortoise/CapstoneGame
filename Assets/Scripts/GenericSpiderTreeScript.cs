using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericSpiderTreeScript : MonoBehaviour {
  public Color color;
  public string material_name;

	// Use this for initialization
	void Start () {
	    Renderer rend = GetComponent<Renderer>();
	    rend.material.shader = Shader.Find(material_name);
	    rend.material.SetColor("_Color", color);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
