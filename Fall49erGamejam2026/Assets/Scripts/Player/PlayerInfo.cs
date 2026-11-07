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

    //player information
    [SerializeField] public GameObject p_body;
    [SerializeField] public Collider p_col;
    [SerializeField] public GameObject p_cam_obj;
    [SerializeField] public CinemachineCamera p_cam;
}
