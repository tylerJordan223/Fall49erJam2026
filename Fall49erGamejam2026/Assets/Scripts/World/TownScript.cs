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

    [Header("Information")]
    public bool interactable;
    [SerializeField] public List<CritterObject> critters;
    public int search_length;
    public int num_critters;

    private void Start()
    {
        //always interactable on start
        interactable = true;
    }

}
