using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Walking Running Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] float accelerationSpeed;
    [SerializeField] float velocityFallOff;
    [SerializeField] float adsMoveSpeedPunishment;
    [SerializeField] float sprintingSpeedMultiplyer;
    [SerializeField] CharacterController controller;
    [SerializeField] Vector2 cameraShakePerFootstep;
    [Header("Jumping")]
    [SerializeField] float jumpVel;
    [SerializeField] float jumpVelFalloff;
    [SerializeField] float jumpEffectTime;
    [SerializeField] float gravity;
    [SerializeField] float airMovementPunishmentMultiplyer;
    [SerializeField] float coyoteFrames;

    float framesUntillGrounded;

    [Header("Camera")]
    [SerializeField] GameObject cameraGameObject;
    [SerializeField] Vector2 camBounds;

    [SerializeField] float mouseSencitivity;
    [Header("Audio")]
    [SerializeField] float distanceBetweenFootstep;
    [Header("Canter")]
    [SerializeField] float tiltAngle;
    [SerializeField] float tiltSpeed;
    [SerializeField] float canterMoveSpeedMultiplyer;
    [Header("Crouching")]
    [SerializeField] float height;
    [SerializeField] float crouchHeight;
    [SerializeField] float crouchSpeed;
    [SerializeField] float timeToCrouch;
    [SerializeField] float crouchMoveSpeedMultiplyer;

    [SerializeField] Animator sprintingAnimator;
    Vector3 velocity;
    Vector3 movementVector;
    Vector2 cameraRotateVelocity;
    float yVelocity;
    static public PlayerController playerInstance;
    bool justJumped = false;
    Vector3 lastPos;
    float lastFootStepDistance;
    [HideInInspector] public bool isAdsIng;
    CapsuleCollider capsualCol;



    float currentHeight;

    public static bool isSprinting;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInstance = this;
        lastPos = transform.position;
        capsualCol = GetComponent<CapsuleCollider>();
        currentHeight = height;
        isSprinting = false;

        
    }
    void Update()
    {
        if (UICommunicator.gamePaused)
            return;

        VerticalMovement();

        DebugManager.DisplayInfo("PGrounded", "Grounded: " + GroundCheck());
        DebugManager.DisplayInfo("YVel", "Y Velocity: " + yVelocity.ToString());
        DebugManager.DisplayInfo("PlayerVelocity", "Player Vel:" + velocity.ToString());

        movementVector = Vector3.zero;
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.z = Input.GetAxisRaw("Vertical");

        Crouch();
        TrySprint();
        CaculateVelocity(); 
        DoMovement(); 
        FootStep();

        MoveCameraFromVelocity();
        CameraRotation();
        TacticalTilt();


    }
    bool crouching = false;
    float currentSprintMultiplyer;
    float crouchVel;
   
    void Crouch()
    {
        if (GameControllsManager.toggleCrouch)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouching = !crouching;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                crouching = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                crouching = false;
            }
        }

        if (crouching)
        {
            /*  currentHeight -= crouchSpeed * Time.deltaTime;
              if (currentHeight <= crouchHeight)
                  currentHeight = crouchHeight;

              controller.height = currentHeight;
              capsualCol.height = currentHeight;*/
            currentHeight = Mathf.SmoothDamp(controller.height, crouchHeight, ref crouchVel, timeToCrouch);
            controller.height = currentHeight;
            capsualCol.height = currentHeight;
            
        }
        else 
        {
            float previousHeight = currentHeight;
            /*
            currentHeight += crouchSpeed * Time.deltaTime;
            if (currentHeight >= height)
                currentHeight = height;

            controller.height = currentHeight;
            capsualCol.height = currentHeight;
            */

            currentHeight = Mathf.SmoothDamp(controller.height, height, ref crouchVel, timeToCrouch);
            controller.height = currentHeight;
            capsualCol.height = currentHeight;

            // Adjust the player's position to reflect the height change
          //  Vector3 pos = transform.position;
          //  pos.y += (currentHeight - previousHeight); // Adjust position by the height difference
            controller.Move(new Vector3(0, (currentHeight - previousHeight), 0)); // Ensure the position update
        }

        DebugManager.DisplayInfo("heigh", "Height: " + currentHeight);
    }


