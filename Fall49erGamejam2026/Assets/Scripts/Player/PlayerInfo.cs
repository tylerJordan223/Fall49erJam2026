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
    [SerializeField] public PlayerCam pc;
    [SerializeField] public PlayerMovement pm;

    [Header("Player Bools")]
    //variable information
    public bool grounded;
    public bool stepping;

    [Header("Player Floats")]
    //player floats
    public float speed;
}
