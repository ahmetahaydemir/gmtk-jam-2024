using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TransitionManager : MonoBehaviour
{
    public CanvasGroup transitionGroup;
    //
    public void TransitionOut()
    {
        transitionGroup.alpha = 1f;
        transitionGroup.DOFade(0f, 0.5f);
    }
    public void TransitionIn()
    {
        transitionGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        });
    }
}