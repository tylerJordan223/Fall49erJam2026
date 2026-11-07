using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

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

    //this is a script to handle all global camera control//

    //global camera fields
    [SerializeField] Camera mainCamera;
    public CinemachineCamera active_cam;
    private CinemachineBrain brain;

    //global canvases
    [Header("Canvas Objects")]
    [SerializeField] Canvas fade_canvas;

    private void Start()
    {
        //initialize variables
        brain = mainCamera.GetComponent<CinemachineBrain>();
    }

    //function to handle a camera transition over a certain amount of time
    public void CameraTransition(CinemachineCamera new_cam, float transition_time = 0f)
    {
        Debug.Log(new_cam);

        //first set the blend time
        brain.DefaultBlend.Time = transition_time;

        //then enable second camera and disable first one
        new_cam.Priority = 1;
        active_cam.Priority = 0;

        //set this as the new active cam
        active_cam = new_cam;

        Debug.Log("RAN TRANSITION");
    }

    //functions to handle the fade canvas
    public void FadeIn(float speed)
    {
        StartCoroutine(DoFade(speed, 1));
    }

    public void FadeOut(float speed)
    {
        StartCoroutine(DoFade(speed, -1));
    }

    private IEnumerator DoFade(float speed, int direction)
    {
        //get the sprite to change alpha of
        Image fade_img = fade_canvas.transform.GetChild(0).GetComponent<Image>();

        if(direction == 1)
        {
            //fade in
            while(fade_img.color.a < 1f)
            {
                Color c = fade_img.color;
                c.a += Time.deltaTime * speed;
                fade_img.color = c;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            //fade out
            while (fade_img.color.a > 0f)
            {
                Color c = fade_img.color;
                c.a -= Time.deltaTime * speed;
                fade_img.color = c;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
