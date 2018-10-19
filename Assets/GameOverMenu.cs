using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour {
    public void DeactivateMenu()
    {
        this.gameObject.transform.parent.gameObject.SetActive(false);
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            var child = this.gameObject.transform.GetChild(i).gameObject;
            if (child.name.Contains("Text"))
                child.SetActive(false);
        }
    }
    public void PlayAgain()
    {
        DeactivateMenu();
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
    public void MainMenu()
    {
        DeactivateMenu();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
