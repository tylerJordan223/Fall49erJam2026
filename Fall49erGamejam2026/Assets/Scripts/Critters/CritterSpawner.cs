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

    [SerializeField] public GameObject critter;

    public void SpawnCritter(float x, float y)
    {
        GameObject c = Instantiate(critter);

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
