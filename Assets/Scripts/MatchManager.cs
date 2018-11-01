using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// TODO: will add another script "GameManager" for things requiring persisting over scenes, like music, etc.
public class MatchManager : MonoBehaviour
{
    // game constants
    public float timeStep = 0.2f;

    // player controllers - note can generalize into array if we do multiplayer
    public PlayerController player1;
    public PlayerController player2;

    // end game menu objects
    public GameObject gameOverMenu;
    public GameObject player1WinsText;
    public GameObject player2WinsText;
    public GameObject tieText;

    // TODO: create a generic map class for fancy map selection?/
    public BasicMap map;

    // store all game data
    private HashSet<Vector3> playerPositions = new HashSet<Vector3>();
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>();
    // HashSet<Vector3> foodPositions = new HashSet<Vector3>();

    // Initialize everything
    void Start()
    {
        InvokeRepeating("Repeat", 0.5f, timeStep);
        gameOverMenu.SetActive(false);
        // Get wall positions
        wallPositions = map.getWallPositions();

        playerPositions.Add(player1.head.transform.position);
        playerPositions.Add(player2.head.transform.position);
    }

    void Repeat()
    {
        CheckMoves();
        player1.Move();
        player2.Move();
    }

    void CheckMoves()
    {
        // modify this code for stuff other than gameOver to happen on collision
        Vector3 nextPosition1 = player1.NextMove();
        Vector3 nextPosition2 = player2.NextMove();
        Debug.Log(nextPosition1);
        
        bool player2Win = playerPositions.Contains(nextPosition1) || wallPositions.Contains(nextPosition1);
        bool player1Win = playerPositions.Contains(nextPosition2) || wallPositions.Contains(nextPosition2);
        bool headCollision = nextPosition1 == nextPosition2;
        bool tie = headCollision || (player1Win && player2Win);

        playerPositions.Add(nextPosition1);
        playerPositions.Add(nextPosition2);

        // if someone lost, activate game over menu with the appropriate message
        if (player2Win || player1Win || tie)
        {
            Time.timeScale = 0;
            gameOverMenu.SetActive(true);
            if (tie)
            {
                tieText.SetActive(true);
            }
            else if (player1Win)
            {
                player1WinsText.SetActive(true);
            }
            else if (player2Win)
            {
                player2WinsText.SetActive(true);
            }
        }
    }
}