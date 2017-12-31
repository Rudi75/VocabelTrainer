using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnswerOptionsScript : MonoBehaviour {

    public GameObject answerOptionPrefab;
    public AnswerFieldScript answerFieldScript;

    private string answerWithoutWhiteSpaces;
    private List<Button> buttons = new List<Button>();
    public void setAnswer(string answer)
    {
        Regex reg = new Regex("\\s+");
        this.answerWithoutWhiteSpaces = reg.Replace(answer,"");
        buttons.ForEach(button => Destroy(button.gameObject));
        buttons.Clear();
        int i = answerWithoutWhiteSpaces.Length > 2?1:0;
        
        for ( ; i < answerWithoutWhiteSpaces.Length; i++)
        {

                GameObject go = Instantiate(answerOptionPrefab, this.transform, false) as GameObject;
                go.transform.localPosition = new Vector3(1, 1, 1);
                Button button = go.GetComponent<Button>();
                button.GetComponentInChildren<Text>().text = answerWithoutWhiteSpaces.Substring(i, 1);
                button.onClick.AddListener(buttonClicked);
                buttons.Add(button);
            
        }
        shuffle(buttons);
    }

    private void buttonClicked()
    {
        GameObject go = EventSystem.current.currentSelectedGameObject;
        Button button = go.GetComponent<Button>();
        answerFieldScript.setChar(button.GetComponentInChildren<Text>().text);
    }

    public void enableButton(String cha)
    {
        foreach (var button in buttons)
        {
            if(!button.interactable && button.GetComponentInChildren<Text>().text.Equals(cha))
            {
                button.interactable = true;
                return;
            }
        }
    }

    public void disableButton(String cha)
    {
        foreach (var button in buttons)
        {
            if (button.interactable && button.GetComponentInChildren<Text>().text.Equals(cha))
            {
                button.interactable = false;
                return;
            }
        }
    }

    public static void shuffle(List<Button> pile)
    {
        int n = pile.Count;
        System.Random rnd = new System.Random(DateTime.Now.Millisecond);
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            Button value = pile[k];
            pile[k] = pile[n];
            pile[n] = value;
        }
        pile.ForEach(button => button.transform.SetAsLastSibling());
    }

    internal void enableAllButtons()
    {
        foreach (var button in buttons)
        {
            button.interactable = true;
        }
    }
}
