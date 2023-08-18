using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;


[System.Serializable]
public class RecordEntry
{
    // all spawned steps, <position + arrow type, index of step contains the first spawned game object at this position with this arrow type>, stamped can potentially be changed into offsets if needed
    public List<KeyValuePair<Vector3Int, int>> Steps = new();
    public (Vector2Int start, Vector2Int end) Stamps = (Vector2Int.zero, Vector2Int.zero);
    public (Vector2 min, Vector2 max) Bounds = (new Vector2(5000, 5000), new Vector2(-5000, -5000));
    public Vector2 PanelAnchorPoint = Vector2.zero;
}


[System.Serializable]
public class Step
{
    public int Index = -1;
    public Vector2Int PrefabType = Vector2Int.zero;
    public Vector2Int Position = Vector2Int.zero;
    // if a stamp is made at Position + PrefabType * StepRectSize, ie. after the step is made
    //public bool Stamped = false;
    public GameObject Spawned = null;
}


public class StepRecorder : MonoBehaviour
{
    [SerializeField] private int StepRectSize = 8;
    public NoteData data;
    public GameEvent OnEndRecord;
    public UnityAction OnNewStep;

    CubeController cubeController;
    Camera previewCam;
    RecordEntry currentEntry;
    List<Step> steps;
    Step previous;
    int stampCount;
    Vector2 moveOffset;
    Dictionary<Vector3Int, int> StepsDict;

    private void Awake()
    {
        cubeController = GameObject.FindGameObjectWithTag("Head").GetComponent<CubeController>();
        if (!cubeController) Debug.LogError("Cannot find cube controller.");
        InistializeData();
        previewCam = GameObject.FindGameObjectWithTag("PreviewCamera").GetComponent<Camera>();
    }

    void InistializeData()
    {
        previous = null;
        stampCount = 0;
        moveOffset = Vector2.zero;
        StepsDict = new Dictionary<Vector3Int, int>();
    }

    public Vector3Int EncodeStepKey(Vector2Int position, Vector2Int prefabType)
    {
        int prefab = (prefabType.x, prefabType.y) switch
        {
            (1, 0) => 0,
            (-1, 0) => 1,
            (0, 1) => 2,
            (0, -1) => 3,
            (-1, -1) => 4,
            _ => -1
        };
        return new Vector3Int(position.x, position.y, prefab);
    }

    public (Vector2Int position, Vector2Int prefabType) DecodeStepKey(Vector3Int key)
    {
        (int x, int y) = key.z switch
        {
            0 => (1, 0),
            1 => (-1, 0),
            2 => (0, 1),
            3 => (0, -1),
            4 => (-1, -1),
            _ => (0, 0)
        };
        Vector2Int position = new Vector2Int(key.x, key.y);
        Vector2Int prefab = new Vector2Int(x, y);

        return (position, prefab);
    }


    public RecordEntry GetCurrentEntry()
    {
        return currentEntry;
    }


    public void BeginRecord()
    {
        Debug.Log("record" + Time.time);

        if (!cubeController.IsAttached)
        {
            steps = new List<Step>();
            currentEntry = new();
            // begin recording with a stamp at current position
            if (cubeController.IsOnHeadTile())
            {   
                PrintandAddStampToRecord(Vector2Int.zero);
            }
            cubeController.SendDirection += RecordSteps;
            //return entry;
        }
        else
        {
            Debug.Log("not taking note");
            //return null;
        }

    }

