using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    //necessary variables

    //sensitivity
    public float sens_X;
    public float sens_Y;

    //rotation values
    float xRotation;
    float yRotation;

    //necessary camera transform
    [SerializeField] Transform camera;

    //Make it so it removes your cursor when enabled
    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    private void Update()
    {
        //actually update the camera rotation//

        //retrieve mouse movement
        //IMPORTANT: Mouse vector is actually X = Y and Y = X
        Vector2 mouseMovement = Mouse.current.delta.ReadValue();

        //only work if actually getting info
        if (mouseMovement != Vector2.zero)
        {
            //calculate the new rotation
            xRotation -= mouseMovement.y * sens_X * Time.deltaTime;
            yRotation += mouseMovement.x * sens_Y * Time.deltaTime;

            //clamp the Y rotation
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //perform the rotation
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
            camera.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        }
        
    }
}
