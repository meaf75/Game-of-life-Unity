using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;

public class FileManager : MonoBehaviour{
    string mainPath;

    public CellsManager cellsManager;

    private void Start() {
        switch (Application.platform) {

            case RuntimePlatform.Android:
                mainPath = Application.persistentDataPath;
                break;

            case RuntimePlatform.WindowsEditor:
                mainPath = Application.dataPath;
                break;

        }
    }

    public void SaveTest(int exampleId){
        string path =  Path.Combine(mainPath,$"Examples/Example_{exampleId}.json");

        CellPos CellsPos = new CellPos();

        for (int x = 0; x < cellsManager.limit; x++){
            for (int y = 0; y < cellsManager.limit; y++){
                if(cellsManager.AllCells[x,y] != null){
                    CellsPos.Positions.Add(new Vector2(x,y));
                }
            }
        }

        string data = JsonConvert.SerializeObject(CellsPos);

        File.WriteAllText(path,data);

        #if UNITY_EDITOR
            AssetDatabase.Refresh();
        #endif
    }

    public async void LoadExample(int exampleId){
        TextAsset dataFile = (TextAsset) Resources.Load($"Examples/Example_{exampleId}");

        CellPos CellsPos = JsonConvert.DeserializeObject<CellPos>(dataFile.text);

        cellsManager.isPaused = true;
        cellsManager.clearCellContainer();

        foreach (var pos in CellsPos.Positions){
            cellsManager.AddCellAtPosition(pos);
        }
    }

    [SerializeField]
    public class CellPos{
        public List<Vector2> Positions = new List<Vector2>();
    }

}