#region TacticalTilt \ Canter
    bool isCantering;
    bool canterLeft; // what direction the canter is held in
 

    [SerializeField] float timeToFullyTilt;
  

 
   
    float currentTiltAngle;
    float tiltVelocity;

    void TacticalTilt()
    {
        float targetTiltAngle = 0f;

        if (GameControllsManager.toggleCanter)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isSprinting)
            {
                isCantering = !isCantering;
                canterLeft = true;
            }
            else if (Input.GetKeyDown(KeyCode.Q) && !isSprinting)
            {
                isCantering = !isCantering;
                canterLeft = false;
            }
        }
        else
        {
            isCantering = true;
            if (Input.GetKey(KeyCode.E) && !isSprinting)
            {
                canterLeft = true;
            }
            else if (Input.GetKey(KeyCode.Q) && !isSprinting)
            {
                canterLeft = false;
            }
            else
            {
                isCantering = false;
            }
        }

        if (isCantering)
        {
            targetTiltAngle = canterLeft ? -tiltAngle : tiltAngle;
        }

        currentTiltAngle = Mathf.SmoothDamp(currentTiltAngle, targetTiltAngle, ref tiltVelocity, timeToFullyTilt, tiltSpeed);
       
        if (currentTiltAngle == 0)
            return;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentTiltAngle);
    }

    #endregion
#region Vertical Movement
    void VerticalMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GroundCheck())
            StartCoroutine(Jump());

        if (GroundCheck() && !justJumped)
        {
            yVelocity = -1;
         /*   Vector3 pos = transform.position;
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);
            pos.y = hit.point.y;
            float dist = pos.y - transform.position.y;
            controller.Move(new Vector3(0, dist, 0));*/
            //transform.position = pos;
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
        }

        controller.Move(new Vector3(0, yVelocity * Time.deltaTime, 0)); // caculate falling velocity
    }
    #endregion
