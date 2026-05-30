using UnityEngine;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour
{
    public Material lineMaterial;
    public float lineWidth = 0.01f; // Small, clean line width

    private LineRenderer currentLine;
    private List<Vector3> points = new List<Vector3>();

    //void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        StartLine();
    //    }

    //    if (Input.GetMouseButton(0))
    //    {
    //        Draw();
    //    }

    //    if (Input.GetMouseButtonUp(0))
    //    {
    //        FinishLine();
    //    }
    //}
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 1. Cast a ray from the camera to see what we are clicking first
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 2. If we hit the ThumbPin or its children, DO NOT start drawing a line
                if (hit.collider.gameObject.name.Contains("ThumbPin") || hit.collider.CompareTag("ThumbPin"))
                {
                    return; // Exit early, allowing ThumbPinDragger to take over
                }
            }

            StartLine();
        }

        // Only draw or finish lines if a line actually exists
        if (currentLine != null)
        {
            if (Input.GetMouseButton(0))
            {
                Draw();
            }

            if (Input.GetMouseButtonUp(0))
            {
                FinishLine();
            }
        }
    }
    void StartLine()
    {
        GameObject lineObj = new GameObject("Line");

        currentLine = lineObj.AddComponent<LineRenderer>();
        currentLine.useWorldSpace = true;
        currentLine.material = lineMaterial;

        // Set widths cleanly without using widthCurve overrides
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;

        currentLine.startColor = Color.black;
        currentLine.endColor = Color.black;

        // Alignment View is much safer when drawing dynamically from a camera view, 
        // but we keep the width extremely thin so it stays a small line.
        currentLine.alignment = LineAlignment.View;
        currentLine.sortingOrder = 5;

        points.Clear();
    }

    void Draw()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Make sure your "Paper" GameObject has the Tag "Paper" assigned to it!
            if (hit.collider.CompareTag("Paper"))
            {
                // Just slightly above the surface to prevent clipping (Z-fighting)
                Vector3 drawingPoint = hit.point + (hit.normal * 0.005f);

                if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], drawingPoint) > 0.01f)
                {
                    points.Add(drawingPoint);
                    currentLine.positionCount = points.Count;
                    currentLine.SetPositions(points.ToArray());
                }
            }
        }
    }

    void FinishLine()
    {
        currentLine = null;
    }
}