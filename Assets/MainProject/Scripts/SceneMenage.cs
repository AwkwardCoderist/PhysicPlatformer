using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class SceneMenage : MonoBehaviour
{
    public static SceneMenage instance;

    [SerializeField] private Image fade;
    [SerializeField] private float duration;
    
    private AsyncOperation sceneLoadingOperation;
    private string sceneName;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);
    }

    public void GoToScene(string name)
    {
        fade.fillOrigin = (int)Image.OriginHorizontal.Left;
        fade.DOFillAmount(1, duration).onComplete += () =>
        {
            StartCoroutine(LoadScene(name));
        };        
    }

    private IEnumerator LoadScene(string name)
    {

        sceneLoadingOperation = SceneManager.LoadSceneAsync(name);

        while (!sceneLoadingOperation.isDone)
        {
            yield return null;
        }

        OnCompleted();
        yield return null;
    }

    private void OnCompleted()
    {
        fade.fillOrigin = (int)Image.OriginHorizontal.Right;
        fade.DOFillAmount(0, duration);
    }
}
