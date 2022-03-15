using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public class HomerScript : MonoBehaviour
{
    private LineRenderer SelectionRayRenderer;

    public LayerMask myLayerMask;

    private RayScript mySelectionRay;
    private GameObject lastSelectedObject = null;

   

    void Awake()
    {
        mySelectionRay = GetComponent<RayScript>();
        mySelectionRay.enableRay();        
    }

    private void OnDisable()
    {
        mySelectionRay.disableRay();
        Debug.Log("HomerScript was disabled");
    }

    private void OnEnable()
    {
        mySelectionRay.enableRay();
        Debug.Log("HomerScript was enabled");
    }

    /*
     * At the moment mainCamera.transform.position is not used which leads to an visibility selection difference
     */
    public LineRenderer createSelectionWithRay(LineRenderer RayRenderer, GameObject HandController, XRController HandXRController)
    {
        mySelectionRay = new RayScript(RayRenderer, HandController, HandXRController, "HomerScript");
        //Debug.Log("RayRenderer created");

        return SelectionRayRenderer;
    }

    public void showSelectionRay(GameObject HandController)
    {
        mySelectionRay.showRay(HandController);
    }

    public void selectByHitRay(GameObject HandController)
    {
        Debug.Log(mySelectionRay.showRay(HandController));

        lastSelectedObject = mySelectionRay.showRay(HandController);
    }

    public void grabHomer()
    {
        if(lastSelectedObject != null)
        {
            Debug.Log("Grab Object: " + lastSelectedObject);
        }
    }

    public void deGrabHomer()
    {
        Debug.Log("deGrabbing not yet implemented");
    }







}
