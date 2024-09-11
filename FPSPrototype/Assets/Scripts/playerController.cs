using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float sprintModifier;
    [SerializeField] int numberOfJumps;
    [SerializeField] float jumpSpeed;
    [SerializeField] float gravity;
    [SerializeField] float shotRate;

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] GameObject bullet;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] Transform shotPosition;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    int jumpCount;
    bool isSprinting;
    bool isShooting;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Sprint();

        // If we aren't shooting already and we aren't paused
        if (Input.GetButton("Fire1") && !isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    void Movement()
    {
        // Check if player is grounded
        if (controller.isGrounded)
        {
            // Reset velocity and jumps
            playerVelocity = Vector3.zero;
            jumpCount = 0;
        }
        // Calculate move direction
        moveDirection = (
            Input.GetAxis("Horizontal") * transform.right +
            Input.GetAxis("Vertical") * transform.forward
        );
        // Move player in X and Z axis
        controller.Move(moveDirection * speed * Time.deltaTime);
        // Check if pressing jump and if we can jump
        if (Input.GetButtonDown("Jump") && jumpCount < numberOfJumps)
        {
            // Increase jump count
            jumpCount++;
            // Set player Y velocity
            playerVelocity.y = jumpSpeed * 2.0f;
        }
        // Move player in Y axis
        controller.Move(playerVelocity * Time.deltaTime);
        // Enact gravity on player
        playerVelocity.y -= gravity * 2.0f * Time.deltaTime;
    }

    void Sprint()
    {
        // Check if pressing sprint
        if (Input.GetButtonDown("Sprint"))
        {
            // Multiply by modifier
            isSprinting = true;
            speed *= sprintModifier;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            // Divide by modifier
            isSprinting = false;
            speed /= sprintModifier;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        // Instantiate bullet
        Instantiate(bullet, shotPosition.position, transform.rotation);
        // Wait for shot rate
        yield return new WaitForSeconds(shotRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        //subtract player health
        HP -= amount;  
        
        if(HP <= 0)
        {
            //the player is dead
            gameManager.instance.youLose();
        }
    }
}
