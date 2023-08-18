//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.IO;


//[System.Serializable]
//public class RecordEntry
//{
//    // the list of steps made in order, repeated steps are also added
//    public List<Vector2Int> StepPos = new();
//    // the map of steps made with the grided position as key
//    public Dictionary<Vector2Int, Step> Steps = new();
//    public (Vector2 min, Vector2 max) Bounds = (Vector2.zero, Vector2.zero);
//}


//[System.Serializable]
//public class Step
//{
//    public int Index = -1;
//    public Vector2Int ArrowType = Vector2Int.zero;
//    public Vector2Int Position = Vector2Int.zero;
//    public Vector2Int OffsetPosition = Vector2Int.zero;
//    // if there are other steps from also made at this position
//    //public bool Returned = false;
//    public List<Step> ReturnedStep = new List<Step>();
//    // if a stamp is made at Position + ArrowType * StepRectSize, ie. after the step is made
//    public bool Stamped = false;
//}


//public class StepRecorder : MonoBehaviour
//{
//    [SerializeField] private int StepRectSize = 8;
//    public NoteData data;
//    //public DataFolderHelper folder;
//    public GameEvent OnEndRecord;

//    CubeController cubeController;
//    bool recording = false;
//    // AllRecords currentRecords;
//    RecordEntry currentEntry;
//    List<GameObject> spawnedArrow;
//    int stampCount = 0;

//    private void Awake()
//    {
//        cubeController = GameObject.FindGameObjectWithTag("Head").GetComponent<CubeController>();
//        if (!cubeController) Debug.LogError("Cannot find cube controller.");
//    }


//    private void OnEnable()
//    {
//        cubeController.SendDirection += RecordSteps;
//    }

//    private void OnDisable()
//    {
//        cubeController.SendDirection -= RecordSteps;
//    }

//    public RecordEntry BeginRecord()
//    {
//        Debug.Log("record" + Time.time);

//        if (!cubeController.IsAttached)
//        {
//            RecordEntry entry = new RecordEntry();
//            currentEntry = entry;
//            spawnedArrow = new List<GameObject>();

//            // begin recording with a stamp at current position
//            if (cubeController.IsOnHeadTile())
//            {
//                PrintStamp(Vector2.zero);
//                Step step = new();
//                step.Stamped = true;
//                currentEntry.Steps[Vector2Int.zero] = step;
//            }
//            return entry;
//        }
//        else
//        {
//            Debug.Log("not taking note");
//            return null;
//        }

//    }

//    void EndRecord()
//    {
//        Debug.Log("endrecord");   

//        if (spawnedArrow.Count > 0)
//            AdjustFinalPosition();
        
//        OnEndRecord.Raise();

//    }


//    //public void RestartEntry()
//    //{
//    //    currentEntry = new RecordEntry();
//    //    foreach (GameObject arrow in spawnedArrow)
//    //    {
//    //        Destroy(arrow);
//    //    }

//    //    foreach (GameObject stamp in stamps)
//    //    {
//    //        Destroy(stamp);
//    //    }
//    //    spawnedArrow = new();
//    //    stamps = new();
//    //}

//    void MoveRecordCenter(Vector2Int arrow)
//    {
//        RectTransform rt = GetComponent<RectTransform>();
//        //Debug.Log(rt.sizeDelta);
//        rt.sizeDelta = currentEntry.Bounds.max - currentEntry.Bounds.min;
//        ////rt.sizeDelta = new Vector2(rt.sizeDelta.x,(currentEntry.Bounds.max - currentEntry.Bounds.min).y);
//        //Debug.Log("fter" + rt.sizeDelta);

//        //Vector2 center_offset = (currentEntry.Bounds.max + currentEntry.Bounds.min) / 2f;
//        //if (!center_offset.x.Equals(0))
//        //{
//        //    rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - center_offset.x, rt.anchoredPosition.y);
//        //}

