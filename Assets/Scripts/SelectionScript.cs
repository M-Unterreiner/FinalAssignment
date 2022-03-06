using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectionScript : MonoBehaviour
{
    private bool isRayFlagOn = false;

    private GameObject rightRayIntersectionSphere = null;
    private GameObject SelectionHandController = null;
    private XRController SelectionXRController = null;

    private LineRenderer SelectionRayRenderer;

    public LayerMask myLayerMask;
    private GameObject rayIntersectionSphere = null;

    private RaycastHit hittedByRayCast;

    void Start()
    {
        isRayFlagOn = true;
    }

    private void Update()
    {
       
    }

    /*
     * Creates a RayRenderer 
     * 
     */
    public LineRenderer createRayRenderer(LineRenderer RayRenderer, GameObject HandController, XRController HandXRController)
    {
        SelectionRayRenderer = RayRenderer;
        SelectionHandController = HandController;
        SelectionXRController = HandXRController;

        SelectionRayRenderer.name = "Ray Renderer";
        SelectionRayRenderer.startWidth = 0.01f;
        SelectionRayRenderer.positionCount = 2;

        createRayIntersectionSphere();

        //Debug.Log("RayRenderer created");

        return SelectionRayRenderer;
    }

    public GameObject startRaySelection()
    {
        // Debug.Log("Start Ray Selection");
        float trigger;
        SelectionXRController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out trigger);

        UpdateRayVisualization(trigger, 0.00001f);

        return null;
    }

    public GameObject SelectByHitRay()
    {
        return null;
    }

    private void UpdateRayVisualization(float inputValue, float threshold)
    {
        // Debug.Log("UpdateRayVisualization input variable is over threshold: " + inputValue + "isRayFlagOn: " + isRayFlagOn);
        // Visualize ray if input value is bigger than a certain treshhold
        if (inputValue > threshold && isRayFlagOn == false)
        {
            SelectionRayRenderer.enabled = true;
            isRayFlagOn = true;
        }
        else if (inputValue < threshold && isRayFlagOn)
        {
            SelectionRayRenderer.enabled = false;
            isRayFlagOn = false;
        }

        // update ray length and intersection point of ray
        if (isRayFlagOn)
        { // if ray is on
            Debug.Log("UpdateRayVisualization enables Hitpoint");
            enableHitpoint();
        }
        else
        {
            Debug.Log("UpdateRayVisualization disable Hitpoint");
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


    private bool hitsRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(SelectionHandController.transform.position,
                    SelectionHandController.transform.TransformDirection(Vector3.forward),
                    out hit, Mathf.Infinity, myLayerMask))
        {
            Debug.Log("hitsRaycast true");
            hittedByRayCast = hit;
            return true;
        } else
        {
            Debug.Log("hitsRaycast false");
            return false;
        }

    }
    
    
    // if nothing is hit set ray length to 100
    private void enableRayIntersectionSphere()
        {
            //Debug.Log("enableRayIntersectionSphere");
            SelectionRayRenderer.SetPosition(0, SelectionHandController.transform.position);
            SelectionRayRenderer.SetPosition(1, hittedByRayCast.point);
            rayIntersectionSphere.SetActive(true);
            rayIntersectionSphere.transform.position = hittedByRayCast.point;
        }

    private void disableRayIntersectionSphere()
    {
        //Debug.Log("disableRayIntersectionSphere");
        SelectionRayRenderer.SetPosition(0, SelectionHandController.transform.position);
        SelectionRayRenderer.SetPosition(1, SelectionHandController.transform.position + SelectionHandController.transform.TransformDirection(Vector3.forward) * 100);
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
