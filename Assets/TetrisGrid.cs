using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public List<Tetromino> PossibleTetrominos;

    public Transform UI_NextTetrominoParent; // Where the next one is stored
    Tetromino NextTetrominoData;

    public Transform TetrominoParent;
    public Transform SpawnPositionsParent;
    public List<Transform> SpawnPositions;

    // Stores blocks after they have landed.
    public TowerBlock.BlockType[,] SettledBlockGrid;
    public Transform SettledBlockParent;
    public const int MAX_BLOCK_HEIGHT = 30;
    public const int BLOCK_WIDTH = 11;
    public int currentMaxBlockHeight;
    public const int MAX_SPAWN_HEIGHT = 14;

    public static float FallSpeed = 4f; // How many blocks it falls per second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNextTetromino();
        SettledBlockGrid = new TowerBlock.BlockType[BLOCK_WIDTH, MAX_BLOCK_HEIGHT];
    }

    // TODO: Add this tetromino's TowerBlocks to a local grid.
    public void TetrominoLanded(Tetromino t)
    {
        // Add each block to the grid
        foreach (TowerBlock block in t.MyBlocks)
        {
            block.transform.SetParent(SettledBlockParent);
            Vector2 localPos = block.transform.localPosition;
            int xGrid = Mathf.RoundToInt(localPos.x);
            int yGrid = Mathf.RoundToInt(localPos.y);

            try
            {
                SettledBlockGrid[xGrid, yGrid] = block.m_BlockType;
            } catch
            {
                // Index is out of bounds - you went over the max height. Stop spawning.
                Debug.Log("REACHED MAX HEIGHT!!");
                return;
            }

            //Debug.Log("Updated grid " + xGrid + ", " + yGrid + " to " + block.m_BlockType);

            currentMaxBlockHeight = Mathf.Max(yGrid, currentMaxBlockHeight);
            //Debug.Log("Current max block: " +  currentMaxBlockHeight);
        }

        // Update the spawn height to be a certain distance above the max block height.
        int spawnHeight = Mathf.Min(MAX_SPAWN_HEIGHT, currentMaxBlockHeight);
        Debug.Log("SpawnHeight: " + spawnHeight);
        SpawnPositionsParent.position = new Vector3(SpawnPositionsParent.position.x, spawnHeight, SpawnPositionsParent.position.z);

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
