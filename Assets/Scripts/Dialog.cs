using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;


public class Dialog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI textComponent;

    public List<string[]> AllDialog = new List<string[]>();
    public string[] lines;

    public float textSpeed;

    private int index;

    public static int dialogNum = 0;
    void Start()
    {
        AllDialog.Add(lines);
        textComponent.text = string.Empty;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if(textComponent.text == AllDialog[dialogNum][index])
            {
                //Debug.Log("Hi");
                NextLine();
            }else{
                            
                StopAllCoroutines();
                textComponent.text = AllDialog[dialogNum][index];
                Debug.Log(AllDialog[dialogNum][index]);
            }
        }
    }

    public void StartDialog(int num)
    {
        index = 0;
        gameObject.SetActive(true);
        dialogNum = num;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in AllDialog[dialogNum][index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }
    
    void NextLine()
    {
        if (index < AllDialog[dialogNum].Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        } else
        {
            gameObject.SetActive(false);
        }
    }
}
