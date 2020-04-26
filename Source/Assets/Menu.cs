using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Menu : MonoBehaviour
{
	static public int mode;
	
	public void PlayGame()
    {
        SceneManager.LoadScene(0);
    }

    public void Instructions()
    {
        SceneManager.LoadScene(4);
    }
	
    public void Easy()
    {
		mode = 0;
        Retry();
    }

    public void Hard()
    {
		mode = 1;
        Retry();
    }
	
    public void Retry()
    {
        SceneManager.LoadScene(2);
    }
	
	public void ChangeMode()
	{
        SceneManager.LoadScene(1);
	}

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
