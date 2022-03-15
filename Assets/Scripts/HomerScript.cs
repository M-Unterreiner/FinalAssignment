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
            SelectObject(handDetector.collidedObject);
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

    private void SelectObject(GameObject collidedObject)
    {
        selectedObject = collidedObject;
        Vector3 lossyScaleOfSelectedObject = selectedObject.transform.lossyScale;

        selectedObject.transform.SetParent(hand.transform, false);

        Matrix4x4 mat_obj = newLocalTranslationRotationScalingSelectedObject(selectedObject);
        Matrix4x4 mat_hand = newTranslationRotationScalingMatrix(hand);
        Matrix4x4 mat_scene = newTranslationRotationScalingMatrix(scene);

        Matrix4x4 mat_SelectedObject = Matrix4x4.Inverse(mat_hand) * mat_scene * mat_obj;

        SetTransformByMatrix(selectedObject, mat_SelectedObject);

        selectedObject.transform.localScale = lossyScaleOfSelectedObject;
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

    void SetTransformByMatrix(GameObject go, Matrix4x4 mat) // helper function
    {
        go.transform.localPosition = mat.GetColumn(3);
        go.transform.localRotation = mat.rotation;
        go.transform.localScale = mat.lossyScale;
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

        SetTransformByMatrix(selectedObject, mat_go);

        selectedObject = null;
    }



}
