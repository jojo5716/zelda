using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class WarpController : MonoBehaviour {
	public GameObject target;
	public GameObject targetMap;

	// Transitions
    bool start = false;
    bool isFadeIn = false;
    float alpha = 0;
    float fadeTime = 1f;

	// Area
	GameObject area;


	void Awake() {
		Assert.IsNotNull (target);
		// We need to hide the transportation box (entrance and exit)
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.enabled = false;

		transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

		Assert.IsNotNull (targetMap);

		area = GameObject.FindGameObjectWithTag ("Area");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator  OnTriggerEnter2D(Collider2D col) {
		if (col.tag == "Player") {
			FadeIn ();

			yield return new WaitForSeconds (fadeTime);
			// When user is on the entrance box transportation
			// we change the player position to the exit transportation box position
			col.transform.position = target.transform.GetChild (0).transform.position;

			// We execute the method SetBound on the CameraController.cs
			Camera.main.GetComponent<CameraController> ().SetBound (targetMap);

			FadeOut ();

			StartCoroutine(area.GetComponent<AreaController>().ShowArea(targetMap.name));
		}


	}

	void OnGUI() {
		if (!start) {
			return;
		}

		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.g, alpha);

		Texture2D texture;
		texture = new Texture2D(1, 1);
		texture.SetPixel(0, 0, Color.black);
		texture.Apply();

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);

		if (isFadeIn) {
			alpha = Mathf.Lerp (alpha, 1.1f, fadeTime * Time.deltaTime);
		} else {
			alpha = Mathf.Lerp (alpha, -0.1f, fadeTime * Time.deltaTime);

			if (alpha < 0) {
				start = false;
			}
		}
	}

	void FadeIn() {
		start = true;
		isFadeIn = true;
	}

	void FadeOut() {
		isFadeIn = false;
	}
}
