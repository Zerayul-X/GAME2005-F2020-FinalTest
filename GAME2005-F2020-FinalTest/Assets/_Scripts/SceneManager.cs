using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    // Start is called before the first frame update
    public void PlayGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
