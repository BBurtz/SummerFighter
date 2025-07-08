using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] RespawnPoints;

    public GameObject giveRespawnPoint()
    {
        return RespawnPoints[0];
    }

    public void GameOver()
    {
        Debug.Log("Game Done");
    }

    void Start()
    {
        Application.targetFrameRate = 60;
    }
}
