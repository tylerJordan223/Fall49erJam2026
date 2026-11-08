using UnityEngine;

public class WildCritter : MonoBehaviour
{
    [Header("Movement")]
    public float speed;

    //private information
    private Vector3 movement_dir;

    private void Start()
    {
        //at the beginning pick a direction and just start moving
        movement_dir = Random.onUnitCircle;
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
        Debug.Log("HIT");
        //just bounce around
        Vector3 normal = collision.contacts[0].normal;
        movement_dir = Vector3.Reflect(movement_dir, normal);
    }
}
