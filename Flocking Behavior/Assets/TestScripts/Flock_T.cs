using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_T : MonoBehaviour
{
    float speed;
    bool turning = false;
    public bool isLeader = false; // ȷ����ǰ�����Ƿ�Ϊ�쵼��

    void Start()
    {
        speed = Random.Range(FlockManager_T.FM.minSpeed, FlockManager_T.FM.maxSpeed);
    }

    void Update()
    {
        Bounds b = new Bounds(FlockManager_T.FM.transform.position, FlockManager_T.FM.swimLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = FlockManager_T.FM.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager_T.FM.rotationSpeed * Time.deltaTime + 10 * Time.deltaTime);
        }
        else
        {
            if (!isLeader)
            {
                GameObject closestLeader = null;
                float closestDistance = float.MaxValue;

                // Ѱ��������쵼��
                foreach (GameObject leader in FlockManager_T.FM.allLeader)
                {
                    float distanceToLeader = Vector3.Distance(this.transform.position, leader.transform.position);
                    if (distanceToLeader < closestDistance)
                    {
                        closestDistance = distanceToLeader;
                        closestLeader = leader; // ����������쵼��
                    }
                }

                // ����ҵ�������쵼�ߣ����䷽����ת
                if (closestLeader != null && closestDistance < FlockManager_T.FM.neighbourDistance)
                {
                    Vector3 direction = closestLeader.transform.position - transform.position;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager_T.FM.rotationSpeed * Time.deltaTime);
                    // �����Ҫ���������쵼�ߵ��ٶ�
                    speed += 0.1f;
                }
            }

            // ��������ٶ�
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManager_T.FM.minSpeed, FlockManager_T.FM.maxSpeed);
            }

            // Ӧ����Ⱥ��Ϊ
            if (Random.Range(0, 100) < 10)
            {
                ApplyFlockingRules();
            }
        }

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void ApplyFlockingRules()
    {
        GameObject[] gos = FlockManager_T.FM.allFish;
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        Vector3 alignment = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);

                if (nDistance < FlockManager_T.FM.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vavoid += this.transform.position - go.transform.position;
                    }

                    Flock_T anotherFlock = go.GetComponent<Flock_T>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }

        // �����쵼�ߵ�Ӱ��
        if (FlockManager_T.FM.allLeader != null && FlockManager_T.FM.allLeader.Length > 0)
        {
            foreach (GameObject leader in FlockManager_T.FM.allLeader)
            {
                Vector3 leaderDirection = leader.transform.position - this.transform.position;
                vcentre += leaderDirection;
            }
            speed += 0.1f;
        }

        if (groupSize > 0)
        {
            vcentre = vcentre / groupSize + (FlockManager_T.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            if (speed > FlockManager_T.FM.maxSpeed)
            {
                speed = FlockManager_T.FM.maxSpeed;
            }

            Vector3 direction = (vcentre + vavoid) - this.transform.position;

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager_T.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
