using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Clone<Player>
{
    // Currently not in use can be added to the Thrust() method
    //[SerializeField]
    // private float ThrustForce; 
    [SerializeField]
    private Bullet bullet;
    // TODO camelcase
    [SerializeField]
    private float rotationStep;
    [SerializeField]
    private float BulletSpawnDist;

    private float PlayerRotation;
    private int hyperDriveCoolDown = 0;
    // Two child gameObjects with different animator components, to get sheild effect
    private Animator[] animators;


    // TODO - this is used to destroy the clone from the when the player is destroyed
    public void DestroyClone () {
        Destroy(InstanceOfMirroredClone);
    }
    
    void Start()
    {
        Turn(-1); //This is a dumb fix but Thrust() & Shoot() work off the player angle, that is returned from Turn()
        Turn(1);
        animators = GetComponentsInChildren<Animator>();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.RightArrow))
        {
            Turn(-1);
        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            Turn(1);
        }
        if(Input.GetKeyDown(KeyCode.Space)) {
            Shoot();
        }
        if(Input.GetKeyDown(KeyCode.H)) {
            EnterHyperDrive();
        }
    }

    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.UpArrow))
        {
            Thurst();
            foreach(Animator anim in animators) {
                anim.SetTrigger("Thrust");
            }
        }
        
    }

    public void TurnOffCollider() {
        ThisCollider.enabled = false;
        animators = GetComponentsInChildren<Animator>();
        foreach(Animator anim in animators) {
            anim.SetTrigger("SheildOn");
        }
    }

    public void TurnOnCollider() {
        ThisCollider.enabled = true;
        foreach(Animator anim in animators) {
            anim.SetTrigger("SheildOff");
        }
    }

    //TODO - Improve player movment, faster acceleration
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Sheild") {
            TurnOffCollider();
            StartCoroutine(SheildTimer());
            GameManager.Instance.PowerCounter = 0;
            Destroy(other.gameObject);
        }
    }

    IEnumerator SheildTimer() {
        yield return new WaitForSeconds(5);
        TurnOnCollider();
    }

    private void Thurst()
    {
        ThisRB.AddForce(new Vector2(playerAngle.x,playerAngle.y),ForceMode.Impulse);
    }

    private void Turn(int turnDir)
    {
        ThisTransform.eulerAngles = Vector3.Lerp(ThisTransform.rotation.eulerAngles, new Vector3(0,0,rotationStep*turnDir)+ThisTransform.rotation.eulerAngles, Time.deltaTime);
        PlayerRotation = ((ThisTransform.eulerAngles.z)*Mathf.PI)/180; 
        playerAngle = new Vector2(Mathf.Cos(PlayerRotation),Mathf.Sin(PlayerRotation));
    }

    private void Shoot()
    {
        Bullet newBullet = Instantiate(bullet);
        Vector3 StartPos = FindBulletSpwan();
        newBullet.transform.position = StartPos;
        newBullet.Bang(playerAngle);
    }

    private Vector3 FindBulletSpwan()
    {
        Vector3 BulletSpawn = ThisTransform.position + new Vector3(playerAngle.x,playerAngle.y,0)*BulletSpawnDist;  
        return BulletSpawn;
    } 

   private void EnterHyperDrive() { 
        if(InstanceOfMirroredClone == null && hyperDriveCoolDown == 0) {
            float randomX = GameManager.Instance.RandomXLocation();
            float randomY = GameManager.Instance.RandomYLocation();
            ThisTransform.position = new Vector3(randomX,randomY,0);
            hyperDriveCoolDown = 1;
            // TODO Add Animation
            StartCoroutine(HyperdriveCoolDown());
            
        }
   }

    IEnumerator HyperdriveCoolDown() {
        yield return new WaitForSeconds(10);
        hyperDriveCoolDown = 0;
    }

}
