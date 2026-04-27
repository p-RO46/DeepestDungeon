using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirstManager : MonoBehaviour
{
    [SerializeField] private Transform _thirstPoint;
    public GameData gameData;

    private int maxCount = 3;
    private List<Image> thirstPoint = new List<Image>();
    private int currentCount = 0;

    private void Start()
    {
        int count = gameData.totalThirst;
        currentCount = count;
        foreach(Transform child in _thirstPoint)
        {
            Image image = child.GetComponent<Image>();
            thirstPoint.Add(image);
        }

        for(int i = maxCount - 1; i >= currentCount; i--)
        {
            thirstPoint[i].enabled = false;
        }
    }

    public bool AddPoint()
    {
        if (currentCount >= maxCount)
        {
            Debug.Log("Point가 가득 차서 충전할 수 없습니다!");
            return false;
        }

        thirstPoint[currentCount].enabled = true;
        currentCount++;
        gameData.totalThirst = currentCount;
        return true;
    }

    public bool UsePoint()
    {
        if(currentCount <= 0)
        {
            Debug.Log("Point가 없어서 사용할 수 없습니다!");
            return false;
        }

        currentCount--;
        gameData.totalThirst = currentCount;
        thirstPoint[currentCount].enabled = false;
        return true;
    }
}
