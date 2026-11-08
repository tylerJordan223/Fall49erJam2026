using System.Collections.Generic;
using UnityEngine;

public class CritterSpawner : MonoBehaviour
{
    public static CritterSpawner instance;
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

    //class to handle spawning critters//
    [SerializeField] public List<CritterObject> all_critters;
    [SerializeField] public GameObject critter_obj;

    public void SpawnCritter(float x, float y)
    {
        GameObject c = Instantiate(critter_obj);

        //fill in all seralized information from a random critter
        CritterObject c_o = all_critters[Random.Range(0, all_critters.Count)];

        c.GetComponent<WildCritter>().critter_object = c_o;
        c.GetComponent<WildCritter>().UpdateInfo();

        c.transform.SetParent(this.transform);

        Vector3 t = c.transform.position;
        t.x = transform.position.x + x;
        t.y = transform.position.y + y;
        t.z = transform.position.z;
        c.transform.position = t;
    }

    public void ResetCritters()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
