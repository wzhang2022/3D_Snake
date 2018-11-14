using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// TODO: will add another script "GameManager" for things requiring persisting over scenes, like music, etc.
public class MatchManager : MonoBehaviour
{
    // game constants
    public float timeStep = 0.2f;
    public float foodSpawnRate = .3f;
    public int roundLength = 30;

    // player controllers - note can generalize into array if we do multiplayer
    public PlayerController player1;
    public PlayerController player2;

    // item prefabas
    public GameObject foodPrefab;

    // end game menu objects
    public GameObject gameOverMenu;
    public GameObject player1WinsText;
    public GameObject player2WinsText;
    public GameObject tieText;

    // TODO: create a generic map class for fancy map selection?/
    public BasicMap map;

    // store all game data
    public HashSet<Vector3> playerPositions = new HashSet<Vector3>();
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>();
    private HashSet<Vector3> foodPositions = new HashSet<Vector3>();

    // Initialize everything
    void Start()
    {
        InvokeRepeating("Repeat", 0.5f, timeStep);
        gameOverMenu.SetActive(false);
        // Get wall positions
        wallPositions = map.GetWallPositions();

        playerPositions.Add(player1.head.transform.position);
        playerPositions.Add(player2.head.transform.position);
        StartCoroutine(RoundTimer());
    }

    IEnumerator RoundTimer()
    {
        yield return new WaitForSeconds(roundLength);
        bool player1Win = player1.length > player2.length;
        bool player2Win = player1.length < player2.length;
        bool tie = player1.length == player2.length; ;
        gameOver(player1Win, player2Win, tie);
    }

    // what happens each timestep
    void Repeat()
    {
        // spawn new food
        if (Random.Range(0f, 1f) < foodSpawnRate)
        {
            Vector3 foodPosition = map.GetRandomPosition();
            if (!playerPositions.Contains(foodPosition) && !foodPositions.Contains(foodPosition))
            {
                GameObject newFood = Instantiate(foodPrefab, foodPosition, Quaternion.identity);
                foodPositions.Add(foodPosition);
            }
        }

        Vector3 position1 = player1.head.transform.position;
        Vector3 position2 = player2.head.transform.position;
        // food consumption
        if (foodPositions.Contains(position1))
        {
            player1.length++;
            foodPositions.Remove(position1);
        }
        if (foodPositions.Contains(position2))
        {
            player2.length++;
            foodPositions.Remove(position2);
        }

        Vector3 nextPosition1 = player1.NextMove();
        Vector3 nextPosition2 = player2.NextMove();

        // detect + handle collisions
        bool headCollision = nextPosition1 == nextPosition2;
        bool player1Crash = playerPositions.Contains(nextPosition1) || wallPositions.Contains(nextPosition1) || headCollision;
        bool player2Crash = playerPositions.Contains(nextPosition2) || wallPositions.Contains(nextPosition2) || headCollision;

        // trigger gameOver if necessary
        bool player1Win = player2Crash && player2.length <= 1;
        bool player2Win = player1Crash && player1.length <= 1;
        bool tie = player1Win && player2Win;
        gameOver(player1Win, player2Win, tie);

        if (player1Crash)
        {
            player1.length -= player1.length / 2;
        }
        else {
            playerPositions.Add(nextPosition1);
            player1.Move();
        }
        if (player2Crash)
        {
            player2.length -= player2.length / 2;
        }
        else {
            playerPositions.Add(nextPosition2);
            player2.Move();
        }
    }

    // function to display the gameover screen
    void gameOver(bool player1Win, bool player2Win, bool tie)
    {
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