//        Vector2 origin = rt.anchoredPosition;
//        origin -= arrow * StepRectSize;
//        rt.anchoredPosition = origin;
//    }

//    void AdjustFinalPosition()
//    {
//        RectTransform rt = GetComponent<RectTransform>();

//        Vector2 center_offset = (currentEntry.Bounds.max + currentEntry.Bounds.min) / 2f;
//        if (!center_offset.x.Equals(0))
//        {
//            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - center_offset.x, rt.anchoredPosition.y);
//        }
//    }

//    public RecordEntry GetCurrentEntry()
//    {
//        return currentEntry;
//    }

//    Vector2 GetStampLocationByStep(Step step)
//    {
//        return step.Position + step.ArrowType * StepRectSize;
//    }

//    void PrintStamp( Vector2 pos) 
//    {

//        Debug.Log("spawn stamp");
//        //Vector2Int pos = currentEntry.StepPos.Count - 1 > 0 ?
//        //    currentEntry.StepPos[currentEntry.StepPos.Count - 1]
//        //    : Vector2Int.zero;
//        //Step step = currentEntry.Steps[pos];

//        //pos += step.ArrowType * StepRectSize;
//        GameObject stamp = Instantiate(data.StampFace, transform);
//        RectTransform rt = stamp.GetComponent<RectTransform>();
//        rt.anchoredPosition = pos;
//        stampCount++;
//        if (stampCount == 2)
//        {
//            EndRecord();
//        }
//    }

//    GameObject GetArrowPrefab(Vector2Int direction)
//    {
//        GameObject arrow = (direction.x, direction.y) switch
//        {
//            (1, 0) => data.RightArrow,
//            (-1, 0) => data.LeftArrow,
//            (0, 1) => data.UpArrow,
//            (0, -1) => data.DownArrow,
//            _ => null
//        };
//        return arrow;
//    }

//    public bool IsInBound(Vector2 position)
//    {
//        RectTransform rt = GetComponent<RectTransform>();
//        return rt.rect.Contains(position);
//    }

//    public void DrawAllSteps(RecordEntry entry)
//    {
//        foreach (Step s in entry.Steps.Values)
//        {
//            SpawnArrowAt(s.ArrowType, transform, s.Position + s.OffsetPosition);

//            if (s.Stamped)
//            {
//                GameObject go = Instantiate(data.StampFace, transform);
//                go.GetComponent<RectTransform>().anchoredPosition = GetStampLocationByStep(s);
//            }

//            foreach (Step follow in s.ReturnedStep)
//            {
//                SpawnArrowAt(follow.ArrowType, transform, follow.Position + follow.OffsetPosition);

//                if (follow.Stamped)
//                {
//                    GameObject go = Instantiate(data.StampFace, transform);
//                    go.GetComponent<RectTransform>().anchoredPosition = GetStampLocationByStep(follow);
//                }
//            }

//        }
//    }


//    void SpawnArrowAt(Vector2Int arrowType, Transform container, Vector2 position)
//    {
//        GameObject arrow = GetArrowPrefab(arrowType);
//        GameObject go = Instantiate(arrow, container);
//        go.GetComponent<RectTransform>().anchoredPosition = position;
//    }


//    /*
//     * When recording, each key press will 1. generate a step object 2. compute its position based on facing and previous step 3. add position to list while add (position, step object) to map
//     */
//    public void RecordSteps(Vector2 input, bool stamped)
//    {
//        bool new_arrow = true;

//        if (recording && input != Vector2.zero)
//        {

//            Step step = new();

//            step.ArrowType = Vector2Int.RoundToInt(input);
//            GameObject arrow = GetArrowPrefab(step.ArrowType);

//            // if can still add step
//            step.Index = currentEntry.StepPos.Count;

