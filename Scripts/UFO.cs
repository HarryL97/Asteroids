using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO - this class shares common methods with player clase better implementation?
// But i dont want UFO class to inherit from clone class
// KNOWNISSUE - UFO can destory asteroids and give points the the player
public class UFO : MonoBehaviour
{

    // TODO - camelCase
    [SerializeField]
    private Bullet bullet;

    [SerializeField]
    private int BulletSpawnDist;
    [SerializeField]
    private float ufoSpeed =1;
    // TODO add method to decrease shootError with roundNumber on UFO-2
    [SerializeField]
    private float ShootError;
    [SerializeField]
    private int ufoValue; 

    private Vector3 ShootAngle;
    private Transform ThisTransform;
    private Rigidbody ThisRB;

    
    void Start()
    {
        ThisTransform = GetComponent<Transform>();
        ThisRB = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(Random.Range(0,120) > 118) {
            shoot();
        }
    }

    public void Move(float maxAngle, float minAngle) {
        ThisRB = GetComponent<Rigidbody>();
        ThisRB.AddForce(DecideShootAngleBetween(minAngle,maxAngle)*ufoSpeed,ForceMode.Impulse);
    }

    private void shoot() {  //copied from player class, repeated code, there is a better way of doing this
        Bullet newBullet = Instantiate(bullet);
        float Angle2Player = FindAngle2Player();
        ShootAngle = DecideShootAngleBetween(Angle2Player - ShootError,Angle2Player + ShootError);
        newBullet.transform.position = FindBulletSpwan();
        newBullet.Bang(ShootAngle);
    }

    private Vector3 FindBulletSpwan() { //also copied from player class
        Vector3 BulletSpawn = ThisTransform.position + ShootAngle*BulletSpawnDist;  
        return BulletSpawn;
    } 

    private Vector3 DecideShootAngleBetween(float min, float max) {
        float angle = Random.Range(min,max);
        Vector3 Angle = new Vector3(Mathf.Cos(angle),Mathf.Sin(angle),0);
        return Angle;
    }

    private float FindAngle2Player() {
        GameObject TargetPlayer = GameObject.FindWithTag("Player");
        Vector3 DirVector = TargetPlayer.transform.position - ThisTransform.position;
        float Angle = Mathf.Atan2(DirVector.y,DirVector.x);
        return Angle;
        
    } 

    private void OnTriggerEnter(Collider collision)  {
        if(collision.gameObject.tag == "Bullet")  {
            GameManager.Instance.UfoCounter = 0;
            GameManager.Instance.AddScore(ufoValue);
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.tag == "Destroy Boundary") {
            GameManager.Instance.UfoCounter = 0;
            Destroy(this.gameObject);
        }
        else if(collision.gameObject.tag == "Player") {
        GameManager.Instance.UfoCounter = 0;
        GameManager.Instance.SubLives();
        Destroy(collision.gameObject);
        Destroy(this.gameObject);
        }
    }

}
