using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] TextMeshProUGUI BestScoreText;
    [SerializeField] Button NewGameButton;
    [SerializeField] Button ReTryButton;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float GameOverAnimationTime=1;
    [SerializeField] float GameOverAnimationDelayTime = 1;
    private int Score;
    private void Awake()
    {
        Instance = this;
        NewGameButton.onClick.AddListener(() =>
        {
            NewGameFunc();
        });
        ReTryButton.onClick.AddListener(() =>
        {
            NewGameFunc();
        });
    }
    
    public void NewGameFunc()
    {
        DOTween.KillAll();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.gameObject.SetActive(false);
        GameController.instance.NewGame();
    }
    public void IncreaseScore(int value)
    {
        SetScore(Score + value);
    }
    public void LoadHighScore()
    {
        BestScoreText.text=PlayerPrefs.GetInt("HighScore",0).ToString();
    }
    public void SetScore(int Score)
    {
        this.Score = Score;
        ScoreText.text = Score.ToString();
        SaveScore();

    }
    public void SaveScore()
    {
        if(PlayerPrefs.GetInt("HighScore", 0)<Score)
        {
            PlayerPrefs.SetInt("HighScore", Score);
        }
    }
    public void GameOver()
    {
        SaveScore();
        StartCoroutine(GameOverAnimation());
    }
    private IEnumerator GameOverAnimation()
    {
        canvasGroup.gameObject.SetActive(true);
        yield return new WaitForSeconds(GameOverAnimationDelayTime);
        canvasGroup.DOFade(1f, GameOverAnimationTime);
        canvasGroup.interactable = true;
    }
}
