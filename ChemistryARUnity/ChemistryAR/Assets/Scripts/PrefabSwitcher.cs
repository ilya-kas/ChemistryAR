using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
 
[Serializable]
public class MarkerType
{
    public string type;
    public GameObject targetPrefab;

    public MarkerType(string type, GameObject targetPrefab)
    {
        this.type = type;
        this.targetPrefab = targetPrefab;
    }
}
 
public class PrefabSwitcher : MonoBehaviour
{
    // Inspector array 
    public MarkerType[] markerPrefabCombos;
    ARTrackedImageManager m_TrackedImageManager;
    
    SceneManager sceneManager;

    void Awake()
    {
        Application.targetFrameRate = 60;
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
        sceneManager = GetComponent<SceneManager>();
    }
 
    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        sceneManager.Init();
    }
 
    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
 
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var addedImage in eventArgs.added)
        {
            AddMarkerPrefab(addedImage);
        }
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateMarkerPrefab(trackedImage);
        }
        foreach (var removedImage in eventArgs.removed)
        {
            DestroyGameObject(removedImage);
        }
        
        sceneManager.UpdateChemicals();
    }

    private void AddMarkerPrefab(ARTrackedImage trackedImage)
    {
        foreach (var variant in markerPrefabCombos)
        {
            if (variant.type == trackedImage.referenceImage.name.Split('-')[0])
            {
                sceneManager.Add(variant, trackedImage);
                
                break;
            }
        }
    }

    private void UpdateMarkerPrefab(ARTrackedImage trackedImage)
    {
        // If an image is properly tracked 
        if (trackedImage.trackingState == TrackingState.Tracking || trackedImage.trackingState == TrackingState.Limited)
        {
            var found = false;
            // Loop through image/prefab-combo array 
            foreach (var agent in sceneManager.placed)
            {
                if (agent.id == trackedImage.trackableId)
                {
                    sceneManager.TeleportModel(agent, trackedImage.transform);

                    found = true;
                    break;
                }
            }
            if (!found)
            {
                AddMarkerPrefab(trackedImage);
            }
        }
        else if (trackedImage.trackingState == TrackingState.None)
        {
            foreach (var agent in sceneManager.placed)
            {
                if (agent.id == trackedImage.trackableId)
                {
                    sceneManager.Delete(agent);
                    break;
                }
            }
        }
    }

    private void DestroyGameObject(ARTrackedImage trackedImage)
    {
        foreach (var agent in sceneManager.placed)
        {
            if (agent.id == trackedImage.trackableId)
            {
                sceneManager.Delete(agent);
                break;
            }
        }
    }

    private void Log(String msg)
    {
        Debug.Log("My debug: "+msg);
    }
}