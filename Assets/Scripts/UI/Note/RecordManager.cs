using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Newtonsoft.Json;

[System.Serializable]
public class AllRecords
{
    public List<RecordEntry> Records = new();
}


public class RecordManager : MonoBehaviour
{
    [SerializeField] private int PanelsCount = 8;
    [SerializeField] private NoteData data;
    [SerializeField] private PreviewCameraSetting NotePreview;

    DataFolderHelper folder;
    AllRecords allRecords;
    List<PanelManager> Panels;
    int record_pointer = 0;
    bool recording = false;
    Vector3 selectedRecordSize = Vector3.one;

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
            allRecords = LoadJson(files[0]);
            if (allRecords == null) allRecords = new();
        }
        else allRecords = new();
        LoadRecords();
    }


    void UpdateRecordPointer(int index)
    {
        Debug.Log(index);

        record_pointer = index;

        if (!Panels[record_pointer].IsActivated())
        {

            Panels[record_pointer].SetActive();

            if (record_pointer + 1 < Panels.Count )
            {
                Panels[record_pointer + 1].WaitForActivation();
            }
        }
        else
        {
            if (record_pointer + 1 < Panels.Count)
            {
                Panels[record_pointer + 1].SetToBackLayer();
            }
            Panels[record_pointer].SetToFrontLayer();
        }
        foreach (PanelManager panel in Panels)
        {
            if (panel.Index != index && panel.Index != index+1)
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
        else if (record_pointer < allRecords.Records.Count)
        {
            Debug.Log("case2 ");

            bounds = allRecords.Records[record_pointer].Bounds.max - allRecords.Records[record_pointer].Bounds.min + new Vector2(16, 16);
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


    public void LoadRecords()
    {
        for(int i = 0; i < allRecords.Records.Count; i++)
        {
            RecordEntry entry = allRecords.Records[i];
            GetStepRecorderFromPanel(i).DrawAllSteps(entry);
            Panels[i].SetActive();
            Panels[i].SetToBackLayer();
        }
        record_pointer = Mathf.Max(0, allRecords.Records.Count - 1);
        if (record_pointer + 1 < Panels.Count)
        {
            Panels[record_pointer].SetActive();
            Panels[record_pointer + 1].WaitForActivation();
        }
        UpdatePreviewSize();
    }

    public void RestartCurrentRecord()
    {
        if (recording)
        {
            recording = false;
            RecordEntry entry = new();
            GetStepRecorderFromPanel(record_pointer).EndRecord(ref entry);
            Panels[record_pointer].GetStepRecorder().OnNewStep -= UpdatePreviewSize;
        }

        Destroy(Panels[record_pointer].GetStepRecorder().gameObject);
        Instantiate(data.StepContainer, Panels[record_pointer].transform);
        //BeginRecord();
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
        allRecords.Records.Add(entry);
        UpdatePreviewSize();
        if (allRecords.Records[record_pointer].Stamps.end.Equals(Vector2Int.zero))
        {
            allRecords.Records.Remove(allRecords.Records[record_pointer]);
            //record_pointer++;
        }
        else
        {
            SaveToJson();

        }
    }

    void DeleteRecord(int index)
    {
        if (!IsPanelEmpty(index))
        {
            if (recording)
            {
                recording = false;
                RecordEntry entry = new();
                GetStepRecorderFromPanel(index).EndRecord(ref entry);
                Panels[index].GetStepRecorder().OnNewStep -= UpdatePreviewSize;
            }

            Destroy(Panels[index].GetStepRecorder().gameObject);
            for (int i = index; i < allRecords.Records.Count - 1; i++)
            {
                Panels[i + 1].GetStepRecorder().gameObject.transform.SetParent(Panels[i].transform);
            }
            //Panels[allRecords.Records.Count - 1].WaitForActivation();
            Instantiate(data.StepContainer, Panels[allRecords.Records.Count - 1].transform);
            if (allRecords.Records.Count < Panels.Count)
            {
                Panels[allRecords.Records.Count].Deactivate();
            }
            allRecords.Records.Remove(allRecords.Records[index]);
            UpdateRecordPointer(record_pointer);
            //if (allRecords.Records.Count < Panels.Count)
            //{
            //    //Destroy(Panels[allRecords.Records.Count - 1].GetStepRecorder().gameObject);
            //    Instantiate(data.StepContainer, Panels[allRecords.Records.Count - 1].transform);
            //    Panels[allRecords.Records.Count - 1].WaitForActivation();
            //}
            SaveToJson();
        }
    }


    bool IsPanelEmpty(int index)
    {
        Debug.Log(index + " recordcount " + allRecords.Records.Count + " entr" + Panels[index].GetStepRecorder().GetCurrentEntry());
        return index >= allRecords.Records.Count && Panels[index].GetStepRecorder().GetCurrentEntry() == null;
    }

    public void SaveToJson()
    {
        //check directory exist, check file exist 
        string all = JsonConvert.SerializeObject(allRecords, Formatting.Indented, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
        File.WriteAllText(Path.Join( folder.GetOrCreateDirFullPath(data.SaveDataDirname), "save.txt"), all);
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
