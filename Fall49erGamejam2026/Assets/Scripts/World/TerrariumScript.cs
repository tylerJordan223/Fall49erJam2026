using P_Input;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("UI Information")]
    [SerializeField] public Canvas terrarium_canvas;
    [SerializeField] public Image pointer;

    [Header("Critter Information")]
    public List<CritterObject> critters;

    //private info
    private bool active;

    private void Update()
    {
        if(terrarium_canvas.isActiveAndEnabled)
        {
            //code to have pointer follow mouse
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    terrarium_canvas.GetComponent<RectTransform>(),
                    Mouse.current.position.ReadValue(),
                    null,
                    out localPoint
            );
            pointer.GetComponent<RectTransform>().anchoredPosition = localPoint;
        }
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
        while(cam.transform.localPosition != end_position)
        {
            //move the position using a lerp
            cam.transform.localPosition = Vector3.Lerp(start_position, end_position, timer);

            //increase timer
            timer += Time.deltaTime / 2f;

            yield return new WaitForEndOfFrame();
        }

        //terrarium is now active
        active = true;
        terrarium_canvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartDeactivate()
    {
        if (!active) return;

        StartCoroutine(DeactivateTerrarium());
    }

    public IEnumerator DeactivateTerrarium()
    {
        //terrarium is no longer active
        active = false;
        terrarium_canvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;

        //fade out overtime
        CameraController.instance.FadeIn(1f);

        //move from end to start position
        float timer = 0;
        while (cam.transform.localPosition != start_position)
        {
            //move the position using a lerp
            cam.transform.localPosition = Vector3.Lerp(end_position, start_position, timer);

            //increase timer
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        //transition back to player
        PlayerInfo.instance.pc.StartTerrariumToPlayer();
    }

    //function used to add critters to the terrarium
    public void AddCritter(CritterObject co)
    {
        critters.Add(co);
    }
}
