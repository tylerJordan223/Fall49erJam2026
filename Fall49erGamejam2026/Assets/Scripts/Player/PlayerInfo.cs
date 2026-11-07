using Unity.Cinemachine;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    //Script for the purpose of holding all player information

    public static PlayerInfo instance;

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

    [Header("Player Fields")]
    //player information
    [SerializeField] public GameObject p_body;
    [SerializeField] public Collider p_col;
    [SerializeField] public GameObject p_cam_obj;
    [SerializeField] public CinemachineCamera p_cam;
    [SerializeField] public Rigidbody rb;

    [Header("Player Scripts")]
    [SerializeField] public PlayerCam pc;
    [SerializeField] public PlayerMovement pm;

    [Header("Player Bools")]
    //variable information
    public bool grounded;
    public bool stepping;
    public bool can_control;

    [Header("Player Floats")]
    //player floats
    public float speed;

    [Header("Player Connections")]
    public TownScript active_town;

    //turns the player off
    public void DisablePlayer()
    {
        can_control = false;
        p_body.SetActive(false);
        p_cam_obj.SetActive(false);
    }

    //turns the player on
    public void EnablePlayer()
    {
        can_control = true;
        p_body.SetActive(true);
        p_cam_obj.SetActive(true);
    }
}
