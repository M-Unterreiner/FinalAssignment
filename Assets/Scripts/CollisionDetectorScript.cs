using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetectorScript : MonoBehaviour
{
    public GameObject collidedObject;
    public bool collided;

    void OnCollisionEnter(Collision other)
    {
        collided = true;
        collidedObject = other.gameObject;
    }

    void OnCollisionExit(Collision other)
    {
        collided = false;
        collidedObject = null;
    }
}
