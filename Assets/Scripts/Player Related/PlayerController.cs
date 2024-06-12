using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float accelerationSpeed;
    [SerializeField] float velocityFallOff;

    [SerializeField] GameObject cameraGameObject;

    [SerializeField] float mouseSencitivity;
    [SerializeField] TextMeshProUGUI debugUI;
    Vector3 velocity;

    Vector3 movementVector;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
      
    }
    void Update()
    {
        movementVector = Vector3.zero;
        movementVector.x = Input.GetAxisRaw("Horizontal");
        movementVector.z = Input.GetAxisRaw("Vertical");
        print(movementVector);
        CaculateVelocity();

        CameraRotation();

        debugUI.text = "Vel: " + velocity;

       

    }

    void CaculateVelocity()
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
            return;

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


        DoMovement(); // caculate movement
    }

    void DoMovement()
    {
        Vector3 move = transform.right * velocity.x + transform.forward * velocity.z;
        transform.position += move * moveSpeed * Time.deltaTime;
        print(move);
    }

    float verticalRotation = 0;
    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSencitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSencitivity * Time.deltaTime;

  
        transform.Rotate(Vector3.up * mouseX);

        
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); 

        cameraGameObject.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }
    
}