#region Camera
    void MoveCameraFromVelocity()
    {
        if (cameraRotateVelocity != Vector2.zero)
        {

            float newRotationY = transform.localEulerAngles.y + cameraRotateVelocity.y;

            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, newRotationY, transform.localEulerAngles.z);


            Vector3 cameraRotation = cameraGameObject.transform.localEulerAngles;


            float desiredXRotation = cameraRotation.x - cameraRotateVelocity.x;
            if (desiredXRotation > 180f) desiredXRotation -= 360f;
            if (desiredXRotation < -180f) desiredXRotation += 360f;


            cameraGameObject.transform.localEulerAngles = new Vector3(Mathf.Clamp(desiredXRotation, camBounds.x, camBounds.y), cameraRotation.y, cameraRotation.z);

            cameraRotateVelocity = Vector2.zero;
        }
    }

    void CameraRotation()
    {
        float mouseX = -Input.GetAxis("Mouse X") * mouseSencitivity * GameControllsManager.mouseSense ;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSencitivity * GameControllsManager.mouseSense ;

        if (mouseX == 0 && mouseY == 0)
            return;

        cameraRotateVelocity.x += mouseY;
        cameraRotateVelocity.y -= mouseX;


    }

    public void AddCameraRotation(Vector2 vector, float duration, float magnitude)
    {
        StartCoroutine(CameraRotate(vector, duration, magnitude));
    }

    IEnumerator CameraRotate(Vector2 vector, float duration, float magnitude)
    {
        float timer = duration;
     
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            cameraRotateVelocity += vector * Time.deltaTime * magnitude;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    #endregion
#region GroundCheck
    bool GroundCheck()
    {
        

        bool hit = Physics.Raycast(transform.position, Vector3.down, currentHeight / 2 + 0.10f);
        if (hit)
        {
            
            framesUntillGrounded = coyoteFrames;
            return true;
        }


        Vector3 offset = transform.position;
        offset.z += controller.radius / 3; // do slightly infront of the collider

        hit = Physics.Raycast(offset, Vector3.down, currentHeight / 2 + 0.12f);

        if (hit)
        {
            framesUntillGrounded = coyoteFrames;
            return true;
        }

        offset.z -= controller.radius / 3; // revert the last move

        offset.z -= controller.radius / 3; // do slightly behind the collider

        hit = Physics.Raycast(offset, Vector3.down, currentHeight / 2 + 0.12f);
        if (hit)
        {
            framesUntillGrounded = coyoteFrames;
            return true;
        }



        framesUntillGrounded -= 1;
        if (framesUntillGrounded <= 0)
            return false;
        else
            return true;
    }
    #endregion
#region BasicMovement\Velocity
    float targetMaxVelocity;
    float velocityEaser = 1;
    float currentXVelocity = 0;
    float currentZVelocity = 0;
    bool CaculateVelocity() // returns true if player has any velocity
    {
        
        if (movementVector.x == 0)
            velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref currentXVelocity, 1 / velocityFallOff);
        if(movementVector.z == 0)
            velocity.z = Mathf.SmoothDamp(velocity.z, 0, ref currentZVelocity, 1 / velocityFallOff);



        if (velocity == Vector3.zero && movementVector == Vector3.zero) // if both are zero, we dont need to caculate any movement
            return false;

        // caculate the new velocity based off player input

        float airMultiplyer = 1;
        if (!GroundCheck())
            airMultiplyer = airMovementPunishmentMultiplyer;


        if (!isAdsIng)
        {
            targetMaxVelocity = 1;
            velocityEaser += Time.deltaTime;
        }
        else
        {
            targetMaxVelocity = adsMoveSpeedPunishment;
            velocityEaser -= Time.deltaTime;
        }
        velocityEaser = Mathf.Clamp(velocityEaser, targetMaxVelocity, 1);

        velocity += (movementVector * airMultiplyer) * accelerationSpeed * Time.deltaTime; // add the current move vector to the velocity. 
        velocity = Vector3.ClampMagnitude(velocity, velocityEaser);


        //ensure that the velocity does not go over 1
        if (velocity.x > 1)
            velocity.x = 1;
        else if (velocity.x < -1)
            velocity.x = -1;

        if (velocity.z > 1)
            velocity.z = 1;
        else if (velocity.z < -1)
            velocity.z = -1;

        return true;

    }

    void DoMovement()
    {
        Vector3 playerRot = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);

        Quaternion playerQaut = Quaternion.Euler(playerRot);

        Vector3 newFoward = playerQaut * Vector3.forward;
        Vector3 newRight = playerQaut * Vector3.right;

        Vector3 moveVector = newRight * velocity.x + newFoward * velocity.z;


        controller.Move(moveVector * moveSpeed * (currentHeight / height * crouchMoveSpeedMultiplyer) * currentSprintMultiplyer * (isCantering == false ? 1 : canterMoveSpeedMultiplyer) * Time.deltaTime);
    }

    void TrySprint()
    {
        DebugManager.DisplayInfo("sprint", "Sprint: " + currentSprintMultiplyer);
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isSprinting = true;
            GetComponent<GunManager>().CancelADS();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }

        if (isSprinting)
        {
            currentSprintMultiplyer += accelerationSpeed * Time.deltaTime;
            if (currentSprintMultiplyer >= sprintingSpeedMultiplyer)
                currentSprintMultiplyer = sprintingSpeedMultiplyer;

            sprintingAnimator.SetBool("sprinting", true);
        }
        else
        {
            currentSprintMultiplyer -= velocityFallOff * Time.deltaTime;
            if (currentSprintMultiplyer <= 1)
                currentSprintMultiplyer = 1;

            sprintingAnimator.SetBool("sprinting", false);
        }

    }

    #endregion
#region FootSteps
    void FootStep()
    {
        if (!GroundCheck() || movementVector == Vector3.zero)
            return;

        lastFootStepDistance += Vector3.Distance(transform.position, lastPos);
        DebugManager.DisplayInfo("footstep", "FootStep:" + lastFootStepDistance);
        if (lastFootStepDistance >= distanceBetweenFootstep)
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, currentHeight / 2 + 0.15f))
            {
                AudioClip[] clips = MaterialPropertiesManager.GetFootStepSounds(hit.transform.gameObject);
                AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length - 1)], transform.position, 0.5f * GameControllsManager.audioVolume);
                lastFootStepDistance = 0;

                
            }


        }

        lastPos = transform.position;
    }
    #endregion
#region Jumping
    
    float currentJumpingVelocity;
  
    IEnumerator Jump()
    {
        justJumped = true;
       
        yVelocity = jumpVel;
      
        float timer = jumpEffectTime;
        currentJumpingVelocity = jumpVel;
        while (timer > 0)
        {

            timer -= Time.deltaTime;
           

            yVelocity += currentJumpingVelocity * Time.deltaTime;
            currentJumpingVelocity -= jumpVelFalloff * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        justJumped = false;
        yield break;
    }
#endregion

}
