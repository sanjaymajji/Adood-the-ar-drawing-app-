using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class ARDraw : MonoBehaviourPunCallbacks
{
    Camera arCamera;

    Vector3 anchor = new Vector3(0, 0, 0.3f);

    bool anchorUpdate = false;
    //should anchor update or not

    public GameObject linePrefab;
    //prefab which generates the line for user

    LineRenderer lineRenderer;
    //LineRenderer which connects and generates

    public List<LineRenderer> lineList = new List<LineRenderer>();
    //List of lines drawn

    public Transform linePool;

    public bool use;
    //code is in use or not

    public bool startLine;
    //already started line or not

    private XRController xrController;

    void Start()
    {
        arCamera = Camera.main;
        xrController = GetComponent<XRController>();
        
        // Check if this is the local player's instance
        if (photonView.IsMine)
        {
            // Enable drawing for the local player only
            EnableDrawing();
        }
    }

    void Update()
    {
        if (use)
        {
            if (startLine)
            {
                UpdateAnchor();
                DrawLineContinue();
            }
        }
    }

    void UpdateAnchor()
    {
        if (anchorUpdate)
        {
            Vector3 temp = Input.mousePosition;
            temp.z = 0.3f;
            anchor = arCamera.ScreenToWorldPoint(temp);
        }
    }

    public void MakeLineRenderer()
    {
        GameObject tempLine = PhotonNetwork.Instantiate(linePrefab.name, Vector3.zero, Quaternion.identity);
        tempLine.transform.SetParent(linePool);
        tempLine.transform.localScale = new Vector3(1, 1, 1);

        anchorUpdate = true;
        UpdateAnchor();

        lineRenderer = tempLine.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, anchor);

        startLine = true;
        lineList.Add(lineRenderer);
    }

    public void DrawLineContinue()
    {
        lineRenderer.positionCount = lineRenderer.positionCount + 1;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, anchor);
    }

    public void StartDrawLine()
    {
        use = true;

        if (!startLine)
        {
            MakeLineRenderer();
        }
    }

    public void StopDrawLine()
    {
        use = false;
        startLine = false;
        lineRenderer = null;

        anchorUpdate = false;
    }

    public void Undo()
    {
        if (lineList.Count > 0)
        {
            LineRenderer undo = lineList[lineList.Count - 1];
            Destroy(undo.gameObject);
            lineList.RemoveAt(lineList.Count - 1);
        }
    }

    public void ClearScreen()
    {
        foreach (LineRenderer item in lineList)
        {
            Destroy(item.gameObject);
        }
        lineList.Clear();
    }

    public void EnableDrawing()
    {
        use = true;
        startLine = false;
    }

    public void DisableDrawing()
    {
        use = false;
        startLine = false;
    }

    private void OnSelectEntered(XRBaseInteractor interactor)
    {
        // Trigger drawing on select entered (e.g., when user presses a button)
        StartDrawLine();
    }
}
