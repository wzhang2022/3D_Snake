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
    float alpha = 0.2f;
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
            float reward = state.player1.length - prevState.player1.length;
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

    private Dictionary<string, float> GetFeatures(GameState state, Vector3 move) {
        /*
            Features of a state after a move:
            - length of snake
            - how far away the closest food is
                - given you have power up
                - give you don't
            - distance to nearest body by the time you reach it
            - whether move made you eat food (1.0)
        */

        Dictionary<string, float> features = new Dictionary<string, float>();

        // syntax to add features to dictionary:
        // features.Add("closest_food", 1.0);
        // weights.Add("closest_food", 0.0);

        // Length of snake
        features.Add("length", ((float)state.player1.length / 40f));
        if (!weights.ContainsKey("length")) {
            weights.Add("length", 0.9f);
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

        foreach (KeyValuePair<string, float> kvp in features) {
            Debug.Log(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
        }
    }
}
