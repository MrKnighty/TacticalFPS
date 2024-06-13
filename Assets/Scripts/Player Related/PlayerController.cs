using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float accelerationSpeed;
    [SerializeField] float velocityFallOff;

    [SerializeField] GameObject cameraGameObject;

    [SerializeField] float mouseSencitivity;

    [SerializeField] CharacterController controller;
    Vector3 velocity;

    Vector3 movementVector;

    Vector2 cameraRotateVelocity;

    static public PlayerController playerInstance;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInstance = this;

    }
    void Update()
    {
        movementVector = Vector3.zero;
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.z = Input.GetAxisRaw("Vertical");
        print(movementVector);

        if (CaculateVelocity()) // caculate velocity, and only move if the player has any
            DoMovement(); // caculate movement

        CameraRotation();

        DebugManager.DisplayInfo("PlayerVelocity", "Player Vel:" + velocity.ToString());

        if(cameraRotateVelocity != Vector2.zero)
        {

            transform.Rotate(Vector3.up * cameraRotateVelocity.x );

            Vector3 cameraRotation = cameraGameObject.transform.localEulerAngles;
          
            float desiredXRotation = cameraRotation.x + cameraRotateVelocity.x;

            if (cameraRotation.x > 90 && cameraRotation.x < 270)
            {
                if (cameraRotation.x < 180)
                    desiredXRotation = 90;
                else
                    desiredXRotation = 270;
            }

            cameraGameObject.transform.localEulerAngles = new Vector3(desiredXRotation, cameraRotation.y, cameraRotation.z);
            cameraRotateVelocity = Vector2.zero;

        }
    }

    bool CaculateVelocity() // returns true if player has any velocity
    {
        if (movementVector.x == 0) // if there is no movement on the x, start taking away velocity
        {
            if (velocity.x > 0) // if the velocity is in the negetive (going left) we need to raise up the velocity back to zero
                velocity.x -= velocityFallOff * Time.deltaTime;
            else if (velocity.x < 0) // inversely, if the velocity is posotive (going right) we need to lower it
                velocity.x += velocityFallOff * Time.deltaTime;
            if (Mathf.Abs(velocity.x) <= 0.01) // if the velocity is close to zero, snap it to zero
                velocity.x = 0;
        }
        // same as above but for the foward movement
        if (movementVector.z == 0)
        {

            if (velocity.z > 0)
                velocity.z -= velocityFallOff * Time.deltaTime;
            else if (velocity.z < 0)
                velocity.z += velocityFallOff * Time.deltaTime;
            if (Mathf.Abs(velocity.z) <= 0.01)
                velocity.z = 0;
        }


        if (velocity == Vector3.zero && movementVector == Vector3.zero) // if both are zero, we dont need to caculate any movement
            return false;

        // caculate the new velocity based off player input

        velocity += movementVector * accelerationSpeed * Time.deltaTime; // add the current move vector to the velocity. 

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
        controller.Move(moveVector * moveSpeed * Time.deltaTime);

    }


    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSencitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSencitivity * Time.deltaTime;

        if (mouseX == 0 && mouseY == 0)
            return;


        transform.Rotate(Vector3.up * mouseX);

       
        Vector3 cameraRotation = cameraGameObject.transform.localEulerAngles;
        float desiredXRotation = cameraRotation.x - mouseY;
        float desiredYRotation = cameraRotation.y + mouseX;



        if (desiredXRotation > 90 && desiredXRotation < 270)
        {
            if (desiredXRotation < 180)
                desiredXRotation = 90;
            else
                desiredXRotation = 270;
        }


       
        cameraGameObject.transform.localEulerAngles = new Vector3(desiredXRotation, cameraRotation.y, cameraRotation.z);
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
            cameraRotateVelocity += vector;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
    

}
