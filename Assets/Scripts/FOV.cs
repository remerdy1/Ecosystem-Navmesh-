using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class FOV : MonoBehaviour
{
    [Range(0, 360)] public float radius;
    public float angle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public List<Transform> foodInViewRadius;
    public List<Transform> preyInViewRadius;
    public List<Transform> predatorsInViewRadius;

    public enum TargetType
    {
        Prey,
        Predator,
        Food
    }

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTarget();
        }
    }

    void FindVisibleTarget()
    {
        foodInViewRadius.Clear();
        preyInViewRadius.Clear();
        predatorsInViewRadius.Clear();

        Collider[] overlap = Physics.OverlapSphere(transform.position, radius, targetMask);

        foreach (Collider collider in overlap)
        {
            if (collider.gameObject == gameObject)
                continue;

            switch (collider.gameObject.tag)
            {
                case "Food":
                    foodInViewRadius.Add(collider.gameObject.transform);
                    break;
                case "Prey":
                    preyInViewRadius.Add(collider.gameObject.transform);
                    break;
                case "Predator":
                    predatorsInViewRadius.Add(collider.gameObject.transform);
                    break;
                default:
                    continue;
            }
        }
    }

    public Transform ClosestTargetInRadius(TargetType targetType)
    {
        List<Transform> inViewRadius;

        switch (targetType)
        {
            case TargetType.Prey:
                inViewRadius = preyInViewRadius;
                break;
            case TargetType.Predator:
                inViewRadius = predatorsInViewRadius;
                break;
            case TargetType.Food:
                inViewRadius = foodInViewRadius;
                break;
            default:
                return null;
        }

        if (inViewRadius.Count == 0) return null;

        (Transform target, float distance) closest = (null, 0);


        foreach (Transform target in inViewRadius)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (closest.target == null || distance < closest.distance)
            {
                closest = (target, distance);
            }
        }

        return closest.target;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}