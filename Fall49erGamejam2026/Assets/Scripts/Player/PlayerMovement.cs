using P_Input;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Script to handle the player's movement

    //private variables
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
        PlayerInfo.instance.rb = GetComponent<Rigidbody>();
        PlayerInfo.instance.rb.freezeRotation = true;
    }

    private void Update()
    {
        //perform grounded raycast
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            //confirm grounded
            PlayerInfo.instance.grounded = true;

            //get the normal for calculation
            g_normal = hit.normal;
        }
        else
        {
            PlayerInfo.instance.grounded = false;
        }

        //get the movement input
        movement = input.Overworld.Movement.ReadValue<Vector2>();

        //zero out velocity dont want that
        PlayerInfo.instance.rb.angularVelocity = Vector3.zero;
        PlayerInfo.instance.rb.linearVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        //get the direction value
        Vector3 dir = Vector3.Cross(transform.right, g_normal);
        //actually move based on speed entirely
        PlayerInfo.instance.rb.MovePosition(PlayerInfo.instance.rb.position + dir.normalized * PlayerInfo.instance.speed * Time.deltaTime);
    }


}
