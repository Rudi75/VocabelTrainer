using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class VocabelScript : MonoBehaviour {

    public InputField english;
    public InputField german;
    public InputField level;
    public InputField date;

    private Vocabel voc;

    public void set(Vocabel voc)
    {
        this.voc = voc;
    }

    public void Start()
    {
        english.text = getText(voc.english);
        german.text = getText(voc.german);
        level.text = voc.level.ToString();
        date.text = voc.lastTrainingTime.ToString("dd.MM.yyyy");
        
    }

    private string getText(List<string> english)
    {
        string result = "";
        for (int i = 0; i < english.Count; i++)
        {
            if(i!=0)
            {
                result += "/";
            }
            result += english[i];
        }
        return result;
    }

    public Vocabel get()
    {
        Regex reg = new Regex("^\\s*$");
        if (reg.IsMatch(english.text) && reg.IsMatch(german.text))
        {
            return null;
        }
        return new Vocabel(english.text, german.text, voc.level, voc.lastTrainingTime);
    }
}
