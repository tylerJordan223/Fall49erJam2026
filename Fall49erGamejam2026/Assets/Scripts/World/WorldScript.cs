using System.Collections;
using TMPro;
using Unity.Cinemachine;
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

    [Header("Information")]
    [SerializeField] public CinemachineCamera cam;
    public float timer_length;

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

        //transition the camera
        CameraController.instance.CameraTransition(cam);

        //swap canvases
        CameraController.instance.fade_canvas.gameObject.SetActive(false);
        CameraController.instance.flashlight_canvas.gameObject.SetActive(true);
        CameraController.instance.town_canvas.gameObject.SetActive(true);

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

        //swap canvases
        CameraController.instance.fade_canvas.gameObject.SetActive(true);
        CameraController.instance.flashlight_canvas.gameObject.SetActive(false);
        CameraController.instance.town_canvas.gameObject.SetActive(false);

        //set the camera to town cam
        CameraController.instance.CameraTransition(PlayerInfo.instance.active_town.cam);

        //pause for effect
        yield return new WaitForSeconds(1f);

        //set town as finished
        PlayerInfo.instance.active_town.interactable = false;

        //do the transition
        PlayerInfo.instance.pc.StartTownToPlayer();
    }
}
