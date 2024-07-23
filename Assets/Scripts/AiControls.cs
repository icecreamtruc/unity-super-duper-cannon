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

    // Public variables related to the AI's turret and shooting
    public Transform turret;             // Transform of the AI's turret
    public GameObject projectile;        // Projectile to be shot
    public Transform muzzle;             // Muzzle from where the projectile is shot

    // Private variables for internal use
    private Rigidbody rb;                // Rigidbody component for physics interactions
    private float t;                     // Timer variable (currently unused)
    
    private GameObject targetObject;     // Reference to the target (e.g., player)
    private Vector3 target;              // Position of the target

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();  // Get and store the Rigidbody component
    }

    // FixedUpdate is called at a fixed interval and is used for physics updates
    void FixedUpdate()
    {
        // Maintain the AI's level orientation by resetting rotation along the X and Z axes
        Vector3 currentRotation = rb.rotation.eulerAngles;
        rb.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);
        
        // Find the player if not already targeted
        if (targetObject != null)
        {
            // Update the target position if the player is already targeted
            target = targetObject.transform.position;
        }
        else
        { 
            // Find the player GameObject by tag and set it as the target
            targetObject = GameObject.FindGameObjectWithTag("Player");
        }
        
        // Calculate the angle between the AI's forward direction and the direction to the target
        float angle = Vector3.SignedAngle(transform.forward, target - transform.position, Vector3.up);
        
        // Adjust turning based on the angle to the target
        if(angle < 0)
        {
            // If angle is negative, target is to the left; turn left
            // Input of -1f means turning left
            turning(-1f);
        }
        else if (angle > 0)
        {
            // If angle is positive, target is to the right; turn right
            // Input of 1f means turning right
            turning(1f);
        }

        // Move towards the target if the target is generally in front of the AI
        if (Mathf.Abs(angle) < 90)
        {
            // Move forward if the target is within a 90-degree arc in front of the AI
            Move(1f);
        }
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
    private void turning(float input)
    {
        // Calculate turning vector based on input and turning speed
        // Input of -1f means turning left, 1f means turning right
        Vector3 turning = Vector3.up * input * turningSpeed;
        // Apply angular velocity to the Rigidbody to make it turn
        rb.angularVelocity = turning;
    }
}
