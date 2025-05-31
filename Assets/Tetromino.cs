using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Tetromino : MonoBehaviour
{
    public TowerBlock RotationPoint;

    public List<TowerBlock> MyBlocks;
    [HideInInspector]
    public TowerBlock.BlockType type1;
    [HideInInspector]
    public TowerBlock.BlockType type2;

    const float timeBetweenHorizontalMoves = 0.1f; // Number of times it can move per second in a single direction

    float landingLeniencyTime = 0.5f; // Amount of time after landing before it settles.
    float settleAtTime = float.MaxValue;
    bool settling;

    bool isSettled;

    float lastHorizontalMoveTime;
    TetrisGrid tetrisGridRef;

    public void RollBlockTypes()
    {
        type1 = (TowerBlock.BlockType)Random.Range(0, 4);
        if (MyBlocks.Count <= 3)
        {
            type2 = type1;
        } else
        {
            type2 = (TowerBlock.BlockType)Random.Range(0, 4);
        }

    }

    public void Init(TetrisGrid grid)
    {
        tetrisGridRef = grid;
        RollBlockTypes();
        lastHorizontalMoveTime = -999;
        for (int i = 0; i < MyBlocks.Count; i++)
        {
            if (i < MyBlocks.Count / 2)
            {
                MyBlocks[i].Init(type1);
            }
            else
            {
                MyBlocks[i].Init(type2);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isSettled)
        {
            // Auto fall (if not settling)
            if (!settling)
            {
                float fallSpeedMultiplier = 1;
                if (Input.GetKey(KeyCode.S))
                {
                    fallSpeedMultiplier *= 5;
                }
                transform.position += Vector3.down * Time.deltaTime * TetrisGrid.FallSpeed * fallSpeedMultiplier;

                // Check for hit bottom. Round to nearest y if it did.
                // TODO: Have some leniency period before the block settles?
                if (AnyBlocksOverlapping())
                {
                    StartCoroutine(BlockSettling());
                    transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);
                    settling = true;
                    settleAtTime = Time.time + landingLeniencyTime;
                }
            } else
            {
                // Settling, Check below to see if the object can start falling again.
                Vector3 belowCheck = Vector3.down * Time.deltaTime * TetrisGrid.FallSpeed * 5;
                transform.position += belowCheck;
                if (!AnyBlocksOverlapping())
                {
                    // Stop settling
                    settleAtTime = float.MaxValue;
                    settling = false;
                    
                }
                else
                {
                    // Still settling - there is still stuff below this.
                    transform.position -= belowCheck;

                    if (Input.GetKey(KeyCode.S))
                    {
                        settleAtTime -= Time.deltaTime * 5;
                    }

                    if (Time.time > settleAtTime)
                    {
                        // Settled.
                        isSettled = true;
                        transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);
                        tetrisGridRef.TetrominoLanded(this);
                    }
                }
            }

            // Rotation
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Rotate");
                transform.RotateAround(RotationPoint.transform.position, new Vector3(0, 0, 1), 90);
                if (AnyBlocksOverlapping())
                {
                    // Failed - rotate back
                    Debug.Log("UnRotate");
                    transform.RotateAround(RotationPoint.transform.position, new Vector3(0, 0, 1), -90);
                }
            }

            // Player input
            if (Time.time > lastHorizontalMoveTime + timeBetweenHorizontalMoves)
            {
                Vector3 moveUpdate = Vector2.zero;
                if (Input.GetAxisRaw("Horizontal") == 1)
                {
                    moveUpdate = Vector2.right;
                }
                else if (Input.GetAxisRaw("Horizontal") == -1)
                {
                    moveUpdate = Vector2.left;
                }

                if (moveUpdate == Vector3.zero) return;

                transform.position += moveUpdate;

                // If any of the blocks are overlapping another block/boundary, undo this move.
                if (AnyBlocksOverlapping())
                {
                    transform.position -= moveUpdate;
                    moveUpdate = Vector2.zero;
                }

                if (moveUpdate != Vector3.zero)
                {
                    lastHorizontalMoveTime = Time.time;
                }
            }
        }
    }

    // While block is settling, settle after waiting x seconds.
    IEnumerator BlockSettling()
    {
        yield return new WaitForSeconds(landingLeniencyTime);


    }

    bool AnyBlocksOverlapping()
    {
        Physics2D.SyncTransforms();
        foreach (var block in MyBlocks)
        {
            if (block.IsOverlappingSomething())
            {
                return true;
            }
        }
        return false;
    }
}
