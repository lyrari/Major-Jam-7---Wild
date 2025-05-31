using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public List<Tetromino> PossibleTetrominos;

    public Transform TetrominoParent;
    public List<Transform> SpawnPositions;

    public static float FallSpeed = 5f; // How many blocks it falls per second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnTetromino(); // Spawns the first one to get it going.
    }

    // TODO: Add this tetromino's TowerBlocks to a local grid.
    public void TetrominoLanded(Tetromino t)
    {
        SpawnTetromino();
    }

    public void SpawnTetromino()
    {
        Vector3 spawnPos = SpawnPositions[Random.Range(0, SpawnPositions.Count)].position;
        Tetromino spawned = Instantiate(PossibleTetrominos[Random.Range(0, PossibleTetrominos.Count)], TetrominoParent);
        spawned.transform.position = spawnPos;
        spawned.Init(this);
    }
}
