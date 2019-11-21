using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Limits : MonoBehaviour {

    public Transform Horizontal1;
    public Transform Horizontal2;
    public Transform Vertical1;
    public Transform Vertical2;

    public void SetLimits(int limits) {
        Vertical1.position = new Vector2 (-1, limits / 2);
        Vertical2.position = new Vector2 (limits + 1, limits / 2);

        Horizontal1.position = new Vector2 (limits / 2, -1);
        Horizontal2.position = new Vector2 (limits / 2, limits + 1);

        Horizontal1.localScale = Horizontal2.localScale = Vertical1.localScale = Vertical2.localScale = new Vector2 (1, limits);

        float posCam = (limits/2);

        Camera.main.transform.position = new Vector3(posCam,posCam,-5);
    }
}