using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NavigationScript : MonoBehaviour
{
    private GameObject mainCamera = null;
    private GameObject platformCenter = null;
    private GameObject rightHandController = null;
    private XRController rightXRController = null;
    
    private Vector3 startPosition = Vector3.zero;
    private Quaternion startRotation = Quaternion.identity;
    private Quaternion rotTowardsHit = Quaternion.identity;

    public bool triggerPressed = false;
    public bool triggerReleased = false;
    private bool secondaryButtonLF = false;

    private GameObject XRRigGameobject;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        mainCamera = GameObject.Find("Main Camera");
        platformCenter = GameObject.Find("Center");
        rightHandController = GameObject.Find("RightHand Controller");

        if (rightHandController != null) // guard
        {
            rightXRController = rightHandController.GetComponent<XRController>();           
            XRRigGameobject = mainCamera.transform.parent.transform.parent.gameObject;
        }
    }

    // Update is called once per frame
    // TODO: write control function for grip and primary button. Rethink if the handController shouldn't be in a own class. 
    void Update()
    {
        if (rightHandController != null) // guard
        {
            // mapping: grip button (middle finger)
            float grip = 0.0f;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.grip, out grip);
            //Debug.Log(grip);

            // mapping: joystick
            Vector2 joystick;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out joystick);
            controlJoystick(joystick);
        

            // mapping: primary button (A)
            bool primaryButton = false;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton);

            // mapping: trigger (index finger)
            float trigger = 0.0f;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out trigger);
            //Debug.Log(trigger);
            controlTrigger(trigger);
            
            float height = mainCamera.transform.position.y - startPosition.y;

            // mapping: secondary button (B)
            bool secondaryButton = false;
            rightXRController.inputDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton);
            controlSecondaryButton(secondaryButton);
 
        }
    }

    /* controlTrigger controls the Trigger
     * No functionality at the moment.
     */

    private void controlTrigger(float trigger)
    {
        triggerPressed = trigger > 0.9f;
        triggerReleased = !triggerPressed;

        if (trigger > 0.0f)
        {
            Debug.Log("Trigger Touched");
        }

    }

    /* controllJoystick controls the steering navigation.
     * TODO: Add Rotation.
     *            0.0,1.0
     *     -1.0,0.0     +1.0,0.0
     *           0.0,-1.0
     */
    private void controlJoystick(Vector2 joystick)
    {
        //Debug.Log(joystick);
        float inputScaleFactor = 0.05f;
        Vector3 translationVector = new Vector3(0.0f, 0.0f, 0.0f);

        if (joystick.x < 0.15 && joystick.y > 0.85)
        {
            //Debug.Log("Move forwards" + joystick);
            translationVector = new Vector3(0.0f, 0.0f, joystick.y * inputScaleFactor);
        }

        if (joystick.x < 0.15 && joystick.y < -0.85)
        {
            //Debug.Log("Move backwards" + joystick);
            translationVector = new Vector3(0.0f, 0.0f, joystick.y * inputScaleFactor);
        }

        if (joystick.x < -0.85 && joystick.y < 0.15)
        {
            //Debug.Log("Move left" + joystick);
            translationVector = new Vector3(joystick.x * inputScaleFactor, 0.0f, 0.0f);
        }

        if (joystick.x > 0.85 && joystick.y < 0.15)
        {
            //Debug.Log("Move right" + joystick);
            translationVector = new Vector3(joystick.x * inputScaleFactor, 0.0f, 0.0f);
        }
        transform.Translate(translationVector);
    }

    private void controlSecondaryButton(bool secondaryButton)
    {
        if (secondaryButton != secondaryButtonLF) // state changed
        {
            if (secondaryButton) // up (0->1)
            {
                ResetXRRig();
            }
        }
        secondaryButtonLF = secondaryButton;
    }


    private void ResetXRRig()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}
