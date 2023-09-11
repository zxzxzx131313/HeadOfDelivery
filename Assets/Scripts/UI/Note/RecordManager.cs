using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine.InputSystem;


[System.Serializable]
public class AllRecords
{
    public List<RecordEntry> Records = new();
    public int SaveID = 0;
}


public class RecordManager : MonoBehaviour
{
    [SerializeField] private int PanelsCount = 8;
    [SerializeField] private NoteData data;
    [SerializeField] private PreviewCameraSetting NotePreview;
    [SerializeField] private GameEvent OnShowDeletePageHint;
    [SerializeField] private GameEvent OnCheckReplaceRecord;
    [SerializeField] private GameEvent OnCheckSaveResponse;
    [SerializeField] private GameEvent OnCheckSaveResponseEnd;

    DataFolderHelper folder;
    CubeController cubeController;
    AllRecords currentRecords;
    List<PanelManager> Panels;
    int record_pointer = 0;
    bool recording = false;
    bool WaitForPageExistResponse = false;
    bool WaitForSaveRecordResponse = false;
    int PendingDrawPanel = -1;
    NoteUIManager note;
    RecordEntry LastCompleteEntry;
    StepRecorder stepRecorder;

    // Start is called before the first frame update
    void Start()
    {
        stepRecorder = GetComponentInChildren<StepRecorder>();


        folder = GetComponent<DataFolderHelper>();
        Panels = new();
        for (int i = 0; i < PanelsCount; i++)
        {
            AddPanel();
            Panels[i].Deactivate();
            Panels[i].GetButton().Clicked += UpdateRecordPointer;
        }


        string[] files = Directory.GetFiles(folder.GetOrCreateDirFullPath(data.SaveDataDirname));
        if (files.Length > 0)
        {
            int index = 0;
            for (int f = 0; f < files.Length; f++)
            {
                int id = Int32.Parse(Path.GetFileNameWithoutExtension(files[f]));
                if (data.SaveID == id)
                {
                    index = f;
                }
            }

            AllRecords saved = LoadJson(files[index]);
            if (saved != null)
            {
                currentRecords = saved;
            }
            else
            {
                CopyInitialRecords();
            }
            currentRecords.SaveID = index;
        }
        else
        {
            CopyInitialRecords();
        }
        FillPanels();
    }

    private void Awake()
    {
        note = GameObject.FindGameObjectWithTag("Note").GetComponentInParent<NoteUIManager>();
        cubeController = GameObject.FindGameObjectWithTag("Head").GetComponent<CubeController>();

    }

    private void Update()
    {
        if (WaitForPageExistResponse)
        {
            cubeController.enabled = false;
            OnCheckReplaceRecord.Raise();
            WaitForPageExistResponse = false;
        }
        if (WaitForSaveRecordResponse)
        {
            StartCoroutine(checkSaveResponse());
        }
        if (PendingDrawPanel != -1)
        {
            stepRecorder.DrawAllSteps(LastCompleteEntry, Panels[PendingDrawPanel].GetStepContainer());
            Panels[PendingDrawPanel].SetStepDisplay(LastCompleteEntry.Steps.Count - 2);
            UpdateRecordPointer(PendingDrawPanel);
            SaveToJson(data.SaveID);
            PendingDrawPanel = -1;
        }
    }

    IEnumerator checkSaveResponse()
    {
        while (WaitForSaveRecordResponse)
        {
            if (Keyboard.current.anyKey.wasPressedThisFrame)
            {
                if (Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    SaveRecord();
                }
                else
                {
                    RestartCurrentRecord();
                }

                WaitForSaveRecordResponse = false;
                OnCheckSaveResponseEnd.Raise();
            }
            yield return null;
        }
    }

