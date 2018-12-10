using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpectimaxAgent : Agent {
    public MatchManager m;
    public int expectimaxDepth = 5;

    public void Start() {
        // obtain reference to match manager script to access game state
        GameObject managerObject = GameObject.Find("MatchManager");
        m = managerObject.GetComponent<MatchManager>();
        // Debug.Log(m.ToString());
    }
    public override Vector3 DecideMove(Agent otherplayer) {
        GameState currState = new GameState(m.wallPositions, m.powerUpPositions, m.foodPositions, this, this.opponent);
        float val = -Mathf.Infinity;
        Vector3 bestMove = Vector3.left;
        Vector3[] validMoves = FindSafeMoves();
        foreach (Vector3 move in validMoves) {
            float newVal = MaxValue(currState.NextState(move, Vector3.zero), 0, 8);
            if (newVal > val) {
                bestMove = move;
                val = newVal;
            }
        }
        return bestMove;
    }
    //returns the best move, calculated by expectimax, of the agent given the game state
    private float MaxValue(GameState state, int index, int depth) {
        if (depth == 0) {
            return Utility(state);
        }
        Vector3[] moves = FindSafeMoves();
        float val = -Mathf.Infinity;
        foreach (Vector3 move in moves) {
            val = Mathf.Max(val, ExpectimaxValue(state.NextState(move, Vector3.zero), index, depth));
        }
        return val;
    }

    private float ExpectimaxValue(GameState state, int index, int depth) {
        if (depth == 0) {
            return Utility(state);
        }
        Vector3[] moves = FindSafeMoves();
        float expVal = 0;
        foreach (Vector3 move in moves) {
            expVal += ExpectimaxValue(state.NextState(move, Vector3.zero), index, depth - 1);
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

    private float Utility(GameState state) {
        return this.length;
    }

}
