using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform spawnPoint;
    public Transform spawnPointFX;
    public GameObject rockFXPrefab;
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
        GameObject rockFX = Instantiate(rockFXPrefab, spawnPointFX.position, Quaternion.identity);
        Destroy(rockFX, 1);
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
