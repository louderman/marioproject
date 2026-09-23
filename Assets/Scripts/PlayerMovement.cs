using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// TODO: unity complained about me using legacy input handling. maybe recode this later down the line? priority: low
public class PlayerMovement : MonoBehaviour
{
    public float speed = 10;
    public float upSpeed = 10;
    public float maxSpeed = 20;   
    private Rigidbody2D marioBody;
    private bool onGroundState = true;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public Transform enemy;
    public JumpOverGoomba scoring;
    [SerializeField] private GameObject gameOverPanel; 
    [SerializeField] private TMP_Text finalScore; 
    private Vector2 startingMarioPosition;
    private Vector3 startingEnemyLocalPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Set to be 30 FPS
        Application.targetFrameRate =  30;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        startingMarioPosition = marioBody.position;
        if (enemy != null) startingEnemyLocalPosition = enemy.localPosition;

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground")) onGroundState = true;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            finalScore.text = "Score: " + scoring.score;                                                                                 
            gameOverPanel.SetActive(true);  
            Time.timeScale = 0f;
        }
    }

    public void RestartButtonCallback()
    {
        if (enemy == null || scoring == null)
        {
            Debug.LogError("Assign Enemy and Scoring on Mario's PlayerMovement component.");
            return;
        }

        Time.timeScale = 1f;
        marioBody.position = startingMarioPosition;
        marioBody.linearVelocity = Vector2.zero;
        enemy.localPosition = startingEnemyLocalPosition;
        faceRightState = true;
        marioSprite.flipX = false;
        onGroundState = true;
        scoring.ResetScore();
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update(){
              // toggle state
      if (Input.GetKeyDown("a") && faceRightState){
          faceRightState = false;
          marioSprite.flipX = true;
      }

      if (Input.GetKeyDown("d") && !faceRightState){
          faceRightState = true;
          marioSprite.flipX = false;
      }
    }

    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(moveHorizontal) > 0){
            Vector2 movement = new Vector2(moveHorizontal, 0);
            // check if it doesn't go beyond maxSpeed
            if (marioBody.linearVelocity.magnitude < maxSpeed)
                    marioBody.AddForce(movement * speed);
        }

        // stop
        if (Input.GetKeyUp("a") || Input.GetKeyUp("d")){
            // stop
            marioBody.linearVelocity = Vector2.zero;
        }


        if (Input.GetKeyDown("space") && onGroundState){
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
        }
    }
  }
