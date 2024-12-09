using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeCamInput : MonoBehaviour
{
    public float swipeSensitivity = 0.5f;
    public Vector3 movementAxis = Vector3.right;

    private Vector2 touchStart;
    
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;

                case TouchPhase.Moved:
                    Vector2 swipeDelta = touchStart - touch.position;


                    Vector3 move = new Vector3(swipeDelta.x, 0, swipeDelta.y) * swipeSensitivity * Time.deltaTime;


                    transform.Translate(move, Space.World);

                    // Update touchStart for smooth movement
                    touchStart = touch.position;
                    break;
            }
        }
    }
}
