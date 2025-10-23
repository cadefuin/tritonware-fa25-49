using UnityEngine;
using TMPro;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEditor;


public class Dialog : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI textComponent;

    public List<string[]> AllDialog = new List<string[]>();
    public string[] lines;

    public string[] lines1;

    public string[] lines2;

    public string[] lines3;

       public string[] lines4;

    public string[] lines5;

    public string[] lines6;

    public string[] lines7;

    public float textSpeed;

    private int index;

    public static int dialogNum = 0;

    public static int dialogAction = 0;

    public bool dialogPlaying = false;
    void Start()
    {
        AllDialog.Add(lines);
        AllDialog.Add(lines1);
        AllDialog.Add(lines2);
        AllDialog.Add(lines3);
        AllDialog.Add(lines4);
        AllDialog.Add(lines5);
        AllDialog.Add(lines6);
        AllDialog.Add(lines7);
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
        if (!dialogPlaying)
        {
            dialogAction = num;
            dialogPlaying = true;
            GameManager.DialogOn = true;
            textComponent.text = string.Empty;
        index = 0;
        gameObject.SetActive(true);
        dialogNum = num;
        StartCoroutine(TypeLine());
        }
        
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
        }
        else
        {
            ActionAfterDialog(dialogAction);
            dialogPlaying = false;
            GameManager.DialogOn = false;
            gameObject.SetActive(false);
        }
    }
    
    void ActionAfterDialog(int actionNum)
    {
        if (actionNum % 2 == 0)
        {
            GameObject player = GameObject.FindWithTag("Player");
            PlayerController ps = player.GetComponent<PlayerController>();
            ps.HP = 5;
        }
    }
}
