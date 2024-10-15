using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leader : MonoBehaviour
{
    public float speed = 3.0f; // 领导者的速度
    public float moveSpeed = 3.0f; // 移动速度
    public float turnSpeed = 5.0f; // 旋转速度
    public float wanderRadius = 5.0f; // 随机游动半径

    void Update()
    {
        // 随机选择目标方向
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        // 确保领导者鱼在游泳区域内
        Vector3 targetPosition = ClampToBounds(randomDirection);

        // 计算目标方向
        Vector3 targetDirection = targetPosition - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, turnSpeed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    // 限制领导者鱼在游泳区域内
    private Vector3 ClampToBounds(Vector3 position)
    {
        if (FlockManager_T.FM == null)
        {
            Debug.LogError("FlockManager.FM is not initialized. Please check your FlockManager instance.");
            return position; // 返回原位置
        }

        Vector3 swimLimits = FlockManager_T.FM.swimLimits;
        Vector3 center = FlockManager_T.FM.transform.position;

        // 限制位置在游泳区域内
        position.x = Mathf.Clamp(position.x, center.x - swimLimits.x, center.x + swimLimits.x);
        position.y = Mathf.Clamp(position.y, center.y - swimLimits.y, center.y + swimLimits.y);
        position.z = Mathf.Clamp(position.z, center.z - swimLimits.z, center.z + swimLimits.z);

        return position;
    }
}
