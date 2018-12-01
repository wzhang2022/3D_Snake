using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState {
    public HashSet<Vector3> walls;
    public HashSet<Vector3> powerups;
    public HashSet<Vector3> foods;
    public Agent player1;
    public Agent player2;

    public GameState(HashSet<Vector3> walls,
                     HashSet<Vector3> powerups,
                     HashSet<Vector3> foods,
                     Agent player1,
                     Agent player2) {
        this.walls = walls;
        this.powerups = powerups;
        this.foods = foods;
        this.player1 = player1;
        this.player2 = player2;
    }
    //returns the state that would result if player1 moved by move1 and player2 moved by move2
    public GameState NextState(Vector3 move1, Vector3 move2) {
        return null;
    }
}