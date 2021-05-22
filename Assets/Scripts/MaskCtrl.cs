﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskCtrl : MonoBehaviour
{
    #region 欄位
    public GameObject viewMask;
    public float[] MaskLevel;
    float targetScal;
    float currectScal;
    float MaskchangePercentage;
    float maskChangePercentageBase;
    int MaskLevelCount;
    #endregion


    private void Awake()
    {
        targetScal = 1;
        currectScal = 0;
        MaskchangePercentage = 0.03f;
        maskChangePercentageBase = 0.003f;
        MaskLevel = new float[5] { 1,2,3,5,7};
        MaskLevelCount = 0;
    }
    void Update()
    {

        MaskLevelupOrDown();
        targetScal = MaskLevel[MaskLevelCount];
        MaskchangePercentage = PlayerCtrl.PlayerIsRun ?  (maskChangePercentageBase*5) : (maskChangePercentageBase);
        if (Input.GetKey(KeyCode.UpArrow)|| Input.GetKey(KeyCode.DownArrow)|| Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.LeftArrow))
        {
            currectScal = MaskChangeFromAtoB(currectScal, targetScal,MaskchangePercentage);
        }
        else
        {
            currectScal = MaskChangeFromAtoB(currectScal, 0,MaskchangePercentage);
        }
        viewMask.transform.localScale = new Vector3(currectScal,currectScal,currectScal);          
    }
   float MaskChangeFromAtoB(float a,float b ,float changePercentage) {
        if (a > b * 0.99 && b!=0)   {return b;}
        if (b==0 && a<0.1)          {return 0;}

        return a + (b - a) * changePercentage;
    }

    void MaskLevelupOrDown() {
        if (Input.GetKeyDown(KeyCode.A) && MaskLevelCount < MaskLevel.Length - 1)
        {
            Debug.Log("MaskLevelUP");
            MaskLevelCount++;
        }
        if (Input.GetKeyDown(KeyCode.S) && MaskLevelCount > 1)
        {
            Debug.Log("MaskLevelDown");
            MaskLevelCount--;
        }
    }
}






