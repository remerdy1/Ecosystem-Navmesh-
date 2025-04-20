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

    public List<Transform> visibleFood = new List<Transform>();
    public List<Transform> visiblePrey = new List<Transform>();
    public List<Transform> visiblePredators = new List<Transform>();

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
        visibleFood.Clear();
        visiblePredators.Clear();
        visiblePrey.Clear();

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

        visibleFood = getVisibleTargets(foodInViewRadius);
        visiblePrey = getVisibleTargets(preyInViewRadius);
        visiblePredators = getVisibleTargets(predatorsInViewRadius);
    }

    public Transform ClosestTargetInView(TargetType targetType)
    {
        List<Transform> visibleTargets;

        switch (targetType)
        {
            case TargetType.Prey:
                visibleTargets = visiblePrey;
                break;
            case TargetType.Predator:
                visibleTargets = visiblePredators;
                break;
            case TargetType.Food:
                visibleTargets = visibleFood;
                break;
            default:
                return null;
        }

        if (visibleTargets.Count == 0) return null;

        (Transform target, float distance) closest = (null, 0);

        foreach (Transform target in visibleTargets)
        {
            float distance = Vector3.Distance(target.position, transform.position);

            if (closest.target == null || distance < closest.distance)
            {
                closest = (target, distance);
            }
        }

        return closest.target;
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

    private List<Transform> getVisibleTargets(List<Transform> inViewRadius)
    {
        List<Transform> visibleTargets = new List<Transform>();

        for (int i = 0; i < inViewRadius.Count; i++)
        {
            Transform target = inViewRadius[i];
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // Check to see if the target is within the viewing range
            if (Vector3.Angle(transform.forward, dirToTarget) < angle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // Check to see if the target is blocked
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }

        return visibleTargets;
    }
}