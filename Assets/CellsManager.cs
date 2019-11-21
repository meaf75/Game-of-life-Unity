using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyEnums;
using System;
using TMPro;
public class CellsManager : MonoBehaviour {

    [Header("GameOptions")]
    public bool isPaused;
    public bool StopPerFrame;

    [Header("Other info")]
    public float refreshTime;
    public GameObject cellPrefab;
    public int limit;

    [Header("Cells references")]
    public GameObject[,] AllCells;

    [Header("Debug Pos")]
    public Vector2 posDebug;

    [Header("Other references")]
    public Limits limits;
    
    [Header("UI references")]
    public TextMeshProUGUI txtBtnPause;
    public TextMeshProUGUI txtGeneration;
    public TMP_InputField inputDelay;

    [Header("Other")]
    public string ExamplesFolder;

    public int generation;

    private void Start() {
        AllCells = new GameObject[limit,limit];
        GenerateCells();
        inputDelay.text = refreshTime.ToString();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space)){
            isPaused = !isPaused;
        }
    }

    public void clearCellContainer(){
        generation = 0;
        foreach (Transform item in transform){
            Destroy(item.gameObject);
        }        
    }

    void GenerateCells(){

        clearCellContainer();

        limits.SetLimits(limit);

        StartCoroutine(CheckCells());
    }

    IEnumerator CheckCells(){
        List<Vector2> CellsToKeepAlive = new List<Vector2>();
        List<GameObject> CellsToDead = new List<GameObject>();

        setPauseText();
        txtGeneration.text = $"Generation: {generation}";

        // Check if game is paused and stop to check cells
        if(isPaused){
            Debug.Log("Paused");
            yield return null;
            StartCoroutine(CheckCells()); // Check again
            yield break;
        }        

        for (int x = 0; x < limit; x++){
            for (int y = 0; y < limit; y++){

                Vector2 pos = new Vector2(x,y);

                GameObject cell = GetCellAtPos(pos);

                bool live = KeepCellAlive(pos);

                if(cell != null){                    
                    if(live){
                        CellsToKeepAlive.Add(pos);
                    }else{
                        CellsToDead.Add(cell);
                    }                        
                }else if(live){
                    CellsToKeepAlive.Add(pos);
                }
            }
        }
        
        // Check all referenced cell to keep alive if these are dead
        foreach (var pos in CellsToKeepAlive){
            AddCellAtPosition(pos);
        }

        // Check all referenced cell to kill alive if these are alive
        foreach (var cell in CellsToDead){
            Destroy(cell);
        }

        yield return new WaitForSeconds(refreshTime);

        if(StopPerFrame){
            isPaused = true;
        }

        generation++;
        StartCoroutine(CheckCells()); // Check again
    }

    bool KeepCellAlive(Vector2 pos){

        GameObject cell = GetCellAtPos(pos);
        int neighbours = NeighboursAlive(pos);

        if(cell != null){
            // Check if should be dead
            if(neighbours <= 1 || neighbours >= 4){
                return false;
            }else{
                return true;
            }
        }else{
            // Check if can revive
            if(neighbours == 3){
                return true;
            }else{
                return false;
            }
        }
    }

    public void RunStep(){
        isPaused = false;
        StopPerFrame = true;
    }

    public void ActionOnPoint(Vector2 pos){

        isPaused = true;

        int realLimit = limit - 1;

        if(pos.x > realLimit || pos.y > realLimit){
            return;
        }

        // Check if in pos exist a cell and is alive
        GameObject cell = GetCellAtPos(pos);

        if(cell == null){
            AddCellAtPosition(pos);
        }else{
            Destroy(cell);
        }
    }

    GameObject GetCellAtPos(Vector2 pos){
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        try{
            return AllCells[x,y];            
        }catch (System.Exception e){
            Debug.Log("******: "+pos + "xx: "+e);
            return null;
        }

    }

    int NeighboursAlive(Vector2 pos){

        int xI = Mathf.RoundToInt(pos.x);
        int yI = Mathf.RoundToInt(pos.y);

        int Neighbours = 0;

        // Check asked for position for alive Neighbours around
        for (int x = xI - 1; x < xI + 2; x++){
            for (int y = yI - 1; y < yI + 2; y++){

                Vector2 newPos = new Vector2(x,y);

                // Avoid "IndexOutOfRangeExceptionx"
                if(x < 0 || y < 0 || x == limit || y == limit){
                    continue;
                }

                if(newPos != pos){
                    if(GetCellAtPos(newPos) != null){
                        Neighbours++;
                    }
                }
            }
        }
        
        return Neighbours;
    }

    /// <summary> Add cell to position </summary>
    public void AddCellAtPosition(Vector2 pos){
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        if(AllCells[x,y] == null){
            AllCells[x,y] = Instantiate(cellPrefab,pos,Quaternion.identity,transform);
        }
    }

    public void setDelay(){
        if(inputDelay.text.Trim().Length == 0){
            return;
        }

        float.TryParse(inputDelay.text,out refreshTime);
        StopAllCoroutines();
        StartCoroutine(CheckCells());
    }

    public void Pause(){
        isPaused = !isPaused;

        if(!isPaused){
            StopPerFrame = false;
        }

        setPauseText();
    }

    void setPauseText(){
        txtBtnPause.text = isPaused ? "Play" : "Pause";
    }
}
