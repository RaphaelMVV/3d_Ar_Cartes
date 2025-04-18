using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageHandler : MonoBehaviour
{
    private ARTrackedImageManager _trackedImageManager;
    // Dictionnaire pour stocker les prefabs instanciés associés à chaque image
    private Dictionary<string, GameObject> _spawnedPrefabs = new Dictionary<string, GameObject>();

    // Prefab qui sera instancié lors de la détection de l'image
    public GameObject imagePrefab;

    void Awake()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Pour les images nouvellement détectées
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        // Pour les images mises à jour
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        // Pour les images qui ne sont plus détectées
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            if (_spawnedPrefabs.ContainsKey(trackedImage.referenceImage.name))
            {
                Destroy(_spawnedPrefabs[trackedImage.referenceImage.name]);
                _spawnedPrefabs.Remove(trackedImage.referenceImage.name);
            }
        }
    }

    void UpdateImage(ARTrackedImage trackedImage)
    {
        if (!_spawnedPrefabs.ContainsKey(trackedImage.referenceImage.name))
        {
            GameObject instantiatedPrefab = Instantiate(imagePrefab, trackedImage.transform.position, trackedImage.transform.rotation);
            _spawnedPrefabs.Add(trackedImage.referenceImage.name, instantiatedPrefab);
        }
        else
        {
            GameObject spawnedPrefab = _spawnedPrefabs[trackedImage.referenceImage.name];
            spawnedPrefab.transform.position = trackedImage.transform.position;
            spawnedPrefab.transform.rotation = trackedImage.transform.rotation;
        }

        // Activation/désactivation du prefab selon l'état de tracking
        if (trackedImage.trackingState == TrackingState.Tracking)
            _spawnedPrefabs[trackedImage.referenceImage.name].SetActive(true);
        else
            _spawnedPrefabs[trackedImage.referenceImage.name].SetActive(false);
    }
}