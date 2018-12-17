using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


//This keeps track of the game state, but should be only used for implementation of algorithms
//this is a simplified version of the game state
public class GameState {
    public HashSet<Vector3> walls;
    public HashSet<Vector3> powerups;
    public HashSet<Vector3> foods;
    public SimplifiedAgent player1;
    public SimplifiedAgent player2;

    public GameState(HashSet<Vector3> walls,
                     HashSet<Vector3> powerups,
                     HashSet<Vector3> foods,
                     Agent player1,
                     Agent player2) {
        this.walls = new HashSet<Vector3>(walls);
        this.powerups = new HashSet<Vector3>(powerups);
        this.foods = new HashSet<Vector3>(foods);
        this.player1 = new SimplifiedAgent(player1);
        this.player2 = new SimplifiedAgent(player2);
    }
    public GameState(HashSet<Vector3> walls,
                     HashSet<Vector3> powerups,
                     HashSet<Vector3> foods,
                     SimplifiedAgent agent1,
                     SimplifiedAgent agent2) {
        this.walls = new HashSet<Vector3>(walls);
        this.powerups = new HashSet<Vector3>(powerups);
        this.foods = new HashSet<Vector3>(foods);
        this.player1 = new SimplifiedAgent(agent1); ;
        this.player2 = new SimplifiedAgent(agent2); ;
    }

    //returns the state that would result if player1 moved by move1 and player2 moved by move2
    public GameState NextState(Vector3 move1, Vector3 move2) {
        GameState nextState = new GameState(walls, powerups, foods, player1, player2);
        nextState.ApplyMove(move1, move2);
        return nextState;
    }

    private void Hurt(SimplifiedAgent agent) {
        if (agent.powerTurns < 1)
        {
            if (agent.length <= 3)
            {
                agent.length = 1;
            }
            agent.length = agent.length - agent.length / 3;
        }
    }

    //modifies agent1 and agent2
    private void ApplyMove(Vector3 move1, Vector3 move2) {
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back, Vector3.zero };
        Debug.Assert(moves.Contains(move1), "move1 not valid input Vector3 move");
        Debug.Assert(moves.Contains(move2), "move2 not valid input Vector3 move");

        player1.powerTurns = Mathf.Max(player1.powerTurns - 1, 0);
        player2.powerTurns = Mathf.Max(player2.powerTurns - 1, 0);

        Vector3 nextPos1 = player1.headPosition + move1;
        Vector3 nextPos2 = player2.headPosition + move2;

        bool headCollision = (nextPos1 == nextPos2);

        if (move1 != Vector3.zero)
        {
            ApplyMoveToAgentIfValid(player1, player2, nextPos1, headCollision);
        }
        if (move2 != Vector3.zero)
        {
            ApplyMoveToAgentIfValid(player2, player1, nextPos2, headCollision);
        }

        while (player1.bodyPositions.Count > player1.length) {
            player1.bodyPositions.RemoveAt(0);
        }
        while (player2.bodyPositions.Count > player2.length) {
            player2.bodyPositions.RemoveAt(0);
        }
    }

    private void ApplyMoveToAgentIfValid(SimplifiedAgent agent, SimplifiedAgent opponent, Vector3 nextPos, bool headCollision) {
        if (IsCrash(nextPos) || headCollision) {
            if (agent.powerTurns < 1) {
                Hurt(agent);
            } else if (opponent.bodyPositions.Contains(nextPos) && agent.powerTurns > 0) { //checks if attacking player2
                Hurt(opponent);
            }
        } else {
            //move is valid
            if (foods.Contains(nextPos)) {
                agent.length++;
                foods.Remove(nextPos);
            }
            if (powerups.Contains(nextPos)) {
                agent.powerTurns += 15; // GAME CONSTANT WAS LAZY
                powerups.Remove(nextPos);
            }
            agent.bodyPositions.Add(agent.headPosition);
            agent.headPosition = nextPos;
            agent.bodyPositions.Add(nextPos);

        }
    }

    //returns true if a position is taken, with respect to the current GameState
    public bool IsCrash(Vector3 position) {
        return (
            player1.bodyPositions.Contains(position) ||
            player2.bodyPositions.Contains(position) ||
            position.y > 1 ||
            position.y < 0 ||
            walls.Contains(position)
        );
    }

    //should only take in the SimplifiedAgents that are fields of this GameState
    private SimplifiedAgent GetOpponent(SimplifiedAgent agent) { 
        if (agent == this.player1) {
            return player2;
        } else if (agent == this.player2) {
            return player1;
        } else {
            Debug.LogError("Invalid input into GetOpponent()");
            return null;
        }
    }
}

public class SimplifiedAgent {
    public List<Vector3> bodyPositions;
    public Vector3 headPosition;
    public int length;
    public int powerTurns;
    public SimplifiedAgent(Agent player) {
        bodyPositions = new List<Vector3>();
        foreach (GameObject body in player.body) {
            bodyPositions.Add(body.transform.position);
        }
        headPosition = player.head.transform.position;
        length = player.length;
        powerTurns = player.powerTurns;
    }
    public SimplifiedAgent(SimplifiedAgent copy) {
        bodyPositions = new List<Vector3>();
        foreach (Vector3 position in copy.bodyPositions) {
            bodyPositions.Add(position);
        }
        headPosition = copy.headPosition;
        length = copy.length;
        powerTurns = copy.powerTurns;
    }
    public Vector3[] ValidMoves(GameState state)
    {
        Vector3[] moves = new[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down, Vector3.forward, Vector3.back };
        moves = moves.Where(move =>
                (powerTurns > 0 && !state.walls.Contains(move + headPosition)) ||
                !state.IsCrash(headPosition + move)).ToArray<Vector3>();
        if (moves.Count() == 0)
        {
            Debug.Log("No valid moves");
            return new[] { Vector3.left };
        }
        return moves;
    }
}