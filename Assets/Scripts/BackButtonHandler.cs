using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButtonHandler : MonoBehaviour
{
    void OnMouseDown()
    {
        Debug.Log("🔙 Back button clicked!");
        SceneManager.LoadScene("LevelSelect");
    }
}
