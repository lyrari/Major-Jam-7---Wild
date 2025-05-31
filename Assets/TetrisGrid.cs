using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public List<Tetromino> PossibleTetrominos;

    public Transform UI_NextTetrominoParent; // Where the next one is stored
    Tetromino NextTetrominoData;

    public Transform TetrominoParent;
    public List<Transform> SpawnPositions;

    public static float FallSpeed = 4f; // How many blocks it falls per second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNextTetromino();
    }

    // TODO: Add this tetromino's TowerBlocks to a local grid.
    public void TetrominoLanded(Tetromino t)
    {
        SpawnNextTetromino();
    }

    // Grabs the next one waiting, and updates the UI with a new one.
    public void SpawnNextTetromino()
    {
        if (NextTetrominoData == null)
        {
            NextTetrominoData = GenerateTetromino();
            NextTetrominoData.InitUITetromino();
        }

        SpawnTetromino(NextTetrominoData);

        // Updates Next block UI
        foreach (Transform child in UI_NextTetrominoParent)
        {
            Destroy(child.gameObject);
        }
        NextTetrominoData = Instantiate(GenerateTetromino(), UI_NextTetrominoParent);
        NextTetrominoData.InitUITetromino();
    }

    public Tetromino GenerateTetromino()
    {
        return PossibleTetrominos[Random.Range(0, PossibleTetrominos.Count)];
    }

    public void SpawnTetromino(Tetromino t)
    {
        Vector3 spawnPos = SpawnPositions[Random.Range(0, SpawnPositions.Count)].position;
        Tetromino spawned = Instantiate(t, TetrominoParent);
        spawned.transform.position = spawnPos;
        spawned.Init(this, t);
    }
}
