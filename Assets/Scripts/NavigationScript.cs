using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationScript : MonoBehaviour
{
    private Vector3 startPosition = Vector3.zero;
    private Quaternion startRotation = Quaternion.identity;
    private Quaternion rotTowardsHit = Quaternion.identity;

    private GameObject mainCamera = null;

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera");

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        float height = mainCamera.transform.position.y - startPosition.y;
    }

    public void ResetXRRig()
    {
        // Debug.Log("ResetXRRig");
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    public void steeringForwards(Vector2 joystick, float inputScaleFactor)
    {
        Vector3 translationVector = new Vector3(0.0f, 0.0f, 0.0f);
        translationVector = new Vector3(0.0f, 0.0f, joystick.y * inputScaleFactor);
        transform.Translate(translationVector);
    }

    public void steeringBackwards(Vector2 joystick, float inputScaleFactor)
    {
        Vector3 translationVector = new Vector3(0.0f, 0.0f, 0.0f);
        translationVector = new Vector3(0.0f, 0.0f, joystick.y * inputScaleFactor);
        transform.Translate(translationVector);
    }

    public void steeringLeft(Vector2 joystick, float inputScaleFactor)
    {
        Vector3 translationVector = new Vector3(0.0f, 0.0f, 0.0f);
        translationVector = new Vector3(joystick.x * inputScaleFactor, 0.0f, 0.0f);
        transform.Translate(translationVector);
    }

    public void steeringRight(Vector2 joystick, float inputScaleFactor)
    {
        Vector3 translationVector = new Vector3(0.0f, 0.0f, 0.0f);
        translationVector = new Vector3(joystick.x * inputScaleFactor, 0.0f, 0.0f);

        transform.Translate(translationVector);
    }
}
