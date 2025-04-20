using System.Collections.Generic;
using UnityEngine;

public class WaterController : MonoBehaviour
{
    [SerializeField] private List<Transform> positionMarkers = new List<Transform>();
    private Dictionary<Vector3, AgentController> occupiedPositions = new Dictionary<Vector3, AgentController>();

    void Start()
    {

        Transform markers = transform.Find("Markers");

        for (int i = 0; i < markers.childCount; i++)
        {
            positionMarkers.Add(markers.GetChild(i));
        }
    }

    public List<Vector3> GetAllPositionMarkers()
    {
        List<Vector3> positions = new List<Vector3>();

        foreach (Transform marker in positionMarkers)
        {
            positions.Add(marker.position);
        }

        return positions;
    }

    public Vector3 GetClosestPositionMarker(Vector3 fromPosition)
    {
        Vector3 closest = Vector3.zero;
        float minDistance = float.MaxValue;

        foreach (Transform marker in positionMarkers)
        {
            float distance = Vector3.Distance(fromPosition, marker.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = marker.position;
            }
        }

        if (!IsPositionOccupied(closest))
        {
            return closest;
        }
        else
        {
            return FindNearestUnoccupiedPosition(closest);
        }
    }

    public bool IsPositionOccupied(Vector3 position)
    {

        if (occupiedPositions.ContainsKey(position))
        {
            return true;
        }

        return false;
    }

    public Vector3 FindNearestUnoccupiedPosition(Vector3 desiredPosition)
    {
        List<Transform> sortedPositions = new List<Transform>(positionMarkers);

        sortedPositions.Sort((a, b) =>
            Vector3.Distance(a.position, desiredPosition).CompareTo(
            Vector3.Distance(b.position, desiredPosition)));

        foreach (var marker in sortedPositions)
        {
            if (!IsPositionOccupied(marker.position))
            {
                return marker.position;
            }
        }

        return desiredPosition;
    }

    public void OccupyPosition(Vector3 position, AgentController agent)
    {
        occupiedPositions[position] = agent;
    }

    public void UnoccupyPosition(Vector3 position)
    {
        if (occupiedPositions.ContainsKey(position))
        {
            occupiedPositions.Remove(position);
        }
    }
}
