using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;



public class HomerScript : MonoBehaviour
{
    private LineRenderer SelectionRayRenderer;

    private GameObject scene = null;
    public LayerMask myLayerMask;

    private RayScript mySelectionRay;
    private GameObject lastSelectedObject = null;

    private GameObject hand;
    private GameObject handController;
    private GameObject handCenter;
    private XRController handXRController;

    private CollisionDetectorScript handDetector;
    private GameObject handColliderProxy;

    private GameObject selectedObject;
    private Vector3 positionOnHandCollision;

    private GameObject grabbedObjectHandPosition;

    void Awake()
    {
        mySelectionRay = GetComponent<RayScript>();
        mySelectionRay.enableRay();

        scene = GameObject.Find("Terrain");
        handController = GameObject.Find("RightHand Controller");
        handXRController = handController.GetComponent<XRController>();
        hand = transform.Find("Camera Offset/RightHand Controller/HandRight").gameObject;
        handCenter = transform.Find("Camera Offset/RightHand Controller/RightHandCenter").gameObject;
        

        handColliderProxy = GameObject.Find("HandColliderProxy");
        handDetector = handColliderProxy.GetComponent<CollisionDetectorScript>();
    }

    private void Update()
    {
        refreshHandColliderPosition();
    }

    private void OnDisable()
    {
        mySelectionRay.disableRay();
        handColliderProxy.GetComponent<BoxCollider>().enabled = false;
        handColliderProxy.SetActive(false);
        Debug.Log("HomerScript was disabled");
    }

    private void OnEnable()
    {
        mySelectionRay.enableRay();
        handColliderProxy.GetComponent<BoxCollider>().enabled = true;
        handColliderProxy.SetActive(true);
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
        deGrabHomer();
    }

    public void selectByHitRay(GameObject HandController)
    {
        //Debug.Log(mySelectionRay.showRay(HandController));
        lastSelectedObject = mySelectionRay.showRay(HandController);
    }

    public void grabHomer()
    {
        
        if (!handDetector.collided)
        {
            moveHandToObject(hand, handCenter);
        } else
        {
            grabObject(handDetector.collidedObject);
        }
    }

    /*
     * Refreshs the hand collider Position
     */
    public void refreshHandColliderPosition()
    {        
        handColliderProxy.transform.localPosition = hand.transform.localPosition;
        if (handDetector.collided)
        {
            positionOnHandCollision = hand.transform.localPosition;
        }
    }

    public void deGrabHomer()
    {
        // Set Hand back to position of Controller, would be nicer if hand moves back
        hand.transform.position = handController.transform.position;

    }

    public void moveHandToObject(GameObject hand, GameObject handCenter)
    {
        hand.transform.position = Vector3.MoveTowards(hand.transform.position, lastSelectedObject.transform.position, 7.5f * Time.deltaTime);
    }

    /*
    * GrabObjects sets new position to the virtual hand.
    */
    private void grabObject(GameObject collidedObject)
    {
        setGrabbedObjectHandPosition(collidedObject.transform.position);
        //changeParentOfHand(grabbedObjectHandPosition);
        //Debug.Log("New Parent of hand: " + hand.transform.parent.name);
    }

    private void setGrabbedObjectHandPosition(Vector3 newHandPosition)
    {
        grabbedObjectHandPosition.transform.position = newHandPosition;
    }

    private void changeParentOfHand(GameObject newParentOfHand)
    {
        hand.transform.SetParent(newParentOfHand.transform, false);

        // Set new TranslationMatrix???
    }
}