    IEnumerator checkPageExistResponse()
    {
        while (!Keyboard.current.anyKey.wasPressedThisFrame)
        {
            cubeController.enabled = false;
            yield return null;
        }
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            int index = currentRecords.Records.Count;
            for (int i = 0; i < currentRecords.Records.Count; i++)
            {
                if (LastCompleteEntry.Steps.Count <= currentRecords.Records[i].Steps.Count)
                {
                    index = i;
                    break;
                }
            }
            ReplaceRecord(index, LastCompleteEntry);
        }
        else
        {
            RestartCurrentRecord();
        }
        cubeController.enabled = true;
        WaitForPageExistResponse = false;
    }

    public void PageExistReplace(bool isReplacing)
    {
        if (isReplacing)
        {
            int index = currentRecords.Records.Count;
            for (int i = 0; i < currentRecords.Records.Count; i++)
            {
                if (LastCompleteEntry.Steps.Count <= currentRecords.Records[i].Steps.Count)
                {
                    index = i;
                    break;
                }
            }
            ReplaceRecord(index, LastCompleteEntry);
        }
        cubeController.enabled = true;
    }

    private void OnEnable()
    {
        data.SaveChanged += ChangeSave;
        note.OnShowNote += SaveCurrentProgress;
    }

    private void OnDisable()
    {
        data.SaveChanged -= ChangeSave;
        note.OnShowNote -= SaveCurrentProgress;
    }


    void CleanAllPanels()
    {
        foreach (PanelManager panel in Panels)
        {
            Destroy(panel.GetStepContainer());
            Instantiate(data.StepContainer, panel.transform);
        }
    }

    AllRecords GetFileByID(int index)
    {
        return LoadJson(Path.Join(folder.GetOrCreateDirFullPath(data.SaveDataDirname), index.ToString() + ".txt"));
    }


    void CopyInitialRecords()
    {
        currentRecords = new();
        string[] files = Directory.GetFiles(folder.GetLocalData());
        try
        {
            currentRecords = LoadJson(files[0]);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Cannot find preset step data with: " + e);
        }
    }


    void ChangeSave()
    {
        SaveToJson(currentRecords.SaveID);
        currentRecords = GetFileByID(data.SaveID);
        CleanAllPanels();
        FillPanels();
    }


    void UpdateRecordPointer(int index)
    {
        Debug.Log(index);

        record_pointer = index;

        if (!Panels[record_pointer].IsActivated())
        {
            Panels[record_pointer].SetActive();

        }
        else
        {
            Panels[record_pointer].SetToFrontLayer();
        }
        foreach (PanelManager panel in Panels)
        {
            if (panel.Index != index)
            {
                panel.SetToBackLayer();
            }
            panel.UpdateCanvasVisibility();
        }
        UpdatePreviewSize();
    }

    public void AddPanel()
    {

        Panels.Add(Instantiate(data.Panel, transform).GetComponent<PanelManager>());
        Panels[Panels.Count - 1].GetComponent<PanelManager>().SetIndex(Panels.Count - 1);
    }


    void UpdatePreviewSize()
    {
        Vector2 bounds;

        Vector3 scale = Vector3.one;
        if (record_pointer < currentRecords.Records.Count)
        {
            Debug.Log("case2 ");

            bounds = currentRecords.Records[record_pointer].Bounds.max - currentRecords.Records[record_pointer].Bounds.min + new Vector2(48, 48);
            scale = Panels[record_pointer].GetStepContainer().transform.localScale;
        }
        // case when selected a new panel but haven't record anything 
        else
        {
            Debug.Log("case3 ");
            bounds = new Vector2(5000, 5000);
        }

        Vector2Int stepSize = new Vector2Int((int)bounds.x, (int)bounds.y);
        

        NotePreview.SetPreviewZoom(stepSize, record_pointer, scale);
    }


    public void BeginRecord()
    {
        if (currentRecords.Records.Count < PanelsCount)
        { 
            recording = true;
            stepRecorder.BeginRecord();
        }
        else
        {
            OnShowDeletePageHint.Raise();
        }
    }


    public void FillPanels()
    {
        for (int i = 0; i < currentRecords.Records.Count; i++)
        {
            RecordEntry entry = currentRecords.Records[i];
            stepRecorder.DrawAllSteps(entry, Panels[i].GetStepContainer());

            Panels[i].SetActive();
            Panels[i].SetToBackLayer();
            Panels[i].SetStepDisplay(entry.Steps.Count - 2);
        }
        record_pointer = currentRecords.Records.Count - 1;

        if (record_pointer < 0)
        {
            Panels[0].SetActive();
            record_pointer = 0;
        }
        else
        {
            record_pointer = Mathf.Min(PanelsCount - 1, record_pointer + 1);
            Panels[record_pointer].SetActive();
        }
        UpdatePreviewSize();
    }

    public void RestartCurrentRecord()
    {

        stepRecorder.RestartRecord();
    }

    public void StopRecord()
    {
        if (recording)
        {
            recording = false;

            stepRecorder.StopRecord();
        }
    }

    public void DeleteCurrentRecord()
    {
        DeleteRecord(record_pointer);
    }

    void CallCheckSaveResponse()
    {
        WaitForSaveRecordResponse = true;
    }

    public void EndRecord()
    {
        if (currentRecords.Records.Count < PanelsCount)
        {
            recording = false;
            Debug.Log("end record: " + record_pointer);
            RecordEntry entry = new();
            stepRecorder.EndRecord(ref entry);

            LastCompleteEntry = entry;
            // for update in recorder to finish 
            Invoke("CallCheckSaveResponse", 0.1f);

            OnCheckSaveResponse.Raise();
        }
    }

    void SaveRecord()
    {
        Debug.Log("save record");
        if (currentRecords.Records.Count == 0)
        {
            AddRecord();
            UpdateRecordPointer(0);

        }
        else
        {
            int index = currentRecords.Records.Count;
            for (int i = 0; i < currentRecords.Records.Count; i++)
            {
                if (LastCompleteEntry.Steps.Count <= currentRecords.Records[i].Steps.Count)
                {
                    index = i;
                    if (LastCompleteEntry.Steps.Count == currentRecords.Records[i].Steps.Count)
                    {
                        WaitForPageExistResponse = true;
                    }
                    else
                    {
                        InsertRecord(index, LastCompleteEntry);
                    }
                    return;
                }
            }
            AddRecord();

            UpdateRecordPointer(index);
        }
    }

    void ReplaceRecord(int index, RecordEntry entry)
    {
        currentRecords.Records[index] = entry;

        Panels[index].GetStepContainer().SetActive(false);
        Panels[index].AddStepContainer();
        PendingDrawPanel = index;


    }

    void AddRecord()
    {
        currentRecords.Records.Add(LastCompleteEntry);

        stepRecorder.DrawAllSteps(LastCompleteEntry, Panels[currentRecords.Records.Count - 1].GetStepContainer());

        Panels[currentRecords.Records.Count - 1].SetStepDisplay(LastCompleteEntry.Steps.Count - 2);
        UpdateRecordPointer(currentRecords.Records.Count-1);
        SaveToJson(data.SaveID);
    }


    void InsertRecord(int index, RecordEntry entry)
    {
        Panels[currentRecords.Records.Count].GetStepContainer().SetActive(false);
        currentRecords.Records.Insert(index, entry);
        for (int j = currentRecords.Records.Count-1; j > index; j--)
        {
            Panels[j-1].GetStepContainer().transform.SetParent(Panels[j].transform);
            Panels[j].SetStepDisplay(currentRecords.Records[j].Steps.Count - 2);

        }
        Panels[index].SetStepDisplay(currentRecords.Records[index].Steps.Count - 2);

        Panels[index].AddStepContainer();

        Panels[currentRecords.Records.Count - 1].SetActive();
        PendingDrawPanel = index;

    }


    void DeleteRecord(int index)
    {

        Panels[index].GetStepContainer().SetActive(false);
        if (currentRecords.Records.Count > index)
            currentRecords.Records.Remove(currentRecords.Records[index]);
        for (int i = index; i < currentRecords.Records.Count; i++)
        {
            Panels[i+1].GetStepContainer().transform.SetParent(Panels[i].transform);

            Panels[i].SetStepDisplay(currentRecords.Records[i].Steps.Count - 2);
        }
        Panels[currentRecords.Records.Count].SetStepDisplay(-1);
            Panels[currentRecords.Records.Count].Deactivate();
        Panels[currentRecords.Records.Count].AddStepContainer();

        if (currentRecords.Records.Count == 0)
            Panels[0].SetActive();

        UpdateRecordPointer(currentRecords.Records.Count);
        SaveToJson(data.SaveID);
    }


    void SaveCurrentProgress(bool IsOpen)
    {
        // we save every time when the note is closed
        if (!IsOpen)
        {
            SaveToJson(data.SaveID);
        }
    }

    public void SaveToJson(int dataID)
    {
        //check directory exist, check file exist 
        string all = JsonConvert.SerializeObject(currentRecords, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        File.WriteAllText(Path.Join(folder.GetOrCreateDirFullPath(data.SaveDataDirname), dataID + ".txt"), all);
    }

    public AllRecords LoadJson(string filepath)
    {
        Debug.Log(Application.persistentDataPath);

        string str = File.ReadAllText(filepath);

        if (str.Length > 0)
        {
            AllRecords records = JsonConvert.DeserializeObject<AllRecords>(str);
            return records;
        }

        return null;
    }
}