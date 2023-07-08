using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
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

public enum PlayerTransitionAnimationEnum{
    GameOver, // 死了（optional）
    TurnBlue, // 变蓝色
    TurnGreen, // 变绿色
    TurnRed, // 变红色
    TurnNone // 变没色
}

public class BaseState: MonoBehaviour
{
    private Dictionary<string, Dictionary<string, string>> objectTransitionRule;
    private Dictionary<string, Dictionary<string, string>> playerTransitionRule;

    public Queue<PlayTransitionAnimationEnum> PlayerTransitionAnimationQueue = new Queue<PlayTransitionAnimationEnum>();

    // Start is called before the first frame update
    void Start()
    {
        objectTransitionRule =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Resources.Load<TextAsset>("objectTransitionRule").text);
        playerTransitionRule =
            JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Resources.Load<TextAsset>("playerTransitionRule").text);
    }
    
    public StateEnum currentState;
    public bool isPlayer;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayer)
        {
            playerTransition(other.gameObject.GetComponent<BaseState>());
        }
        else
        {
            objectTransition(other.gameObject.GetComponent<BaseState>());
        }
    }
    
    public void OnCollisionEnter2D(Collision2D collision)
    {
        var other = collision.otherCollider.gameObject.GetComponent<BaseState>();
        if (isPlayer)
        {
            playerTransition(other);
        }
        else
        {
            objectTransition(other);
        }
    }

    private void objectTransition(BaseState other)
    {
        var otherState = other.currentState;
        var myState = currentState;

        if (aboutToDestory(myState, otherState))
        {
            return;
        }

        StateEnum newState = myState; // 无转移结果时，保持状态

        var newStateStr = objectTransitionRule?
                              [myState.ToString()]?
                              [otherState.ToString()]
                          ?? "Unknown";
        if (newStateStr == "Unknown" || !Enum.TryParse(newStateStr,out newState))
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
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(0, 0, 0,0);
                break;
            case StateEnum.BlueGreen:
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(81, 205, 224,255);

                // 物体生长
                break;
            case StateEnum.DeepGreen:
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(60, 180, 71,255);
                // 物体变得更绿
                break;
            default:
                // 普通变色
                break;
        }

        StartCoroutine(UpdateStatusAtNextFrame(this, newState));
        Debug.Log($"set object newState: {newState.ToString()}");
    }

    private bool aboutToDestory(StateEnum myState, StateEnum otherState)
    {
        if (myState == StateEnum.Destroy || myState == StateEnum.GameOver)
        {
            return true;
        }
        if (otherState == StateEnum.Destroy || otherState == StateEnum.GameOver)
        {
            return true;
        }

        return false;
    }

    private IEnumerator UpdateStatusAtNextFrame(BaseState baseState, StateEnum newState)
    {
        yield return new WaitForFixedUpdate();
        baseState.currentState = newState;
    }

    private void playerTransition(BaseState other)
    {
        var otherState = other.currentState;
        var myState = currentState;
        
        if (aboutToDestory(myState, otherState))
        {
            return;
        }
        
        StateEnum newState = myState; // 无转移结果时，保持状态

        var newStateStr = playerTransitionRule?
                                  [myState.ToString()]?
                              [otherState.ToString()]
                          ?? "Unknown";
        if (newStateStr == "Unknown" || !Enum.TryParse(newStateStr,out newState))
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
                PlayerTransitionAnimationQueue.Enqueue(PlayerTransitionAnimationEnum.GameOver);
                break;
            case StateEnum.PlayerBlue:
                PlayerTransitionAnimationQueue.Enqueue(PlayerTransitionAnimationEnum.TurnBlue);
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(159, 212, 250,255);
                break;
            case StateEnum.PlayerGreen:
                PlayerTransitionAnimationQueue.Enqueue(PlayerTransitionAnimationEnum.TurnGreen);
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(159, 250, 167,255);
                break;
            case StateEnum.PlayerRed:
                PlayerTransitionAnimationQueue.Enqueue(PlayerTransitionAnimationEnum.TurnRed);
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(250, 164, 159,255);
                break;
            case StateEnum.PlayerNone:
                PlayerTransitionAnimationQueue.Enqueue(PlayerTransitionAnimationEnum.TurnNone);
                gameObject.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255,255);

                break;
        }
        StartCoroutine(UpdateStatusAtNextFrame(this, newState));
        Debug.Log($"set player newState: {newState.ToString()}");
    }
}
