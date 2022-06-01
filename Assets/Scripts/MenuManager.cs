using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{

    public static MenuManager MenuManagerInstance;
    public bool GameState;
    public bool first;
    public GameObject[] menuElement = new GameObject[4];

    // Start is called before the first frame update
    void Start()
    {
        GameState = false;
        MenuManagerInstance = this;
        first = true;
    }

    public void StartTheGame()
    {

        if (!first)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            first = false;
            GameState = true;
            menuElement[0].SetActive(false);
            GameObject.FindWithTag("air").GetComponent<ParticleSystem>().Play();
            GameObject.FindWithTag("trail").GetComponent<ParticleSystem>().Play();

        }



    }

}
