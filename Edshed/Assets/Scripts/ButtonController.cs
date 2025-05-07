using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonController : MonoBehaviour
{

    public string word, wordClass, wordDefinition;
    public Action<string, string, string> WordData;
    [SerializeField] private TextMeshProUGUI UIText;


    public Action<DefinitionGame, int> setValue;

    private void Start()
    {
        WordData = SetWordData;
    }
    //This data is set through the DataHandler script at the end of the 'PickRandomWord' function
    private void SetWordData(string tempWord, string tempClass, string tempDefinition)
    {
        word = tempWord;
        wordClass = tempClass;
        wordDefinition = tempDefinition;
        Debug.Log(this.name + " " + word);
        UIText.text = wordDefinition;
    }
}
