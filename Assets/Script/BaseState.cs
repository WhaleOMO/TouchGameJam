using System;
using System.Collections.Generic;
using UnityEngine;


public enum StateEnum
{
    // 物体状态
    Red,
    Blue,
    Green,
    DeepGreen,
    DeepBlue,
    BlueGreen,
    
    // 玩家状态
    PlayerNone, // 玩家初态
    PlayerRed,
    PlayerGreen,
    PlayerBlue,

    Destroy,     // 物体终态
    GameOver     // 全局终态

}

public class BaseState: MonoBehaviour
{
    private TextAsset objectTransitionRuleText;
    private TextAsset playerTransitionRuleText;

    private Dictionary<string, Dictionary<string, string>> objectTransitionRule;
    private Dictionary<string, Dictionary<string, string>> playerTransitionRule;

    // Start is called before the first frame update
    void Start()
    {
        objectTransitionRule =
            JsonUtility.FromJson<Dictionary<string, Dictionary<string, string>>>(objectTransitionRuleText.text);
        playerTransitionRule =
            JsonUtility.FromJson<Dictionary<string, Dictionary<string, string>>>(playerTransitionRuleText.text);
    }
    
    public StateEnum currentState;
    public bool isPlayer;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.otherCollider.gameObject.GetComponent<BaseState>();
        if (isPlayer)
        {
            playerTransition(collision,other);
        }
        else
        {
            objectTransition(collision,other);
        }
    }

    private void objectTransition(Collision2D collision, BaseState other)
    {
        var otherState = other.currentState;
        var myState = currentState;
        
        StateEnum newState = myState; // 无转移结果时，保持状态

        var newStateStr = objectTransitionRule?.
            GetValueOrDefault(myState.ToString(), null)
            ?.GetValueOrDefault(otherState.ToString(), "")
                          ?? "Unknown";
        if (newStateStr != "Unknown")
        {
            myState = Enum.Parse<StateEnum>(newStateStr);
        }
        else
        {
            throw new Exception("Object Final State Unknown");
        }

        if (newState==myState){ // 未转移状态，过
            return;
        }

        switch (newState)
        {
            case StateEnum.GameOver:
                // 游戏结束
                break;
            case StateEnum.Destroy:
                // 销毁自己
                break;
            case StateEnum.BlueGreen:
                // 物体生长
                break;
            case StateEnum.DeepGreen:
                // 物体变得更绿
                break;
            default:
                // 普通变色
                break;
        }

        currentState = newState;
        Debug.Log($"set object newState: {newState.ToString()}");
    }

    private void playerTransition(Collision2D collision2D, BaseState other)
    {
        var otherState = other.currentState;
        var myState = currentState;
        
        StateEnum newState = myState; // 无转移结果时，保持状态

        var newStateStr = playerTransitionRule?.
                              GetValueOrDefault(myState.ToString(), null)
                              ?.GetValueOrDefault(otherState.ToString(), "")
                          ?? "Unknown";
        if (newStateStr != "Unknown")
        {
            myState = Enum.Parse<StateEnum>(newStateStr);
        }
        else
        {
            throw new Exception("Object Final State Unknown");
        }

        if (newState==myState){ // 未转移状态，过
            return;
        }

        switch (newState)
        {
            case StateEnum.GameOver:
                // 游戏结束
                break;
            case StateEnum.PlayerBlue:
                break;
            case StateEnum.PlayerGreen:
                break;
            case StateEnum.PlayerRed:
                break;
            case StateEnum.PlayerNone:
                break;
        }
        currentState = newState;
        Debug.Log($"set player newState: {newState.ToString()}");
    }
}
