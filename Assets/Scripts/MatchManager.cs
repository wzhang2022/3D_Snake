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
    public int maxFood = 30;
    public float powerUpSpawnRate = .1f;
    public int maxPowerUp = 1;
    public int powerUpDuration = 10;
    public int roundLength = 30;

    // player controllers - note can generalize into array if we do multiplayer
    public PlayerController player1;
    public PlayerController player2;

    // item prefabs
    public GameObject foodPrefab;
    public GameObject powerUpPrefab;
    public GameObject player1TerritoryPrefab;
    public GameObject player2TerritoryPrefab;

    // game menu objects
    public GameObject timerText;
    public GameObject gameOverMenu;
    public GameObject player1WinsText;
    public GameObject player2WinsText;
    public GameObject tieText;

    // TODO: create a generic map class for fancy map selection?/
    public BasicMap map;

    // store all game data
    private HashSet<Vector3> player1Positions = new HashSet<Vector3>();
    private HashSet<Vector3> player2Positions = new HashSet<Vector3>();
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>();
    private HashSet<Vector3> foodPositions = new HashSet<Vector3>();
    private HashSet<Vector3> powerUpPositions = new HashSet<Vector3>();
    private HashSet<Vector3> player1Territory = new HashSet<Vector3>();
    private HashSet<Vector3> player2Territory = new HashSet<Vector3>();


    // Initialize everything
    void Start()
    {
        InvokeRepeating("Repeat", 0.5f, timeStep);
        gameOverMenu.SetActive(false);
        // Get wall positions
        wallPositions = map.GetWallPositions();

        player1Positions.Add(player1.head.transform.position);
        player2Positions.Add(player2.head.transform.position);
        StartCoroutine(RoundTimer());
    }

    IEnumerator RoundTimer()
    {
        int secondsLeft = roundLength;
        while (secondsLeft > 0)
        {
            timerText.GetComponent<Text>().text = secondsLeft.ToString();
            yield return new WaitForSeconds(1);
            secondsLeft--;

        }
        bool player1Win = player1.length > player2.length;
        bool player2Win = player1.length < player2.length;
        bool tie = player1.length == player2.length; ;
        GameOver(player1Win, player2Win, tie);
    }

    // helper function for detecting if position is open
    bool IsOpen(Vector3 position)
    {
        return (
            !player1Positions.Contains(position) &&
            !player2Positions.Contains(position) &&
            !foodPositions.Contains(position) &&
            !powerUpPositions.Contains(position) &&
            !wallPositions.Contains(position) &&
            !player1Territory.Contains(position) &&
            !player2Territory.Contains(position)
        );
    }

    // helper function for detecting if position is a crash
    bool IsCrash(Vector3 position)
    {
        return (
            player1Positions.Contains(position) ||
            player2Positions.Contains(position) ||
            wallPositions.Contains(position)
        );
    }

    // helper function for hurting a player
    void Hurt(PlayerController player)
    {
        // lose length unless powered up
        if (player.powerTurns < 1)
        {
            player.length = player.length - player.length / 2;
        }
    }

    // what happens each timestep
    void Repeat()
    {
        // spawn new food
        int numFood = foodPositions.Count;
        if (numFood == 0 || Random.Range(0f, 1f) < foodSpawnRate && numFood < maxFood)
        {
            Vector3 foodPosition = map.GetRandomPosition();
            if (IsOpen(foodPosition))
            {
                GameObject newFood = Instantiate(foodPrefab, foodPosition, Quaternion.identity);
                foodPositions.Add(foodPosition);
            }
        }
        // spawn new powerups - for now only spawn one at a time
        if (Random.Range(0f, 1f) < powerUpSpawnRate && powerUpPositions.Count < maxPowerUp)
        {
            Vector3 powerUpPosition = map.GetRandomPosition();
            if (IsOpen(powerUpPosition))
            {
                GameObject newPowerUp = Instantiate(powerUpPrefab, powerUpPosition, Quaternion.identity);
                powerUpPositions.Add(powerUpPosition);
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

        // power up consumption
        if (powerUpPositions.Contains(position1))
        {
            player1.powerTurns += powerUpDuration;
            powerUpPositions.Remove(position1);
        }
        if (powerUpPositions.Contains(position2))
        {
            player2.powerTurns += powerUpDuration;
            powerUpPositions.Remove(position2);
        }

        // claim territory on powerup end
        if (player1.powerTurns == 1)
        {
            foreach (Vector3 pos in player1Positions)
            {
                Instantiate(player1TerritoryPrefab, pos, Quaternion.identity);
                player1Territory.Add(pos);
            }
        }
        if (player2.powerTurns == 1)
        {
            foreach (Vector3 pos in player2Positions)
            {
                Instantiate(player2TerritoryPrefab, pos, Quaternion.identity);
                player2Territory.Add(pos);
            }
        }      

        Vector3 nextPosition1 = player1.NextMove(player1Positions);
        Vector3 nextPosition2 = player2.NextMove(player2Positions);

        // detect + handle collisions
        bool headCollision = nextPosition1 == nextPosition2;
        bool player1Crash = IsCrash(nextPosition1) || player2Territory.Contains(nextPosition1) || headCollision;
        bool player2Crash = IsCrash(nextPosition2) || player1Territory.Contains(nextPosition2) || headCollision;

        // trigger gameOver if necessary
        bool player1Win = player2Crash && player2.length <= 1;
        bool player2Win = player1Crash && player1.length <= 1;
        bool tie = player1Win && player2Win;
        GameOver(player1Win, player2Win, tie);

        if (player1Crash)
        {
            Hurt(player1);
            // powerUp allow attack opponent's body
            if (player1.powerTurns > 0 && player2Positions.Contains(nextPosition1))
            {
                Hurt(player2);
            }
        }
        else {
            player1Positions.Add(nextPosition1);
            player1.Move();
        }
        if (player2Crash)
        {
            // powerUp allow attack opponent's body
            Hurt(player2);
            if (player2.powerTurns > 0 && player1Positions.Contains(nextPosition2))
            {
                Hurt(player1);
            }
        }
        else {
            // player2Positions.Add(nextPosition2);
            // player2.Move();
        }
    }

    // function to display the gameover screen
    void GameOver(bool player1Win, bool player2Win, bool tie)
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