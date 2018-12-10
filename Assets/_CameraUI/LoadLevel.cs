using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.CameraUI
{
    public class LoadLevel : MonoBehaviour
    {
        public void LoadGame()
        {
            SceneManager.LoadScene(1);
        }

    }
}
