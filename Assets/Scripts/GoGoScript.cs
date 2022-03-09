using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GoGoScript : MonoBehaviour
{
    private GameObject scene = null;
    private GameObject rightHandController;
    private XRController rightXRController;

    public float threshhold = 0.35f;
    private GameObject head;
    private GameObject leftHand;
    private GameObject rightHand;
    private CollisionDetectorScript rightDetector;
    private GameObject leftHandCenter;
    private GameObject rightHandCenter;
    private GameObject rightHandColliderProxy;

    private GameObject selectedObject;

    // Start is called before the first frame update
    void Awake()
    {
        scene = GameObject.Find("Terrain");

        rightHandController = GameObject.Find("RightHand Controller");
        rightXRController = rightHandController.GetComponent<XRController>();

        head = transform.Find("Camera Offset/Main Camera").gameObject;
        leftHand = transform.Find("Camera Offset/LeftHand Controller/HandLeft").gameObject;
        rightHand = transform.Find("Camera Offset/RightHand Controller/HandRight").gameObject;
        leftHandCenter = transform.Find("Camera Offset/LeftHand Controller/LeftHandCenter").gameObject;
        rightHandCenter = transform.Find("Camera Offset/RightHand Controller/RightHandCenter").gameObject;

        rightHandColliderProxy = GameObject.Find("HandColliderProxy");
        rightDetector = rightHandColliderProxy.GetComponent<CollisionDetectorScript>();
    }

    // Update is called once per frame
    void Update()
    {        
        float distanceHandHead = calculateHandHeadDistance(rightHandCenter, head);
        float isomorphicDistance = Vector3.Distance(rightHandCenter.transform.position, head.transform.position); 

        if (distanceHandHead >= threshhold)
        {
            moveHandNonIsomorphic(distanceHandHead, rightHandCenter, rightHand, head, isomorphicDistance, 25.0f);
        }
        else
        {
            moveHandIsomorphic(rightHand, rightHandCenter, rightHandColliderProxy);
        }
        rightHandColliderProxy.transform.localPosition = rightHand.transform.localPosition;
    }

    private float calculateHandHeadDistance(GameObject hand, GameObject head)
    {
        Vector3 HeadToHandCenter = rightHandCenter.transform.position - head.transform.position;
        HeadToHandCenter.y = 0;
        float distanceHandHhead = HeadToHandCenter.magnitude;

        return distanceHandHhead;
    }

    public void startGoGoHand()
    {
        if (rightDetector.collided && selectedObject == null)
        {
            SelectObject(rightDetector.collidedObject);
        }
    }

    public void stopGoGoHand()
    {
        if (selectedObject != null)
        {
            DeselectObject();
        }
    }

    /*
     * Moves hand non isomrphic with factor k.
     */

    public void moveHandNonIsomorphic(float distanceHandHhead, GameObject handCenter, GameObject hand, GameObject head, float isomorphicDistance, float k)
    {
        float nonIsomorphicDistance = isomorphicDistance + k * Mathf.Pow(isomorphicDistance - threshhold, 2);

        float f = distanceHandHhead + k * Mathf.Pow((distanceHandHhead - threshhold), 2);
        Vector3 target = (hand.transform.position - head.transform.position).normalized * nonIsomorphicDistance + handCenter.transform.position;

        hand.transform.position = Vector3.MoveTowards(hand.transform.position, target, 2.0f * Time.deltaTime);        
    }

    /*
    * Moves hand isomrphic.
    */
    public void moveHandIsomorphic(GameObject hand, GameObject handCenter, GameObject handCollider)
    {
        hand.transform.position = Vector3.MoveTowards(hand.transform.position, handCenter.transform.position, 1.0f * Time.deltaTime);
        rightHandColliderProxy.transform.position = Vector3.MoveTowards(handCollider.transform.position, handCenter.transform.position, 1.0f * Time.deltaTime);
    }

    private Matrix4x4 newLocalTranslationRotationScalingSelectedObject(GameObject myObject)
    {
        Matrix4x4 mat_obj;
         return  mat_obj = Matrix4x4.TRS(myObject.transform.localPosition, myObject.transform.localRotation, myObject.transform.localScale);
    }

    private Matrix4x4 newTranslationRotationScalingMatrix(GameObject myObject)
    {
        Matrix4x4 mat_hand = Matrix4x4.TRS(myObject.transform.position, myObject.transform.rotation, myObject.transform.localScale);
        return mat_hand;
    }

    /* SelectObjects gets the scale of the object, sets the parent to the hand.
     * 
     */
    private void SelectObject(GameObject collidedObject)
    {
        selectedObject = collidedObject;
        Vector3 lossyScaleOfSelectedObject = selectedObject.transform.lossyScale;

        selectedObject.transform.SetParent(rightHand.transform, false);

        Matrix4x4 mat_obj = newLocalTranslationRotationScalingSelectedObject(selectedObject);
        Matrix4x4 mat_hand = newTranslationRotationScalingMatrix(rightHand);
        Matrix4x4 mat_scene = newTranslationRotationScalingMatrix(scene);

        Matrix4x4 mat_SelectedObject = Matrix4x4.Inverse(mat_hand) * mat_scene * mat_obj;

        SetTransformByMatrix(selectedObject, mat_SelectedObject);

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
        Matrix4x4 mat_hand = newTranslationRotationScalingMatrix(rightHand);
        Matrix4x4 mat_scene = newTranslationRotationScalingMatrix(scene);
        
        // sets the position to the selectedObjects
        Matrix4x4 mat_go = Matrix4x4.Inverse(mat_scene) * mat_hand * mat_obj;

        SetTransformByMatrix(selectedObject, mat_go);

        selectedObject = null;
    }

    void SetTransformByMatrix(GameObject go, Matrix4x4 mat) // helper function
    {
        go.transform.localPosition = mat.GetColumn(3);
        go.transform.localRotation = mat.rotation;
        go.transform.localScale = mat.lossyScale;
    }

    private void OnDisable()
    {
        rightHand.transform.position = rightHandCenter.transform.position;
        leftHand.transform.position = leftHandCenter.transform.position;
        rightHandColliderProxy.GetComponent<BoxCollider>().enabled = false;
        rightHandColliderProxy.SetActive(false);
    }

    private void OnEnable()
    {
        rightHand.transform.position = rightHandCenter.transform.position;
        leftHand.transform.position = leftHandCenter.transform.position;
        rightHandColliderProxy.GetComponent<BoxCollider>().enabled = true;
        rightHandColliderProxy.SetActive(true);
    }

}
