using P_Input;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Script to handle the player's movement

    //necessary variable
    [Header("Movement")]
    public float speed;

    //private variables
    private bool grounded;
    private Rigidbody rb;
    private Vector2 movement;
    private Vector3 g_normal;

    #region input
    //controls
    PlayerInput input;

    private void OnEnable()
    {
        input = new PlayerInput();
        input.Overworld.Enable();
    }

    private void OnDisable()
    {
        input.Overworld.Disable();
    }

    #endregion input

    private void Start()
    {
        //initialize necessary variables
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        //perform grounded raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            //confirm grounded
            grounded = true;

            //get the normal for calculation
            g_normal = hit.normal;
        }
        else
        {
            grounded = false;
        }

        //get the movement input
        movement = input.Overworld.Movement.ReadValue<Vector2>();

        //zero out velocity dont want that
        rb.angularVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        //get the direction value
        Vector3 dir = Vector3.Cross(transform.right, g_normal) * movement.y + Vector3.Cross(transform.forward, g_normal) * -movement.x;

        //actually give the movement to the player
        if(grounded)
        {
            rb.MovePosition(rb.position + dir.normalized * speed * Time.deltaTime);
        }
    }


}
