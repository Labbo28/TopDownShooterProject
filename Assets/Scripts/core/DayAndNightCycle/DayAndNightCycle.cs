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

    [SerializeField] private DayAndNightMark[] _marks;
    [SerializeField] private Light2D _light;
    [SerializeField] private float _cycleLength = 24; // in seconds

    private const float _TIME_CHECK_EPSILON = 0.1f;
    private float _currentCycleTime;
    private int _currentMarkIndex, _nextMarkIndex;
    private float _currentMarkTime, _nextMarkTime;

    // Start is called before the first frame update
    void Start()
    {
        _nextMarkIndex = -1;
        _CycleMarks();
    }

    // Update is called once per frame
    void Update()
    {
        _currentCycleTime = (_currentCycleTime + Time.deltaTime) % _cycleLength;
        // passed a mark?
        if (Mathf.Abs(_currentCycleTime - _nextMarkTime) < _TIME_CHECK_EPSILON)
        {
            DayAndNightMark next = _marks[_nextMarkIndex];
            _light.color = next.color;
            _light.intensity = next.intensity;
            _CycleMarks();
        }
    }


    private void _CycleMarks()
    {
        _currentMarkIndex = (_currentMarkIndex + 1) % _marks.Length;
        _nextMarkIndex = (_currentMarkIndex + 1) % _marks.Length;
        _currentMarkTime = _marks[_currentMarkIndex].timeRatio * _cycleLength;
        _nextMarkTime = _marks[_nextMarkIndex].timeRatio * _cycleLength;
    }
}
