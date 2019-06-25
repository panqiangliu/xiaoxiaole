using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsPaint : MonoBehaviour
{
    public Color paintColor = Color.red;
    public float paintSize = 0.1f;

    private LineRenderer currentLineRender;
    public Material lineRenderMaterial;
    public List<Vector3> positions = new List<Vector3>();
    private bool isMouseDown = false;
    private Vector3 lasetPosition;
    private float lineDistance=0.02f;

    #region
    public void OnRedClolorchanged(bool isOn)
    {
        if (isOn)
        {
            paintColor = Color.red;
        }
    }
    public void OnGreenClolorchanged(bool isOn)
    {
        if (isOn)
        {
            paintColor = Color.green;
        }
    }
    public void OnBlueClolorchanged(bool isOn)
    {
        if (isOn)
        {
            paintColor = Color.blue;
        }
    }
    public void OnPoint1Changed(bool isOn)
    {
        if (isOn)
        {
            paintSize = 0.1f;
        }
    }

    public void OnPoint2Changed(bool isOn)
    {
        if (isOn)
        {
            paintSize = 0.2f;
        }
    }
    public void OnPoint4Changed(bool isOn)
    {
        if (isOn)
        {
            paintSize = 0.4f;
        }
    }

    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject go = new GameObject();
            go.transform.parent = this.transform;
            currentLineRender = go.AddComponent<LineRenderer>();
            currentLineRender.material = lineRenderMaterial;
            currentLineRender.startWidth = paintSize;
            currentLineRender.endWidth = paintSize;
            currentLineRender.startColor = paintColor;
            currentLineRender.endColor = paintColor;
            currentLineRender.numCapVertices = 5;
            currentLineRender.numCornerVertices = 5;
            positions.Clear();
            Vector3 position = MousePosition();
            AddPosition(position);
            isMouseDown = true;
            lineDistance += 0.02f;
        }
        if (isMouseDown == true)
        {
            Vector3 position = MousePosition();
            if (Vector3.Distance(position, lasetPosition) > 0.1)
            {
                AddPosition(position);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDown = false;
            currentLineRender = null;
            positions.Clear();
        }
    }

    void AddPosition(Vector3 position)
    {
        position.z -= lineDistance;
        positions.Add(position);
        currentLineRender.positionCount = positions.Count;
        currentLineRender.SetPositions(positions.ToArray());
        lasetPosition = position;
    }

    private Vector3 MousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isCollider = Physics.Raycast(ray, out hit);
        if (isCollider)
            return hit.point;
        return Vector3.zero;
    }
}