    public void EndRecord(ref RecordEntry entry)
    {
        Debug.Log("endrecord");

        //TODO: add fading, sand transition, earsing kind of effect
        Vector3Int key = EncodeStepKey(currentEntry.Stamps.start, new Vector2Int(-1, -1));
        int stamp_index;
        if (StepsDict.ContainsKey(key))
        {
            stamp_index = StepsDict[key];
        }
        else
        {
            // remove everything if there is no stamp down?
            stamp_index = steps.Count;
        }
        for (int i  = 0; i < steps.Count; i++)
        {
            if (i >= stamp_index) break;
            GameObject go = steps[i].Spawned;
            if (go != null)
            {
                Destroy(go);
                steps[i].Spawned = null;
            }
        }
        if (stamp_index > 0)
            AdjustAnchorOnEnd();


        currentEntry.PanelAnchorPoint = GetComponent<RectTransform>().anchoredPosition;
        cubeController.SendDirection -= RecordSteps;
        currentEntry.Steps = StepsDict.ToList();
        entry = currentEntry;
    }


    void AdjustAnchorOnEnd()
    {
        RectTransform rt = GetComponent<RectTransform>();
        currentEntry.Bounds.min = new Vector2(5000, 5000);
        currentEntry.Bounds.max = new Vector2(-5000,  -5000);
        for (int i = 0; i < steps.Count; i++)
        {
            if (steps[i].Spawned != null)
            {
                currentEntry.Bounds.min = Vector2.Min(currentEntry.Bounds.min, steps[i].Position);
                currentEntry.Bounds.max = Vector2.Max(currentEntry.Bounds.max, steps[i].Position);

            }
        }
        MoveRecordCenter();

    }


    void AutoScale()
    {
        RectTransform rt = GetComponent<RectTransform>();
        //Debug.Log(rt.sizeDelta);
        Vector2 size = currentEntry.Bounds.max - currentEntry.Bounds.min + new Vector2(16, 16);
        float scale = Mathf.Max(size.x / previewCam.pixelWidth, size.y / previewCam.pixelHeight);
        if (scale > 1)
        {
            float down_scale = 1 / scale;
            gameObject.transform.localScale = down_scale * Vector3.one;
        }else 
        {
            gameObject.transform.localScale = Vector3.one;
        }
    }

    void MoveRecordCenter()
    {

        AutoScale();
        RectTransform rt = GetComponent<RectTransform>();
        //Debug.Log(rt.sizeDelta);
        GetComponent<RectTransform>().sizeDelta = currentEntry.Bounds.max - currentEntry.Bounds.min;
        ////rt.sizeDelta = new Vector2(rt.sizeDelta.x,(currentEntry.Bounds.max - currentEntry.Bounds.min).y);

        Vector2 center_offset = (currentEntry.Bounds.max + currentEntry.Bounds.min ) / 2f * transform.localScale;
        center_offset -= moveOffset;
        Debug.Log("offset" + center_offset);
        moveOffset += center_offset;

        if (!center_offset.Equals(Vector2.zero))
        {
            rt.anchoredPosition = rt.anchoredPosition - center_offset;
        }

        //Vector2 origin = rt.anchoredPosition;
        //origin -= arrow * StepRectSize;
        //rt.anchoredPosition = origin;
    }



    Vector2Int GetStampLocationByStep(Step step)
    {
        return step.Position + step.PrefabType * StepRectSize;
    }

    GameObject PrintStamp( Vector2Int pos) 
    {

        Debug.Log("spawn stamp");
        GameObject stamp = Instantiate(data.StampFace, transform);
        stamp.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
        SetAnchor(stamp);
        RectTransform rt = stamp.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        stampCount++;
        if (stampCount == 1) 
            currentEntry.Stamps.start = pos;
        else if (stampCount == 2)
        {
            currentEntry.Stamps.end = pos;
        }
        return stamp;
    }

    GameObject GetPrefab(Vector2Int direction)
    {
        GameObject arrow = (direction.x, direction.y) switch
        {
            (1, 0) => data.RightArrow,
            (-1, 0) => data.LeftArrow,
            (0, 1) => data.UpArrow,
            (0, -1) => data.DownArrow,
            (-1, -1) => data.StampFace,
            _ => null
        };
        return arrow;
    }

