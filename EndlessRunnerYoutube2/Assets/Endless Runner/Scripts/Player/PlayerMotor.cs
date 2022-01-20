using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    // Private Variables
    private const float LANE_DISTANCE = 2f;                 //define the lane width
    private const float TURN_SPEED = 0.05f;                 // As character moves across less turn a bit in direction of movement   
    private float jumpForce = 6f;                           // Jump force to be applied to player
    private float gravity = 12f;                            // Definition of gravity for the game
    private float verticalVelocity;                         // Float to manage the vertical movement of the player when jumping
    // Player Objects
    private Animator anim;                                  // Reference the Animator controller on the player object
    private CharacterController controller;                 // Reference the Character controller on the player object
    public GameObject Player;                               // Reference to the player object
    private int desiredLane = 1;                             // Which Lane to start in 0=left 1=middle 2=right 
    private bool isRunning = false;                         // Character is not yet running so set to false
    private bool isSliding = false;                         // determine if we are sliding or not
    // speed Modifier variables
    private float originalSpeed = 9.0f;                     // Speed at beginning of game  // was 9
    private float speed = 9.0f;                             // Float to manage current speed // was 9 


    private void Start()
    {
        speed = originalSpeed;                              // Set current speed to be original starting speed
        controller = GetComponent<CharacterController>();   // reference attached character controller on player object
        anim = GetComponent<Animator>();                    // Reference Animator on player object
    }

    private void Update()
    {

        //TEMPcode to start running
        if (Input.GetKeyDown(KeyCode.S))
        {
            StartRunning();
        }

        if (!isRunning)                                       // if is running false then game not started so return from function
        {
            return;                                           // if game is not started, dont run below code
        }
        // gather the inputs on which lane we should be in

        if ((MobileInput.Instance.SwipeLeft) || (Input.GetKeyDown(KeyCode.LeftArrow)))         // if Mobile or keyboard input swipe left or left arrow sp long a we are not sliding
        {
            MoveLane(false);                                                                    // Run move lane function passing in False variable
        }
        if ((MobileInput.Instance.SwipeRight) || (Input.GetKeyDown(KeyCode.RightArrow)))        // if Mobile input swipe right so long a we are not sliding
        {
            MoveLane(true);                                                                     // Run move lane function passing in False variable
        }

        // Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;                        // Set out target position to be current position on z * Forward
        if (desiredLane == 0)                                                                   // If the new lane will be 0 (far left)
        {
            targetPosition += Vector3.left * LANE_DISTANCE;                                     // move to the left lane by the Lane distance amount
        }
        else if (desiredLane == 2)                                                              // if new lane will be 2 (far right)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;                                    // move to the right lane by the Lane distance amount
        }


        // Calcuate move vector
        Vector3 moveVector = Vector3.zero;                                                      // Create and set movevector to be zero
        moveVector.x = (targetPosition - transform.position).x * speed;                         // move on x is target position - current position.x * speed moving
        // Calc Y movement

        if (controller.isGrounded)                                                              //if charcter controller is grounded
        {
            anim.SetBool("Grounded", true);                                                     // Set the animator state of Grounded to be true
            verticalVelocity = -0.1f;                                                           // give a little downward force to give move solid look

            if (MobileInput.Instance.SwipeUp || Input.GetKeyDown(KeyCode.UpArrow))              // if swipe up or up arrow then we are Jumping
            {
                //Jumping section
                if (isSliding)                                                                  //if we are sliding and jump
                {
                    anim.SetBool("Sliding", false);                                                         // Stop the animation for sliding
                    isSliding = false;                                                                       // we have stopped sliding
                }
                anim.SetTrigger("Jump");                                                        // Run the Jump Animtion
                verticalVelocity = jumpForce;                                                   // Apply jump force to vertical velocity
            }
            else if (MobileInput.Instance.SwipeDown || Input.GetKeyDown(KeyCode.DownArrow))     // if swipe down or down arrow then slide player
            {
                //sliding section
                StartSliding();                                                                 // Run the start Sliding function
            }
        }
        else                                                                                   // If we are not grounded we must be in the air
        {
            verticalVelocity -= (gravity * Time.deltaTime);                                     // slowly fall to ground level
            if (MobileInput.Instance.SwipeDown || Input.GetKeyDown(KeyCode.DownArrow))          // If we are in the air and swipe down or down arrow pressed, cancel the jump and fall immediatly 
            {
                verticalVelocity = -jumpForce;                                                  //drop immediatly to ground
            }
        }



        moveVector.y = verticalVelocity;                                                        // Set movevector y to be current vertical velocity
        moveVector.z = speed;                                                                   // Set movevector z to be current speed

        controller.Move(moveVector * Time.deltaTime);                                           // Use character controller to move player to movevector position

        //Rotate charatcter in direction of travel
        Vector3 dir = controller.velocity;                                                      // Which direction to turn variable
        if (dir != Vector3.zero)                                                                // if not zero then we are turning
        {
            dir.y = 0;                                                                          // prevent movement on y axis
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);               // rotate the player in direction of the lane change turn

        }

    }

    private void MoveLane(bool goingRight)                                                      // Function to MoveLane
    {
        desiredLane += (goingRight) ? 1 : -1;                                                   // if goingright is true add one or minus one from current lane
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);                                           // Prevent the movement outside of defined lanes by clamping to 0 or 2

    }



    public void StartRunning()                                                                  // Function to let game know to start the player running
    {
        isRunning = true;                                                                       // Set isruning to be true
        anim.SetTrigger("StartRunning");                                                        // Start the Animation for running
    }

    private void StartSliding()                                                                 // Function to manage the slide 
    {
        anim.SetBool("Sliding", true);                                                          // Set the sliding animation to be true
        isSliding = true;                                                                       // we are currently sliding
        controller.height /= 2;                                                                 // Half the defined controller height to go under obstacles
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z); // reset the center of the controler to allow player to fit under obstacles
        Invoke("StopSliding", 1.0f);                                                            // Invoke the stopSliding function after 1 second
    }

    private void StopSliding()                                                                  // Function to stop Sliding
    {
        anim.SetBool("Sliding", false);                                                         // Stop the animation for sliding
        isSliding = false;                                                                       // we have stopped sliding
        controller.height *= 2;                                                                 // Restet controler height to normal
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z); // reset the controller center back to normal

    }

}
