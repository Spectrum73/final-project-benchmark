using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BenchmarkFlythrough : MonoBehaviour
{
    public List<Transform> TravelPoints = new List<Transform>();
    public float Speed = 5;
    public int RecordingsPerTravelPoint = 10;

    private int _currentPoint = 0;
    private Vector3 _lastPosition;
    private Quaternion _lastRotation;

    private bool _complete =false;
    private float _timer = 0;
    private int _recordings = 0;

    public event Action OnStart;
    // Triggered when the profiler should record a value for the average calculation
    public event Action RecordAverage;
    public event Action OnComplete;

    private void OnEnable()
    {
        transform.position = TravelPoints[0].position;
        transform.rotation = TravelPoints[0].rotation;
        _currentPoint = 1;

        _lastPosition = transform.position;
        _lastRotation = transform.rotation;
        _complete = false;
        OnStart?.Invoke();
    }
    private void Update()
    {
        if (_currentPoint < TravelPoints.Count)
        {
            float targetDistance = Vector3.Distance(transform.position, _lastPosition);
            if (_timer < 1.0f)
            {
                float totalDistance = Vector3.Distance(_lastPosition, TravelPoints[_currentPoint].position);

                _timer += Time.deltaTime * Speed;
                if (_timer > (_recordings / RecordingsPerTravelPoint))
                {
                    _recordings++;
                    RecordAverage?.Invoke();
                }

                transform.position = Vector3.Lerp(_lastPosition, TravelPoints[_currentPoint].position, _timer);
                transform.rotation = Quaternion.Lerp(_lastRotation, TravelPoints[_currentPoint].rotation, _timer);
            }
            else
            {
                // Reached a travel point!
                _lastPosition = TravelPoints[_currentPoint].position;
                _lastRotation = TravelPoints[_currentPoint].rotation;
                _timer = 0;
                _recordings = 0;
                _currentPoint++;
            }
        }
        else if (!_complete)
        {
            _complete = true;
            OnComplete?.Invoke();
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        
    }
}
