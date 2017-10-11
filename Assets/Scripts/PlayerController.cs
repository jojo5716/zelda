using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


public class PlayerController : MonoBehaviour {
	// Movement speed
	public float speed = 4f;

	// Animations
	private Animator playerAnimator;

	// Collisions
	private Rigidbody2D rb2d;
	private Vector2 playerMovement;

	// Map
	public GameObject initialMap;

	// Attack collider
	CircleCollider2D attackCollider;

	// Slash attack
	public GameObject slashPrefab;

	bool movePrevent;

	// Aura
	Aura aura;

    // Variables to move the player with a click buttom
    Vector3 targetPosition;

	// ---------- Methods -------------

	// When user press up or down keyword
	// Return: 0, 1, or -1 (0=not pressed, 1 = up pressed, -1 = down pressed)
	private float onVerticalKeywordPressed() {
		return Input.GetAxisRaw ("Vertical");
	}

	// When user press left or rigth keyword
	// Return: 0, 1, or -1 (0=not pressed, 1 = rigth pressed, -1 = left pressed)
	private float onHorizontalKeywordPressed() {
		return Input.GetAxisRaw ("Horizontal");
	}

	private Vector2 getPlayerMovement() {
		return new Vector2 (
			onHorizontalKeywordPressed(),
			onVerticalKeywordPressed()
		);
	}

	// Change player position
	private void setTransformMovement(Vector2 movement) {
		rb2d.MovePosition (rb2d.position + playerMovement * speed * Time.deltaTime);
	}

	// When user press on array keywords, we change the player position view
	private void changePlayerPositionView(Vector2 movement) {
		playerAnimator.SetFloat ("movementX", movement.x);
		playerAnimator.SetFloat ("movementY", movement.y);
	}

	// When user press any array keywords, we change the player animation
	// to simulate is walking
	private void changePlayerAnimationToWalking() {
		playerAnimator.SetBool ("walking", true);
	}

	// When user stop pressing on any array keywords, we change the player animation
	// to simulate is idle
	private void changePlayerAnimationToIdle() {
		playerAnimator.SetBool ("walking", false);
	}

	// If player is not walking we return false, else return true
	private bool hasUpdatePlayerAnimation() {
		return playerMovement != Vector2.zero;
	}

	private void SwordAttack() {
		AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo (0);
		bool attacking = stateInfo.IsName ("Player_attack");


		if (Input.GetKeyDown ("space") && !attacking) {
			playerAnimator.SetTrigger ("attacking");
		}

		if (hasUpdatePlayerAnimation ()) {
			attackCollider.offset = new Vector2 (playerMovement.x / 2, playerMovement.y / 2);
		}

		if (attacking) {
			float playbackTime = stateInfo.normalizedTime;

			if (playbackTime > 0.33 && playbackTime < 0.66) {
				attackCollider.enabled = true;
			} else {
				attackCollider.enabled = false;
			}
		}
	}

	void SlashAttack() {
		AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo (0);
		bool loading = stateInfo.IsName ("Player_slash");

		if (Input.GetKeyDown (KeyCode.LeftAlt)) {
			playerAnimator.SetTrigger ("loading");
			aura.StartAura ();
		} else if(Input.GetKeyUp(KeyCode.LeftAlt)) {
			playerAnimator.SetTrigger("attacking");

			if (aura.IsLoaded()) {
                // Obtenemos los angulos hacia donde esta mirando el player
                // Segun la animacion.
                float[] anglePosition = GetAnglePosition();
                
                float angle = Mathf.Atan2(
                    anglePosition[0],
                    anglePosition[1]
                ) * Mathf.Rad2Deg;

                // Creamos la instancia de Slash
                GameObject slashObj = Instantiate(
                    slashPrefab, transform.position,
                    Quaternion.AngleAxis(angle, Vector3.forward));

                // Asignamos el movimiento
                SlashController slash = slashObj.GetComponent<SlashController>();
                slash.movement.x = playerAnimator.GetFloat("movementX");
                slash.movement.y = playerAnimator.GetFloat("movementY");


            }
			aura.StopAura ();
			// Esperamos unos segundos y continuamos con el movimiento
			// del personaje
			StartCoroutine(EnableMovementAfter(0.4f));
		}

		if (loading) {
			movePrevent = true;
		}
	}

    float[] GetAnglePosition() {
        return new float[] {
            playerAnimator.GetFloat("movementY"),
            playerAnimator.GetFloat("movementX")
        };

    }

    void movePlayerWithClick() {
        if (Input.GetMouseButtonDown(0)) {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition.z = 0f;
        }

        // Move the player;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        playerMovement = targetPosition;
    }

	void Awake() {
		Assert.IsNotNull (initialMap);
		Assert.IsNotNull (slashPrefab);
	}

	// Use this for initialization
	void Start () {
        // At start the target position is the current position
        targetPosition = transform.position;

        playerAnimator = GetComponent<Animator> ();
		rb2d = GetComponent<Rigidbody2D> ();
		attackCollider = transform.GetChild (0).GetComponent<CircleCollider2D> ();
		attackCollider.enabled = false;

		// We execute the method SetBound on the CameraController.cs
		Camera.main.GetComponent<CameraController> ().SetBound (initialMap);

		aura = transform.GetChild (1).GetComponent<Aura> ();
	}
	
	// Update is called once per frame
	void Update () {

        // movePlayerWithClick();

        // Getting player movement
        playerMovement = getPlayerMovement ();

		if (hasUpdatePlayerAnimation ()) {
			changePlayerPositionView (playerMovement);
			changePlayerAnimationToWalking ();
		} else {
			changePlayerAnimationToIdle ();
		}

		SwordAttack ();

		SlashAttack ();

		PreventMovement ();

	}

	void FixedUpdate() {
		setTransformMovement (playerMovement);
	}

	void PreventMovement() {
		if (movePrevent) {
			playerMovement = Vector2.zero;
		}
	}

	IEnumerator EnableMovementAfter(float seconds) {
		yield return new WaitForSeconds (seconds);
		movePrevent = false;
	}
}

