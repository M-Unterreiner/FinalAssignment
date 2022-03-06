using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerScript : MonoBehaviour
{
    
    private GameObject platformCenter = null;
    private GameObject rightHandController = null;
    private XRController rightXRController = null;
    
    public bool triggerPressed = false;
    public bool triggerReleased = false;
    private bool secondaryButtonLF = false;

    private GameObject XRRigGameobject;
    private GameObject mainCamera = null;

    // QUESTION: Is this LineRenderer clean code? Because the Controller shouldn't be interested 
    private LineRenderer rightRayRenderer = null;

    private SelectionScript select;
    private NavigationScript navigate;

    // Start is called before the first frame update
    void Start()
    {
        select = GetComponent<SelectionScript>();
        navigate = GetComponent<NavigationScript>();

        mainCamera = GameObject.Find("Main Camera");
        rightHandController = GameObject.Find("RightHand Controller");
        rightXRController = rightHandController.GetComponent<XRController>();

        if (rightHandController != null) // guard
        {
            rightXRController = rightHandController.GetComponent<XRController>();           
            XRRigGameobject = mainCamera.transform.parent.transform.parent.gameObject;

            rightRayRenderer = rightHandController.AddComponent<LineRenderer>();
            rightXRController = rightHandController.GetComponent<XRController>();
            rightRayRenderer = select.createRayRenderer(rightRayRenderer, rightHandController, rightXRController);
        }   
    }

    // Update is called once per frame
    // TODO: write control function for grip and primary button.
    void Update()
    {
        if (rightHandController != null) // guard
        {
            // mapping: grip button (middle finger)
            float grip = 0.0f;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.grip, out grip);
            controlGrip(grip);
            //Debug.Log(grip);

            // mapping: joystick
            Vector2 joystick;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out joystick);
            controlJoystick(joystick);                       

            // mapping: trigger (index finger)
            float trigger = 0.0f;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out trigger);
            //Debug.Log(trigger);
            controlTrigger(trigger);

            // mapping: primary button (A)
            bool primaryButton = false;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton);
            controlPrimaryButton(primaryButton);

            // mapping: secondary button (B)
            bool secondaryButton = false;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton);
            controlSecondaryButton(secondaryButton);
 
        }
    }

    private void controlGrip(float grip)
    {
        if (grip > 0.1f)
        {
            Debug.Log("Grip not implemented yet");
        }       
    }

    private void controlTrigger(float trigger)
    {
        triggerPressed = trigger > 0.9f;
        triggerReleased = !triggerPressed;

        if (trigger > 0.0f)
        {
            // Debug.Log("Trigger Touched");
            select.startRaySelection(rightHandController);
        }

    }

    /* controllJoystick controls the steering navigation.
     *            0.0,1.0
     *     -1.0,0.0     +1.0,0.0
     *           0.0,-1.0
     */
    private void controlJoystick(Vector2 joystick)
    {
        //Debug.Log(joystick);
        float inputScaleFactor = 0.05f;
       
        if (joystick.x < 0.15 && joystick.y > 0.85)
        {
            //Debug.Log("Move forwards" + joystick);
            navigate.steeringForwards(joystick, inputScaleFactor);
        }

        if (joystick.x < 0.15 && joystick.y < -0.85)
        {
            //Debug.Log("Move backwards" + joystick);
            navigate.steeringBackwards(joystick, inputScaleFactor);
        }

        if (joystick.x < -0.85 && joystick.y < 0.15)
        {
            //Debug.Log("Move left" + joystick);
            navigate.steeringLeft(joystick, inputScaleFactor);
        }

        if (joystick.x > 0.85 && joystick.y < 0.15)
        {
            //Debug.Log("Move right" + joystick);
            navigate.steeringRight(joystick, inputScaleFactor);
        }
    }
    private void controlPrimaryButton(bool primaryButton)
    {
        if (primaryButton)
        {
            Debug.Log("Primary Button not implemented yet");
        }
    }

    private void controlSecondaryButton(bool secondaryButton)
    {
        if (secondaryButton != secondaryButtonLF) // state changed
        {
            if (secondaryButton) // up (0->1)
            {
                // Debug.Log("Secondary Button pushed");
                navigate.ResetXRRig();                
            }
        }
        secondaryButtonLF = secondaryButton;
    }



}
