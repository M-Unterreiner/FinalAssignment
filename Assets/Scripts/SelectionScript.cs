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
        float trigger = 0.0f;

        //SelectionXRController.inputDevice.TryGetFeatureValue(CommonUsages.trigger, out trigger);
        //UpdateRayVisualization(trigger, 0.00001f);
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
}
