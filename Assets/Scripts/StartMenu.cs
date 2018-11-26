using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	public void Play2Player()
    {
        Debug.Log("Play 2-player");
        SceneManager.LoadScene(2);
    }
    public void Play1Player()
    {
        Debug.Log("Play 1-player");
        SceneManager.LoadScene(1);
    }
}
