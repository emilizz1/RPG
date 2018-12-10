using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Quit : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKey("escape"))
            {
                Debug.Log("quit requested");
                Application.Quit();
            }
        }
    } 
}
