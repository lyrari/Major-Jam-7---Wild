using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    public List<GameObject> TetrisBlocks;

    public float timeBetweenHorizontalMoves = 0.1f; // Number of times it can move per second in a single direction
    public float nextDropTime;

    bool isSettled;
    float lastHorizontalMoveTime;

    float fallSpeed = 1f; // How many blocks it falls per second

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastHorizontalMoveTime = -999;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSettled)
        {
            // Auto fall
            float fallSpeedMultiplier = 1;
            if (Input.GetKey(KeyCode.S))
            {
                fallSpeedMultiplier *= 5;
            }
            transform.position += Vector3.down * Time.deltaTime * fallSpeed * fallSpeedMultiplier;

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
                transform.position += moveUpdate;
                if (moveUpdate != Vector3.zero)
                {
                    lastHorizontalMoveTime = Time.time;
                }
            }

        }
    }
}
