using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameManager : MonoBehaviour
{
    private LevelData levelData;

    [Inject]
    public void Construct(LevelData levelData)
    {
        this.levelData = levelData;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(levelData.LevelIndex - 1);
    }
    public void LoadNextLevel()
    {
        int maxIndex = SceneManager.sceneCountInBuildSettings - 1;
        int nextIndex = levelData.LevelIndex;

        if(nextIndex > maxIndex)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(nextIndex);
        }
    }
}
