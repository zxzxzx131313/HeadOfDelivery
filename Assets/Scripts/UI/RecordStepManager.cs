using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class AllRecords
{
    public List<RecordEntry> Records = new();
}

[System.Serializable]
public class RecordEntry
{
    // the list of steps made in order, repeated steps are also added
    public List<Vector2Int> StepPos = new();
    // the map of steps made with the grided position as key
    public Dictionary<Vector2Int, Step> Steps = new();
    public int EntryIndex;
}


[System.Serializable]
public class Step
{
    public int Index = -1;
    public Vector2Int ArrowType;
    public Vector2Int Position = Vector2Int.zero;
    public Vector2Int OffsetPosition = Vector2Int.zero;
    // if there are other steps from also made at this position
    //public bool Returned = false;
    public List<Step> ReturnedStep = new List<Step>();
}


public class RecordStepManager : MonoBehaviour
{
    [SerializeField] private int StepRectSize = 8;
    [SerializeField] private GameObject StampContainer;
    public NoteData data;
    public DataFolder folder;


    CubeController cubeController;
    bool recording = false;
    AllRecords allRecords;
    RecordEntry currentEntry;
    List<GameObject> spawnedArrow;
    int stampCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        StampContainer.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        string[] files = Directory.GetFiles(folder.GetDataPath());
        if (files.Length > 0)
        {
            allRecords = LoadJson(folder.GetDataFileFullPath(files[0]));
            if (allRecords == null) allRecords = new();
        }
        else allRecords = new();
    }

    private void Awake()
    {
        cubeController = GameObject.FindGameObjectWithTag("Head").GetComponent<CubeController>();
        if (!cubeController) Debug.LogError("Cannot find cube controller.");
    }


    private void OnEnable()
    {
        cubeController.TileSpawned += OnPrintStamp;
        cubeController.SendDirection += RecordSteps;
    }

    private void OnDisable()
    {
        cubeController.TileSpawned -= OnPrintStamp;
        cubeController.SendDirection -= RecordSteps;
    }

    public void BeginRecord()
    {
        Debug.Log("record" + Time.time);
        if (recording)
        {
            EndRecord();
            return;
        }
        if (!cubeController.IsAttached)
        {
            recording = true;
            // TODO: can select, delete, change records
            RecordEntry entry = new();
            entry.EntryIndex = allRecords.Records.Count;
            allRecords.Records.Add(entry);
            currentEntry = entry;
            spawnedArrow = new List<GameObject>();
            cubeController.TileSpawned += OnPrintStamp;

            if (cubeController.IsOnHeadTile())
            {
                GameObject stamp = Instantiate(data.StampFace, StampContainer.transform);
                stampCount++;
            }
        }
        else
        {
            Debug.Log("not taking note");
        }

    }

    void EndRecord()
    {
        Debug.Log("endrecord");
        cubeController.TileSpawned -= OnPrintStamp;
        recording = false;
        stampCount = 0;

        SaveToJson(folder.GetDataFileFullPath(Time.time.ToString() + ".txt"), allRecords);
    }

    void OnPrintStamp() 
    {
        if (!recording) return;

        Debug.Log("spawn stamp");
        Vector2Int pos = currentEntry.StepPos.Count - 1 > 0 ?
            currentEntry.StepPos[currentEntry.StepPos.Count - 1]
            : Vector2Int.zero;
        Step step = currentEntry.Steps[pos];
        //if (step.Returned) pos -= step.OffsetPosition;
        pos += step.ArrowType * StepRectSize;
        GameObject stamp = Instantiate(data.StampFace, StampContainer.transform);
        RectTransform rt = stamp.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        stampCount++;
        if (stampCount == 2)
        {
            EndRecord();
        }
    }

    /*
     * When recording, each key press will 1. generate a step object 2. compute its position based on facing and previous step 3. add position to list while add (position, step object) to map
     */
    public void RecordSteps(Vector2 input)
    {
        Debug.Log(input);
        if (recording && input != Vector2.zero)
        {


            Step step = new();
            step.ArrowType = Vector2Int.RoundToInt(input);
            GameObject arrow = (input.x, input.y) switch
            {
                (1, 0) => data.RightArrow,
                (-1, 0) => data.LeftArrow,
                (0, 1) => data.UpArrow,
                (0, -1) => data.DownArrow
            };

            // if can still add step
            step.Index = currentEntry.StepPos.Count;

            if (step.Index == 0)
            {
                step.Position = step.ArrowType * StepRectSize;
                currentEntry.StepPos.Add(step.Position);
                currentEntry.Steps[step.Position] = step;
                GameObject spawned = Instantiate(arrow, transform);
                spawned.GetComponent<RectTransform>().anchoredPosition = step.Position;
                spawnedArrow.Add(spawned);
            }
            else
            {
                Vector2Int previous_pos = currentEntry.StepPos[step.Index - 1];
                Vector2Int previous_arrow = currentEntry.Steps[previous_pos].ArrowType;
                List<Step> steps = currentEntry.Steps[previous_pos].ReturnedStep;
                if (steps.Count > 0)
                {
                    previous_arrow = steps[steps.Count-1].ArrowType;
                }
                Vector2Int pos = previous_pos + previous_arrow * StepRectSize + step.ArrowType * StepRectSize;
                // if the position already has a arrow, we move it to the side by a bit and insert in the newer one

                step.Position = pos;
                currentEntry.StepPos.Add(step.Position);

                if (currentEntry.Steps.ContainsKey(pos))
                {
                    Step returnFromStep = currentEntry.Steps[pos];

                    Step s = returnFromStep.ReturnedStep.Find(o => o.ArrowType == step.ArrowType);
                    returnFromStep.ReturnedStep.Add(step);
                    if (s != null)
                    {
                        spawnedArrow.Add(spawnedArrow[s.Index]);
                        goto SetColor;
                    }
                    if (returnFromStep.ArrowType == step.ArrowType)
                    {

                        spawnedArrow.Add(spawnedArrow[returnFromStep.Index]);
                        goto SetColor;
                    }
                    
                    // first one in this direction at Position

                    Vector2Int y_offset = Vector2Int.up * StepRectSize / 2;
                    Vector2Int x_offset = Vector2Int.right * StepRectSize / 2;
                    int qudrant = pos.y >= 0 ? 1 : -1;
                    // newer steps added to the top/right side of existing step if above y=0
                    returnFromStep.OffsetPosition = step.ArrowType.x != 0 ? -qudrant * y_offset : -qudrant * x_offset;
                    step.OffsetPosition = step.ArrowType.x != 0 ? qudrant * y_offset : qudrant * x_offset;

                    //returnFromStep.Position += returnFromStep.OffsetPosition;
                    //step.Position += step.OffsetPosition;


                    //currentEntry.StepPos[returnFromStep.Index] = returnFromStep.Position;
                    returnFromStep.ReturnedStep.Add(step);

                    spawnedArrow[returnFromStep.Index].GetComponent<RectTransform>().anchoredPosition = returnFromStep.Position += returnFromStep.OffsetPosition;
                    GameObject spawned = Instantiate(arrow, transform);
                    spawned.GetComponent<RectTransform>().anchoredPosition = step.Position +step.OffsetPosition;
                    spawnedArrow.Add(spawned);
                }
                else
                {
                    currentEntry.Steps[step.Position] = step;

                    GameObject spawned = Instantiate(arrow, transform);
                    spawned.GetComponent<RectTransform>().anchoredPosition = pos;
                    spawnedArrow.Add(spawned);
                }
            }
            SetColor:
            GameObject curr_arrow = spawnedArrow[spawnedArrow.Count - 1];
            curr_arrow.GetComponent<Image>().color = Vector4.one;
            if (spawnedArrow.Count > 1) 
                spawnedArrow[spawnedArrow.Count-2].GetComponent<Image>().color = new Vector4(1, 1, 1, 0.5f);
        }
    }

    public void SaveToJson(string filepath, AllRecords records)
    {
        //check directory exist, check file exist 
        string all = JsonUtility.ToJson(records);
        System.IO.File.WriteAllText(filepath, all);
    }

    public AllRecords LoadJson(string filename)
    {
        Debug.Log(Application.persistentDataPath);

        string str = System.IO.File.ReadAllText(folder.GetDataFileFullPath(filename));

        if (str.Length > 0)
        {
            AllRecords records = JsonUtility.FromJson<AllRecords>(str);
            return records;
        }
        
        return null;
    }
}
