using UnityEngine;
using System.Collections;
using System;

public class Controller : MonoBehaviour
{
    public static Action<Vector3, Vector3> DragEvent;
    public static Action<Vector3> TouchEvent;
    public static Action<Vector3> TouchPressEvent;

    float dragThreshold = 15f;

    Vector3 touchStart;
    float touchTime;

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
            touchTime = Time.time;
        }
        if (Input.GetMouseButtonUp(0))
        {
            SendTouchEvent(Input.mousePosition);
        }
        #endif

        #if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            int touches = Input.touchCount;
            Touch touch = Input.GetTouch(touches - 1); // Consider the last touch to support multitasking (eg hitting while walking)

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStart = touch.position;
                    break;

                case TouchPhase.Moved:
                    break;

                case TouchPhase.Ended:
                    // Use melee weapon if the touches were made within permissable area
                    SendTouchEvent(touch.position);
                    break;
            }
        }
        #endif
    }

    private void SendTouchEvent(Vector3 touchPos)
    {
        if (IsSingleTouch(touchPos))
        {
            TouchEvent?.Invoke(touchPos);
        }
        else
        {
            DragEvent?.Invoke(touchStart, touchPos);
        }
    }

private bool IsSingleTouch(Vector3 endTouchPos)
    {
        return Vector3.Distance(touchStart, endTouchPos) < dragThreshold;
    }
}

