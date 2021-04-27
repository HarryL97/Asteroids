using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidMed : Asteroid
{
    protected override void SpawnRandomChildAsteroids() {
        int numSpawned = Random.Range(2,4);
        SpawnChildAsteroids(SmallAsteroid,numSpawned);
      
    }
}
