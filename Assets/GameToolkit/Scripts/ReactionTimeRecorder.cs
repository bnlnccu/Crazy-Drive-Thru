using UnityEngine;
using System.Collections.Generic;

public class ReactionTimeRecorder : MonoBehaviour
{
    private List<float> records = new List<float>();
    private float timingStartTime;
    private bool isTiming;

    public int RecordCount => records.Count;

    public void StartTiming()
    {
        timingStartTime = Time.time;
        isTiming = true;
    }

    public float StopTimingMs()
    {
        if (!isTiming) return 0f;
        isTiming = false;
        float ms = (Time.time - timingStartTime) * 1000f;
        records.Add(ms);
        return ms;
    }

    public void RecordTime(float ms)
    {
        records.Add(ms);
    }

    public float GetAverageMs()
    {
        if (records.Count == 0) return 0f;
        float total = 0f;
        foreach (float rt in records)
            total += rt;
        return total / records.Count;
    }

    public List<float> GetAllRecords()
    {
        return new List<float>(records);
    }

    public void ClearRecords()
    {
        records.Clear();
        isTiming = false;
    }
}
