using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] public int HP;
    [SerializeField] float speed;
    [SerializeField] float sprintModifier;
    [SerializeField] int numberOfJumps;
    [SerializeField] float jumpSpeed;
    [SerializeField] float gravity;

    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreMask;

    [SerializeField] GameObject bullet;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] Transform shotPosition;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    int maxHP;
    int jumpCount;
    bool isSprinting;
    bool isShooting;

    [SerializeField] float slideDuration;
    [SerializeField] float slideInitialSpeedMultiplier;
    [SerializeField] float slideEndSpeedMultiplier;
    [SerializeField] float slideSpeedMultiplier;
    bool isSliding;
    bool jumpCancel;
    Vector3 slideMomentum;

    // Start is called before the first frame update
    void Start()
    {
        maxHP = HP;
        UpdatePlayerUI();
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

        if (Input.GetButtonDown("Slide") && controller.isGrounded && !isSliding)
        {
            StartCoroutine(Slide());
        }

        if(Input.GetButtonDown("Jump") && isSliding)
        {
            jumpCancel = true;
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

        if (!isSliding)
        {
            controller.Move(moveDirection * speed * Time.deltaTime);
        }

        // Move player in X and Z axis
        controller.Move(moveDirection * speed * Time.deltaTime);
        // Check if pressing jump and if we can jump
        if (Input.GetButtonDown("Jump") && jumpCount < numberOfJumps)
        {
            if (isSliding && jumpCancel)
            {
                Vector3 currentForward = Camera.main.transform.forward;
                currentForward.y = 0;
                currentForward.Normalize();

                playerVelocity = currentForward * slideMomentum.magnitude;
                playerVelocity.y = jumpSpeed * 2.0f;
                playerVelocity.y = jumpSpeed * 2.0f;
                isSliding = false;
                jumpCancel = false;
            }
            else if (!isSliding)
            {
                // Increase jump count
                jumpCount++;
                // Set player Y velocity
                playerVelocity.y = jumpSpeed * 2.0f;
            }
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

    void UpdatePlayerUI()
    {
        gameManager.instance.playerHPBarFill.fillAmount = (float)HP / maxHP;
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        // Instantiate bullet
        Instantiate(bullet, shotPosition.position, Camera.main.transform.rotation);
        // Wait for shot rate
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int amount)
    {
        //subtract player health
        HP -= amount;
        // Update UI
        UpdatePlayerUI();
        StartCoroutine(damageFlash());

        if (HP <= 0)
        {
            //the player is dead
            gameManager.instance.youLose();
        }
    }

    IEnumerator damageFlash()
    {
        gameManager.instance.damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damagePanel.SetActive(false);
    }


    IEnumerator Slide()
    {
        isSliding = true;
        jumpCancel = false;

        float initialSpeed = speed * slideInitialSpeedMultiplier;
        float finalSpeed = speed * slideEndSpeedMultiplier;
        float currentSpeed = initialSpeed;

        Vector3 slideDirection = transform.forward;
        slideMomentum = slideDirection * initialSpeed;

        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            // Linearly interpolate between initialSpeed and finalSpeed based on elapsed time
            currentSpeed = Mathf.Lerp(initialSpeed, finalSpeed, elapsedTime/slideDuration);

            // Move the player forward with the current speed
            controller.Move(slideDirection * currentSpeed * Time.deltaTime);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isSliding = false;

        if(jumpCancel)
        {
            playerVelocity = slideMomentum;
            playerVelocity.y = jumpSpeed * 2.0f;
            jumpCancel = false;
        }
    }
}
    

