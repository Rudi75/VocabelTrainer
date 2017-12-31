using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class ManagerScript : MonoBehaviour
{

    public GameObject vocabelPrefab;
    public Transform contentPanel;
    // Use this for initialization
    void Start()
    {
        prepare();
    }

    private void prepare()
    {
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Vocabel vocabel in VocabelDb.instance.vocabels)
        {
            GameObject go = inst();

            go.GetComponent<VocabelScript>().set(vocabel);
        }
    }

    public void back()
    {
        save();

        SceneManager.LoadScene("StartScene");
    }

    private void save()
    {
        VocabelDb.instance.vocabels.Clear();
        var scripts = contentPanel.GetComponentsInChildren<VocabelScript>();
        foreach (VocabelScript voc in scripts)
        {
            Vocabel vocabel = voc.get();
            if (vocabel != null)
            {
                VocabelDb.instance.vocabels.Add(vocabel);
            }
        }
        VocabelDb.instance.writeToFile();
    }

    public void addVocabel()
    {
        GameObject go = inst();
        go.GetComponent<VocabelScript>().set(new Vocabel("", ""));
        go.GetComponentInChildren<InputField>().ActivateInputField();
    }

    private GameObject inst()
    {
        GameObject go = Instantiate(vocabelPrefab, contentPanel) as GameObject;
        go.transform.localScale = new Vector3(1, 1, 1);
        return go;
    }

    public void reset()
    {
        prepare();
    }
}