    public bool IsInBound(Vector2 position)
    {
        RectTransform rt = GetComponent<RectTransform>();
        return rt.rect.Contains(position);
    }

    public void DrawAllSteps(RecordEntry entry)
    {
        foreach (KeyValuePair<Vector3Int, int> pair in entry.Steps)
        {
            Vector3Int key = pair.Key;
            Vector2Int position, prefabType;
            (position, prefabType) = DecodeStepKey(key);


            GameObject spawned = Instantiate(GetPrefab(prefabType), transform);
            SetAnchor(spawned);
            spawned.GetComponent<RectTransform>().anchoredPosition = position;
            spawned.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.7f);
        }

        GetComponent<RectTransform>().anchoredPosition = entry.PanelAnchorPoint;
    }

    public bool IsEmpty()
    {
        return StepsDict.ContainsKey(EncodeStepKey(currentEntry.Stamps.end, new Vector2Int(-1, -1)));
    }


    public void SetAnchor(GameObject go)
    {
        go.GetComponent<RectTransform>().anchorMin = new Vector2(0.375f, 0.5f);
        go.GetComponent<RectTransform>().anchorMax = new Vector2(0.375f, 0.5f);
        go.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
    }


    public void RecordSteps(Vector2 input, bool stamped)
    {

        if (input != Vector2.zero)
        {
            Step step = new();

            step.PrefabType = Vector2Int.RoundToInt(input);
            GameObject arrow = GetPrefab(step.PrefabType);

            // if can still add step
            step.Index = steps.Count;

            //if (steps.Count > 0)
            //    previous = steps[steps.Count - 1];
            if (step.Index == 0 || (step.Index == 1 && previous == null))
            //if (step.Index == 0)
            {
                step.Position = step.PrefabType * StepRectSize;
            }
            else
            {
                Vector2Int pos = previous.Position + previous.PrefabType * StepRectSize + step.PrefabType * StepRectSize;

                step.Position = pos;
            }

            currentEntry.Bounds.min = Vector2.Min(currentEntry.Bounds.min, step.Position);
            currentEntry.Bounds.max = Vector2.Max(currentEntry.Bounds.max, step.Position);

            Vector3Int stepKey = EncodeStepKey(step.Position, step.PrefabType);
            if (StepsDict.ContainsKey(stepKey))
            {
                step.Spawned = steps[StepsDict[stepKey]].Spawned;
            }
            else
            {
                // discard any steps that happened before the first stamp
                if (stampCount > 0)
                    StepsDict[stepKey] = step.Index;
                GameObject spawned = Instantiate(arrow, transform);
                SetAnchor(spawned);
                spawned.GetComponent<RectTransform>().anchoredPosition = step.Position;
                step.Spawned = spawned;
                OnNewStep?.Invoke();
                MoveRecordCenter();
            }

            // set active arrow color
            step.Spawned.GetComponent<Image>().color = Vector4.one;
            //if (steps.Count > 1 && previous != null) 
            if (steps.Count > 0 && previous != null)
                previous.Spawned.GetComponent<Image>().color = new Vector4(1, 1, 1, 0.7f);

            steps.Add(step);
            if (stamped)
            {
                Vector2Int stamp_pos = GetStampLocationByStep(step);
                PrintandAddStampToRecord(stamp_pos);
                if (stampCount == 2)
                    OnEndRecord.Raise();
            }
                

            previous = step;
        }
    }

    void PrintandAddStampToRecord(Vector2Int stamp_pos)
    {
        Step stamp_step = new Step();
        stamp_step.Index = steps.Count;
        stamp_step.Position = stamp_pos;
        stamp_step.PrefabType = new Vector2Int(-1, -1);
        stamp_step.Spawned = PrintStamp(stamp_pos);
        steps.Add(stamp_step);
        StepsDict[EncodeStepKey(stamp_pos, stamp_step.PrefabType)] = stamp_step.Index;
    }
}
