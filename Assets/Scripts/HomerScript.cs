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
    private GameObject selectedObject;

    private GameObject head;
    private GameObject hand;
    private GameObject handController;
    private GameObject handCenter;
    private XRController handXRController;

    private CollisionDetectorScript handDetector;
    private GameObject handColliderProxy;

    private Vector3 handPositionOnCollision;
    private GameObject initialHandPosition;
    private GameObject newHandCenterNode;  

    bool isNewHandCenterInUse = false;
    bool isGrabHomerFlagOn = false;



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
        initialHandPosition = handCenter;

        newHandCenterNode = GameObject.Find("NewHandCenterNode");
        setNewHandCenterNodePosition(initialHandPosition.transform.position);

        Debug.Log(initialHandPosition);
    }

    private void Update()
    {
        refreshHandColliderPosition();
        //if (lastSelectedObjectIsEmptyFlag) Debug.Log("Last selected Object is empty.");

        if (isGrabHomerFlagOn)
        {
            grabHomer();
        }

    }

    private void OnDisable()
    {
        mySelectionRay.disableRay();
        handColliderProxy.GetComponent<BoxCollider>().enabled = false;
        handColliderProxy.SetActive(false);
        // Debug.Log("HomerScript was disabled");
    }

    private void OnEnable()
    {
        initialHandPosition.transform.position = handCenter.transform.position;
        setNewHandCenterNodePosition(initialHandPosition.transform.position);
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

    public void setGrabHomerFlagTo(bool state)
    {
        isGrabHomerFlagOn = state;
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
        if (!handDetector.collided && isNewHandCenterInUse == false)
        {
            moveHandToObject(hand, handCenter);
        } else if (handDetector.collidedObject != null && isNewHandCenterInUse == true)
        {
            selectObject(handDetector.collidedObject);
        }  else
        {
            // Debug.Log("Collided Object: " + handDetector.collidedObject.name);
            setNewHandCenterNodePosition(handPositionOnCollision);
            setHandnewCenterFlagTo(true);
            resetHandPosition();
            grabObject(handDetector.collidedObject);
            //resetCollidedObject();
            //resetLastSelectedObject();
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
        setNewHandCenterNodePosition(initialHandPosition.transform.position);
        changeParentOfHandControllerTo(head);
        setHandnewCenterFlagTo(false);
        //resetCollidedObject();
        //resetLastSelectedObject();
        // resetScaling();
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
            hand.transform.position = Vector3.MoveTowards(hand.transform.position, lastSelectedObject.transform.position, 5f * Time.deltaTime);
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
        Vector3 oldPosition = newHandCenterNode.transform.position;
        setNewHandCenterNodePosition(handPositionOnCollision);
        // Debug.Log("Set NewHandCenterNodePosition from: " + oldPosition + " to " + newHandCenterNode.transform.position);
        changeParentOfHandControllerTo(newHandCenterNode);        
        //resetCollidedObject();
        //resetLastSelectedObject();


    }

    private void setNewHandCenterNodePosition(Vector3 newPosition)
    {
        newHandCenterNode.transform.position = newPosition;
    }

    private void resetCollidedObject()
    {
        handDetector.collided = false;
        handDetector.collidedObject = null;
        // Debug.Log("Resetted handDetector");
    }
    private void changeParentOfHandControllerTo(GameObject newParentOfHand)
    {
        handController.transform.SetParent(newParentOfHand.transform, true);
        //correctScaling();
        //handCenter.transform.SetParent(newParentOfHand.transform, false);
        //Debug.Log("New Parent of hand: " + hand.transform.parent.name);
        //Debug.Log("New Parent of handCenter: " + handCenter.transform.parent.name);
    }

    private void setHandnewCenterFlagTo(bool set)
    {
        isNewHandCenterInUse = set;
    }

    private void resetLastSelectedObject() 
    {
        lastSelectedObjectIsEmptyFlag = true;
        lastSelectedObject = emptyGameObject;
        //Debug.Log("Resetted lastSelectedObject: " + lastSelectedObjectIsEmptyFlag);
    }

    public void setLastSelectedObjectIsEmptyFlag(bool toStatus)
    {
        lastSelectedObjectIsEmptyFlag = toStatus;
    }

    public void correctScaling()
    {
        //Vector3 newHandCenterNodeScale = new Vector3(newHandCenterNode.transform.localScale.x, newHandCenterNode.transform.localScale.y, newHandCenterNode.transform.localScale.z);
        //Vector3 handControllerScale = handController.transform.localScale;
        //handController.transform.localScale = new Vector3(handControllerScale.x / newHandCenterNodeScale.x, handControllerScale.y / newHandCenterNodeScale.y, handControllerScale.z / newHandCenterNodeScale.z);

        handController.transform.localScale = initialHandPosition.transform.lossyScale;
    }

    public void resetScaling()
    {
        handController.transform.localScale = initialHandPosition.transform.localScale;
    }

    private void selectObject(GameObject collidedObject)
    {
        selectedObject = collidedObject;
        Vector3 lossyScaleOfSelectedObject = selectedObject.transform.lossyScale;

        selectedObject.transform.SetParent(hand.transform, false);

        Matrix4x4 mat_obj = newLocalTranslationRotationScalingSelectedObject(selectedObject);
        Matrix4x4 mat_hand = newTranslationRotationScalingMatrix(hand);
        Matrix4x4 mat_scene = newTranslationRotationScalingMatrix(scene);

        Matrix4x4 mat_SelectedObject = Matrix4x4.Inverse(mat_hand) * mat_scene * mat_obj;

        setTransformByMatrix(selectedObject, mat_SelectedObject);

        selectedObject.transform.localScale = lossyScaleOfSelectedObject;
    }

    /*
 * DeselectObject sets parent to the selectedObject. Also it creates a new local translation rotation scaling matrix for the selectedObject and 
 * hand by their actual position, and scene. At the end it sets the position to the selectedObjects.
 * 
 */
    private void DeselectObject()
    {
        selectedObject.transform.SetParent(scene.transform, false);

        Matrix4x4 mat_obj = newLocalTranslationRotationScalingSelectedObject(selectedObject);
        Matrix4x4 mat_hand = newTranslationRotationScalingMatrix(hand);
        Matrix4x4 mat_scene = newTranslationRotationScalingMatrix(scene);

        // sets the position to the selectedObjects
        Matrix4x4 mat_go = Matrix4x4.Inverse(mat_scene) * mat_hand * mat_obj;

        setTransformByMatrix(selectedObject, mat_go);

        selectedObject = null;
    }

    void setTransformByMatrix(GameObject go, Matrix4x4 mat) // helper function
    {
        go.transform.localPosition = mat.GetColumn(3);
        go.transform.localRotation = mat.rotation;
        go.transform.localScale = mat.lossyScale;
    }

    private Matrix4x4 newLocalTranslationRotationScalingSelectedObject(GameObject myObject)
    {
        Matrix4x4 mat_obj;
        return mat_obj = Matrix4x4.TRS(myObject.transform.localPosition, myObject.transform.localRotation, myObject.transform.localScale);
    }

    private Matrix4x4 newTranslationRotationScalingMatrix(GameObject myObject)
    {
        Matrix4x4 mat_hand = Matrix4x4.TRS(myObject.transform.position, myObject.transform.rotation, myObject.transform.localScale);
        return mat_hand;
    }
}
