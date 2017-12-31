using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine.SceneManagement;

public class TrainerScript : MonoBehaviour
{
    public Text message;
    public Text question;
    public Text questionLanguage;
    public Text answerLanguage;
    public Image questionImage;
    public InputField answer;
    public AnswerFieldScript answerField;
    public AnswerOptionsScript optionScript;

    public Button helpButton;

    private List<Vocabel> questionPool = new List<Vocabel>();
    private bool helpEnabled = false;

    private int questionCount = 0;
    private bool germanAsked;
    private

    System.Random rnd = new System.Random();
    private Color baseColor;

    // Use this for initialization
    void Start()
    {

        baseColor = questionImage.color;

        helpButton.onClick.AddListener(enableHelp);
        prepare();
    }

    private void prepare()
    {
        questionPool.Clear();
        questionCount = 0;
        foreach (var voc in VocabelDb.instance.vocabels)
        {
            if (voc.level < 5 && voc.german.Count > 0 && voc.english.Count > 0)
            {
                questionPool.Add(voc);
            }
        }

        if (questionPool.Count == 0)
        {
            message.transform.parent.gameObject.SetActive(true);
            message.text = "No more Vocabels to train!";
            return;
        }

        shuffle(questionPool);

        setQuestion();
    }

    private void setQuestion()
    {
        helpEnabled = false;
        helpButton.interactable = true;
        answerField.gameObject.SetActive(false);
        optionScript.gameObject.SetActive(false);
        answer.gameObject.SetActive(true);
        questionImage.color = baseColor;

        Vocabel voc = questionPool[questionCount];
        if (rnd.Next() % 2 == 0)
        {
            question.text = voc.german[rnd.Next(0, voc.german.Count - 1)];
            germanAsked = true;
            questionLanguage.text = "Deutsch";
            answerLanguage.text = "English";
        }
        else
        {
            question.text = voc.english[rnd.Next(0, voc.english.Count - 1)]; 
            germanAsked = false;
            questionLanguage.text = "English";
            answerLanguage.text = "Deutsch";
        }
        answer.text = "";
        answer.transform.FindChild("Text").GetComponent<Text>().color = Color.black;
        answer.Select();
        answer.ActivateInputField();

    }


    public void checkAnswer()
    {
        Vocabel voc = questionPool[questionCount];
        if (helpEnabled)
        {
            if (answerField.isAnswerCorrect())
            {
                StartCoroutine(confirmCorrect(voc));
            }
            else
            {
                StartCoroutine(confirmWrong(voc));
            }
        }
        else
        {
            List<String> answerTextList = germanAsked ? voc.english : voc.german;
            string whiteChar = "\\s";
            bool correctAnswerGiven = false;
            foreach (string listEntry in answerTextList)
            {
                string answerText = listEntry.ToUpper();

                Regex reg = new Regex(whiteChar + "+");
                string[] answerParts = reg.Split(answerText);

                StringBuilder answerStringBuilder = new StringBuilder();
                answerStringBuilder.Append("^"+ whiteChar + "*");
                for (int i = 0; i < answerParts.Length; i++)
                {
                    if (i != 0)
                    {
                        answerStringBuilder.Append(whiteChar + "+");
                    }
                    string answerPart = answerParts[i];
                    answerPart = handleSpecialChars(answerPart);
                    answerStringBuilder.Append(answerPart);
                }
                answerStringBuilder.Append(whiteChar + "*(!|.)*" + whiteChar+ "*$");

                Regex answerReg = new Regex(answerStringBuilder.ToString());
                string userAnswer = answer.text.ToUpper();
                if (answerReg.IsMatch(userAnswer))
                {
                    correctAnswerGiven = true;
                    break;
                }
            }
            if (correctAnswerGiven)
            {
                StartCoroutine(confirmCorrect(voc));
            }
            else
            {
                StartCoroutine(confirmWrong(voc));
                answer.Select();
                answer.ActivateInputField();
            }
        }
    }

    private string handleSpecialChars(string answerPart)
    {
        Regex reg = new Regex(Regex.Escape("`") + "|" + Regex.Escape("´") + "|" + Regex.Escape("'"));
        answerPart = reg.Replace(answerPart,"(" + Regex.Escape("'") + "|" + Regex.Escape("`") + "|" + Regex.Escape("´") + ")");
        answerPart = answerPart.Replace("Ä", "(Ä|AE)");
        answerPart = answerPart.Replace("Ö", "(Ö|OE)");
        answerPart = answerPart.Replace("Ü", "(Ü|UE)");
        answerPart = answerPart.Replace(".", "");
        answerPart = answerPart.Replace("!", "");
        return answerPart;
    }

    private IEnumerator confirmWrong(Vocabel voc)
    {
        voc.wrong();
        questionImage.color = Color.red;
        yield return new WaitForSeconds(2);
        questionImage.color = baseColor;
    }

    private IEnumerator confirmCorrect(Vocabel voc)
    {
        voc.right();
        questionImage.color = Color.green;
        yield return new WaitForSeconds(2);
        questionCount++;

        if (questionCount >= questionPool.Count)
        {
            prepare();
        }
        else
        {
            setQuestion();
        }
    }

    public void enableHelp()
    {
        helpEnabled = true;
        helpButton.interactable = false;

        answer.gameObject.SetActive(false);
        answerField.gameObject.SetActive(true);
        Vocabel voc = questionPool[questionCount];
        List<String> answerTextList = germanAsked ? voc.english : voc.german;
        string answerText = answerTextList[rnd.Next(0, answerTextList.Count - 1)];
        answerField.setAnswer(answerText);
        optionScript.gameObject.SetActive(true);
        optionScript.setAnswer(answerText);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            checkAnswer();
        }
    }

    public void back()
    {
        VocabelDb.instance.writeToFile();

        SceneManager.LoadScene("StartScene");
    }

    public static void shuffle(List<Vocabel> pile)
    {
        int n = pile.Count;
        System.Random rnd = new System.Random(DateTime.Now.Millisecond);
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            var value = pile[k];
            pile[k] = pile[n];
            pile[n] = value;
        }
    }
    private void OnApplicationQuit()
    {
        VocabelDb.instance.writeToFile();
    }

    public void skip()
    {
        questionCount++;
        if (questionCount >= questionPool.Count)
        {
            prepare();
        }
        else
        {
            setQuestion();
        }
    }
}
