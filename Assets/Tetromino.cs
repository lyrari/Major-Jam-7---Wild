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
    bool uiOnly;

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

    // Inits a Tetromino's type only and sets its layer to UI layer.
    public void InitUITetromino()
    {
        transform.localPosition = Vector3.zero;
        uiOnly = true;
        RollBlockTypes();
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
        UpdateLayer("UI");

        // Start with random rotation between 0 and 3 times
        int numRotations = Random.Range(0, 4);
        for (int i = 0; i < numRotations; i++)
        {
            Rotate(true);
        }
    }

    void UpdateLayer(string layerName)
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    public void Init(TetrisGrid grid, Tetromino data = null)
    {
        tetrisGridRef = grid;
        uiOnly = false;
        UpdateLayer("Tetromino");

        // If another tetromino is given - copy its types.
        if (data != null)
        {
            type1 = data.type1;
            type2 = data.type2;
        } else
        {
            RollBlockTypes();
        }

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
        if (uiOnly) return;
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
            // TODO: Attempt to rotate and move left/right if the original rotation doesn't work
            bool rotateLeft = Input.GetKeyDown(KeyCode.Q);
            bool rotateRight = Input.GetKeyDown(KeyCode.E);
            if (rotateLeft || rotateRight)
            {
                Rotate(rotateRight);
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

    public void Rotate(bool rotateRight)
    {
        float rotationDirectionMult = 1;
        if (rotateRight)
            rotationDirectionMult = -1;

        transform.RotateAround(RotationPoint.transform.position, new Vector3(0, 0, 1), 90 * rotationDirectionMult);
        if (AnyBlocksOverlapping())
        {
            // Try moving left and right as well
            transform.position += Vector3.right;
            if (AnyBlocksOverlapping())
            {
                // Still overlapping, try left
                transform.position += 2 * Vector3.left;
                if (AnyBlocksOverlapping())
                {
                    // Both failed - move back to start and rotate back
                    transform.position += Vector3.right;
                    transform.RotateAround(RotationPoint.transform.position, new Vector3(0, 0, 1), -90 * rotationDirectionMult);
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