//            if (step.Index == 0)
//            {
//                step.Position = step.ArrowType * StepRectSize;
//                currentEntry.StepPos.Add(step.Position);
//                currentEntry.Steps[step.Position] = step;
//                GameObject spawned = Instantiate(arrow, transform);
//                spawned.GetComponent<RectTransform>().anchoredPosition = step.Position;
//                spawnedArrow.Add(spawned);
//            }
//            else
//            {
//                Vector2Int previous_pos = currentEntry.StepPos[step.Index - 1];
//                Vector2Int previous_arrow = currentEntry.Steps[previous_pos].ArrowType;
//                List<Step> steps = currentEntry.Steps[previous_pos].ReturnedStep;
//                if (steps.Count > 0)
//                {
//                    previous_arrow = steps[steps.Count-1].ArrowType;
//                }
//                Vector2Int pos = previous_pos + previous_arrow * StepRectSize + step.ArrowType * StepRectSize;
//                // if the position already has a arrow, we move it to the side by a bit and insert in the newer one

//                step.Position = pos;
//                currentEntry.StepPos.Add(step.Position);

//                if (!IsInBound(pos))
//                {
//                    currentEntry.Bounds.min = Vector2.Min(currentEntry.Bounds.min, pos);
//                    currentEntry.Bounds.max = Vector2.Max(currentEntry.Bounds.max, pos);
//                }

//                if (currentEntry.Steps.ContainsKey(pos))
//                {
//                    Step returnFromStep = currentEntry.Steps[pos];

//                    Step s = returnFromStep.ReturnedStep.Find(o => o.ArrowType == step.ArrowType);
//                    returnFromStep.ReturnedStep.Add(step);
//                    if (s != null)
//                    {
//                        new_arrow = false;
//                        spawnedArrow.Add(spawnedArrow[s.Index]);
//                        goto SetColor;
//                    }
//                    if (returnFromStep.ArrowType == step.ArrowType)
//                    {
//                        new_arrow = false;
//                        spawnedArrow.Add(spawnedArrow[returnFromStep.Index]);
//                        goto SetColor;
//                    }
                    
//                    // first one in this direction at Position
//                    Vector2Int y_offset = Vector2Int.up * StepRectSize / 2;
//                    Vector2Int x_offset = Vector2Int.right * StepRectSize / 2;
//                    int qudrant = pos.y >= 0 ? 1 : -1;
//                    // newer steps added to the top/right side of existing step if above y=0
//                    returnFromStep.OffsetPosition = step.ArrowType.x != 0 ? -qudrant * y_offset : -qudrant * x_offset;
//                    step.OffsetPosition = step.ArrowType.x != 0 ? qudrant * y_offset : qudrant * x_offset;

//                    returnFromStep.ReturnedStep.Add(step);

//                    spawnedArrow[returnFromStep.Index].GetComponent<RectTransform>().anchoredPosition = returnFromStep.Position += returnFromStep.OffsetPosition;
//                    GameObject spawned = Instantiate(arrow, transform);
//                    spawned.GetComponent<RectTransform>().anchoredPosition = step.Position +step.OffsetPosition;
//                    spawnedArrow.Add(spawned);
//                }
//                else
//                {
//                    currentEntry.Steps[step.Position] = step;

//                    GameObject spawned = Instantiate(arrow, transform);
//                    spawned.GetComponent<RectTransform>().anchoredPosition = pos;
//                    spawnedArrow.Add(spawned);
//                }
//            }
//            SetColor:
//            GameObject curr_arrow = spawnedArrow[spawnedArrow.Count - 1];
//            curr_arrow.GetComponent<Image>().color = Vector4.one;
//            if (spawnedArrow.Count > 1) 
//                spawnedArrow[spawnedArrow.Count-2].GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);

//            if (stamped)
//            {
//                step.Stamped = true;
//                PrintStamp(GetStampLocationByStep(step));
//            }

//            if(new_arrow) MoveRecordCenter(step.ArrowType);
//        }
//    }
//}
