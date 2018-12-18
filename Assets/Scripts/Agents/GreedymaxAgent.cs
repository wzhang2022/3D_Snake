using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedymaxAgent : Agent {
    public MatchManager m;
    public int greedymaxDepth = 0;

    public void Start() {
        // obtain reference to match manager script to access game state
        GameObject managerObject = GameObject.Find("MatchManager");
        m = managerObject.GetComponent<MatchManager>();
    }
    public override Vector3 DecideMove(Agent otherplayer) {
        // save reference to opponent
        opponent = otherplayer;

        // this agent will visualize itself as player 1 always
        GameState currState = new GameState(m.wallPositions, m.powerUpPositions, m.foodPositions, this, otherplayer);

        float val = -Mathf.Infinity;
        Vector3 bestMove = Vector3.left;
        Vector3[] validMoves = currState.player1.ValidMoves(currState);
        foreach (Vector3 move in validMoves) {
            GameState nextState = currState.NextState(move, Vector3.zero);
            float newVal = OpponentMoveValue(nextState, greedymaxDepth - 1);
            // tiebreak greedily - based on one move lookahead
            newVal += .01f * Utility(currState.NextState(move, Vector3.zero));
            if (newVal > val) {

                bestMove = move;
                val = newVal;
            }
        }

        //Debug.Log(bestMove);
        //Debug.Log(val);
        return bestMove;
    }
    
    private float OurMoveValue(GameState state, int depth) {
        if (depth == 0) {
            return Utility(state);
        }
        Vector3[] moves = state.player1.ValidMoves(state);
        float val = -Mathf.Infinity;
        // take the move that maximizes utility when allowing opponent to move next
        foreach (Vector3 move in moves) {
            GameState nextState = state.NextState(move, Vector3.zero);
            val = Mathf.Max(val, OpponentMoveValue(nextState, depth - 1));
        }
        return val;
    }

    // Assume that opponent picks a move greedily, opponent is player 2
    private float OpponentMoveValue(GameState state, int depth)
    {
        if (depth == 0)
        {
            return Utility(state);
        }
        HashSet<Vector3> targets = new HashSet<Vector3>(state.foods);
        targets.UnionWith(state.powerups);
        // add body as targets when powered up
        if (state.player2.powerTurns > 1)
        {
            targets.UnionWith(state.player1.bodyPositions);
            targets.Add(state.player1.headPosition);
        }
        // find closest target
        Vector3 bestTarget = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        Vector3 head = state.player2.headPosition;
        foreach (Vector3 target in targets)
        {
            if (MDist(target, head) < MDist(bestTarget, head))
            {
                bestTarget = target;
            }
        }
        // find move which takes us closer to closest target
        Vector3[] moves = state.player2.ValidMoves(state);
        Vector3 bestMove = Vector3.left;
        if (moves.Length > 0)
        {
            bestMove = moves[0];
        }
        foreach (Vector3 move in moves)
        {
            if (MDist(move + head, bestTarget) < MDist(bestMove + head, bestTarget))
            {
                bestMove = move;
            }
        }
        // return our value of the state after opponent takes the greedy move
        GameState nextState = state.NextState(Vector3.zero, bestMove);
        return OurMoveValue(nextState, depth); ;
    }
    // player1 is always the expectimax agent in the gamestate
    private float Utility(GameState state) {
        // certain loss condition
        //Debug.Log(state.player2.length);
        if (state.player1.length < 1)
        {
            return -Mathf.Infinity;
        }
        // certain win condition
        if (state.player2.length < 1)
        {
            return Mathf.Infinity;
        }
        float lengthDifference = state.player1.length - state.player2.length;
        float distToTarget = DistToTarget(state, state.player1);
        float powerTurnsDiff = state.player1.powerTurns - state.player2.powerTurns;
        float distBetweenPlayers = MDist(state.player1.headPosition, state.player2.headPosition);
        // main utility is length differential, tie breaking greedily 
        return lengthDifference - distToTarget * 0.01f + powerTurnsDiff * 0.1f;
    }

    private float DistToTarget(GameState state, SimplifiedAgent player) {
        Vector3 target = FindTarget(state, player);
        return MDist(target, player.headPosition);
    }

    // find the closest target to a player
    private Vector3 FindTarget(GameState state, SimplifiedAgent player) {
        Vector3 target = new Vector3(0, 0, 0);
        Vector3 head = player.headPosition;
        // food and powerups are goals
        HashSet<Vector3> goals = new HashSet<Vector3>(state.foods);
        goals.UnionWith(state.powerups);
        // if currently powered up, so is the other player's body
        if (player.powerTurns> 1) {
            goals.UnionWith(state.GetOpponent(player).bodyPositions);
        }
        foreach (Vector3 goal in goals) {
            if (target == Vector3.zero || MDist(target, head) > MDist(goal, head)) {
                target = goal;
            }
        }
        return target;
    }
}
