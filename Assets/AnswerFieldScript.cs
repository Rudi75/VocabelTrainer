using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class AnswerFieldScript : MonoBehaviour
{

    public GameObject answerLetterPrefab;
    public GameObject spacePrefab;
    public AnswerOptionsScript optionScript;


    private string answer;
    private string answerWithoutWhitespaces;
    private List<InputField> inputs = new List<InputField>();
    private int activeLetter = 1;
    private int startLetter;
    private bool deleted = false;
    Regex reg = new Regex("\\s+");
    public void setAnswer(string answer)
    {
        inputs.ForEach(input => Destroy(input.gameObject));
        inputs.Clear();
        this.answer = answer;
        
        answerWithoutWhitespaces = reg.Replace(answer, "");
        
        for (int i = 0; i < answer.Length; i++)
        {
            if (reg.IsMatch(answer.Substring(i,1)))
            {
                GameObject go = Instantiate(spacePrefab, this.transform) as GameObject;
                go.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                GameObject go = Instantiate(answerLetterPrefab, this.transform) as GameObject;
                go.transform.localScale = new Vector3(1, 1, 1);
                InputField field = go.GetComponent<InputField>();
                field.interactable = false;
                field.text = "";
                inputs.Add(field);
            }
        }

        activeLetter = 0;
        if (answerWithoutWhitespaces.Length > 2)
        {
            inputs[activeLetter].text = answerWithoutWhitespaces.Substring(0, 1);
            activeLetter++;
        }

        inputs[activeLetter].interactable = true;
        inputs[activeLetter].ActivateInputField();
        inputs[activeLetter].onValueChanged.AddListener(valueChanged);
        startLetter = activeLetter;
    }

    private void Update()
    {
        if (!deleted && Input.GetKeyDown(KeyCode.Backspace) && inputs[activeLetter].text == "")
        {
            goOneBack();
        }
        deleted = false;
    }

    private void test(string arg0)
    {
        Debug.Log("test");
    }

    public bool isAnswerCorrect()
    {
        for (int i = 0; i < answerWithoutWhitespaces.Length; i++)
        {
            string letter = inputs[i].text;
            Regex regex = new Regex(Regex.Escape("`") + "|" + Regex.Escape("´"));
            if (regex.IsMatch(letter))
            {
                letter = "'";
            }
            if (!answerWithoutWhitespaces.Substring(i, 1).Equals(letter))
            {
                inputs[activeLetter].ActivateInputField();
                return false;
            }
        }
        return true;
    }

    internal void setChar(string text)
    {
        inputs[activeLetter].text = text;

    }

    private void valueChanged(string value)
    {
        if (value != "")
        {

                Regex regex = new Regex(Regex.Escape("`") + "|" + Regex.Escape("´"));
                if (regex.IsMatch(value))
                {
                    optionScript.disableButton("'");
                }
                else
                {
                    optionScript.disableButton(value);
                }
 
            if (activeLetter +  1 < answerWithoutWhitespaces.Length)
            {
                inputs[activeLetter].interactable = false;
                inputs[activeLetter].onValueChanged.RemoveAllListeners();
                activeLetter++;
                inputs[activeLetter].interactable = true;
                
                inputs[activeLetter].onValueChanged.AddListener(valueChanged);
            }
            inputs[activeLetter].ActivateInputField();
            StartCoroutine(setCurser());

        }
        else
        {
            updateOptions();
            deleted = true;
        }
    }

    private void updateOptions()
    {
        optionScript.enableAllButtons();
        for (int i = startLetter; i < activeLetter; i++)
        {
            string value = inputs[i].text;
            Regex regex = new Regex(Regex.Escape("`") + "|" + Regex.Escape("´"));
            if (regex.IsMatch(value))
            {
                optionScript.disableButton("'");
            }
            else
            {
                optionScript.disableButton(value);
            }
        }
    }

    private void goOneBack()
    {
        if (activeLetter > startLetter)
        {
            inputs[activeLetter].interactable = false;
            inputs[activeLetter].onValueChanged.RemoveAllListeners();
            activeLetter--;
            inputs[activeLetter].interactable = true;
            inputs[activeLetter].ActivateInputField();
            inputs[activeLetter].text = "";
            updateOptions();
            StartCoroutine(setCurser());
            inputs[activeLetter].onValueChanged.AddListener(valueChanged);
        }
    }

    private IEnumerator setCurser()
    {
        yield return null;
        inputs[activeLetter].MoveTextEnd(true);
    }
}
