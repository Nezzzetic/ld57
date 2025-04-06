using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform spawnPoint;

    private Rock currentRock;

    void Start()
    {
        SpawnNextRock();
    }

    public void SpawnNextRock()
    {

        GameObject rockGO = Instantiate(rockPrefab, spawnPoint.position, Quaternion.identity);
        //rockGO.transform.rota0210tion = Random.rotation;
        currentRock = rockGO.GetComponent<Rock>();
        currentRock.OnStateChanged += OnRockStateChanged;
    }

    private void OnRockStateChanged(RockState state)
    {
        if (state == RockState.Held)
        {
            // When player picks this rock up, spawn the next one
            currentRock.OnStateChanged -= OnRockStateChanged;
            SpawnNextRock();
        }
    }
}
