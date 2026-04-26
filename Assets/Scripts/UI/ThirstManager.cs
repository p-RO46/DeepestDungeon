using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirstManager : MonoBehaviour
{
    [SerializeField] private Transform _thirstPoint;

    private int maxCount;
    private List<Image> thirstPoint = new List<Image>();
    private int currentCount = 0;

    private void Start()
    {
        foreach(Transform child in _thirstPoint)
        {
            Image image = child.GetComponent<Image>();

            thirstPoint.Add(image);
            currentCount++;
        }

        maxCount = currentCount;
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
        thirstPoint[currentCount].enabled = false;
        return true;
    }
}
