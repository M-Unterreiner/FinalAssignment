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
        float distanceR = Vector3.Distance(new Vector3(rightHandCenter.transform.position.x, 0, rightHandCenter.transform.position.z), new Vector3(head.transform.position.x, 0, head.transform.position.z));

        float distanceRx = (rightHandCenter.transform.position.x - head.transform.position.x);
        float distanceRy = (rightHandCenter.transform.position.y - head.transform.position.y);
        float distanceRz = (rightHandCenter.transform.position.z - head.transform.position.z);
        float k = 1.5f; // What is k again standing for?

        if (distanceR >= threshhold)
        {
            moveHand(distanceR, distanceRx, distanceRy, distanceRz, k);
        }
        else
        {
            rightHand.transform.localPosition = rightHandCenter.transform.localPosition;
        }
        rightHandColliderProxy.transform.localPosition = rightHand.transform.localPosition;
    }

    public void startGoGoHand()
    {
        if (rightDetector.collided && selectedObject == null)
        {
            // Debug.Log("Collided with Object: " + rightDetector.collidedObject.name);
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

    public void moveHand(float distanceR, float distanceRx, float distanceRy, float distanceRz, float k)
    {
        // non-isomorphic
        float fx = Mathf.Abs(distanceRx) + k * Mathf.Pow((Mathf.Abs(distanceRx) - threshhold), 2);
        float fy = distanceRy + k * Mathf.Pow((distanceRy - threshhold), 2);
        float fz = Mathf.Abs(distanceRz) + k * Mathf.Pow((Mathf.Abs(distanceRz) - threshhold), 2);

        float f = distanceR + k * Mathf.Pow((distanceR - threshhold), 2);


        float gogoz = Mathf.Sqrt(Mathf.Pow(fz, 2) + Mathf.Pow(fx, 2));

        rightHand.transform.localPosition = new Vector3(rightHand.transform.localPosition.x, rightHand.transform.localPosition.y, Mathf.Max(gogoz - threshhold - 0.06f, rightHandCenter.transform.localPosition.z));
        // Debug.Log(distanceR);
        // Debug.Log(gogoz - threshhold);
    }

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
        // Debug.Log("local sc a " + rightHandColliderProxy.transform.localScale);
        // Debug.Log("lossy sc a " + rightHandColliderProxy.transform.lossyScale);
        // Debug.Log(go.transform.name + go.transform.parent + selectedObject.transform.position);
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

    /*
     * DeselectObject sets parent to the selectedObject. Also it creates a new translation rotation sacling matrix for the selectedObject,
     * hand by ther actual position, and scene.
     * 
     */
    private void DeselectObject()
    {
        selectedObject.transform.SetParent(scene.transform, false);

        Matrix4x4 mat_obj = newLocalTranslationRotationScalingSelectedObject(selectedObject);
        Matrix4x4 mat_hand = newTranslationRotationScalingMatrix(rightHand);
        Matrix4x4 mat_scene = newTranslationRotationScalingMatrix(scene);

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
        //rightHandCenter.transform.position = rightHandCenter.transform.parent.position;
        rightHand.transform.position = rightHandCenter.transform.position;
        leftHand.transform.position = leftHandCenter.transform.position;
        rightHandColliderProxy.GetComponent<BoxCollider>().enabled = true;
        rightHandColliderProxy.SetActive(true);
    }

}
