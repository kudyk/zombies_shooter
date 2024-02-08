using System.Collections.Generic;
using UnityEngine;

namespace ZombiesShooter
{
    [ExecuteInEditMode]
    public class ItemsByMatrixSpawner : MonoBehaviour
    {
        [SerializeField] private bool             updateState       = false;
        [SerializeField] private bool             clearAll          = false;
        [Space]                                                     
        [SerializeField] private Transform        itemsContainer    = null;
        [SerializeField] private GameObject       itemPrefab        = null;
        [SerializeField] private Vector2          distanceBetween   = Vector2.zero;
        [SerializeField] private Vector2          matrixSize        = Vector2.zero;
        [SerializeField] private bool             randomizeRotation = false;
        [SerializeField] private List<GameObject> spawnedItems      = new List<GameObject>();

        private float yPosition = 0.0f;

        private void Update()
        {
            if (clearAll)
            {
                DestroyOldItems();
                clearAll = false;
                return;
            }

            if (!updateState)
                return;

            DestroyOldItems();
            SpawnNewItems();

            updateState = false;
        }

        private void SpawnNewItems()
        {
            if (matrixSize == Vector2.zero)
                return;

            for (int i = 0; i < matrixSize.y; i++)
            {
                yPosition = distanceBetween.y * i;
                for (int j = 0; j < matrixSize.x; j++)
                {
                    GameObject instance = Instantiate(itemPrefab, itemsContainer);
                    spawnedItems.Add(instance);

                    instance.transform.localPosition = new Vector3(distanceBetween.x * j, 0, yPosition);

                    if (randomizeRotation)
                        instance.transform.localEulerAngles = new Vector3(0, Random.Range(0.0f, 360.0f), 0);
                }
            }
        }

        private void DestroyOldItems()
        {
            if (spawnedItems == null || spawnedItems.Count == 0)
                return;

            foreach (GameObject spawnedItem in spawnedItems)
                DestroyImmediate(spawnedItem);

            spawnedItems.Clear();
        }
    }
}