using UnityEngine;
using UnityEngine.InputSystem;
using P_Input;
using Unity.VisualScripting;
using System.Collections;

public class PlayerCam : MonoBehaviour
{
    //necessary variables

    //sensitivity
    public float sens_X;
    public float sens_Y;

    //rotation values
    float xRotation;
    float yRotation;

    //handling the height
    [Header("Head Bobbing")]
    public float bob_threshold;
    public float bob_speed;
    public float height_offset = 0f;
    float height_base;

    #region input
    //controls
    //Make it so it removes your cursor when enabled

    P_Input.PlayerInput input;

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        input = new P_Input.PlayerInput();
        input.Overworld.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        input.Overworld.Disable();
    }

    #endregion input

    private void Start()
    { 
        height_base = PlayerInfo.instance.p_cam_obj.transform.localPosition.y;
    }

    private void Update()
    {
        //actually update the camera rotation//

        //retrieve mouse movement
        //IMPORTANT: Mouse vector is actually X = Y and Y = X
        Vector2 mouseMovement = Mouse.current.delta.ReadValue();
        Vector2 movement = input.Overworld.Movement.ReadValue<Vector2>();

        //only work if actually getting info
        //calculate the new rotation
        xRotation -= mouseMovement.y * sens_X * Time.deltaTime;
        yRotation += movement.x * sens_Y * Time.deltaTime;

        //clamp the Y rotation
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //perform the rotation, only rotates camera up/down
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        PlayerInfo.instance.p_cam_obj.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        
    }

    private void FixedUpdate()
    {
        //handle the camera height offset
        Vector2 movement = input.Overworld.Movement.ReadValue<Vector2>();

        //only run this if not forced stepping
        if(!PlayerInfo.instance.stepping)
        {
            //handle stepping / camera bob
            if(movement.y != 0)
            {
                height_offset += bob_speed * Time.deltaTime;
                PlayerInfo.instance.speed = 0.1f;
            }
            else if(height_offset > 0f)
            {
                height_offset -= bob_speed * Time.deltaTime;
                PlayerInfo.instance.speed = -0.1f;
            }
            else
            {
                PlayerInfo.instance.speed = 0f;
            }

            //check for step threshold, and if past then run coroutine
            if (height_offset >= bob_threshold)
            {
                //make it true and set the movement to that direction
                PlayerInfo.instance.stepping = true;
                PlayerInfo.instance.speed = 2f * movement.y;
                StartCoroutine(MovementStep());
            }
        }

        //apply the movement of camera
        Vector3 t = PlayerInfo.instance.p_cam_obj.transform.localPosition;
        t.y = height_base + height_offset;
        PlayerInfo.instance.p_cam_obj.transform.localPosition = t;
    }

    private IEnumerator MovementStep()
    {
        //step one is to move down until low necessity
        while(height_offset > -bob_threshold * 0.4f)
        {
            height_offset -= bob_speed * 2.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //step two is to go back to regular height
        while (height_offset < 0f)
        {
            height_offset += bob_speed * 1.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //reset everything
        height_offset = 0f;
        PlayerInfo.instance.speed = 0f;
        PlayerInfo.instance.stepping = false;
    }
}
