using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class clicktoplace : MonoBehaviour
{
    public GameObject gameobjecttoInstantiate; // Prefab for the object to be placed
    public GameObject placementGuidePrefab; // Prefab for the placement guide
    private ARRaycastManager arRaycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject placementGuide; // Reference to the instantiated placement guide
    private GameObject placedObject; // Reference to the instantiated object

    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        if (arRaycastManager == null)
        {
            Debug.LogError("ARRaycastManager component not found on GameObject.");
        }
    }

    private void Update()
    {
        // Raycast to detect planes
        if (arRaycastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.PlaneWithinPolygon))
        {
            // If a plane is detected, show the placement guide at the hit position
            Pose hitPose = hits[0].pose;
            Vector3 spawnPosition = hitPose.position;
            Quaternion spawnRotation = hitPose.rotation;

            // Instantiate the placement guide prefab if it doesn't exist
            if (placementGuide == null)
            {
                placementGuide = Instantiate(placementGuidePrefab, spawnPosition, spawnRotation);
            }
            else
            {
                placementGuide.transform.position = spawnPosition;
                placementGuide.transform.rotation = spawnRotation;
            }
        }
        else
        {
            // If no plane is detected, destroy the placement guide
            if (placementGuide != null)
            {
                Destroy(placementGuide);
                placementGuide = null;
            }
        }
    }

    // Function to place the object
    public void PlaceObject()
    {
        // Check if a plane is detected
        if (placementGuide != null)
        {
            // Destroy the previously placed object, if any
            if (placedObject != null)
            {
                Destroy(placedObject);
            }

            // Instantiate the new object at the placement guide position
            placedObject = Instantiate(gameobjecttoInstantiate, placementGuide.transform.position, placementGuide.transform.rotation);
        }
        else
        {
            Debug.Log("No plane detected. Cannot place object.");
        }
    }
}
