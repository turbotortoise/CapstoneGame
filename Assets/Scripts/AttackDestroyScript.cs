using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDestroyScript : MonoBehaviour {
	public float length;
	public int tempo;
	public Color approxColor;
	private float timeMultiplier;
	private float sizeMultiplier = 1.0f;

	// Use this for initialization
	void Start () {
		timeMultiplier = (60.0f / (float)tempo) * length;
		StartCoroutine(Self_Destruct());
		Renderer rend = GetComponent<Renderer>();
    	//rend.material.shader = Shader.Find("Diffuse");

    	approxColor.r += Random.Range(-0.05f, 0.05f);
    	approxColor.g += Random.Range(-0.05f, 0.05f);
    	approxColor.b += Random.Range(-0.05f, 0.05f);
    	//Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    	rend.material.SetColor("_Color", approxColor);
	}

	IEnumerator Self_Destruct() {
		//print("destroying: " + this.gameObject.name);
		yield return new WaitForSeconds(timeMultiplier);
		//Destroy(this);
		Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale += new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier);
		
	}
}
