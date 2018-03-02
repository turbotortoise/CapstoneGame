using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	//player information
	[SerializeField]Transform target;
	public Vector3 diameter;
	private Vector3 RotateScale;
	private Vector3 FocusPosition;

	//rotation
	private float rotate_x = 0.0f;
	private float rotate_z = -80.0f;

	private float deadMvt = 0.2f;
  	private float horzMovement;
  	private float vertMovement;
  	private float trigMovement;
  	private float scaling_factor = 0.02f; //To be used on enemies

	Transform thisTransform;

	void Awake() {
		FocusPosition = target.position;
		thisTransform = transform;
		thisTransform.position = target.position + diameter;
		thisTransform.position = new Vector3(thisTransform.position.x, -1.8f, thisTransform.position.z);
	}

	void Start() {
	}

	void LateUpdate() {
		//print("rotate_z: " + rotate_z);
    	trigMovement = Input.GetAxisRaw("LeftTrigger");
		horzMovement = (-1.0f) * Input.GetAxisRaw("HorizontalTurn");
    	vertMovement = (-1.0f) * Input.GetAxisRaw("VerticalTurn");

    	if (Mathf.Abs(horzMovement) > deadMvt) {
    		rotate_x += horzMovement;
    	}
    	if ((Mathf.Abs(vertMovement) > deadMvt) && (Mathf.Abs(Mathf.Deg2Rad * rotate_z) < 1.5f)) {
    		rotate_z += vertMovement;
    		if (Mathf.Abs(Mathf.Deg2Rad * rotate_z) > 1.5f)
    			rotate_z = Mathf.Sign(vertMovement) * Mathf.Rad2Deg * 1.49f;
    		else if (Mathf.Abs(Mathf.Deg2Rad * rotate_z) < 0.5f)
    			rotate_z = Mathf.Sign(rotate_z) * (Mathf.Rad2Deg * 0.49f);
		}

		if (GameManager.GM.walkMode) {
			//x - radius around player
			//y - height
			//z - radius around player
			RotateScale = new Vector3(0.85f, 0.80f, 0.85f);
			FocusPosition = target.position;
		}
		else if (GameManager.GM.converseMode) {
			Vector3 converseToTarget = (GameManager.GM.converseObject.transform.position - target.position);
			Vector3 midpoint = target.position + (0.5f * converseToTarget);
			FocusPosition = midpoint;

			RotateScale = new Vector3(0.8f, 0.55f, 0.8f);
		}
		else if (GameManager.GM.fightMode) {
			if (trigMovement > 0) {
				//rotate_x = 0.0f;
				//rotate_z = -80.0f;
				Transform fightTransform = GameManager.GM.fightObject.transform;
				RotateScale = Vector3.Distance(GameManager.GM.fightObject.transform.position, target.position) * new Vector3(0.01f, 0.01f, 0.01f);
				FocusPosition = fightTransform.position;
			}
			else {
				RotateScale = new Vector3(0.85f, 0.80f, 0.85f);
				FocusPosition = target.position;
			}
		}
		thisTransform.position = target.position + new Vector3(
			(diameter.x * RotateScale.x) * Mathf.Sin(Mathf.Deg2Rad * rotate_z) * Mathf.Cos(Mathf.Deg2Rad * rotate_x),
			(diameter.y * RotateScale.y) * Mathf.Cos(Mathf.Deg2Rad * rotate_z),
			(diameter.z * RotateScale.z) * Mathf.Sin(Mathf.Deg2Rad * rotate_z) * Mathf.Sin(Mathf.Deg2Rad * rotate_x));
		thisTransform.LookAt(FocusPosition);
		if (GameManager.GM.fightMode) {
			Vector3 playerToEnemy = (GameManager.GM.fightObject.transform.position - target.position) * scaling_factor;
			thisTransform.position = thisTransform.position - playerToEnemy;
		}

	}
}
