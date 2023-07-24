using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Random = UnityEngine.Random;
public struct PosData
{
    public int x;
    public int y;
    public override string ToString()
    {
        return "" + x + " " + y;
    }
}

//抽象类
abstract class Block
{
    protected PosData[] blockData;//方块数据
    protected int curBlockState = 0;//方块状态

    protected Game game;

    public Block(Game game)
    {
        this.game = game;
    }

    public void MoveDown()
    {
        move(0, 1);
    }

    public void MoveLeft()
    {
        move(-1, 0);
    }

    public void MoveRight()
    {
        move(1, 0);
    }

    public void DownImmediately()
    {
        while (move(0, 1)) ;
    }

    public void Draw()
    {
        game.DrawBlockData(blockData, blockData);
    }

    protected bool tryMoveOneIndex(int index, int dx, int dy)
    {
        var data = blockData[index];
        if (game.HasBlock(data.x + dx, data.y + dy))
        {
            return false;//有方块挡住，不能旋转
        }
        blockData[index].x += dx;
        blockData[index].y += dy;
        return true;
    }

    bool move(int dx, int dy)
    {
        bool canmove = true;
        for (int i = 0; i < blockData.Length; i++)
        {
            var data = blockData[i];
            if (game.HasBlock(data.x + dx, data.y + dy))//检测所有方块是否能往下移动
            {
                canmove = false;//不能移动
                if (dy >= 1)//到底了
                {
                    game.DrawBlockData(blockData, blockData, true);
                    game.DoneBlockDown();
                }
                break;
            }
        }
        if (canmove)
        {
            var last = blockData.Clone() as PosData[];//为什么要克隆？涉及语言核心语法——引用
            for (int i = 0; i < blockData.Length; i++)
            {
                blockData[i].x += dx;
                blockData[i].y += dy;
            }
            game.DrawBlockData(blockData, last);
        }
        return canmove;
    }

    //虚函数
    public abstract void Change();
}

class ConvexBlock : Block
{
    
    public ConvexBlock(Game game) : base(game) {
        PosData[] pos = new PosData[4];
        pos[0] = new PosData() { x = 5, y = 0 };//规定下标为0的数据是方块的基准位置
        pos[1] = new PosData() { x = -1, y = 0 };//左
        pos[2] = new PosData() { x = 1, y = 0 };//右
        pos[3] = new PosData() { x = 0, y = -1 };//上
        for (int i = 1; i < pos.Length; i++)
        {
            pos[i].x += pos[0].x;
            pos[i].y += pos[0].y;
        }
        blockData = pos;
    }

    public override void Change()
    {
        var last = blockData.Clone() as PosData[];

        switch (curBlockState)
        {
            case 0://默认状态
                //  0      0
                // 000 -> 00 
                //         0
                //思路：移动【右】到【下】即可，注意：此后右一变成下面
                if (tryMoveOneIndex(2, -1, 1))
                {
                    curBlockState = 1;
                }
                break;

            case 1:
                //  0      
                // 00  -> 000 
                //  0      0
                //移动【上】到【右】
                if (tryMoveOneIndex(3, 1, 1))
                {
                    curBlockState = 2;
                }
                break;

            case 2:
                //          0
                // 000  ->  00 
                //  0       0
                //移动【左】到【上】
                if (tryMoveOneIndex(1, 1, -1))
                {
                    curBlockState = 3;
                }
                break;

            case 3:
                //复原
                var copydata = blockData.Clone() as PosData[];//为什么要克隆？涉及语言核心语法——引用
                copydata[1] = new PosData() { x = -1, y = 0 };//左
                copydata[2] = new PosData() { x = 1, y = 0 };//右
                copydata[3] = new PosData() { x = 0, y = -1 };//上
                bool canmove = true;
                for (int i = 1; i < copydata.Length; i++)
                {
                    copydata[i].x += copydata[0].x;
                    copydata[i].y += copydata[0].y;
                    if (game.HasBlock(copydata[i].x, copydata[i].y))
                    {
                        canmove = false;
                        break;
                    }
                }
                if (canmove)
                {
                    blockData = copydata;
                    curBlockState = 0;
                }
                break;
        }
        game.DrawBlockData(blockData, last);
    }
}

class LongBlock : Block
{
    public LongBlock(Game game) : base(game)
    {
        PosData[] poses = new PosData[4];
        poses[0] = new PosData() { x = 5, y = 0 };//规定下标为0的数据是方块的基准位置
        poses[1] = new PosData() { x = -1, y = 0 };
        poses[2] = new PosData() { x = 1, y = 0 };
        poses[3] = new PosData() { x = 2, y = 0 };
        for (int i = 1; i < poses.Length; i++)
        {
            poses[i].x += poses[0].x;
            poses[i].y += poses[0].y;
        }
        blockData = poses;

    }

    public override void Change()
    {
        var last = blockData.Clone() as PosData[];
        var poses = last;
        switch (curBlockState)
        {
            case 0://默认状态
                poses[1] = new PosData() { x = 0, y = -1 };
                poses[2] = new PosData() { x = 0, y = 1 };
                poses[3] = new PosData() { x = 0, y = 2 };
                for (int i = 1; i < poses.Length; i++)
                {
                    poses[i].x += poses[0].x;
                    poses[i].y += poses[0].y;
                }
                foreach (var pos in poses)//这种遍历方法也要会
                {
                    if (game.HasBlock(pos))
                    {
                        return;
                    }
                }
                last = blockData;
                blockData = poses;
                curBlockState = 1;
                break;

            case 1:
                poses[1] = new PosData() { x = -1, y = 0 };
                poses[2] = new PosData() { x = 1, y = 0 };
                poses[3] = new PosData() { x = 2, y = 0 };
                for (int i = 1; i < poses.Length; i++)
                {
                    poses[i].x += poses[0].x;
                    poses[i].y += poses[0].y;
                }
                foreach (var pos in poses)//这种遍历方法也要会
                {
                    if (game.HasBlock(pos))
                    {
                        return;
                    }
                }
                last = blockData;
                blockData = poses;
                curBlockState = 0;
                break;
        }
        game.DrawBlockData(blockData, last);
    }
}

