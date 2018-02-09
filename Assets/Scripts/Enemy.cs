using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {/*
	//for stability
	public Rigidbody rb;
	public Vector3	origPos;
	public bool		isStable = true;
	private bool	isHit = false;
	private bool	hasTransformed = false;
	private bool	isTransforming = false;
	//for moving
	private float	runRadius = 12f;
	private float	walkRadius = 5f;
	private float	sRadius = 2f;
	private float	runSpeed = 10f;
	private float	walkSpeed = 2.5f;
	private float 	curSpeed = 0f;
	private float	accelRate = 0.1f;
	//for health/color
	private float	health = 1f;
	private float	regenSpeed;
	private float	transformSpeed = 0.1f;
	private Color	dormantColor;
	private float	dormantPerc = 0;
	//for player
	private GameObject 	player;
	private float		playerDistance; //distance player currently is
	private float		playerPower = 0f; //power of player's attacks
	//for attacking
	public Weapon 	weaponPrefab;
	private float		laTime = 2.8f; //long ranged attack time
	private float		saTime = 1f; //short ranged attack time
	private float		waitTime = 0f; //uses last attack wait time (recharge)
	private float		prevAttTime = 0f;
	private bool		isAttacking = false;
	private float		addTime; //if the enemy doesn't feel like attacking
	private float		srPower = 0.2f;
	private float 		lrPower = 0.4f;
	public float		attPower = 0f;
	private Vector3		tempPosition;
	private List<Weapon>	attackList;
	private List<Weapon> 	removeList;
	//for text
	public Text 	displayText;
	private bool 	newCivilian = true;
	private string  firstText;
	private string  secondText;

void Start() {
	this.transform.position = origPos;
	this.tag = "Enemy";
	player = GameObject.Find("Player");
	attackList = new List<Weapon> ();
	removeList = new List<Weapon> ();
	//texts for testing
	//displayText.text = "";
	//firstText = "Hello there.";
	//secondText = "Hello again.";
}

void ApproachPlayer() {
	//float curSpeed = 0f;
	if (!isStable) {
		if (playerDistance <= sRadius) {
			//physical attack, enemy does not move
			if (curSpeed > 0f)
				curSpeed -= accelRate;
			else
				curSpeed = 0f;
		}
		else if (playerDistance <= walkRadius) {
			//enemy approaches player slowly
			if (curSpeed > walkSpeed)
				curSpeed -= accelRate;
			else
				curSpeed = walkSpeed;
		}
		else if (playerDistance <= runRadius) {
			//gotta run man
			if (curSpeed < runSpeed)
				curSpeed += accelRate;
			else
				curSpeed = runSpeed;
		}
		if ((curSpeed != 0) && (!hasTransformed)) {
			hasTransformed = true;
			isTransforming = true;
		}
		//for rotating
		//print("Distance: " + playerDistance + "\nSpeed: " + curSpeed);
		Vector3 relativePos = player.transform.position - transform.position;
		rb.transform.rotation = Quaternion.LookRotation(relativePos);
		rb.MovePosition (transform.position + transform.forward * (
			curSpeed * Time.deltaTime));
	}
	return;
}

void Transform() {
	//changes color of civilian to show health
	if (!isStable) {
		if (isHit) {
			isHit = false;
			health += playerPower;
			//do isHit animation yay
			//maybe also a delay?
		}
		if (isTransforming) {
			health -= transformSpeed * Time.deltaTime;
			if (health <= 0f) {
				isTransforming = false;
			}
		}
		if (health >= 1f) {
			isTransforming = false;
			isStable = true;
			//do transformation animation
		}
		this.GetComponent<Renderer>().material.color = new Color (health, 
			health, health);
	}
	else {
		if (isHit) {
			if (dormantPerc < 1) {
				dormantPerc += transformSpeed * Time.deltaTime;
				Color tempColor = new Color (1f - (dormantColor.r * dormantPerc),
					1f - (dormantColor.g * dormantPerc), 
					1f - (dormantColor.b * dormantPerc));
				this.GetComponent<Renderer>().material.color = tempColor;
			}
		}
	}
	return;
}*/

/*void Attack() {
	if (playerDistance <= sRadius) {
		//physically attack the player
	}
	if (playerDistance <= walkRadius) {
		addTime = Random.Range(0.0f, 2.0f);
		if (prevAttTime + waitTime + addTime <= Time.time) {
			prevAttTime = Time.time;
			isAttacking = true;
			addTime = Mathf.Floor(addTime);
			Weapon newAttack = Instantiate<Weapon>(weaponPrefab);
			if (addTime == 0.0) {
				newAttack.name = "Short";
				newAttack.GetComponent<Weapon>().power = srPower;
				waitTime = saTime;
			}
			else {
				print("in long attack");
				newAttack.name = "Long";
				newAttack.GetComponent<Weapon>().power = lrPower;
				waitTime = laTime;
			}
			newAttack.tag = "Attack";
			attackList.Add(newAttack);
			tempPosition = player.transform.position; //finds player
		}
		else if (isAttacking && (prevAttTime + waitTime <= Time.time)) {
			print("attacking");
			prevAttTime = Time.time;
			Weapon newAttack = attackList[0];
			newAttack.transform.position = tempPosition;
			isAttacking = false;
			attackList.Remove()
		}
	}
	return;
}*/


/*void Speak() {
	//when player speaks to civilian
	//print ("new: " + newCivilian);
	if (!newCivilian) {
		displayText.text = secondText;
	}
	else {
		displayText.text = firstText;
		newCivilian = false;
	}
	return;
}


void OnTriggerEnter(Collider other) {
	if (!isTransforming) {
		//can't be hit during initial transformation (need to think about more)
		if (other.gameObject.name == "Note") {
			//if the player attacked take damage
			isHit = true;
			playerPower = player.GetComponent<Player1>().notePower;
			dormantColor = other.gameObject.GetComponent<Renderer>().material.color;
			dormantColor.r = 1f - dormantColor.r;
			dormantColor.g = 1f - dormantColor.g;
			dormantColor.b = 1f - dormantColor.b;
		}
	}
	return;
}

void Update() {

	//always check where the player is
	playerDistance = Vector3.Distance (player.transform.position, this.transform.position);

	//when civilian is unstable, attack the player
	if (!isHit && !isAttacking) {
		ApproachPlayer();
	}
	if (isTransforming || isHit) {
		Transform();
	}

	//civilian is stable, player can talk to them
	/*if (isStable) {
		if (isHit)
			Transform();
		if (playerDistance <= 5f) {
			//rotate to face player
			Vector3 relativePos = player.transform.position - transform.position;
			rb.transform.rotation = Quaternion.LookRotation(relativePos);
			//speak
			player.GetComponent<Player1>().canTalk = true; //player won't jump
			if (Input.GetKey("space")) {
				Speak();
			}
		}
		else {
			player.GetComponent<Player1>().canTalk = false;
		}
	}*/
	//else if (!isStable) {
	//	Attack();
	//}
//}

}