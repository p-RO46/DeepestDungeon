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

    [SerializeField] private List<AudioClip> keyboards = new List<AudioClip>();

    [SerializeField] private string malt_name = "MALT";
    [SerializeField] private string player_name = "BOB";

    [SerializeField] private TextMeshProUGUI text_name;
    [SerializeField] private TextMeshProUGUI text_log;
    [SerializeField] private Image image_speaker;

    [SerializeField] private GameObject speechBox;

    [SerializeField] private GameData gameData;
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
            if(MapManager.Instance.currentFloor >= 3 && UnityEngine.Random.Range(0, 3) == 0)
            {
                log.Add(new Dialogue("MALT", textDB.malt_trap[curFloor].text));
            }
            else
            {
                log.Add(new Dialogue("MALT", textDB.malt_gift[curFloor].text));
            }
        }
        else
        {
            if (MapManager.Instance.currentFloor >= 3 && UnityEngine.Random.Range(0, 3) == 0)
            {
                log.Add(new Dialogue("MALT", textDB.malt_gift[curFloor].text));
            }
            else
            {
                log.Add(new Dialogue("MALT", textDB.malt_trap[curFloor].text));
            }
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

    public IEnumerator EndingRoutine()
    {
        if (gameData == null) yield break;

        speechBox.SetActive(true);

        string json = "";
        if (gameData.isDead)
        {
            json = textDB.deadEnd.text;
        }
        else if (gameData.totalScore < 2500)
        {
            json = textDB.ending[0].text;
        }
        else if (gameData.totalScore < 6500)
        {
            json = textDB.ending[1].text;
        }
        else
        {
            json = textDB.ending[2].text;
        }
        Conversation conv = JsonConvert.DeserializeObject<Conversation>(json);
        yield return PrintText(conv.conv);
    }

    public IEnumerator StartRoutine()
    {
        if (gameData == null) yield break;

        speechBox.SetActive(true);

        string json = "";
        json = textDB.starting.text;
        Conversation conv = JsonConvert.DeserializeObject<Conversation>(json);
        yield return PrintText(conv.conv);
    }


    public IEnumerator PrintText(List<Dialogue> log)
    {
        endPrint = false;
        nextPrint = false;

        AudioSource audioSource = GameObject.Find("SE Player").GetComponent<AudioSource>();

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
                audioSource.PlayOneShot(keyboards[UnityEngine.Random.Range(0, 2)]);
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
        if(playerManager != null) playerManager.isBusy = false;
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
