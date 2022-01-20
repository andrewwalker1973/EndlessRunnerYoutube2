using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// script gotten from https://www.youtube.com/watch?v=sCgAb2cy6BY&list=PLLH3mUGkfFCXQcNBz_FZDpqJfQlupTznd&index=4

public class MobileInput : MonoBehaviour
{
    private const float DEADZONE = 100;                                     // Define Deadzone area to check if valid swipe

    public static MobileInput Instance { set; get; }                        // Able to referece from outside

    // Define public Variables
    private bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;            // bool to determine on/off ststus 
    private Vector2 swipeDelta, startTouch;                                 // Start touch -> where touch started swipe delta -> where ended 

    public bool Tap { get { return tap; } }
    public Vector2 SwipeDelta { get { return swipeDelta; } }
    public bool SwipeLeft { get { return swipeLeft; } }
    public bool SwipeRight { get { return swipeRight; } }
    public bool SwipeUp { get { return swipeUp; } }
    public bool SwipeDown { get { return swipeDown; } }

    public void Awake()
    {
        Instance = this;
    }



    private void Update()
    {
        // reset all the booleans to be false
        tap = swipeLeft = swipeRight = swipeUp = swipeDown = false;             // Nothing active so set all to false

        // Check for the inputs from mouse or mobile
        #region Standalone inputs for mouse input                               // Define the begining of the mouse input region
        if (Input.GetMouseButtonDown(0))                                       // if left mouse button clicked
        {
            tap = true;                                                         // Set Tap to true
            startTouch = Input.mousePosition;                                   // Set Starttouch equal to current mouse position

        }
        else if (Input.GetMouseButtonUp(0))                                     // if releasing the mouse click
        {
            startTouch = swipeDelta = Vector2.zero;                             // Set start touch to zero
        }
        #endregion                                                              // End of mouse input region

        #region Mobile inputs                                                   // Define begin region for mobile inputs
        if (Input.touches.Length != 0)                                          // If at least one touch on screen
        {
            if (Input.touches[0].phase == TouchPhase.Began)                     // if Touch Phase is begin
            {
                tap = true;                                                     // Set Tap = true
                startTouch = Input.mousePosition;                               // Set location of touch begin
            }
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)   // cancel if end touch phase or its cancellled phase 
            {
                startTouch = swipeDelta = Vector2.zero;                         // Set Starttouch back to zero
            }
        }

        #endregion                                                              // End region for mobile inputs

        // Now to Calculate distance of swipe
        swipeDelta = Vector2.zero;                                          // Set Swipe delta to zero
        if (startTouch != Vector2.zero)                                     // if starttouch not zero
        {
            // check with mobile inputs
            if (Input.touches.Length != 0)                                  // make sure there is at least one touch  for mobile
            {
                swipeDelta = Input.touches[0].position - startTouch;          // Set Swipe delta to where touch began minus begin position

            }

            // check with standalone mouse
            else if (Input.GetMouseButton(0))                               // if left mouse button clicked     
            {
                swipeDelta = (Vector2)Input.mousePosition - startTouch;     // Set swipe delta to current mouse position - start position
            }
        }

        // Lets check if we are in the dead zone to make sure this is a proper swipe
        if (swipeDelta.magnitude > DEADZONE)                                // if magnitude greated than dead zone then this is a confimed swipe
        {
            // This is now a confirmed swipe, which direction are we swiping ?
            float x = swipeDelta.x;                                         // set x to be swipe delta.x to begin the math function
            float y = swipeDelta.y;                                         // set y to be swipe delta.y to begin the math function

            if (Mathf.Abs(x) > Mathf.Abs(y))                            // if x bigger than y then it is a left/right swipe
            {
                // left or right swipe area
                if (x < 0)                                              // if x less than 0 then swiping left
                {
                    swipeLeft = true;                                   // Set the bool to be true for swipeleft for one frame
                }
                else                                                    // if x greater than 0 then moving right
                {
                    swipeRight = true;                                   // set swipe right to be true for one frame
                }
            }
            else                                                        // x bigger than y therefore up/down movement
            {
                // up or down movemnet area
                if (y < 0)                                              // if y less than 0 then swiping down                     
                {
                    swipeDown = true;                                   // Set swipedown true for one frame
                }
                else                                                    // if y greater than 0 then swiping up
                {
                    swipeUp = true;                                     // Set swipe up to be true for one frame
                }
            }

            startTouch = swipeDelta = Vector2.zero;                        // reset start touch and swipe delta to zero
        }

    }
}
