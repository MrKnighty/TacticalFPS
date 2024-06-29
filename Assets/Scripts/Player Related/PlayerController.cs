using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float accelerationSpeed;
    [SerializeField] float velocityFallOff;

    [SerializeField] float jumpVel;
    [SerializeField] float jumpEffectTime;
    [SerializeField] float gravity;
    [SerializeField] float airMovementPunishmentMultiplyer;
    [SerializeField] float adsMoveSpeedPunishment;

    [SerializeField] GameObject cameraGameObject;

    [SerializeField] float mouseSencitivity;

    [SerializeField] CharacterController controller;
    Vector3 velocity;

    Vector3 movementVector;

    Vector2 cameraRotateVelocity;

    float yVelocity;

    static public PlayerController playerInstance;

    bool justJumped = false;

    [SerializeField] float distanceBetweenFootstep;
    Vector3 lastPos;
    float lastFootStepDistance;

    [SerializeField] float tiltAngle;
    [SerializeField] float tiltSpeed;
    bool tiltingLeft;
    bool tiltingRight;
    [HideInInspector] public bool isAdsIng;

    CapsuleCollider capsualCol;
    [SerializeField] float height, crouchHeight;
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchMoveSpeedMultiplyer;


    float currentHeight;
 



    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInstance = this;
        lastPos = transform.position;
        capsualCol = GetComponent<CapsuleCollider>();
        currentHeight = height;


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


        CaculateVelocity(); // caculate velocity, and only move if the player has any
        DoMovement(); // caculate movement

        if (Time.deltaTime <= 0.2f) // dirty fix to stop camera from snapping down during scene start
            CameraRotation();

        MoveCameraFromVelocity();
        TacticalTilt();

        Crouch();




    }
    bool crouching = false;
    void Crouch()
    {
        if(GameControllsManager.toggleCrouch)
        {
            if(Input.GetKeyDown(KeyCode.LeftControl))
            {
                crouching = !crouching;
            }
        }
        else
        {
            if(Input.GetKey(KeyCode.LeftControl))
            {
                crouching = true;
            }
            if(Input.GetKeyUp(KeyCode.LeftControl))
            {
                crouching = false;
            }
        }

        if (crouching)
        {
            currentHeight -= crouchSpeed * Time.deltaTime;
            if (currentHeight <= crouchHeight)
                currentHeight = crouchHeight;

            controller.height = currentHeight;
            capsualCol.height = currentHeight;
        }
        else if (currentHeight < height)
        {
            currentHeight += crouchSpeed * Time.deltaTime;
            if (currentHeight >= height)
                currentHeight = height;
            controller.height = currentHeight;
            capsualCol.height = currentHeight;
            Vector3 pos = transform.position;
            pos.y += crouchSpeed * Time.deltaTime;
            transform.position = pos;
        }

        DebugManager.DisplayInfo("heigh", "Height" + currentHeight);


    }
    bool isCantering;
    bool canterLeft;
    bool canterRight;
    bool canterDirL;

    void TacticalTilt()
    {

        float zRotation = Camera.main.transform.localEulerAngles.z;

        if (zRotation > 180)
        {
            zRotation -= 360;
        }
        DebugManager.DisplayInfo("ISCantering", "Is Cantering" + isCantering.ToString());

        if (GameControllsManager.toggleCanter)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isCantering = !isCantering;
                canterLeft = true;

            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                isCantering = !isCantering;
                canterLeft = false;
            }

            if (isCantering)
            {
                if (canterLeft)
                    zRotation -= tiltSpeed * Time.deltaTime;
                else
                    zRotation += tiltSpeed * Time.deltaTime;


                if (zRotation > 0)
                    canterDirL = true;
                else
                    canterDirL = false;
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.E))
            {
                zRotation -= tiltSpeed * Time.deltaTime;
                isCantering = true;
                if (zRotation > 0)
                    canterDirL = true;
                else
                    canterDirL = false;

            }
            else if (Input.GetKey(KeyCode.Q))
            {
                zRotation += tiltSpeed * Time.deltaTime;
                isCantering = true;
                if (zRotation > 0)
                    canterDirL = true;
                else
                    canterDirL = false;

            }
            else
            {
                isCantering = false;
            }
        }


        if (!isCantering)
        {
            print("oi");
            if (zRotation > 0)
            {

                zRotation -= tiltSpeed * Time.deltaTime;
            }
            else if (zRotation < 0)
            {
                zRotation += tiltSpeed * Time.deltaTime;
            }
        }

        if (canterDirL && zRotation <= 0)
        {
            zRotation = 0;
        }
        else if (!canterDirL && zRotation >= 0)
        {
            zRotation = 0;
        }
        if (zRotation == 0)
            return;

        zRotation = Mathf.Clamp(zRotation, -tiltAngle, tiltAngle);
        Camera.main.transform.localEulerAngles = new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.localEulerAngles.y, zRotation);
    }

    void VerticalMovement()
    {
        if (Input.GetKeyDown(KeyCode.Space) && GroundCheck())
            StartCoroutine(Jump());

        if (GroundCheck() && !justJumped)
        {
            yVelocity = 0f;
            Vector3 pos = transform.position;
            Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit);
            pos.y = hit.point.y;
            transform.position = pos;
        }
        else
        {
            yVelocity -= gravity * Time.deltaTime;
        }

        controller.Move(new Vector3(0, yVelocity * Time.deltaTime, 0)); // caculate falling velocity
    }


    void MoveCameraFromVelocity()
    {

        if (cameraRotateVelocity != Vector2.zero)
        {
            transform.Rotate(Vector3.up * cameraRotateVelocity.y);

            Vector3 cameraRotation = cameraGameObject.transform.localEulerAngles;


            float desiredXRotation = cameraRotation.x - cameraRotateVelocity.x;
            if (desiredXRotation > 180f) desiredXRotation -= 360f;
            if (desiredXRotation < -180f) desiredXRotation += 360f;

            cameraGameObject.transform.localEulerAngles = new Vector3(Mathf.Clamp(desiredXRotation, -45, 45), cameraRotation.y, cameraRotation.z);

            cameraRotateVelocity = Vector2.zero;
        }
    }

    bool GroundCheck()
    {
        bool hit = Physics.Raycast(transform.position, Vector3.down, currentHeight / 2 + 0.12f);
        if (hit)
            return true;
        Vector3 offset = transform.position;
        offset.z += controller.radius / 3; // do slightly infront of the collider

        hit = Physics.Raycast(offset, Vector3.down, currentHeight / 2 + 0.15f);
        if (hit)
            return true;

        offset.z -= controller.radius / 3; // revert the last move

        offset.z -= controller.radius / 3; // do slightly behind the collider

        hit = Physics.Raycast(offset, Vector3.down, currentHeight / 2 + 0.15f);
        if (hit)
            return true;
        return false;
    }
    float targetMaxVelocity;
    float velocityEaser = 1;

    bool CaculateVelocity() // returns true if player has any velocity
    {
        if (movementVector.x == 0) // if there is no movement on the x, start taking away velocity
        {
            if (velocity.x > 0) // if the velocity is in the negetive (going left) we need to raise up the velocity back to zero
                velocity.x -= velocityFallOff * Time.deltaTime;
            else if (velocity.x < 0) // inversely, if the velocity is posotive (going right) we need to lower it
                velocity.x += velocityFallOff * Time.deltaTime;
            if (Mathf.Abs(velocity.x) <= 0.09) // if the velocity is close to zero, snap it to zero
                velocity.x = 0;
        }
        // same as above but for the foward movement
        if (movementVector.z == 0)
        {

            if (velocity.z > 0)
                velocity.z -= velocityFallOff * Time.deltaTime;
            else if (velocity.z < 0)
                velocity.z += velocityFallOff * Time.deltaTime;
            if (Mathf.Abs(velocity.z) <= 0.09)
                velocity.z = 0;
        }


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
        Vector3 moveVector = transform.right * velocity.x + transform.forward * velocity.z;


        controller.Move(moveVector * moveSpeed * (currentHeight / height * crouchMoveSpeedMultiplyer) * Time.deltaTime);
        lastFootStepDistance += Vector3.Distance(transform.position, lastPos);
        if (lastFootStepDistance >= distanceBetweenFootstep && GroundCheck())
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.09f))
            {
                AudioClip[] clips = MaterialPropertiesManager.GetFootStepSounds(hit.transform.gameObject);
                AudioSource.PlayClipAtPoint(clips[Random.Range(0, clips.Length - 1)], transform.position, 0.5f * GameControllsManager.audioVolume);
                lastFootStepDistance = 0;
            }
        }

        lastPos = transform.position;

    }
    IEnumerator Jump()
    {
        Vector3 currentPos = transform.position;
        currentPos.y += 0.3f;
        transform.position = currentPos;
        yVelocity = jumpVel;
        justJumped = true;
        float timer = jumpEffectTime;
        while (timer > 0)
        {

            timer -= Time.deltaTime;
            if (timer >= 0.05)
                justJumped = false;

            yVelocity += jumpVel * timer * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }


    void CameraRotation()
    {
        float mouseX = -Input.GetAxis("Mouse X") * mouseSencitivity * GameControllsManager.mouseSense * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSencitivity * GameControllsManager.mouseSense * Time.deltaTime;

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


    float EaseOut(float x)
    {
        return 1 - (1 - x) * (1 - x);
    }


}
