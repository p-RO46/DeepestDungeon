using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using Newtonsoft.Json;

[Serializable]
public class Dialogue
{
    public string speaker;
    public string log;

    public Dialogue(string speaker, string log)
    {
        this.speaker = speaker;
        this.log = log;
    }
}

[Serializable]
public class Conversation
{
    public List<Dialogue> conv = new List<Dialogue>();
}

public class DialogueManager : MonoBehaviour
{
    private PlayerManager playerManager;

    [SerializeField] private string malt_name = "MALT";
    [SerializeField] private string player_name = "BOB";

    [SerializeField] private TextMeshProUGUI text_name;
    [SerializeField] private TextMeshProUGUI text_log;
    [SerializeField] private Image image_speaker;

    [SerializeField] private GameObject speechBox;

    [SerializeField] private TextDB textDB;

    [SerializeField] private float textSpeed = 0.05f;

    private Coroutine printRoutine = null;

    private bool endPrint = false;
    private bool nextPrint = false;

    public void AskMalt(bool isTrap)
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        speechBox.SetActive(true);

        int curFloor = MapManager.Instance.currentFloor;
        List<Dialogue> log = new List<Dialogue>();

        log.Add(new Dialogue("MALT", textDB.malt_start[curFloor].text));

        if (!isTrap)
        {
            log.Add(new Dialogue("MALT", textDB.malt_gift[curFloor].text));
        }
        else
        {
            log.Add(new Dialogue("MALT", textDB.malt_trap[curFloor].text));
        }

        if(printRoutine == null)
        {
            StartCoroutine(PrintText(log));
        }
    }

    public void MaltThirsty()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        speechBox.SetActive(true);

        List<Dialogue> log = new List<Dialogue> { new Dialogue("MALT", textDB.malt_thirst.text) };
        StartCoroutine(PrintText(log));
    }

    public void FloorStartDialogue()
    {
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        speechBox.SetActive(true);

        string json = textDB.floor_start[MapManager.Instance.currentFloor].text;
        Conversation conv = JsonConvert.DeserializeObject<Conversation>(json);

        if (printRoutine == null)
        {
            StartCoroutine(PrintText(conv.conv));
        }
    }

    public IEnumerator PrintText(List<Dialogue> log)
    {
        endPrint = false;
        nextPrint = false;

        // 1. 한 글자씩 출력
        // 2. 키 누를 시 전부 출력. 
        // 3. 한번 더 누를 시 다음으로 넘어감.
        foreach (Dialogue dlg in log)
        {
            image_speaker.sprite = textDB.name_sprite[dlg.speaker];
            text_name.text = dlg.speaker;

            string logText = "";

            for (int i = 0; i < dlg.log.Length; i++)
            {
                logText += dlg.log[i];
                text_log.text = logText;
                yield return new WaitForSeconds(textSpeed);

                if (endPrint)
                {
                    text_log.text = dlg.log;
                    break;
                }
            }

            endPrint = true;
            while (!nextPrint)
            {
                yield return null;
            }

            endPrint = false;
            nextPrint = false;
        }

        speechBox.SetActive(false);
        playerManager.isBusy = false;
        printRoutine = null;
    }

    public void SkipPrint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (endPrint) nextPrint = true;
            else endPrint = true;
        }
    }
}
