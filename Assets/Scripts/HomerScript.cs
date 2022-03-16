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

    private GameObject selectedObject = new GameObject();
    private Vector3 handPositionOnCollision;

    Vector3 initialHandPosition;
    private GameObject newHandCenterNode;
    bool isNewHandCenterInUse = false;

    void Awake()
    {
        mySelectionRay = GetComponent<RayScript>();
        mySelectionRay.enableRay();

        scene = GameObject.Find("Terrain");
        handController = GameObject.Find("RightHand Controller");
        handXRController = handController.GetComponent<XRController>();
        hand = transform.Find("Camera Offset/RightHand Controller/HandRight").gameObject;
        handCenter = transform.Find("Camera Offset/RightHand Controller/RightHandCenter").gameObject;

        newHandCenterNode = GameObject.Find("NewHandCenterNode");
        handColliderProxy = GameObject.Find("HandColliderProxy");
        handDetector = handColliderProxy.GetComponent<CollisionDetectorScript>();

        initialHandPosition = handController.transform.position;
    }

    private void Update()
    {
        refreshHandColliderPosition();
        if (isNewHandCenterInUse) addMatrixtoHand();
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
            Debug.Log("Collided Object: " + handDetector.collidedObject.name);
            setHandnewCenterFlag(true);
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
            handPositionOnCollision = hand.transform.position;
        }
    }

    public void deGrabHomer()
    {
        // Set Hand back to position of Controller, would be nicer if hand moves back
        hand.transform.position = handController.transform.position;
        setNewHandCenterNodePosition(initialHandPosition);
        changeParentOfHandTo(handController);
        setHandnewCenterFlag(false);
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
        setNewHandCenterNodePosition(handPositionOnCollision);
        //changeParentOfHandTo(newHandCenterNode);
        
        resetCollidedObject();        
    }

    private void setNewHandCenterNodePosition(Vector3 newPosition)
    {
        newHandCenterNode.transform.position = newPosition;
    }

    private void changeParentOfHandTo(GameObject newParentOfHand)
    {
        hand.transform.SetParent(newParentOfHand.transform, false);
        handCenter.transform.SetParent(newParentOfHand.transform,false);
        Debug.Log("New Parent of hand: " + hand.transform.parent.name);
        Debug.Log("New Parent of handCenter: " + handCenter.transform.parent.name);
    }

    private void resetCollidedObject()
    {
        handDetector.collided = false;
        handDetector.collidedObject = null;
        Debug.Log("Resetted handDetector");
    }

    private void setHandnewCenterFlag(bool set)
    {
        isNewHandCenterInUse = set;
    }
    private Matrix4x4 newTranslationRotationScalingMatrix(GameObject myObject)
    {
        Matrix4x4 mat_hand = Matrix4x4.TRS(myObject.transform.position, myObject.transform.rotation, myObject.transform.localScale);
        return mat_hand;
    }

    private void addMatrixtoHand()
    {
        Matrix4x4 mat_hand = newTranslationRotationScalingMatrix(hand);
        Matrix4x4 mat_newCenter = newTranslationRotationScalingMatrix(newHandCenterNode);

        Matrix4x4 mat_newHandCenter = /*Matrix4x4.Inverse(mat_newCenter) * */ mat_hand;
        SetTransformByMatrix(hand, mat_newHandCenter);
        setHandnewCenterFlag(false);
    }

    void SetTransformByMatrix(GameObject go, Matrix4x4 mat) // helper function
    {
        go.transform.localPosition = mat.GetColumn(3);
        go.transform.localRotation = mat.rotation;
        //go.transform.localScale = mat.lossyScale;
    }
}
