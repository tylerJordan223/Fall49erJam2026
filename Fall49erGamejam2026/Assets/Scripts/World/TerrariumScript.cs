using P_Input;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerrariumScript : MonoBehaviour
{
    public static TerrariumScript instance;
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

    [Header("Camera Information")]
    [SerializeField] public CinemachineCamera cam;
    public Vector3 start_position;
    public Vector3 end_position;

    //private info
    private bool active;

    P_Input.PlayerInput input;
    private void OnEnable()
    {
        input = new P_Input.PlayerInput();
        input.Terrarium.Enable();
    }

    private void OnDisable()
    {
        input.Terrarium.Disable();
    }

    public void StartDeactivation(InputAction.CallbackContext context)
    {
        Debug.Log("RAN");
        if (!active) return;

        if(context.performed)
        {
            StartCoroutine(DeactivateTerrarium());
        }
    }

    //scripts when turning terrarium on/off
    public IEnumerator ActivateTerrarium()
    {
        //move camera to this
        CameraController.instance.CameraTransition(cam);

        //being unfade
        CameraController.instance.FadeOut(0.5f);

        //move from start to end position
        float timer = 0;
        while(transform.position != end_position)
        {
            //move the position using a lerp
            cam.transform.localPosition = Vector3.Lerp(start_position, end_position, timer);

            //increase timer
            timer += Time.deltaTime / 2f;

            yield return new WaitForEndOfFrame();
        }

        //terrarium is now active
        active = true;
    }

    public IEnumerator DeactivateTerrarium()
    {
        //terrarium is no longer active
        active = false;

        //fade out overtime
        CameraController.instance.FadeOut(0.7f);

        //move from end to start position
        float timer = 0;
        while (transform.position != end_position)
        {
            //move the position using a lerp
            cam.transform.position = Vector3.Lerp(end_position, start_position, timer);

            //increase timer
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
