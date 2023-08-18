using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;


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

    DataFolderHelper folder;
    AllRecords currentRecords;
    List<PanelManager> Panels;
    int record_pointer = 0;
    bool recording = false;
    NoteUIManager note;

    // Start is called before the first frame update
    void Start()
    {

        folder = GetComponent<DataFolderHelper>();
        Panels = new();
        for (int i = 0; i < PanelsCount; i++)
        {
            AddPanel();
            Panels[i].Deactivate();
            Panels[i].GetButton().Clicked += UpdateRecordPointer;
            Panels[i].GetStepRecorder().OnNewStep += UpdatePreviewSize;
        }
        
        string[] files = Directory.GetFiles(folder.GetOrCreateDirFullPath(data.SaveDataDirname));
        if (files.Length > 0)
        {
            int index = 0;
            for(int f = 0; f < files.Length; f++)
            {
                int id = Int32.Parse(Path.GetFileNameWithoutExtension(files[f]));
                if(data.SaveID == id)
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
            Destroy(panel.GetStepRecorder().gameObject);
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
        }catch(Exception e)
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

            //if (record_pointer + 1 < Panels.Count )
            //{
            //    Panels[record_pointer + 1].WaitForActivation();
            //}
        }
        else
        {
            //if (record_pointer + 1 < Panels.Count)
            //{
            //    Panels[record_pointer + 1].SetToBackLayer();
            //}
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
        Panels[Panels.Count-1].GetComponent<PanelManager>().SetIndex(Panels.Count - 1);
    }


    StepRecorder GetStepRecorderFromPanel(int index)
    {
        return Panels[index].transform.GetComponentInChildren<StepRecorder>();
    }


    void UpdatePreviewSize()
    {
        Vector2 bounds;
        // case when currently or already recorded in this turn of game
        if (GetStepRecorderFromPanel(record_pointer).GetCurrentEntry() != null)
        {
            Debug.Log("case1 ");
            bounds = GetStepRecorderFromPanel(record_pointer).GetCurrentEntry().Bounds.max -
                GetStepRecorderFromPanel(record_pointer).GetCurrentEntry().Bounds.min + new Vector2(16, 16);
        }
        // case when loading from save
        else if (record_pointer < currentRecords.Records.Count)
        {
            Debug.Log("case2 ");

            bounds = currentRecords.Records[record_pointer].Bounds.max - currentRecords.Records[record_pointer].Bounds.min + new Vector2(16, 16);
        }
        // case when selected a new panel but haven't record anything 
        else
        {
            Debug.Log("case3 ");
            bounds = new Vector2(5000, 5000);
        }

        Vector2Int stepSize = new Vector2Int((int)bounds.x, (int)bounds.y);

        NotePreview.SetPreviewZoom(stepSize, record_pointer, GetStepRecorderFromPanel(record_pointer).gameObject.transform.localScale);
            
        
    }


    public void BeginRecord()
    {
        if (recording)
        {
            EndRecord();
        }
        else
        {
            recording = true;
            Debug.Log("record: " + record_pointer);


            Panels[record_pointer].GetStepRecorder().OnNewStep += UpdatePreviewSize;
            GetStepRecorderFromPanel(record_pointer).BeginRecord();
        }
    }


    public void FillPanels()
    {
        for(int i = 0; i < currentRecords.Records.Count; i++)
        {
            RecordEntry entry = currentRecords.Records[i];
            GetStepRecorderFromPanel(i).DrawAllSteps(entry);
            Panels[i].SetActive();
            Panels[i].SetToBackLayer();
        }
        record_pointer = currentRecords.Records.Count - 1;
        //if (record_pointer >= 0)
        //{
        //    if (record_pointer + 1 < Panels.Count)
        //    {
        //        Panels[record_pointer + 1].SetActive();
        //    }
        //}
        //else
        //{
        //    Panels[0].SetActive();
        //}
        if (record_pointer < 0)
        {
            Panels[0].SetActive();
            record_pointer = 0;
        }
        else
        {
            record_pointer = Mathf.Min(Panels.Count - 1, record_pointer+1);
            Panels[record_pointer].SetActive();
        }
        UpdatePreviewSize();
    }

    public void RestartCurrentRecord()
    {
        // dont delete current record if its a complete one
        if (recording)
        {
            recording = false;
            RecordEntry entry = new();
            GetStepRecorderFromPanel(record_pointer).EndRecord(ref entry);
            Panels[record_pointer].GetStepRecorder().OnNewStep -= UpdatePreviewSize;

            Destroy(Panels[record_pointer].GetStepRecorder().gameObject);
            Instantiate(data.StepContainer, Panels[record_pointer].transform);
            //BeginRecord();

            Invoke("BeginRecord", 0.5f);
        }
    }

    public void DeleteCurrentRecord()
    {
        DeleteRecord(record_pointer);
    }


    public void EndRecord()
    {
        recording = false;
        Debug.Log("end record: " + record_pointer); 
        RecordEntry entry = new();
        GetStepRecorderFromPanel(record_pointer).EndRecord(ref entry);
        Panels[record_pointer].GetStepRecorder().OnNewStep -= UpdatePreviewSize;
        currentRecords.Records.Add(entry);
        UpdatePreviewSize();
        if (currentRecords.Records[record_pointer].Stamps.end.Equals(Vector2Int.zero))
        {
            currentRecords.Records.Remove(currentRecords.Records[record_pointer]);
            //record_pointer++;
            if (currentRecords.Records.Count > 0)
                Panels[record_pointer].WaitForActivation();
        }
        else
        {
            SaveToJson(data.SaveID);
            if (record_pointer + 1 < Panels.Count)
            {

                Panels[record_pointer+1].WaitForActivation();
                //Panels[record_pointer+1].UpdateCanvasVisibility();
            }
        }
    }

    void DeleteRecord(int index)
    {
        if (recording)
        {
            recording = false;
            RecordEntry entry = new();
            GetStepRecorderFromPanel(index).EndRecord(ref entry);
            Panels[index].GetStepRecorder().OnNewStep -= UpdatePreviewSize;
        }

        Destroy(Panels[index].GetStepRecorder().gameObject);
        for (int i = index; i < currentRecords.Records.Count - 1; i++)
        {
            Panels[i + 1].GetStepRecorder().gameObject.transform.SetParent(Panels[i].transform);
        }
        //Panels[currentRecords.Records.Count - 1].WaitForActivation();
        Instantiate(data.StepContainer, Panels[Mathf.Max(index, currentRecords.Records.Count - 1)].transform);
        if (currentRecords.Records.Count > 0 && currentRecords.Records.Count < Panels.Count)
        {
            Panels[currentRecords.Records.Count].Deactivate();
        }

        Panels[ Mathf.Max(index, currentRecords.Records.Count-1)].WaitForActivation();
        if (currentRecords.Records.Count > index) 
            currentRecords.Records.Remove(currentRecords.Records[index]);
        if (currentRecords.Records.Count == 0)
            Panels[0].SetActive();
        //if (currentRecords.Records.Count < Panels.Count)
        //{
        //    //Destroy(Panels[currentRecords.Records.Count - 1].GetStepRecorder().gameObject);
        //    Instantiate(data.StepContainer, Panels[currentRecords.Records.Count - 1].transform);
        //    Panels[currentRecords.Records.Count - 1].WaitForActivation();
        //}
        SaveToJson(data.SaveID);
        
    }


    bool IsPanelEmpty(int index)
    {
        Debug.Log(index + " recordcount " + currentRecords.Records.Count + " entr" + Panels[index].GetStepRecorder().GetCurrentEntry());
        return index >= currentRecords.Records.Count && Panels[index].GetStepRecorder().GetCurrentEntry() == null;
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
        File.WriteAllText(Path.Join( folder.GetOrCreateDirFullPath(data.SaveDataDirname), dataID + ".txt"), all);
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
