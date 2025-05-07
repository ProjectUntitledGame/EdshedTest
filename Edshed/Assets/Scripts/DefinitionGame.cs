using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DefinitionGame : MonoBehaviour
{
    [SerializeField] ButtonController[] buttons;
    private int correctAnswer;
    public DataHandler wordGenerator;
    [SerializeField] private GameObject winPanel, startPanel;
    [SerializeField] private TextMeshProUGUI wordDisplay;
    private Action setAnswer;

    //Set action
    private void Start()
    {
        setAnswer = SetAnswerFunction;
    }
    //triggers through the start button to begin setting the words through DataHandler script
    public void GameStart()
    {
        wordGenerator.Begin(setAnswer);
        startPanel.SetActive(false);
        correctAnswer = UnityEngine.Random.Range(0, buttons.Length);
    }

    //Called at the end of DataHandler to set the correct answer on the display
    private void SetAnswerFunction()
    {
        wordDisplay.text = buttons[correctAnswer].word;
    }

    //When clicking a button, this will check to see if it is correct
    public void CheckAnswer(ButtonController thisButton)
    {
        if (thisButton.word == wordDisplay.text)
        {
            winPanel.gameObject.SetActive(true);
        }
    }

    //Reload the scene if the player is correct.
    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }
}