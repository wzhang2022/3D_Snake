using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
    TODO
    1. Determine features
    2. Still need to call UpdateWeights (probably at the end of PrepareNextMove or Decidemove) so that weights
    are properly updated.
        2.1. Need to define what rewards are: (state.getValue() - prevState.getValue())
*/
 
public class RLAgent : Agent
{
    public static System.Random rnd = new System.Random();
    Dictionary<string, float> weights = new Dictionary<string, float>();

    // learning rate, exploration rate, and discount factor
    float alpha = 0.01f;
    float epsilon = 0.05f;
    float gamma = 0.8f;

    public MatchManager m;

    GameState prevState = null;
    Vector3 prevMove = new Vector3(0, 0, 0);

    // Look into this later
    public void Start() {
        // obtain reference to match manager script to access game state
        GameObject managerObject = GameObject.Find("MatchManager");
        m = managerObject.GetComponent<MatchManager>();
        // Debug.Log(m.ToString());
    }

    public override Vector3 DecideMove(Agent otherplayer) {
        // this agent will visualize itself as player 1 always
        GameState state = new GameState(m.wallPositions, m.powerUpPositions, m.foodPositions, this, otherplayer);

        // Update weights here
        if (prevState != null) {
            float reward = state.player1.length - state.player2.length;
            UpdateWeights(prevState, prevMove, state, reward);
        }

        Vector3[] validMoves = state.player1.ValidMoves(state);

        // with prob epsilon, pick a random valid move
        Vector3 move = new Vector3(0, 0, 0);
        if ((float)rnd.NextDouble() < epsilon) {
            int r = rnd.Next(validMoves.Length);
            move = validMoves[r];
        } else {
            move = GetActionFromQValues(state);
        }

        prevMove = move;
        prevState = state;
        return move;
    }

    private float GetValueFromQValues(GameState state) {
        Vector3[] validMoves = state.player1.ValidMoves(state);
        float best_value = -Mathf.Infinity;

        // Calculate best value from QValues
        foreach (Vector3 move in validMoves) {
            float value = EvaluateQValue(state, move);
            best_value = Mathf.Max(best_value, value);
        }

        return best_value;
    }
    private Vector3 GetActionFromQValues(GameState state) {
        Vector3[] validMoves = state.player1.ValidMoves(state);
        List<Vector3> bestMoves = new List<Vector3>();
        float best_value = GetValueFromQValues(state);

        // Calculate best action
        foreach (Vector3 move in validMoves) {
            if (EvaluateQValue(state, move) == best_value) {
                bestMoves.Add(move);
            }
        }

        // Randomly select one of the best moves
        int r = rnd.Next(bestMoves.Count);
        return bestMoves[r];
    }

    private float EvaluateQValue(GameState state, Vector3 move) {
        float q_value = 0;
        Dictionary<string, float> features = GetFeatures(state, move);

        foreach (KeyValuePair<string, float> kvp in features) {
            q_value += weights[kvp.Key] * kvp.Value;
        }

        return q_value;
    }

    // straight from Greedy Agent
    private Vector3 FindTarget(HashSet<Vector3> goals)
    {
        Vector3 target = new Vector3(0, 0, 0);
        Vector3 head = this.head.transform.position;

        foreach (Vector3 goal in goals)
        {
            if (target == Vector3.zero || MDist(target, head) > MDist(goal, head))
            {
                target = goal;
            }
        }
        return target;
    }

    private Dictionary<string, float> GetFeatures(GameState state, Vector3 move) {
        /*
            Features of a state after a move:
            - how far away the closest food is
                - given you have power up
                - give you don't
            - distance to nearest body by the time you reach it
        */

        Dictionary<string, float> features = new Dictionary<string, float>();

        // syntax to add features to dictionary:
        // features.Add("closest_food", 1.0);
        // weights.Add("closest_food", 0.0);

        // Closest food
        Vector3 head = state.player1.headPosition;
        Vector3 target = FindTarget(m.foodPositions);
        float dist = MDist(head + move, target);

        features.Add("closest_food", (dist / 30f));
        if (!weights.ContainsKey("closest_food")) {
            weights.Add("closest_food", 0f);
        }
        
        // Closest power-up
        target = FindTarget(m.powerUpPositions);
        dist = MDist(head + move, target);

        features.Add("closest_pup", (dist / 30f) /* * ((float)state.player2.length / 10f) */);
        if (!weights.ContainsKey("closest_pup")) {
            weights.Add("closest_pup", 0f);
        }

        // avoid enemy if enemy has power up
        target = state.player2.headPosition;
        dist = MDist(head + move, target);

        if (state.player2.powerTurns > 0) {
            features.Add("avoid_enemy", (1f / dist));
        } else {
            features.Add("avoid_enemy", 0f);
        }
        
        if (!weights.ContainsKey("avoid_enemy")) {
            weights.Add("avoid_enemy", 0f);
        }

        // go towards enemy if you have power up
        HashSet<Vector3> enemy_body = new HashSet<Vector3>(state.player2.bodyPositions);
        target = FindTarget(enemy_body);
        if (state.player1.powerTurns > MDist(head, target)) {
            features.Add("go_to_enemy", (dist / 30f));
        } else {
            features.Add("go_to_enemy", 0f);
        }

        if (!weights.ContainsKey("go_to_enemy")) {
            weights.Add("go_to_enemy", 0f);
        }

        return features;
    }

    private void UpdateWeights(GameState state, Vector3 move, GameState nextState, float reward) {
        // Lecture 11 Slide 16
        float difference = (reward + gamma * GetValueFromQValues(nextState)) - EvaluateQValue(state, move);

        Dictionary<string, float> features = GetFeatures(state, move);
        foreach (KeyValuePair<string, float> kvp in features) {
            weights[kvp.Key] += alpha * difference * kvp.Value;
        }
        
        foreach (KeyValuePair<string, float> kvp in weights) {
            Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        }
    }
}
