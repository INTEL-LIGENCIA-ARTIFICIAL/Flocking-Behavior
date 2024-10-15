using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_T : MonoBehaviour
{
    float speed; // 
    bool turning = false; // 

    void Start()
    {
        speed = Random.Range(FlockManager_T.FM.minSpeed, FlockManager_T.FM.maxSpeed);
    }

    void Update()
    {
        Bounds b = new Bounds(FlockManager_T.FM.transform.position, FlockManager_T.FM.swimLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true; // 
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = FlockManager_T.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager_T.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManager_T.FM.minSpeed, FlockManager_T.FM.maxSpeed); // 
            }

            if (Random.Range(0, 100) < 10)
            {
                ApplyFlockingRules(); // 
            }
        }

        this.transform.Translate(0, 0, speed * Time.deltaTime); // 
    }

    void ApplyFlockingRules()
    {
        GameObject[] gos = FlockManager_T.FM.allFish;
        Vector3 vcentre = Vector3.zero; // 
        Vector3 vavoid = Vector3.zero; // 
        Vector3 alignment = Vector3.zero; // 
        float gSpeed = 0.01f; // 
        float nDistance; // 
        int groupSize = 0; // 

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject) // 
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                if (nDistance < FlockManager_T.FM.neighbourDistance) // 
                {
                    vcentre += go.transform.position; // 
                    groupSize++;

                    if (nDistance < 1.0f) // 
                    {
                        vavoid += this.transform.position - go.transform.position;
                    }

                    Flock_T anotherFlock = go.GetComponent<Flock_T>();
                    gSpeed += anotherFlock.speed; // 
                }
            }
        }

        
        if (FlockManager_T.FM.leader != null)
        {
            Vector3 leaderDirection = FlockManager_T.FM.leader.transform.position - this.transform.position;
            vcentre += leaderDirection; // 
            speed += 0.1f; // 
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (FlockManager_T.FM.goalPos - this.transform.position); // 
            speed = gSpeed / groupSize; // 

            if (speed > FlockManager_T.FM.maxSpeed)
            {
                speed = FlockManager_T.FM.maxSpeed; // 
            }

            Vector3 direction = (vcentre + vavoid) - this.transform.position;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager_T.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
