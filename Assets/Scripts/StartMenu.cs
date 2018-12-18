using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

    public void WatchGreedy_GreedyMax_Basic() {
        SceneManager.LoadScene(5);
    }
    public void WatchGreedySearch_GreedyMax_Basic() {
        SceneManager.LoadScene(6);
    }
    public void WatchGreedy_Expectimax_Basic() {
        SceneManager.LoadScene(2);
    }
    public void WatchGreedy_GreedySearch_Basic() {
        SceneManager.LoadScene(3);
    }
    public void WatchReflex_Expectimax_Basic() {
        SceneManager.LoadScene(8);
    }
    public void WatchGreedy_GreedySearch_Cross() {
        SceneManager.LoadScene(4);
    }
    public void WatchGreedySearch_GreedyMax_Cross() {
        SceneManager.LoadScene(7);
    }
    public void WatchRL_Greedy_Basic() {
        SceneManager.LoadScene(9);
    }
    public void WatchRL_GreedySearch_Basic() {
        SceneManager.LoadScene(11);
    }
    public void WatchRL_GreedyMax_Basic() {
        SceneManager.LoadScene(10);
    }

    public void Play2Player()
    {
        Debug.Log("Play 2-player");
        SceneManager.LoadScene(1);
    }
    public void Play1Player()
    {
        Debug.Log("Play 1-player");
        SceneManager.LoadScene(1);
    }
}
