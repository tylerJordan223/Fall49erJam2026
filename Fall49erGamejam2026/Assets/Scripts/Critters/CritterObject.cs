using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CritterObject", menuName = "Scriptable Objects/CritterObject")]
public class CritterObject : ScriptableObject
{
    //Object script for all the different collectable characters//

    [Header("Default Information")]
    public string c_name;
    public Sprite base_sprite;

    [Header("Bools")]
    public bool shiny;
    public bool collected;

    [Header("Extra Info")]
    [SerializeField] public List<string> dialogue;
    public int rarity;

    [Header("Animations")]
    public Animation walking;
    public Animation talking;
}
