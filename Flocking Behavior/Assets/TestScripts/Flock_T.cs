using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_T : MonoBehaviour
{
    float speed;
    bool turning = false;
    public bool isLeader = false; // 确定当前对象是否为领导者

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

                // 寻找最近的领导者
                foreach (GameObject leader in FlockManager_T.FM.allLeader)
                {
                    float distanceToLeader = Vector3.Distance(this.transform.position, leader.transform.position);
                    if (distanceToLeader < closestDistance)
                    {
                        closestDistance = distanceToLeader;
                        closestLeader = leader; // 更新最近的领导者
                    }
                }

                // 如果找到最近的领导者，朝其方向旋转
                if (closestLeader != null && closestDistance > FlockManager_T.FM.neighbourDistance)
                {
                    Vector3 direction = closestLeader.transform.position - transform.position;
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager_T.FM.rotationSpeed * Time.deltaTime);
                    // 如果需要，增加向领导者的速度
                    //speed += 0.1f;
                }
            }

            // 随机调整速度
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManager_T.FM.minSpeed, FlockManager_T.FM.maxSpeed);
            }
           
            // 应用鱼群行为
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
                        // 增加分离力
                        vavoid += this.transform.position - go.transform.position;
                    }

                    Flock_T anotherFlock = go.GetComponent<Flock_T>();
                    gSpeed += anotherFlock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            // 计算中心点和分离力
            vcentre = vcentre / groupSize + (FlockManager_T.FM.goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            // 控制速度范围
            if (speed > FlockManager_T.FM.maxSpeed)
            {
                speed = FlockManager_T.FM.maxSpeed;
            }

            // 计算方向并调整旋转
            Vector3 direction = (vcentre + vavoid) - this.transform.position;

            // 增加分离力度
            if (vavoid.magnitude > 0)
            {
                direction += vavoid.normalized * 2; // 调整分离力度
            }

            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), FlockManager_T.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
