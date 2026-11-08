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
    float width_base;

    //camera shake values
    [Header("Shake values")]
    public float max_shake_y;
    private float shake_y;
    public float max_shake_x;
    private float shake_x;

    [Header("extra")]
    [SerializeField] GameObject visionEffect;

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
        //get the base values
        height_base = PlayerInfo.instance.p_cam_obj.transform.localPosition.y;
        width_base = PlayerInfo.instance.p_cam_obj.transform.localPosition.x;

        //set the active camera to player
        CameraController.instance.active_cam = PlayerInfo.instance.p_cam;
    }

    private void Update()
    {
        Debug.DrawRay(PlayerInfo.instance.p_cam_obj.transform.position, (PlayerInfo.instance.p_cam_obj.transform.position + PlayerInfo.instance.p_cam_obj.transform.forward) * 10f, Color.pink);

        //cancel out of not allowed to control
        if (!PlayerInfo.instance.can_control) return;

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

        //UPDATING THE ACTIVE TOWN//
        RaycastHit hit;
        Debug.DrawRay(PlayerInfo.instance.p_cam_obj.transform.position, PlayerInfo.instance.p_cam_obj.transform.forward, Color.purple);
        if (Physics.Raycast(PlayerInfo.instance.p_cam_obj.transform.position, PlayerInfo.instance.p_cam_obj.transform.forward, out hit, PlayerInfo.instance.p_col.bounds.size.y * 0.75f, LayerMask.GetMask("Interactable")))
        {
            //get the town from the hit
            TownScript h_t = hit.transform.parent.GetComponent<TownScript>();
            //update if necessary
            if (h_t != PlayerInfo.instance.active_town && h_t.interactable)
            {
                h_t.ring.SetActive(true);
                PlayerInfo.instance.active_town = h_t;
            }
        }
        else
        {
            TownScript t = PlayerInfo.instance.active_town;
            //disable down if there is one 
            if (t != null)
            {
                t.ring.SetActive(false);
                PlayerInfo.instance.active_town = null;
            }
        }
    }

    private void FixedUpdate()
    {
        //cancel out of not allowed to control
        if (!PlayerInfo.instance.can_control) return;

        //handle the camera height offset
        Vector2 movement = input.Overworld.Movement.ReadValue<Vector2>();

        //only run this if not forced stepping
        if (!PlayerInfo.instance.stepping)
        {
            //handle stepping / camera bob
            if (movement.y != 0)
            {
                height_offset += bob_speed * Time.deltaTime;
                PlayerInfo.instance.speed = 0.1f;
            }
            else if (height_offset > 0f)
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
        t.y = height_base + height_offset + shake_y;
        t.x = width_base + shake_x;
        PlayerInfo.instance.p_cam_obj.transform.localPosition = t;
    }

    private IEnumerator MovementStep()
    {
        //step one is to move down until low necessity
        while (height_offset > -bob_threshold * 0.4f)
        {
            height_offset -= bob_speed * 2.5f * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //shake camera
        StartCoroutine(CameraShake(0.75f));

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

    private IEnumerator CameraShake(float time)
    {
        //get the necessary variables
        float timer = 0f;
        float sx = max_shake_x;
        float sy = max_shake_y;
        float shrink_factor = sx / time;

        //shake loop
        while (timer < time)
        {
            //do the shake
            shake_x = Random.Range(-sx, sx);
            shake_y = Random.Range(-sy, sy);
            //increase timer
            timer += Time.deltaTime;
            //shrink max shake
            sx = max_shake_x * (1 - (timer / time));
            sy = max_shake_y * (1 - (timer / time));
            yield return new WaitForEndOfFrame();
        }

        //stop shake
        shake_x = 0f;
        shake_y = 0f;
    }

    //functions to transition between player and town
    public void StartPlayerToTown(InputAction.CallbackContext context)
    {
        //make sure its not already in town
        if (!PlayerInfo.instance.can_control) return;

        if (context.performed && PlayerInfo.instance.active_town != null)
        {
            StartCoroutine(PlayerToTown());
        }
    }

    public void StartTownToPlayer()
    {
        if (PlayerInfo.instance.active_town != null)
        {
            StartCoroutine(TownToPlayer());
        }
    }

    private IEnumerator PlayerToTown()
    {
        //make sure the camera is set so the up is away from the player
        //NOT WORKING PROPERLY
        //PlayerInfo.instance.active_town.cam.transform.up = (PlayerInfo.instance.active_town.transform.position - transform.position);

        //start the camera transition
        CameraController.instance.CameraTransition(PlayerInfo.instance.active_town.cam, 1f);

        //disable the player
        PlayerInfo.instance.can_control = false;

        //pause for camera to align
        yield return new WaitForSeconds(1.5f);

        //start the fadeout
        CameraController.instance.FadeIn(0.5f);

        //lower the camera for a period of time
        float y_offset = PlayerInfo.instance.active_town.cam.transform.position.y - 3f;
        while (PlayerInfo.instance.active_town.cam.transform.position.y > y_offset)
        {
            //lower the camera
            Vector3 pos = PlayerInfo.instance.active_town.cam.transform.position;
            pos.y -= Time.deltaTime * 1f;
            PlayerInfo.instance.active_town.cam.transform.position = pos;
            yield return new WaitForEndOfFrame();
        }

        //swap to town area
        WorldScript.instance.ActivateTown();
    }
    
    //transitioning from town to player
    private IEnumerator TownToPlayer()
    {
        //do the TransitionToTown in reverse

        //start the fadein
        CameraController.instance.FadeOut(0.5f);

        //raise the camera for a period of time
        float y_offset = PlayerInfo.instance.active_town.cam.transform.position.y + 3f;
        while (PlayerInfo.instance.active_town.cam.transform.position.y < y_offset)
        {
            //lower the camera
            Vector3 pos = PlayerInfo.instance.active_town.cam.transform.position;
            pos.y += Time.deltaTime * 1f;
            PlayerInfo.instance.active_town.cam.transform.position = pos;
            yield return new WaitForEndOfFrame();
        }

        //start the camera transition
        CameraController.instance.CameraTransition(PlayerInfo.instance.p_cam, 1f);

        //pause for camera to align
        yield return new WaitForSeconds(1.5f);

        //set town as finished
        PlayerInfo.instance.active_town.interactable = false;
        PlayerInfo.instance.active_town.ring.SetActive(false);
        PlayerInfo.instance.active_town = null;

        //disable the player
        PlayerInfo.instance.EnablePlayer();
        
    }

    //functions to transition between player and terrarium//
    public void StartPlayerToTerrarium(InputAction.CallbackContext context)
    {
        //make sure its not already in town
        if (!PlayerInfo.instance.can_control) return;

        if (context.performed)
        {
            StartCoroutine(PlayerToTerrarium());
        }
    }

    public void StartTerrariumToPlayer()
    {
        StartCoroutine(TerrariumToPlayer());
    }

    private IEnumerator PlayerToTerrarium()
    {
        //line up the vision effect and turn it on
        visionEffect.SetActive(true);
        visionEffect.transform.rotation = PlayerInfo.instance.p_cam_obj.transform.rotation;
        visionEffect.transform.position = PlayerInfo.instance.p_cam_obj.transform.position;
        PlayerInfo.instance.p_body.SetActive(false);

        //disable the player
        PlayerInfo.instance.can_control = false;

        //start the fadeout
        CameraController.instance.FadeIn(0.5f);

        //move the camera backwards into the player
        Vector3 start = PlayerInfo.instance.p_cam_obj.transform.position;
        Vector3 end = start - PlayerInfo.instance.p_cam_obj.transform.forward * 2f;
        float timer = 0f;
        while (PlayerInfo.instance.p_cam_obj.transform.position != end)
        {
            //lerp the camera towards the end position
            PlayerInfo.instance.p_cam_obj.transform.position = Vector3.Lerp(start, end, timer);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);

        //swap to town area
        StartCoroutine(TerrariumScript.instance.ActivateTerrarium());
    }

    private IEnumerator TerrariumToPlayer()
    {
        //set the camera
        CameraController.instance.CameraTransition(PlayerInfo.instance.p_cam);

        //start the fadeout
        CameraController.instance.FadeOut(0.5f);

        //move the camera backwards into the player
        Vector3 start = PlayerInfo.instance.p_cam_obj.transform.position;
        Vector3 end = start + PlayerInfo.instance.p_cam_obj.transform.forward * 2f;
        float timer = 0f;
        while (PlayerInfo.instance.p_cam_obj.transform.position != end)
        {
            //lerp the camera towards the end position
            PlayerInfo.instance.p_cam_obj.transform.position = Vector3.Lerp(start, end, timer);

            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        //enable the player
        PlayerInfo.instance.can_control = true;

        //line up the vision effect and turn it off
        visionEffect.transform.rotation = PlayerInfo.instance.p_cam_obj.transform.rotation;
        visionEffect.transform.position = PlayerInfo.instance.p_cam_obj.transform.position;
        visionEffect.SetActive(false);
        PlayerInfo.instance.p_body.SetActive(true);
    }
}
