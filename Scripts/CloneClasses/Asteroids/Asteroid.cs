using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : Clone<Asteroid>
{
    [SerializeField]
    private Asteroid[] MedAsteroid;
    [SerializeField]
    protected Asteroid[] SmallAsteroid;
    [SerializeField]
    private GameObject explosion;

    [SerializeField]
    private float maxVelocity;
    [SerializeField]
    private int asteroidValue;

    void Start()
    {
        ThisRB.angularVelocity = new Vector3(0,0,1);
    }

    public void Move() {
        ThisRB.velocity = new Vector3(Random.Range(-maxVelocity,maxVelocity),Random.Range(-maxVelocity,maxVelocity),0);

    }

    private void OnTriggerEnter(Collider collision)  {
        // TODO - Score can be inscreased by UFO bullets

        if(collision.gameObject.tag == "Bullet")   {
            AsteroidHit();
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "Player")   {
            //TODO find a way to call DestroyClone()
            Destroy(collision.gameObject);
            GameManager.Instance.SubLives();

        }
        else if(collision.gameObject.tag == "UFO") {
            GameManager.Instance.UfoCounter = 0; //UFO counter is spagetti code called in 3 different classes fix this!;
            Destroy(collision.gameObject);          //collision trigger is also spagetti code!
            AsteroidHit();
        }

        
    }

    private void AsteroidHit() {

        if(InstanceOfMirroredClone != null) {
            Destroy(InstanceOfMirroredClone.gameObject);
        }

        SpawnRandomChildAsteroids();
        
        GameManager.Instance.AddScore(asteroidValue);
        GameObject boom = Instantiate(explosion);
        boom.transform.position = ThisTransform.position;

        Destroy(this.gameObject);
    }

    protected virtual void SpawnRandomChildAsteroids() {
        int numSpawned = Random.Range(2,4);
        int medSpawned = Random.Range(numSpawned,4);
        int smallSpawned = numSpawned - medSpawned;
        SpawnChildAsteroids(MedAsteroid,medSpawned);
        SpawnChildAsteroids(SmallAsteroid,smallSpawned);
    }

    protected void SpawnChildAsteroids(Asteroid[] asteroidType, int number2Spawn) {
        for(int i = 0; i < number2Spawn; i++)   {
            Asteroid newAsteroid = Instantiate(asteroidType[Random.Range(0,asteroidType.Length)]);

            newAsteroid.GetTransform();
            newAsteroid.SetVelocity = ThisRB.velocity;

            PlaceAsteroid(newAsteroid);
            AddRandomForce(newAsteroid);
        }
    }

    private void PlaceAsteroid(Asteroid asteroid) {
        float newX = ThisTransform.position.x + Random.Range(-1f,1f);
        float newY = ThisTransform.position.y + Random.Range(-1f,1f);
        asteroid.transform.position = new Vector3(newX,newY,0);
    }

    private void AddRandomForce(Asteroid asteroid) {
        asteroid.ThisRB.AddForce(new Vector2(Random.Range(-2f,2f),Random.Range(-2f,2f)),ForceMode.Impulse);
    }
}