using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellsGenerator : MonoBehaviour, IPointerDownHandler{

    public CellsManager cellsManager;

    //Se ha hecho click sobre el lienzo
    public virtual void OnPointerDown ( PointerEventData eventData ) {
        //Tomo la posicion del click/touch con respecto a la camara→Canvas
        Vector2 pos = Camera.main.ScreenToWorldPoint(eventData.position);

        //La posicion nueva del objeto a donde se va a poner
        pos = new Vector2(Mathf.RoundToInt(pos.x),Mathf.RoundToInt(pos.y));

        // Creo o borro el objeto
        cellsManager.ActionOnPoint(pos);
    }
}
