using System.Collections.Generic;
using System.Text;
using Unity.Profiling;
using Unity.Profiling.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.XR;

public class ProfilerController : MonoBehaviour
{
    string statsText;
    ProfilerRecorder systemMemoryRecorder;
    ProfilerRecorder gcMemoryRecorder;
    ProfilerRecorder mainThreadTimeRecorder;

    [SerializeField]
    private BenchmarkFlythrough _flythrough;
    private int _recordings = 0;
    private float _meanFrameTime = 0;

    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;

            var samples = new List<ProfilerRecorderSample>(samplesCount);
            recorder.CopyTo(samples);
            for (var i = 0; i < samples.Count; ++i)
                r += samples[i].Value;
            r /= samplesCount;


        return r;
    }

    void OnEnable()
    {
        systemMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        gcMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 15);

        _flythrough.RecordAverage += Record;
        _flythrough.OnComplete += () => Debug.Log("Mean Frame Time: " + GetMeanFrameTime()+ " ms");

        //ProfilerRecorder.StartNew(recorderDescription.Category, recorderDescription.Name,
        //    recorderCapacity, ProfilerRecorderOptions.GpuRecorder | ProfilerRecorderOptions.SumAllSamplesInFrame);
        //Debug.Log(Recorder.gpuElapsedNanoseconds);
    }

    void OnDisable()
    {
        systemMemoryRecorder.Dispose();
        gcMemoryRecorder.Dispose();
        mainThreadTimeRecorder.Dispose();

        _flythrough.RecordAverage -= Record;
        _flythrough.OnComplete -= () => Debug.Log(GetMeanFrameTime());
    }

    void Update()
    {
        var sb = new StringBuilder(500);
        sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f):F1} ms");
        sb.AppendLine($"GC Memory: {gcMemoryRecorder.LastValue / (1024 * 1024)} MB");
        sb.AppendLine($"System Memory: {systemMemoryRecorder.LastValue / (1024 * 1024)} MB");
        statsText = sb.ToString();

        float renderTime;
        if (XRStats.TryGetGPUTimeLastFrame(out renderTime))
            Debug.Log(renderTime);
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 30, 250, 50), statsText);
    }

    void Record()
    {
        _meanFrameTime += (float)GetRecorderFrameAverage(mainThreadTimeRecorder) * (1e-6f);
        _recordings++;
    }

    float GetMeanFrameTime()
    {
        return (_meanFrameTime / _recordings);
    }
}
