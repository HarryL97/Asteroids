using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO - change bullet GameObject 
// TODO - add sounds
// KNOWNISSUE - can shoot self;

public class Bullet : Clone<Bullet>
{
    [SerializeField]
    private int shootForce = 10;

    void Update()
    {
        bulletLife -= Time.deltaTime;
        if(bulletLife <= 0) {
            Destroy(this.gameObject);
        }
    }

    public void Bang(Vector3 ShootAngle)  {
        ThisRB = GetComponent<Rigidbody>();
        ThisRB.AddForce(ShootAngle*shootForce,ForceMode.Impulse);
    }

    // TODO - standardies collider over fewer classes 
    private void OnTriggerEnter(Collider collider) {
        if(collider.gameObject.tag == "Player") {
            GameManager.Instance.SubLives();
            Destroy(collider.gameObject);
            Destroy(this.gameObject);
        }
    }

}
