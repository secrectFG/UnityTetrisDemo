using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    public int height = 20;
    public int width = 10;

    public float spacing = 50;

    public Image imagePrototype;

    public Image[,] images;

    public float downSpeedNormalInterval = 1;
    public float downSpeedFastInterval = 0.3f;

    float timestamp = 0;

    

    private float curInterval;

    Block curBlock;//当前方块

    void Start()
    {
        curInterval = downSpeedNormalInterval;
        timestamp = Time.time;
        Random.InitState(DateTime.Now.Millisecond);
        images = new Image[width,height];
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                var image = images[w, h] = Instantiate(imagePrototype,imagePrototype.transform.parent);
                image.transform.localPosition = imagePrototype.transform.localPosition + new Vector3(spacing*w,-spacing*h);
            }
        }
        Destroy(imagePrototype.gameObject);
        genBlock();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.KeypadEnter))
        //{
        //    OnFallDownStep();
        //}
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnDownImmediately();
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnLetf();
        }
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnDownKeyDown();
        }
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
        {
            OnDownKeyUp();
            //OnFallDownStep();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnRight();
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnChange();
        }
        if (Time.time - timestamp > curInterval)
        {
            timestamp = Time.time;
            OnFallDownStep();
            //print("fdsfs");
        }
    }

    public void DoneBlockDown()
    {
        resetCurBlock();
        genBlock();
    }

    void resetCurBlock()
    {
        curBlock = null;
    }

    //函数重载
    public bool HasBlock(PosData posData)
    {
        return HasBlock(posData.x,posData.y);
    }

    //这个格子是否有方块
    public bool HasBlock(int x, int y)
    {
        if (x < 0 || x >= width) return true;

        //正常情况下，y<0是不可能发生的，所以不去做判断，做了判断会导致方块刚出现的时候没法移动
        if (y < 0) return false;
        if (y >= height) return true; 

        return images[x, y].color == Color.red;
    }

    private void OnFallDownStep()
    {
        curBlock.MoveDown();
    }



    //生成方块
    void genBlock()
    {

        switch (Random.Range(1, 3))//Unity的随机数不会生成到3，注意看说明
        {
            case 1:
                curBlock = new ConvexBlock(this);//生成凸性方块
                break;
            case 2:
                curBlock = new LongBlock(this);//生成长条方块
                break;
        }
        curBlock.Draw();
    }

    void setColorByData(PosData[] posDatas, Color color)
    {
        for (int i = 0; i < posDatas.Length; i++)//下标从1开始
        {
            var data = posDatas[i];
            if (data.y >= height || data.y < 0) continue;//防止Y越界，不用防止X，如果X越界说明操作错误
            var image = images[data.x, data.y];
            image.color = color;
        }
    }

    public void DrawBlockData(PosData[] posDatas, PosData[] lastPosDatas, bool red = false)
    {
        setColorByData(lastPosDatas, Color.white);
        setColorByData(posDatas, red?Color.red:Color.yellow);
    }

    

    private void OnChange()
    {
        curBlock.Change();
    }

    private void OnRight()
    {
        curBlock.MoveRight();
    }

    private void OnLetf()
    {
        curBlock.MoveLeft();
    }

    private void OnDownKeyDown()
    {
        curInterval = downSpeedFastInterval;
    }

    private void OnDownKeyUp()
    {
        curInterval = downSpeedNormalInterval;
    }
    

    private void OnDownImmediately()
    {
        curBlock.DownImmediately();
    }
}
