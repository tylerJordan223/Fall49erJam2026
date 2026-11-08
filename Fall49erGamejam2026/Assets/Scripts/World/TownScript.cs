using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class TownScript : MonoBehaviour
{
    //this is the script to handle all the information related to the town areas

    //town objects
    [Header("Objects")]
    [SerializeField] public GameObject ring;
    [SerializeField] public CinemachineCamera cam;
    [SerializeField] public TextMeshPro cooldown_timer_text;

    [Header("Information")]
    public bool interactable;
    [SerializeField] public List<CritterObject> critters;
    public int search_length;
    public int num_critters;
    public float cooldown_length;

    //private info
    private float cooldown_timer;

    private void Start()
    {
        //always interactable on start
        interactable = true;
    }

    private void LateUpdate()
    {
        if (interactable) return;

        if(!cooldown_timer_text.gameObject.activeSelf)
        {
            cooldown_timer_text.gameObject.SetActive(true);
        }

        //make it so the text always faces player
        cooldown_timer_text.transform.LookAt(CameraController.instance.mainCamera.transform);
        cooldown_timer_text.transform.Rotate(Vector3.up, 180f);

        //update
        if(cooldown_timer > 0f)
        {
            cooldown_timer -= Time.deltaTime;
            cooldown_timer_text.text = cooldown_timer.ToString("F2");
        }
        else
        {
            interactable = true;
            cooldown_timer_text.gameObject.SetActive(false);
        }
    }

    //function to call when finished with town
    public void Cooldown()
    {
        interactable = false;
        ring.SetActive(false);
        cooldown_timer = cooldown_length;
    }

}
