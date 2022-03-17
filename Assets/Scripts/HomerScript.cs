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
    private bool lastSelectedObjectIsEmptyFlag = true;
    private GameObject emptyGameObject = null;

    private GameObject head;
    private GameObject hand;
    private GameObject handController;
    private GameObject handCenter;
    private XRController handXRController;

    private CollisionDetectorScript handDetector;
    private GameObject handColliderProxy;

    private Vector3 handPositionOnCollision;

    private Vector3 initialHandPosition;
    private GameObject newHandCenterNode;
    bool isNewHandCenterInUse = false;

    void Awake()
    {
        mySelectionRay = GetComponent<RayScript>();
        mySelectionRay.enableRay();

        scene = GameObject.Find("Terrain");
        head = GameObject.Find("Camera Offset");
        
        handController = GameObject.Find("RightHand Controller");
        handXRController = handController.GetComponent<XRController>();
        hand = transform.Find("Camera Offset/RightHand Controller/HandRight").gameObject;
        handCenter = transform.Find("Camera Offset/RightHand Controller/RightHandCenter").gameObject;
       
        handColliderProxy = GameObject.Find("HandColliderProxy");
        handDetector = handColliderProxy.GetComponent<CollisionDetectorScript>();
        initialHandPosition = handCenter.transform.position;

        newHandCenterNode = GameObject.Find("NewHandCenterNode");
        setNewHandCenterNodePosition(initialHandPosition);

        Debug.Log(initialHandPosition);
    }

    private void Update()
    {
        refreshHandColliderPosition();
        if (lastSelectedObjectIsEmptyFlag) Debug.Log("Last selected Object is empty.");
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
        initialHandPosition = handCenter.transform.position;
        setNewHandCenterNodePosition(initialHandPosition);
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
        GameObject selectedGameObjectByHitRay = mySelectionRay.showRay(HandController);
        if (selectedGameObjectByHitRay)
        {
            lastSelectedObject = selectedGameObjectByHitRay;
            setLastSelectedObjectIsEmptyFlag(false);
        }
    }

    public void grabHomer()
    {        
        if (!handDetector.collided)
        {
            moveHandToObject(hand, handCenter);
        } else
        {
            Debug.Log("Collided Object: " + handDetector.collidedObject.name);
            setHandnewCenterFlag(true);
            resetHandPosition();
            grabObject(handDetector.collidedObject);
            resetCollidedObject();
            resetLastSelectedObject();
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
            handPositionOnCollision = hand.transform.position;
        }
    }

    /*
     * Sets hand position back to handController position, change parten to handcontroller 
     */
    public void deGrabHomer()
    {
        // Set Hand back to position of Controller, would be nicer if hand moves back
        /*
        setNewHandCenterNodePosition(initialHandPosition);
        changeParentOfHandControllerTo(head);
        setHandnewCenterFlag(false);
        resetCollidedObject();
        resetLastSelectedObject();
        */

        //hand.transform.position = handController.transform.position;
        hand.transform.position = newHandCenterNode.transform.position;
    }

    public void resetHomer()
    {
        hand.transform.position = handController.transform.position;
        setNewHandCenterNodePosition(initialHandPosition);
        changeParentOfHandControllerTo(head);
        setHandnewCenterFlag(false);
        resetCollidedObject();
        resetLastSelectedObject();
    }

    /*
     * Moves Hand to object position.
     */
    public void moveHandToObject(GameObject hand, GameObject handCenter)
    {
        if (lastSelectedObjectIsEmptyFlag)
        {
            // Debug.Log("Move to lastSelectedObject: " + lastSelectedObject.name + " With Position: " + lastSelectedObject.transform.position);
            Debug.Log("lastSelectedObjectIsEmpty");
        } else
        {
            hand.transform.position = Vector3.MoveTowards(hand.transform.position, lastSelectedObject.transform.position, 7.5f * Time.deltaTime);
        }        
    }

    public void resetHandPosition()
    {
        hand.transform.position = handController.transform.position;
    }

    /*
    * GrabObjects sets new position to the virtual hand and resets CollidedObject.
    */
    private void grabObject(GameObject collidedObject)
    {
        setNewHandCenterNodePosition(handPositionOnCollision);
        changeParentOfHandControllerTo(newHandCenterNode);
        
        resetCollidedObject();
        resetLastSelectedObject();
    }

    private void setNewHandCenterNodePosition(Vector3 newPosition)
    {
        newHandCenterNode.transform.position = newPosition;
    }

    private void resetCollidedObject()
    {
        handDetector.collided = false;
        handDetector.collidedObject = null;
        Debug.Log("Resetted handDetector");
    }
    private void changeParentOfHandControllerTo(GameObject newParentOfHand)
    {
        handController.transform.SetParent(newParentOfHand.transform, true);
        //handCenter.transform.SetParent(newParentOfHand.transform, false);
        Debug.Log("New Parent of hand: " + hand.transform.parent.name);
        Debug.Log("New Parent of handCenter: " + handCenter.transform.parent.name);
    }

    private void setHandnewCenterFlag(bool set)
    {
        isNewHandCenterInUse = set;
    }

    private void resetLastSelectedObject() 
    {
        lastSelectedObjectIsEmptyFlag = true;
        lastSelectedObject = emptyGameObject;
        Debug.Log("Resetted lastSelectedObject: ");
    }

    public void setLastSelectedObjectIsEmptyFlag(bool toStatus)
    {
        lastSelectedObjectIsEmptyFlag = toStatus;
    }


}
