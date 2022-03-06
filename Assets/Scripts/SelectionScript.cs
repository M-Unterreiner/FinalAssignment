using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SelectionScript : MonoBehaviour
{
    private bool rayOnFlag = false;

    private GameObject rightRayIntersectionSphere = null;
    private GameObject SelectionHandController = null;
    private XRController SelectionXRController = null;

    private LineRenderer SelectionRayRenderer;

    public LayerMask myLayerMask;
    private RaycastHit hit;

    void Start()
    {
        rayOnFlag = true;
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

        if (SelectionXRController == null)
        {
            Debug.Log("SelectionXRController == null");
        }

        if (SelectionHandController == null)
        {
            Debug.Log("SelectionHandController == null");
        }
        
        SelectionRayRenderer.name = "Ray Renderer";
        SelectionRayRenderer.startWidth = 0.01f;
        SelectionRayRenderer.positionCount = 2;

        if (SelectionRayRenderer == null)
        {
            Debug.Log("SelectionRayRenderer == null");
        }

        Debug.Log("RayRenderer created");

        return SelectionRayRenderer;
    }

    public GameObject startRaySelection()
    {

        return null;
    }

    public GameObject SelectByHitRay()
    {

        return null;
    }

    // TODO: ImplementRayInterSectionSphere
    private void UpdateRayVisualization(float inputValue, float threshold)
    {
        // Visualize ray if input value is bigger than a certain treshhold
        if (inputValue > threshold && rayOnFlag == false)
        {
            SelectionRayRenderer.enabled = true;
            rayOnFlag = true;
        }
        else if (inputValue < threshold && rayOnFlag)
        {
            SelectionRayRenderer.enabled = false;
            rayOnFlag = false;
        }

        // update ray length and intersection point of ray
        if (rayOnFlag)
        { // if ray is on

            // Check if something is hit and set hit point
            if (Physics.Raycast(SelectionHandController.transform.position,
                        SelectionHandController.transform.TransformDirection(Vector3.forward),
                        out hit, Mathf.Infinity, myLayerMask))
            {
                SelectionRayRenderer.SetPosition(0, SelectionHandController.transform.position);
                SelectionRayRenderer.SetPosition(1, hit.point);

                // TODO: Implement the RayIntersectionSphere
                //rightRayIntersectionSphere.SetActive(true);
                //rightRayIntersectionSphere.transform.position = hit.point;
            }
            else
            { // if nothing is hit set ray length to 100
                SelectionRayRenderer.SetPosition(0, SelectionHandController.transform.position);
                SelectionRayRenderer.SetPosition(1, SelectionHandController.transform.position + SelectionHandController.transform.TransformDirection(Vector3.forward) * 100);

                rightRayIntersectionSphere.SetActive(false);
            }
        }
        else
        {
            rightRayIntersectionSphere.SetActive(false);
        }
    }
}
