using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToTurnOn;
    [SerializeField] GameObject startGameImage;

    void Start()
    {
        foreach(GameObject gameO in objectsToTurnOn)
        {
            gameO.SetActive(false);
        }
        Time.timeScale = 0.33f;
    }

    public void StartTheGame()
    {
        foreach (GameObject gameO in objectsToTurnOn)
        {
            gameO.SetActive(true);
        }
        Destroy(startGameImage);
        Time.timeScale = 1f;
        Destroy(gameObject);
    }
}
