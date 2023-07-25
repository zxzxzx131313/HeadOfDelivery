using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiceFaceCode { Left, Right, Above, Bottom, Opposite, Top }

public class Diceface
{
    public int Index = 0;
    public bool IsColored = false;
    public bool IsUp = false;
    public Diceface opposite = null;
}
public class HeadDice : MonoBehaviour
{
    private Diceface[] Dicefaces = new Diceface[6];
    public Diceface top_face { get; private set; }
    public Diceface above_face { get; private set; }
    public Diceface down_face { get; private set; }
    public Diceface left_face { get; private set; }
    public Diceface right_face { get; private set; }

    public LevelStats stats;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            Dicefaces[i] = new Diceface();
            Dicefaces[i].Index = i;
        }
        SetInitialState(0);
    }

    public void SetInitialState(int level)
    {
        top_face = Dicefaces[0];
        above_face = Dicefaces[4];
        down_face = Dicefaces[5];
        left_face = Dicefaces[3];
        right_face = Dicefaces[1];
        top_face.IsColored = true;
        top_face.IsUp = true;

        // set opposite faces;
        SetOpposite(Dicefaces[0], Dicefaces[2]);
        SetOpposite(Dicefaces[1], Dicefaces[3]);
        SetOpposite(Dicefaces[4], Dicefaces[5]);

        ColorDiceTo(stats.LevelBeginColorFacePosition(level));
    }

    public void Restart()
    {
        for (int i = 0; i < 6; i++)
        {
            Dicefaces[i].IsUp = false;
            Dicefaces[i].IsColored = false;
        }

        top_face = Dicefaces[0];
        above_face = Dicefaces[4];
        down_face = Dicefaces[5];
        left_face = Dicefaces[3];
        right_face = Dicefaces[1];
        top_face.IsColored = true;
        top_face.IsUp = true;

        ColorDiceTo(stats.LevelBeginColorFacePosition(stats.Level));
    }

    void SetOpposite(Diceface d1, Diceface d2)
    {
        d1.opposite = d2;
        d2.opposite = d1;
    }

    void ColorDiceTo(DiceFaceCode face)
    {
        if (face == DiceFaceCode.Above)
            TurnUp();
        if (face == DiceFaceCode.Right)
            TurnRight();
        if (face == DiceFaceCode.Left)
            TurnLeft();
        if (face == DiceFaceCode.Bottom)
            TurnDown();
        if (face == DiceFaceCode.Opposite)
        {
            TurnUp();
            TurnUp();
        }
            
    }


    public Diceface GetFaceByPosition(DiceFaceCode index)
    {
        // Up = 0; Right = 1; Down = 2; Left = 3;
        switch (index)
        {
            case DiceFaceCode.Above:
                return above_face;
            case DiceFaceCode.Right:
                return right_face;
            case DiceFaceCode.Bottom:
                return down_face;
            case DiceFaceCode.Left:
                return left_face;
            default:
                return null;
        }
    }

    public void HandleTurn(Vector2 input)
    {
        if (input.x != 0)
        {
            if(input.x > 0)
            {
                TurnRight();
            }
            else
            {
                TurnLeft();
            }
        }
        else
        {
            if (input.y > 0)
            {
                TurnUp();
            }
            else
            {
                TurnDown();
            }
        }
    }

    void TurnLeft()
    {
        top_face.IsUp = false;
        Diceface opp = top_face.opposite;
        left_face = top_face;
        top_face = right_face;
        right_face = opp;
        top_face.IsUp = true;

    }

    void TurnRight()
    {
        top_face.IsUp = false;
        Diceface opp = top_face.opposite;
        right_face = top_face;
        top_face = left_face;
        left_face = opp;
        top_face.IsUp = true;
    }

    void TurnUp()
    {
        top_face.IsUp = false;
        Diceface opp = top_face.opposite;
        above_face = top_face;
        top_face = down_face;
        down_face = opp;
        top_face.IsUp = true;
    }

    void TurnDown()
    {
        top_face.IsUp = false;
        Diceface opp = top_face.opposite;
        down_face = top_face;
        top_face = above_face;
        above_face = opp;
        top_face.IsUp = true;
    }
}
