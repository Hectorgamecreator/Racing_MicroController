using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class PlayerController : MonoBehaviour
    {
        public bool JumpBoost = false;

        [SerializeField] private float jumpForce = 10;
        [SerializeField] private int JumpPower = 10;
        private float moveInput;

        public bool speedUp = false;
        [SerializeField] private float movingSpeed = 5;
        [SerializeField] private int speedBoost = 5;

        private bool facingRight = false;
        [HideInInspector]
        public bool deathState = false;

        private bool isGrounded;
        public Transform groundCheck;

        private Rigidbody2D rigidbody;
        private Animator animator;
        private GameManager gameManager;


        private Vector3 respawnPoint;
        public GameObject fallDetector;

        void Start()
        {
            rigidbody = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            respawnPoint = transform.position;
        }

        private void FixedUpdate()
        {
            CheckGround();
        }

        void Update()
        {
            if (Input.GetButton("Horizontal")) 
            {
                moveInput = Input.GetAxis("Horizontal");
                Vector3 direction = transform.right * moveInput;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, movingSpeed * Time.deltaTime);
                animator.SetInteger("playerState", 1); // Turn on run animation
            }
            else
            {
                if (isGrounded) animator.SetInteger("playerState", 0); // Turn on idle animation
            }
            if(Input.GetKeyDown(KeyCode.Space) && isGrounded )
            {
                rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
            if (!isGrounded)animator.SetInteger("playerState", 2); // Turn on jump animation

            if(facingRight == false && moveInput > 0)
            {
                Flip();
            }
            else if(facingRight == true && moveInput < 0)
            {
                Flip();
            }

            fallDetector.transform.position = new Vector2(transform.position.x, fallDetector.transform.position.y);
        }

        private void Flip()
        {
            facingRight = !facingRight;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
        }

        private void CheckGround()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.2f);
            isGrounded = colliders.Length > 1;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                deathState = true; // Say to GameManager that player is dea

            }
            else
            {
                deathState = false;
                
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "FallDetector")
            {
                transform.position = respawnPoint;
            }
            else if (collision.tag == "CheckPoint")
            {
                respawnPoint = transform.position;
            }

            else if (collision.gameObject.tag == "Coin")
            {
                gameManager.coinsCounter += 1;
                Destroy(collision.gameObject);
            }
            else if (collision.tag == "NextLevel")
            {
                SceneManager.LoadScene("Win");
                //respawnPoint = transform.position;
            }
        }

        public void SpeedUpEnabled()
        {
            speedUp = true;
            movingSpeed *= speedBoost;
            StartCoroutine(SpeedUpDisableRoutine());
        }

        IEnumerator SpeedUpDisableRoutine()
        {
            yield return new WaitForSeconds(2.0f);

            movingSpeed /= speedBoost;
        }

        public void JumpBoostEnabled()
        {
            JumpBoost = true;
            jumpForce *= JumpPower;
            StartCoroutine(JumpBoostDisableRoutine());
        }

        IEnumerator JumpBoostDisableRoutine ()
        {
            yield return new WaitForSeconds(3.0f);

            jumpForce /= JumpPower;
        }
    }
}
