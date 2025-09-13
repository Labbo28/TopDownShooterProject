/*
============================================
    DAY AND NIGHT CYCLE - PARAMETRI DEFAULT E FUNZIONAMENTO
============================================

Questo script gestisce il ciclo giorno-notte tramite la struttura DayAndNightMark.
Ogni "mark" rappresenta un punto nel tempo del ciclo con un colore e un'intensità della luce.

PARAMETRI DEFAULT (come visti nell'Inspector):
------------------------------------------------------------
Element 0:
    Time Ratio = 0      // Inizio ciclo (giorno)
    Color = Bianco      // Colore della luce
    Intensity = 1       // Intensità massima

Element 1:
    Time Ratio = 0.5    // Metà ciclo (notte)
    Color = Blu         // Colore della luce
    Intensity = 0.4     // Intensità minima

Element 2:
    Time Ratio = 0.9    // Fine ciclo (tramonto)
    Color = Blu         // Colore della luce
    Intensity = 0.6     // Intensità intermedia

COME FUNZIONA:
------------------------------------------------------------
1. Il ciclo è definito da _cycleLength (in secondi).
2. Ad ogni frame, lo script calcola la posizione attuale nel ciclo (_currentCycleTime).
3. Viene interpolato (lerp) il colore e l'intensità tra il mark corrente e il successivo.
4. Quando si raggiunge il prossimo mark, la luce viene aggiornata e si passa al mark successivo.
5. Il ciclo è continuo e si ripete all'infinito.

Utilizza la struttura DayAndNightMark[] _marks per definire i punti chiave del ciclo.
Modifica i valori nell'Inspector per personalizzare il ciclo giorno-notte.
*/
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
    private float _marksTimeDifference;

    

    // Start is called before the first frame update
    void Start()
    {
        _nextMarkIndex = -1;
        _CycleMarks();
    }

        // Metodo pubblico per retry
        public void RetryCycle()
        {
            _currentCycleTime = 0f;
            _currentMarkIndex = -1;
            _nextMarkIndex = -1;
            _CycleMarks();
        }

    // Update is called once per frame
    void Update()
    {
        _currentCycleTime = (_currentCycleTime + Time.deltaTime) % _cycleLength;

        // blend color/intensity
        float t = (_currentCycleTime - _currentMarkTime) / _marksTimeDifference;
        DayAndNightMark cur = _marks[_currentMarkIndex], next = _marks[_nextMarkIndex];
        _light.color = Color.Lerp(cur.color, next.color, t);
        _light.intensity = Mathf.Lerp(cur.intensity, next.intensity, t);
        // passed a mark?
        if (Mathf.Abs(_currentCycleTime - _nextMarkTime) < _TIME_CHECK_EPSILON)
        {
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
        _marksTimeDifference = _nextMarkTime - _currentMarkTime;
        if (_marksTimeDifference < 0)
            _marksTimeDifference += _cycleLength;
    }
}
