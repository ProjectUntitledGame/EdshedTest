using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Tracing;


public class DataHandler : MonoBehaviour
{
    private string databaseURL = "https://api.edshed.com/lists/Y12";
    public Action<ButtonController> pickAWord;
    [SerializeField] private ButtonController[] buttons;
    private ButtonController currentButton;
    private int buttonCounter = 0;
    public Action<Action> Begin;
    public Action finishSetup;
    void Start()
    {
        Begin = StartGame;
    }
    private void StartGame(Action tempFinishSetup)
    {
        currentButton = buttons[0];
        StartCoroutine(FetchWord());
        finishSetup = tempFinishSetup;
    }

    // Coroutine to fetch the word from the API
    private IEnumerator FetchWord()
    {
        //Send the request
        UnityWebRequest request = UnityWebRequest.Get(databaseURL);

        //return the request
        yield return request.SendWebRequest();

        //Make sure the request returns successfully
        if (request.result == UnityWebRequest.Result.Success)
        {
            //This is the response from the request
            string jsonResponse = request.downloadHandler.text;
            Debug.Log("raw Json: \n" + jsonResponse);
            //This try is used to attempt at parsing the Json. If it fails, the catch will return the error
            try{
                //This root object is used to 
                Root tempRoot = JsonConvert.DeserializeObject<Root>(jsonResponse);
                //Checking if the list is empty. If it is, return an error
                if (tempRoot?.list?.words != null && tempRoot.list.words.Length > 0)
                {
                    //Trigger PickRandomWord with the temporary root as the list to read from
                    PickRandomWord(tempRoot.list);
                }
                else
                {
                    Debug.LogError("Json empty!");
                }
            }catch(System.Exception e)
            {
                Debug.Log("Error parsing: " + e.Message);
            }
            int startIndex = 0; 
            if (startIndex < 0)
            {
                Debug.LogError("No words in list!");
                yield break;
            }
        }
        else
        {
            // Handle request error
            Debug.LogError("Request failed: " + request.error);
        }
    }

    private void PickRandomWord(WordWrapper wordList)
    {
        if(wordList.words == null || wordList.words.Length == 0)
        {
            Debug.Log("There are no words in the list!");
            return;
        }
        int randomIndex = UnityEngine.Random.Range(0, wordList.words.Length);
        Word selectedWord = wordList.words[randomIndex];


        if (!string.IsNullOrEmpty(selectedWord.definitions))
        {
            try
            {
                // Parse definitions JSON string into a list of Definition objects
                List<Definition> definitions = JsonConvert.DeserializeObject<List<Definition>>(selectedWord.definitions);
                foreach (Definition tempDefinition in definitions)
                {
                    currentButton.WordData(selectedWord.text, tempDefinition.wordClass, tempDefinition.definitionText);
                    
                }

            }
            catch (Exception e)
            {
                Debug.LogError("Error parsing definitions: " + e.Message);
            }
        }
        else
        {
            Debug.Log("No definitions available.");
        }


        //This loops  back to the top if there are more buttons to be set
        if (currentButton != buttons[buttons.Length - 1])
        {
            currentButton = buttons[buttonCounter + 1];
            buttonCounter++;
            StartCoroutine(FetchWord());
        }
        else
        {
            //This triggers the word to come up on the display through the DefinitionGame script
            finishSetup();
        }
    }
}

//Json classes
public class Root
{
    [JsonProperty("list")]
    public WordWrapper list { get; set; }
}

[System.Serializable]
public class Word
{
    [JsonProperty("text")]
    public string text { get; set; }

    [JsonProperty("definitions")]
    public string definitions { get; set; }
}

[System.Serializable]
public class Definition
{
    [JsonProperty("class")]
    public string wordClass { get; set; }

    [JsonProperty("definition")]
    public string definitionText { get; set; }
}

[System.Serializable]
public class WordWrapper
{
    [JsonProperty("words")]
    public Word[] words;
}

