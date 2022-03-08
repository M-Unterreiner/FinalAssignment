using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RayScript : MonoBehaviour
{
    private bool isRayFlagOn = false;
    private LineRenderer rayRenderer;

    private GameObject rayIntersectionSphere = null;

    private RaycastHit hittedByRayCast;

    private GameObject rayHandController = null;
    private XRController rayXRController = null;

    public void enableRay()
    {
        isRayFlagOn = true;
    }

    public void disableRay()
    {
        isRayFlagOn = false;
    }

    public RayScript(LineRenderer RayRenderer, GameObject HandController, XRController HandXRController, String rayRendererName)
    {
        rayRenderer = RayRenderer;
        rayXRController = HandXRController;

        rayRenderer.name = rayRendererName;
        rayRenderer.startWidth = 0.01f;
        rayRenderer.positionCount = 2;

        createRayIntersectionSphere();

        //Debug.Log("RayRenderer created");
    }

    /*
    *  
    */
    public GameObject showRay(GameObject HandController)
    {
        // Debug.Log("Start Ray Selection");
        float trigger;
        rayXRController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out trigger);
        rayHandController = HandController;

        UpdateRayVisualization(trigger, 0.00001f);

        return null;
    }

    /*
     * This function updates the ray and hitpoint visualization
    */
    private void UpdateRayVisualization(float inputValue, float threshold)
    {
        // Debug.Log("UpdateRayVisualization input variable is over threshold: " + inputValue + "isRayFlagOn: " + isRayFlagOn);
        // Visualize ray if input value is bigger than a certain treshhold
        if (inputValue > threshold && isRayFlagOn == false)
        {
            rayRenderer.enabled = true;
            enableRay();
        }
        else if (inputValue < threshold && isRayFlagOn == true)
        {
            rayRenderer.enabled = false;
            disableRay();
        }

        // update ray length and intersection point of ray
        if (isRayFlagOn)
        { // if ray is on
            // Debug.Log("UpdateRayVisualization enables Hitpoint");
            enableHitpoint();
        }
        else
        {
            // Debug.Log("UpdateRayVisualization disable Hitpoint");
            disableHitpoint();
        }
    }

    // Enables hit point to the ray
    private void enableHitpoint()
    {
        if (hitsRaycast())
        {
            //Debug.Log("enableHitpoints tries to enableRayIntersectionSphere");
            enableRayIntersectionSphere();
        }
        else
        {
            disableRayIntersectionSphere();
        }
    }
    private void disableHitpoint()
    {
        //Debug.Log("Disable hitpoint");
        rayIntersectionSphere.SetActive(false);
    }

    // 
    private bool hitsRaycast()
    {
        //Debug.Log("HandControllerPosition: " + SelectionHandController.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(rayHandController.transform.position,
                    rayHandController.transform.TransformDirection(Vector3.forward),
                    out hit, Mathf.Infinity /*, myLayerMask */))
        {
            //Debug.Log("hitsRaycast true");
            hittedByRayCast = hit;
            return true;
        }
        else
        {
            // Debug.Log("hitsRaycast false");
            return false;
        }

    }

    // if nothing is hit set ray length to 100
    private void enableRayIntersectionSphere()
    {
        //Debug.Log("enableRayIntersectionSphere");
        rayRenderer.SetPosition(0, rayHandController.transform.position);
        rayRenderer.SetPosition(1, hittedByRayCast.point);
        rayIntersectionSphere.SetActive(true);
        rayIntersectionSphere.transform.position = hittedByRayCast.point;
    }

    private void disableRayIntersectionSphere()
    {
        //Debug.Log("disableRayIntersectionSphere");
        rayRenderer.SetPosition(0, rayHandController.transform.position);
        rayRenderer.SetPosition(1, rayHandController.transform.position + rayHandController.transform.TransformDirection(Vector3.forward) * 100);
        rayIntersectionSphere.SetActive(false);
    }


    // geometry for intersection visualization
    public void createRayIntersectionSphere()
    {
        rayIntersectionSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rayIntersectionSphere.name = "Ray Intersection Sphere";
        rayIntersectionSphere.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        rayIntersectionSphere.GetComponent<MeshRenderer>().material.color = Color.blue;
        rayIntersectionSphere.GetComponent<SphereCollider>().enabled = false;
        rayIntersectionSphere.SetActive(false);

        //Debug.Log("RayIntersectionSphere created");
    }

}
