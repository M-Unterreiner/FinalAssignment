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

    private GameObject hand;
    private GameObject handController;
    private GameObject handCenter;
    private XRController handXRController;

    private CollisionDetectorScript handDetector;
    private GameObject handColliderProxy;


    void Awake()
    {
        mySelectionRay = GetComponent<RayScript>();
        mySelectionRay.enableRay();

        handController = GameObject.Find("RightHand Controller");
        handXRController = handController.GetComponent<XRController>();
        hand = transform.Find("Camera Offset/RightHand Controller/HandRight").gameObject;
        handCenter = transform.Find("Camera Offset/RightHand Controller/RightHandCenter").gameObject;

        handColliderProxy = GameObject.Find("HandColliderProxy");
        handDetector = handColliderProxy.GetComponent<CollisionDetectorScript>();
    }

    private void Update()
    {
        // Debug.Log("Does hand collide: " + handDetector.collided);
        Debug.Log("Position hand: " + hand.transform.position + "Collider "+ handDetector.transform.position);
        checkHandCollition();
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
        //Debug.Log(mySelectionRay.showRay(HandController));
        lastSelectedObject = mySelectionRay.showRay(HandController);
    }

    public void grabHomer()
    {
        if(lastSelectedObject != null)
        {
            Debug.Log("Grab Object: " + lastSelectedObject);
            if (!handDetector.collided)
            {
                moveHandToObject(hand, handCenter);
            }

        }
    }

    public void checkHandCollition()
    {
        handColliderProxy.transform.localPosition = hand.transform.localPosition;
    }

    public void deGrabHomer()
    {
        // Set Hand back to position of Controller
        hand.transform.position = handController.transform.position;
    }

    public void moveHandToObject(GameObject hand, GameObject handCenter)
    {
        hand.transform.position = Vector3.MoveTowards(hand.transform.position, lastSelectedObject.transform.position, 5.0f * Time.deltaTime);
    }







}
