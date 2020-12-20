using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
public class KeyEvent : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {

            UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene", LoadSceneMode.Single);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
