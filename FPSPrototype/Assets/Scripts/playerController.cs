using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour,
    // Interfaces
    IDamage
{
    [SerializeField] private int hp;
    [SerializeField] float speed;
    [SerializeField] float sprintModifier;
    [SerializeField] int numberOfJumps;
    [SerializeField] float jumpSpeed;
    [SerializeField] float gravity;

    [SerializeField] CharacterController controller;
    [SerializeField] AudioSource aud;
    [SerializeField] LayerMask ignoreMask;
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject bullet;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] float shootDist;
    [SerializeField] Transform shotPosition;
    [SerializeField] Renderer muzzleFlash;

    [SerializeField] AudioClip[] audStep;
    [SerializeField] float audStepVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    int maxHP;
    int jumpCount;
    bool isSprinting;
    bool isShooting;
    bool isPlayingStep;
    bool hasGun = false;
    int selectGunPos;
    

    [SerializeField] float slideDuration;
    [SerializeField] float slideInitialSpeedMultiplier;
    [SerializeField] float slideEndSpeedMultiplier;
    [SerializeField] float slideSpeedMultiplier;
    bool isSliding;
    bool jumpCancel;
    Vector3 slideMomentum;

    //Getter/setter for HP
    public int HP
    {
        get { return hp; }
        set { hp = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        maxHP = HP;
        UpdatePlayerUI();
    }

    // Update is called once per frame  
    void Update()
    {
        // If we aren't paused
        if (!GameManager.GetInstance().IsPaused)
        {
            Movement();
            SelectGun();
            // If we aren't shooting already
            if (Input.GetButton("Fire1") && hasGun && gunList[selectGunPos].ammoCur > 0 && !isShooting)
            {
                StartCoroutine(Shoot());
            }
            if(Input.GetButton("Reload") && !isShooting)
            {
                Reload();
                
            }

            if (Input.GetButtonDown("Slide") && controller.isGrounded && !isSliding)
            {
                StartCoroutine(Slide());
            }

            if (Input.GetButtonDown("Jump") && isSliding)
            {
                jumpCancel = true;
            }
        }

        Sprint();
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

        if (!isPlayingStep && controller.isGrounded && moveDirection.magnitude > 0)
        {
            StartCoroutine(PlayStep());
        }

        if (!isSliding)
        {
            controller.Move(moveDirection * speed * Time.deltaTime);
        }

        // Move player in X and Z axis
        //controller.Move(moveDirection * speed * Time.deltaTime);
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
                aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            }
            else if (!isSliding)
            {
                // Increase jump count
                jumpCount++;
                // Set player Y velocity
                playerVelocity.y = jumpSpeed * 2.0f;

                aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);

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
        GameManager.GetInstance().playerHPBarFill.fillAmount = (float)HP / maxHP;
        if (gunList.Count > 0)
        {
            GameManager.GetInstance().ammoCurrent.text = gunList[selectGunPos].ammoCur.ToString("F0");
            GameManager.GetInstance().ammoMax.text = gunList[selectGunPos].ammoMax.ToString("F0");
        }

    }

    IEnumerator PlayStep()
    {
        isPlayingStep = true;
        aud.PlayOneShot(audStep[Random.Range(0, audStep.Length)], audStepVol);
        yield return new WaitForSeconds(isSprinting ? 0.3f : 0.5f);
        isPlayingStep = false;
    }

    IEnumerator Shoot()
    {
        if (!hasGun)
        {
              yield break;
        }
        isShooting = true;
        gunList[selectGunPos].ammoCur--;
        UpdatePlayerUI();

        aud.PlayOneShot(gunList[selectGunPos].shootSound[Random.Range(0, gunList[selectGunPos].shootSound.Length)], gunList[selectGunPos].shootVol);


        // Instantiate bullet
        Instantiate(bullet, shotPosition.position, Camera.main.transform.rotation);
        StartCoroutine(FlashMuzzle());
        // Wait for shot rate
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
        
    }

    IEnumerator FlashMuzzle()
    {
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.enabled = false;

    }

    public void TakeDamage(int amount)
    {
        //subtract player health
        HP -= amount;
        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        // Update UI
        UpdatePlayerUI();
        StartCoroutine(DamageFlash());

        if (HP <= 0)
        {
            //the player is dead
            GameManager.GetInstance().YouLose();
        }
    }

    IEnumerator DamageFlash()
    {
        GameManager.GetInstance().damagePanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.GetInstance().damagePanel.SetActive(false);
    }


    public void GetGunStats(GunStats gun)
    {
        gunList.Add(gun);
        selectGunPos = gunList.Count - 1;
        UpdatePlayerUI();

        shootDamage = gun.shootDamage;
        shootDist = gun.shootDist;
        shootRate = gun.shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        hasGun = true;
    }

    void ChangeGun()
    {
        UpdatePlayerUI();
        shootDamage = gunList[selectGunPos].shootDamage;
        shootDist = gunList[selectGunPos].shootDist;
        shootRate = gunList[selectGunPos].shootRate;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectGunPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectGunPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }


    void SelectGun()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectGunPos < gunList.Count - 1)
        {
            selectGunPos++;
            ChangeGun();

        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectGunPos > 0)
        {
            selectGunPos--;
            ChangeGun();
        }
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

    void Reload()
    {
        if (gunList.Count > 0 && !isShooting)
        {
            int ammoNeeded = gunList[selectGunPos].ammoMax - gunList[selectGunPos].ammoCur;
            if (ammoNeeded > 0)
            {
                int ammoToLoad = Mathf.Min(ammoNeeded, gunList[selectGunPos].ammoMax);
                gunList[selectGunPos].ammoCur += ammoToLoad;
                //gunList[selectGunPos].ammoMax -= ammoToLoad;
                UpdatePlayerUI();
            }
        }

        //if(gunList.Count > 0)
        //{
        // gunList[selectGunPos].ammoCur = gunList[selectGunPos].ammoMax;
        //UpdatePlayerUI();
        //}
    }

    public void AddAmmoToCurrentGun(int amount)
    {
        if (gunList.Count > 0)
        {
            gunList[selectGunPos].ammoMax += amount;
            UpdatePlayerUI(); 
        }
    }

}
    

