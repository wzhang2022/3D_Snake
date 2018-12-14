using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
    TODO
    1. Determine features
    2. Still need to call UpdateWeights (probably at the end of PrepareNextMove) so that weights
    are properly updated.
        2.1. Need to define what rewards are (state.getValue() - prevState.getValue())
    3. In DecideMove, put in a random chance of picking a suboptimal move with prob epsilon
 */
public class RLAgent : Agent
{
    static Random rnd = new Random();
    Dictionary<string, float> weights = new Dictionary<string, float>();

    // learning rate, exploration rate, and discount factor (open for tweaking)
    float alpha = 0.2;
    float epsilon = 0.05;
    float gamma = 0.8;

    public override Vector3 DecideMove(GameState state) {
        Vector3[] validMoves = FindSafeMoves();

        // with prob epsilon, pick a random valid move. Else, 
        return GetActionFromQValues(state);
    }

    private float GetValueFromQValues(GameState state) {
        Vector3[] validMoves = FindSafeMoves();
        float best_value = -Mathf.Infinity;

        // Calculate best value from QValues
        foreach (Vector3 move in validMoves) {
            float value = EvaluateQValue(state, move);
            best_value = Math.Max(best_value, value);
        }

        return best_value;
    }
    private Vector3 GetActionFromQValues(GameState state) {
        Vector3[] validMoves = FindSafeMoves();
        List<Vector3> bestMoves = new List<Vector3>();
        float best_value = GetValueFromQValues();

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
        q_value = 0;
        Dictionary<string, float> features = GetFeatures(state, move);

        foreach (KeyValuePair<string, float> kvp in features) {
            q_value += weights[kvp.Key] * kvp.Value;
        }

        return q_value;
    }

    private Hashtable GetFeatures(GameState state, Vector3 move) {
        // look in featureExtractors.py (PSet 3)
        Dictionary<string, float> features = new Dictionary<string, float>();

        // syntax to add features to dictionary:
        // features.Add("closest_food", 1.0);

        return features;
    }

    private void UpdateWeights(GameState state, Vector3 move, GameState nextState, float reward) {
        // Lecture 11 Slide 16
        difference = (reward + gamma * GetValueFromQValues(nextState)) - EvaluateQValue(state, move);

        Dictionary<string, float> features = GetFeatures(state, move);
        foreach (KeyValuePair<string, float> kvp in features) {
            weights[kvp.Key] += alpha * difference * kvp.Value;
        }
    }
}