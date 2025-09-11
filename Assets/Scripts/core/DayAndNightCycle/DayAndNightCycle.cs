using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Rendering.Universal;

public class DayAndNightCycle : MonoBehaviour
{
    [System.Serializable]
    public struct DayAndNightMark
    {
        public float timeRatio;
        public Color color;
        public float intensity;
    }

    [SerializeField] private DayAndNightMark[] marks;
    [SerializeField] private Light2D light2D;
    [SerializeField] private float cycleLength = 24; // in seconds

    private const float timeCheckEpsilon = 0.1f;
    private float currentCycleTime;
    private int currentMarkIndex, nextMarkIndex;
    private float currentMarkTime, nextMarkTime;
    private float marksTimeDifference;

    void Start()
    {
        nextMarkIndex = -1;
        CycleMarks();
    }

    public void RetryCycle()
    {
        currentCycleTime = 0f;
        currentMarkIndex = -1;
        nextMarkIndex = -1; 
        CycleMarks();
    }

    void Update()
    {
        currentCycleTime = (currentCycleTime + Time.deltaTime) % cycleLength;

        float t = (currentCycleTime - currentMarkTime) / marksTimeDifference;
        DayAndNightMark cur = marks[currentMarkIndex], next = marks[nextMarkIndex];
        light2D.color = Color.Lerp(cur.color, next.color, t);
        light2D.intensity = Mathf.Lerp(cur.intensity, next.intensity, t);

        if (Mathf.Abs(currentCycleTime - nextMarkTime) < timeCheckEpsilon)
        {
            light2D.color = next.color;
            light2D.intensity = next.intensity;
            CycleMarks();
        }
    }

    private void CycleMarks()
    {
        currentMarkIndex = (currentMarkIndex + 1) % marks.Length;
        nextMarkIndex = (currentMarkIndex + 1) % marks.Length;
        currentMarkTime = marks[currentMarkIndex].timeRatio * cycleLength;
        nextMarkTime = marks[nextMarkIndex].timeRatio * cycleLength;
        marksTimeDifference = nextMarkTime - currentMarkTime;
        if (marksTimeDifference < 0)
            marksTimeDifference += cycleLength;
    }
}
