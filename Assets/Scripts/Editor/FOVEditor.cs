using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FOV))]
public class FOVEditor : Editor
{
    void OnSceneGUI()
    {
        FOV fov = (FOV)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.radius);

        // Vector3 viewAngleA = fov.DirFromAngle(-fov.angle / 2, false);
        // Vector3 viewAngleB = fov.DirFromAngle(fov.angle / 2, false);

        // Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.radius);
        // Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.radius);

        Handles.color = Color.red;

        foreach (Transform visibleTarget in fov.foodInViewRadius)
        {
            if (visibleTarget != null)
            {
                Handles.DrawLine(fov.transform.position, visibleTarget.position);
            }
        }
    }
}