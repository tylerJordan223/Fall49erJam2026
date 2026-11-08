using P_Input;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldScript : MonoBehaviour
{
    public static WorldScript instance;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
    }

    //this is a script ot handle the segment of 2d overworld

    [Header("Object Info")]
    [SerializeField] public CinemachineCamera cam;
    [SerializeField] public CritterSpawner spawner;

    [Header("Unique Info")]
    public float timer_length;
    public float spawn_amount;
    public float spawn_width;
    public float spawn_height;

    //private information
    private float timer;
    private bool active;

    private void Update()
    {
        if (!active) return;

        //updating the timer
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            CameraController.instance.town_canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = timer.ToString("F2");
        }
        else
        {
            StartCoroutine(DeactivateTown());
        }

        //handle mouse control
        if(Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            //perform a raycat through mouse to see if hitting critter
            Ray ray = CameraController.instance.mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                //actually handle hit if its a critter
                if (hit.collider.gameObject.CompareTag("critter"))
                {
                    hit.collider.GetComponent<WildCritter>().CritterClick();
                }
            }
        }
    }

    //click function
    public void ClickAction(InputAction.CallbackContext context)
    {
        //make sure not to run if not active
        if (!active) return;


    }

    private void FixedUpdate()
    {
        if (active)
        {
            //code to have flashlight follow mouse
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    CameraController.instance.flashlight_canvas.GetComponent<RectTransform>(),
                    Mouse.current.position.ReadValue(),
                    null,
                    out localPoint
            );
            CameraController.instance.flashlight.GetComponent<RectTransform>().anchoredPosition = localPoint;
        }
    }

    //this will turn on the town canvas with the appropriate information
    public void ActivateTown()
    {
        //quickly fill in all the necessary information
        timer_length = PlayerInfo.instance.active_town.search_length;
        spawn_amount = PlayerInfo.instance.active_town.num_critters;

        //transition the camera
        CameraController.instance.CameraTransition(cam);

        //swap canvases
        CameraController.instance.fade_canvas.gameObject.SetActive(false);
        CameraController.instance.flashlight_canvas.gameObject.SetActive(true);
        CameraController.instance.town_canvas.gameObject.SetActive(true);

        //spawn the critters within spawn space
        for(int i = 0; i < spawn_amount; i++)
        {
            float x = Random.Range(-spawn_width, spawn_width);
            float y = Random.Range(-spawn_height, spawn_height);

            spawner.SpawnCritter(x, y);
        }

        //start the timer
        timer = timer_length;

        //unlock the cursor
        Cursor.lockState = CursorLockMode.None;

        //turn on the town
        active = true;
    }

    //this will undo everything
    public IEnumerator DeactivateTown()
    {
        //turn off the town
        active = false;

        //lock the cursor
        Cursor.lockState = CursorLockMode.Locked;

        //swap canvases
        CameraController.instance.fade_canvas.gameObject.SetActive(true);
        CameraController.instance.flashlight_canvas.gameObject.SetActive(false);
        CameraController.instance.town_canvas.gameObject.SetActive(false);

        //set the camera to town cam
        CameraController.instance.CameraTransition(PlayerInfo.instance.active_town.cam);

        //delete all leftover critters
        spawner.ResetCritters();

        //pause for effect
        yield return new WaitForSeconds(1f);

        //do the transition
        PlayerInfo.instance.pc.StartTownToPlayer();
    }
}
