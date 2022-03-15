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
        //Debug.Log("Position hand: " + hand.transform.position + "Collider "+ handDetector.transform.position);
        checkHandCollition();
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

            moveHandToObject(hand, handCenter);           
            // Debug.Log("Grab Object: " + lastSelectedObject);
            if (handDetector.collided == true)
            {
                Debug.Log("Collided Object: " + handDetector.collidedObject.name);
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
        if (!handDetector.collided)
        {
            Debug.Log("Hand is not collided.");
            checkHandCollition();
            hand.transform.position = Vector3.MoveTowards(hand.transform.position, lastSelectedObject.transform.position, 5.0f * Time.deltaTime);
        } else
        {
            Debug.Log("Hand collided with: " + handDetector.collidedObject.name);
        }
    }
}
