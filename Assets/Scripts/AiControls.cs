using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiControls : MonoBehaviour
{
    // Public variables for setting AI behavior parameters
    public float movementSpeed;          // Speed at which the AI moves forward
    public float turningSpeed;           // Speed at which the AI turns
    public float turretTurningSpeed;     // Speed at which the AI's turret turns
    public float shootingCooldown;       // Cooldown period between shots
    public float detectRange;            // Range at which the AI detects the target
    public float stoppingRange;          // Range at which the AI stops moving towards the target
    public float switchTargetRange;      // Range to switch to a new target
    public float switchDistance;         // Distance to switch between states
    public float AIDelay;                // Delay between AI state updates

    public string stringState;           // String representation of the current state

    // Public variables related to the AI's turret and shooting
    public Transform turret;             // Transform of the AI's turret
    public GameObject projectile;        // Projectile to be shot
    public Transform muzzle;             // Muzzle from where the projectile is shot

    // Private variables for internal use
    private Rigidbody rb;                // Rigidbody component for physics interactions
    private float t;                     // Timer variable (currently unused)
    private float AIt;                   // AI timer variable

    private GameObject targetObject;     // Reference to the target (e.g., player)
    private Vector3 target;              // Position of the target

    private int obstacleMask;            // Layer mask for obstacles

    // Enum representing the possible states of the AI
    private enum State
    {
        forward,    // Moving forward state
        left,       // Turning left state
        right,      // Turning right state
        back,       // Moving backward state
        stop        // Stopped state
    };

    private State state;                 // Current state of the AI
    private State nextState;             // Next state to transition to

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get and store the Rigidbody component
        t = 0f;                          // Initialize the timer variable
        AIt = 0f;                        // Initialize the AI timer variable
        obstacleMask = LayerMask.GetMask("Obstacle"); // Set the layer mask for obstacles
        state = State.forward;           // Set the initial state to forward
        nextState = State.forward;       // Set the initial next state to forward
    }

    // FixedUpdate is called at a fixed interval and is used for physics updates
    void FixedUpdate()
    {
        t -= Time.deltaTime;
        
        // Maintain the AI's level orientation by resetting rotation along the X and Z axes
        Vector3 currentRotation = rb.rotation.eulerAngles;
        rb.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);

        if (Vector3.Distance(transform.position, target) < switchTargetRange)
        {
            float randomx = Random.Range(-switchDistance, switchDistance);
            float randomz = Random.Range(-switchDistance, switchDistance);

            target += new Vector3(randomx, 0f, randomz);
        }

        // Find the player if not already targeted
        if (targetObject != null)
        {
            if (Vector3.Distance(transform.position, targetObject.transform.position) < detectRange)
            {
                if (!Physics.Linecast(transform.position, targetObject.transform.position, obstacleMask))
                {
                    //enemyShooting
                    if (t < 0)
                    {
                        target = targetObject.transform.position;
                        GameObject proj = Instantiate(projectile, muzzle.position, muzzle.rotation);
                        proj.GetComponent<Projectile>().shooterTag = tag;
                        t = shootingCooldown;
                    }

                    if (Vector3.Distance(target, transform.position) < stoppingRange)
                    {
                        nextState = State.stop;
                    }
                }
            }
        }
        else
        {
            // Find the player GameObject by tag and set it as the target
            targetObject = GameObject.FindGameObjectWithTag("Player");
        }
        Debug.DrawLine(target, target + new Vector3(0f, 5f, 0f), Color.green);
        // Calculate the angle between the AI's forward direction and the direction to the target
        float angle = Vector3.SignedAngle(transform.forward, target - transform.position, Vector3.up);

        // Manage AI delay for state changes
        if (AIt < 0)
        {
            state = nextState;
            AIt = AIDelay;
        }
        else
        {
            AIt -= Time.deltaTime;
        }

        // State machine logic for AI behavior
        if (state == State.forward)
        {
            stringState = "forward";
            // Adjust turning based on the angle to the target
            if (angle < 0)
            {
                // If angle is negative, target is to the left; turn left
                // Input of -1f means turning left
                Turning(-1f);
            }
            else if (angle > 0)
            {
                // If angle is positive, target is to the right; turn right
                // Input of 1f means turning right
                Turning(1f);
            }

            // Move towards the target if the target is generally in front of the AI
            if (Mathf.Abs(angle) < 90)
            {
                // Move forward if the target is within a 90-degree arc in front of the AI
                Move(1f);
            }
        }
        else if (state == State.left)
        {
            stringState = "left";
            Turning(-1f);
            Move(1f);
        }
        else if (state == State.right)
        {
            stringState = "right";
            Turning(1f);
            Move(1f);
        }
        else if (state == State.back)
        {
            stringState = "back";
            Move(1f);
            nextState = State.forward;
        }
        else if (state == State.stop)
        {
            stringState = "stop";
            Move(0f);
            nextState = State.forward;
        }
        Vector3 targetDirection = target - turret.position;
        targetDirection.y = 0f;
        Vector3 turningDirection = Vector3.RotateTowards(turret.forward, targetDirection, turretTurningSpeed * Time.deltaTime, 0f);
        turret.rotation = Quaternion.LookRotation(turningDirection);
    }

    // Method to move the AI forward
    private void Move(float input)
    {
        // Calculate movement vector based on forward direction and input
        Vector3 movement = transform.forward * input * movementSpeed;
        // Apply movement to the Rigidbody
        rb.velocity = movement;
    }

    // Method to turn the AI
    private void Turning(float input)
    {
        // Calculate turning vector based on input and turning speed
        // Input of -1f means turning left, 1f means turning right
        Vector3 turning = Vector3.up * input * turningSpeed;
        // Apply angular velocity to the Rigidbody to make it turn
        rb.angularVelocity = turning;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Obstacle") && !other.gameObject.CompareTag("Wall"))
        {
            return;
        }

        float leftLength = Mathf.Infinity;
        float rightLength = Mathf.Infinity;

        RaycastHit leftHit;
        RaycastHit rightHit;

        if (Physics.Raycast(transform.position, transform.forward + transform.right * -1, out leftHit, Mathf.Infinity, obstacleMask))
        {
            leftLength = leftHit.distance;
        }
        if (Physics.Raycast(transform.position, transform.forward + transform.right, out rightHit, Mathf.Infinity, obstacleMask))
        {
            rightLength = rightHit.distance;
        }

        if (leftLength > rightLength)
        {
            state = State.left;
        }
        else
        {
            state = State.right;
            target = rightHit.point;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        nextState = State.forward;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Obstacle") && !collision.gameObject.CompareTag("Wall"))
        {
            return;
        }
    }
}
