using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpectimaxAgent : Agent {
    public MatchManager m;
    public int expectimaxDepth = 0;

    public void Start() {
        // obtain reference to match manager script to access game state
        GameObject managerObject = GameObject.Find("MatchManager");
        m = managerObject.GetComponent<MatchManager>();
        // Debug.Log(m.ToString());
    }
    public override Vector3 DecideMove(Agent otherplayer) {
        // save reference to opponent
        opponent = otherplayer;

        // this agent will visualize itself as player 1 always
        GameState currState = new GameState(m.wallPositions, m.powerUpPositions, m.foodPositions, this, otherplayer);

        float val = -Mathf.Infinity;
        Vector3 bestMove = Vector3.left;
        Vector3[] validMoves = currState.player1.ValidMoves(currState);
        // Debug.Log(currState.NextState(validMoves[0], Vector3.zero));
        foreach (Vector3 move in validMoves) {
            GameState nextState = currState.NextState(move, Vector3.zero);
            float newVal = OpponentMoveValue(nextState, expectimaxDepth - 1);
            if (newVal > val) {

                bestMove = move;
                val = newVal;
            }
        }
        // Debug.Log(val);
        return bestMove;
    }
    //returns the best move, calculated by expectimax, of the agent given the game state
    // TODO: use greedy heuristic if there is no best move
    private float OurMoveValue(GameState state, int depth) {
        if (depth == 0) {
            return Utility(state);
        }
        Vector3[] moves = state.player1.ValidMoves(state);
        float val = -Mathf.Infinity;
        foreach (Vector3 move in moves) {
            GameState nextState = state.NextState(move, Vector3.zero);
            val = Mathf.Max(val, OpponentMoveValue(nextState, depth - 1));
        }
        return val;
    }

    // TODO: it would make sense to do minimax (make opponent minimize instead of random) since it's an adversarial game
    private float OpponentMoveValue(GameState state, int depth) {
        // return OurMoveValue(state, depth - 1); // testing: pretend opponent never moves
        if (depth == 0) {
            return Utility(state);
        }
        Vector3[] moves = state.player2.ValidMoves(state);
        float expVal = 0;
        foreach (Vector3 move in moves) {
            GameState nextState  = state.NextState(Vector3.zero, move);
            expVal += OurMoveValue(nextState, depth);
        }
        expVal = expVal / moves.Length;
        return expVal;
    }
    /*
        def max_Val(gameState, depth):
                if depth == 0 or gameState.isWin() or gameState.isLose():
                    return self.evaluationFunction(gameState)
                actions = gameState.getLegalActions()
                val = -float ("inf")
                for action in actions:
                    val = max(val, E(gameState.generateSuccessor(0, action), 1, depth))
                return val

            def E(gameState, agent, depth) :
                if depth == 0 or gameState.isWin() or gameState.isLose():
                    return self.evaluationFunction(gameState)
                actions = gameState.getLegalActions(agent)
                e = 0
                for action in actions:
                    if agent != gameState.getNumAgents() - 1:
                        e += E(gameState.generateSuccessor(agent, action), agent + 1, depth)
                    else:
                        e += max_Val(gameState.generateSuccessor(agent, action), depth - 1)
                e = e/len(actions)
                return e
    */

     // assume that player2 is the expectimax agent
    private float Utility(GameState state) {
        float lengthDifference = -state.player1.length + state.player2.length;
        float distToTarget = DistToTarget(state);
        return -lengthDifference + distToTarget * 0.1f;
    }

    private float DistToTarget(GameState state) {
        Vector3 target = FindTarget(state);
        return MDist(target, state.player2.headPosition);
    }

    private Vector3 FindTarget(GameState state) {
        Vector3 target = new Vector3(0, 0, 0);
        Vector3 head = state.player2.headPosition;
        // food and powerups are goals
        HashSet<Vector3> goals = new HashSet<Vector3>(state.foods);
        goals.UnionWith(state.powerups);
        // if currently powered up, so is the other player's body
        if (state.player2.powerTurns> 1) {
            goals.UnionWith(state.player1.bodyPositions);
        }
        foreach (Vector3 goal in goals) {
            if (target == Vector3.zero || MDist(target, head) > MDist(goal, head)) {
                target = goal;
            }
        }
        return target;
    }
}
