using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the "half in half out" effect when a gameObject gets to the screen boundary
/// It does this by cloning the object
/// </summary>

// KNOWNISSUE - Objects & Clones can escape the game area through the corners of the screen

public class Clone<T>: MonoBehaviour where T : MonoBehaviour
{

    private int cloneCounter = 0;   
    private int boundaryNumber;

    // TODO - moves these vars to child class
    protected Vector2 playerAngle = new Vector2(0,0);
    protected float bulletLife = 0.5f;
    // TODO - camelCase these Vars
    protected Clone<T> InstanceOfMirroredClone;
    protected Rigidbody ThisRB;
    protected Collider ThisCollider;
    protected Transform ThisTransform;

    private Vector3 ThisPosition;
    private Vector3 ThisRotation; 
    private Vector3 BoundaryPosition;
    
    //TODO - remove protctected getter and setters

    protected Vector3 SetVelocity {
        set {
            ThisRB = GetComponent<Rigidbody>();   
            ThisRB.velocity = value;
        }
        
    }
    protected int CloneCounter   {
        get {
            return cloneCounter;
        }
        set {
            cloneCounter = value;
        }
    }

    void Awake()
    {
        ThisTransform = GetComponent<Transform>();
        ThisRB = GetComponent<Rigidbody>();
        ThisCollider = GetComponent<Collider>();
    }

    protected void GetTransform() {
        ThisTransform = GetComponent<Transform>();
    }

    protected void AddClonePair(Clone<T> clone)    {
        InstanceOfMirroredClone = clone;
    }

    private void RemoveClonePair()    {
        InstanceOfMirroredClone = null;
    }

    private void OnTriggerStay(Collider collision)  {
        if(collision.gameObject.tag == "Boundary" && InstanceOfMirroredClone == null)  {
            boundaryNumber = collision.gameObject.GetComponent<InvsBoundary>().OppositeBoundaryNumber;
            CreateClone();
            PositionOfClone();
            PassCloneAtributes();
        }
        else if(collision.gameObject.tag == "Destroy Boundary") {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerExit(Collider collision) {
        if(collision.gameObject.tag == "Inner Boundary") {
            InstanceOfMirroredClone.RemoveClonePair(); 
            Destroy(this.gameObject);
        }

    }

    private void CreateClone()  {
        InstanceOfMirroredClone = Instantiate(this);
        InstanceOfMirroredClone.AddClonePair(this);
    }

    private void PositionOfClone() {
        InstanceOfMirroredClone.GetTransform();
        GetTransform();

        ThisPosition = ThisTransform.position;
        BoundaryPosition = GameManager.Instance.GetBoundaryPosition(boundaryNumber); 

        Vector3 objectSize = ThisCollider.bounds.max-ThisCollider.bounds.min;
        float objectMaxSize = Mathf.Max(Mathf.Max(objectSize.x,objectSize.y),objectSize.z)/2;
        

        switch(boundaryNumber) {
            case 0 :
                InstanceOfMirroredClone.transform.position = new Vector3(ThisPosition.x,BoundaryPosition.y+objectMaxSize,0);
                break;
            case 1 :
                InstanceOfMirroredClone.transform.position = new Vector3(ThisPosition.x,BoundaryPosition.y-objectMaxSize,0);
                break;
            case 2 :
                InstanceOfMirroredClone.transform.position = new Vector3(BoundaryPosition.x-objectMaxSize,ThisPosition.y,0);
                break;
            case 3 :
                InstanceOfMirroredClone.transform.position = new Vector3(BoundaryPosition.x+objectMaxSize,ThisPosition.y,0);
                break;
        }

    }

    private void PassCloneAtributes() {
        InstanceOfMirroredClone.transform.rotation = ThisTransform.rotation;
        InstanceOfMirroredClone.SetVelocity = ThisRB.velocity;
        InstanceOfMirroredClone.bulletLife = bulletLife;
        InstanceOfMirroredClone.playerAngle = playerAngle;
    }
}
    
