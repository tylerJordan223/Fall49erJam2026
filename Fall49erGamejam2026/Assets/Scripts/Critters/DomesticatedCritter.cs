using UnityEngine;
using UnityEngine.InputSystem;

public class DomesticatedCritter : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public bool held;

    [Header("Objects")]
    [SerializeField] SpriteRenderer sr;
    [SerializeField] GameObject shadow;
    [SerializeField] public CritterObject critter_object;
    [SerializeField] public Collider col;

    //private information
    private Vector3 movement_dir;
    private float caught_percent = 0;
    private Rigidbody rb;

    private void Start()
    {
        //at the beginning pick a direction and just start moving
        movement_dir = Random.onUnitCircle;
        held = false;
        rb = GetComponent<Rigidbody>();
    }

    //function to update info when given a new critter object
    public void UpdateInfo()
    {
        sr.sprite = critter_object.base_sprite;
    }

    //function to move to being held
    public void CritterClick()
    {

    }
    private void Update()
    {

        //always update sprite renderer direction based on direction moving
        //also update sprite
        sr.flipX = (Mathf.Sign(movement_dir.x) == 1) ? true : false;
        if (held) sr.sprite = critter_object.falling_sprite; else sr.sprite = critter_object.base_sprite;

        //also update the shadow so it stays on the ground
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 20f, LayerMask.GetMask("Ground")))
        {
            Vector3 s_pos = shadow.transform.position;
            s_pos.y = hit.point.y + 0.01f;
            shadow.transform.position = s_pos;
        }
    
    }
        
    private void FixedUpdate()
    {
        //different code for if they're being held
        if(held)
        {
            //handle rb gravity
            rb.useGravity = false;

            //follow the mouse on your y/z//

            //get the mouse movement
            Vector2 mouse_pos = Mouse.current.position.ReadValue();

            //create a ray that points from the camera to mouse position
            Ray mouse_ray = CameraController.instance.mainCamera.ScreenPointToRay(mouse_pos);

            //create a movement plane for the object to move on in the right space
            Plane movement_plane = new Plane(Vector3.right, transform.position);

            //check where the ray intersects this plane
            if(movement_plane.Raycast(mouse_ray, out float enterDistance))
            {
                //get the point in 3d space where this is
                Vector3 target = mouse_ray.GetPoint(enterDistance);

                //update the position
                Vector3 pos = transform.position;
                //offset to middle of collider
                pos.y = target.y + col.bounds.size.y/2;
                pos.z = target.z + col.bounds.size.x/2;
                transform.position = pos;
            }

            //disable the hold
            if (!Mouse.current.leftButton.IsPressed())
            {
                held = false;
            }    
        }
        else
        {
            rb.useGravity = true;

            //update the position moving in whatever direction you're supposed to
            Vector3 t = transform.position;
            t += movement_dir * speed * Time.deltaTime;
            transform.position = t;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //disable the hold if this is the case
        if(held)
        {
            held = false;
            return;
        }

        //just bounce around
        Vector3 normal = collision.contacts[0].normal;
        movement_dir = Vector3.Reflect(movement_dir, normal);

    }
}
