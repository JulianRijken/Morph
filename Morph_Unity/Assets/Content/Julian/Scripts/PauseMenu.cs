using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Morph.Julian
{
    public class PauseMenu : MonoBehaviour
    {
        public void QuitGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}