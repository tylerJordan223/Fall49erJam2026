using UnityEngine;

public class WildCritter : MonoBehaviour
{
    [Header("Movement")]
    public float speed;

    [Header("Objects")]
    [SerializeField] GameObject charge_bar;

    //private information
    private Vector3 movement_dir;
    private float caught_percent = 0;

    private void Start()
    {
        //at the beginning pick a direction and just start moving
        movement_dir = Random.onUnitCircle;
        charge_bar.transform.localScale = new Vector3(0f, charge_bar.transform.localScale.y, charge_bar.transform.localScale.z);
    }

    public void CritterClick()
    {
        caught_percent += 0.5f;
        charge_bar.transform.localScale = new Vector3(caught_percent / 100f, charge_bar.transform.localScale.y, charge_bar.transform.localScale.z);

        if(caught_percent >= 100f)
        {
            DestroyImmediate(this.gameObject);
        }
    }


    private void FixedUpdate()
    {
        //update the position moving in whatever direction you're supposed to
        Vector3 t = transform.position;
        t += movement_dir * speed * Time.deltaTime;
        transform.position = t;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //just bounce around
        Vector3 normal = collision.contacts[0].normal;
        movement_dir = Vector3.Reflect(movement_dir, normal);
    }
}
