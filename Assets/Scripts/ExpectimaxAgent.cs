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
        return expectimaxMove(this, currState);
    }
    //returns the best move, calculated by expectimax, of the agent given the game state
    private Vector3 expectimaxMove(Agent player, GameState state) {
        throw new NotImplementedException();
    }

    private float utility() {
        return this.length;
    }

}
