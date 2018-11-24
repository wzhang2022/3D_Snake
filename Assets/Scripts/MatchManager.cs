﻿using System.Collections;
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
    public int powerUpDuration = 12;
    public int roundSteps = 300;

    // game mode choice
    public bool territoryOn = false;
    public bool territoryDoubleLayer = false;
    public bool territoryPermanent = false;

    // player controllers - note can generalize into array if we do multiplayer
    public PlayerController player1;
    public PlayerController player2;

    // item prefabs
    public GameObject foodPrefab;
    public GameObject powerUpPrefab;

    // game menu objects
    public GameObject timerText;
    public GameObject gameOverMenu;
    public GameObject player1WinsText;
    public GameObject player2WinsText;
    public GameObject tieText;

    // TODO: create a generic map class for fancy map selection?/
    public BasicMap map;

    // game data
    private HashSet<Vector3> wallPositions = new HashSet<Vector3>();
    private HashSet<Vector3> foodPositions = new HashSet<Vector3>();
    private HashSet<Vector3> powerUpPositions = new HashSet<Vector3>();

    // Initialize everything
    void Start()
    {
        InvokeRepeating("Repeat", 0.5f, timeStep);
        gameOverMenu.SetActive(false);
        // Get wall positions
        wallPositions = map.GetWallPositions();

        player1.positions.Add(player1.head.transform.position);
        player2.positions.Add(player2.head.transform.position);
        StartCoroutine(RoundTimer());
    }

    IEnumerator RoundTimer()
    {
        int secondsLeft = (int)(roundSteps * timeStep);
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
    bool IsOpen(Vector3 position, bool includeTerritory)
    {
        return (
            !player1.positions.Contains(position) &&
            !player2.positions.Contains(position) &&
            !foodPositions.Contains(position) &&
            !powerUpPositions.Contains(position) &&
            !wallPositions.Contains(position) &&
            (includeTerritory ||
             (!player1.territory.Contains(position) && !player2.territory.Contains(position)))
        );
    }

    // helper function for detecting if position is a crash
    bool IsCrash(Vector3 position)
    {
        return (
            player1.positions.Contains(position) ||
            player2.positions.Contains(position) ||
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

    void ClearTerritory(PlayerController player)
    {
        foreach (GameObject g in player.territoryBlocks)
        {
            Destroy(g);
        }
        player.territory.Clear();
        player.territoryBlocks.Clear();
    }

    void AddTerritory(PlayerController player)
    {
        foreach (Vector3 pos in player.positions)
        {
            if (!player.territory.Contains(pos))
            {
                Vector3 pos1 = new Vector3(pos.x, 1, pos.z);
                Vector3 pos2 = new Vector3(pos.x, 0, pos.z);
                if (territoryDoubleLayer)
                {
                    GameObject t1 = Instantiate(player.territoryPrefab, pos1, Quaternion.identity);
                    GameObject t2 = Instantiate(player.territoryPrefab, pos2, Quaternion.identity);
                    player.territory.Add(pos1);
                    player.territory.Add(pos2);
                    player.territoryBlocks.Add(t1);
                    player.territoryBlocks.Add(t2);
                } else
                {
                    GameObject t = Instantiate(player.territoryPrefab, pos, Quaternion.identity);
                    player.territory.Add(pos);
                    player.territoryBlocks.Add(t);
                }
                
            }
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
            if (IsOpen(foodPosition, true))
            {
                GameObject newFood = Instantiate(foodPrefab, foodPosition, Quaternion.identity);
                foodPositions.Add(foodPosition);
            }
        }
        // spawn new powerups - for now only spawn one at a time
        if (Random.Range(0f, 1f) < powerUpSpawnRate && powerUpPositions.Count < maxPowerUp)
        {
            Vector3 powerUpPosition = map.GetRandomPosition();
            if (IsOpen(powerUpPosition, false))
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

        // claim territory during power up
        if (territoryOn)
        {
            if (player1.powerTurns > 0)
            {
                AddTerritory(player1);
            }
            else if (!territoryPermanent)
            {
                ClearTerritory(player1);
            }
            if (player2.powerTurns > 0)
            {
                AddTerritory(player2);
            }
            else if (!territoryPermanent)
            {
                ClearTerritory(player2);
            }
        }

        player1.DecideMove(player2, wallPositions, foodPositions, powerUpPositions);
        player2.DecideMove(player1, wallPositions, foodPositions, powerUpPositions);

        Vector3 nextPosition1 = player1.NextMove();
        Vector3 nextPosition2 = player2.NextMove();

        // detect + handle collisions
        bool headCollision = nextPosition1 == nextPosition2;
        bool player1Crash = IsCrash(nextPosition1) || player2.territory.Contains(nextPosition1) || headCollision;
        bool player2Crash = IsCrash(nextPosition2) || player1.territory.Contains(nextPosition2) || headCollision;

        // trigger gameOver if necessary
        bool player1Win = player2Crash && player2.length <= 1;
        bool player2Win = player1Crash && player1.length <= 1;
        bool tie = player1Win && player2Win;
        GameOver(player1Win, player2Win, tie);

        if (player1Crash)
        {
            Hurt(player1);
            // powerUp allow attack opponent's body
            if (player1.powerTurns > 0 && player2.positions.Contains(nextPosition1))
            {
                Hurt(player2);
            }
        }
        else {
            player1.positions.Add(nextPosition1);
            player1.Move();
        }
        if (player2Crash)
        {
            // powerUp allow attack opponent's body
            Hurt(player2);
            if (player2.powerTurns > 0 && player1.positions.Contains(nextPosition2))
            {
                Hurt(player1);
            }
        }
        else {
            player2.positions.Add(nextPosition2);
            player2.Move();
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