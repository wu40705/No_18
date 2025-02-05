﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI_ByState : MonoBehaviour
{
    public float speed;
    public float AttackDistance;//追擊距離
    public float Distance;//攻擊距離
    public Vector3 O;//出生點
    public IdleState idleState;
    public MoveState moveState;
    public AttackState attackState;
    public Istate currentState;
    public Vector3 AiPosition;
    public Animator _animator;
    public SpriteRenderer _spriteRenderer;
    private AI_ByState aI_ByState;
    public float MoveAniSpeed;
    public float AttAniSpeed;
    public float IdleAniSpeed;
    private float KillFoxSec;
    private bool killfox;
    private bool ScareLock;
    private bool KillLock;
    private bool state2;

    public static float MoveSpeed;
    public static float MoveSpeed2;
    
    private void Start()
    {
        idleState = new IdleState(this);
        moveState =new MoveState(this);
        attackState = new AttackState(this);
        currentState = idleState;
        O = transform.position;
        AttackDistance = 8;
        Distance = 5;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        killfox = true;
        KillFoxSec = 1;
        ScareLock = true;
        KillLock = true;
        state2 = false;
        MoveSpeed = 10f;
        MoveSpeed2 = 0.0001f;
        // _animator.SetFloat("AniSpeed",AniSpeed);
    }
    
    
    void Update()
    {
        _animator.SetFloat("MoveAniSpeed",MoveAniSpeed);
        _animator.SetFloat("AttAniSpeed",AttAniSpeed);
        _animator.SetFloat("IdleAniSpeed",IdleAniSpeed);

        currentState.OnStateExecution();
        if (Vector2.Distance(transform.position, PlayerCtrl.PlayerPosition) < MaskCtrl.currectScal
            ||Vector2.Distance(transform.position, PlayerCtrl.PlayerPosition) < 1)
        {
            ChangeState(attackState);
        }
        AiPosition = transform.position;
        if (Vector2.Distance(transform.position, PlayerCtrl.PlayerPosition) < 1&& !PlayerCtrl.IsScare&&PlayerCtrl.Player.animator !=null)
        {
            if (killfox)
            {
                KillFoxSec = Time.time + 5;
                killfox = false;
            }
            //PlayerCtrl.IsScare = true;
            if (ScareLock&&PlayerCtrl.Player.animator !=null)
            {
                PlayerCtrl.Player.animator.Play("Gumi_Scare");
                ScareLock = false;
            }
            PlayerCtrl.CanMove = false;
        }

        if (KillFoxSec<Time.time&&KillFoxSec>2&&KillLock)
        {
            //_animator.SetBool("IsAttack",true);
            KillAni();
            PlayerCtrl.Player.KillPlayer();
            KillLock = false;
        }

        

    }

    void KillAni()
    {
        if (state2)
        {
            _animator.Play("Attack");
            
        }
        else
        {
            _animator.Play("Attack2");

            
        }
        
    }

    public void StopAni()
    {
        _animator.enabled = false;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.tag =="Player"&&KillLock)
        {
            //_animator.SetBool("IsAttack",true);
            KillAni();
            other.transform.GetComponent<PlayerCtrl>().KillPlayer();
            KillLock = false;
        }
    }
    public  void ChangeState(Istate nextstate)
    {
        currentState.OnstateExit();
        nextstate.OnStateEnter();
        currentState = nextstate;
    }
    public void MoveToPosition(Vector3 position,float v)
    {
        transform.position = Vector3.Lerp(transform.position,position,v);
    }
    
    public  AI_ByState GetAi()
    {
        if (aI_ByState == null)
        {
            return aI_ByState = new AI_ByState();
        }
        else
        {
            return aI_ByState;
        }
    }
    
    public  void SetState1()
    {
        state2 = true;
        _animator.SetBool("IsState2",false);
    }
    
}

public class IdleState :Istate
{
    AI_ByState aiBystate;
    public IdleState(AI_ByState _aI_ByState)
    {
        aiBystate = _aI_ByState;
    }
    float timeEnd ;
    void Istate.OnStateEnter()
    {     
        timeEnd = Time.time + 3f;
        aiBystate.speed = 0;
        aiBystate._animator.SetBool("IsMove",false);
    }

    void Istate.OnStateExecution()
    {
        Debug.Log(aiBystate.name + ":IdleEx");
        if (Time.time > timeEnd) {
            aiBystate.ChangeState(aiBystate.moveState);
        }
    }

   void Istate.OnstateExit()
    {
    }
}

public class MoveState : Istate
{
    float timeEnd;
    AI_ByState aiBystate;
    Vector3 Target;
    bool dirold = true;
    public MoveState(AI_ByState aI_ByState)
    {
        aiBystate = aI_ByState;
    }
    void Istate.OnStateEnter()
    {
        timeEnd = Time.time + 3f;
        aiBystate.speed = AI_ByState.MoveSpeed;
        Target = aiBystate.O + new Vector3(Random.Range(-12,12),Random.Range(-12,12),0);
        if (aiBystate.AiPosition.x - Target.x > 0 != dirold)
        {
            aiBystate._spriteRenderer.flipX = !aiBystate._spriteRenderer.flipX;
            dirold = aiBystate.AiPosition.x - Target.x > 0;
        }
        aiBystate._animator.SetBool("IsMove",true);
    }

    void Istate.OnStateExecution()
    {
        aiBystate.MoveToPosition(Target,AI_ByState.MoveSpeed2);//0.0001f
        if (Time.time > timeEnd)
        {
            aiBystate.ChangeState(aiBystate.idleState);
        }
    }

    void Istate.OnstateExit()
    {
    }
}

public class AttackState : Istate
{
    AI_ByState aistate;
    bool dirold = true;
    public AttackState(AI_ByState aI_ByState)
    {
        aistate = aI_ByState;
    }

    void Istate.OnStateEnter()
    {
    }

    void Istate.OnStateExecution()
    {

        aistate.MoveToPosition(PlayerCtrl.PlayerPosition, AI_ByState.MoveSpeed2);//0.0001f
        if (aistate.AiPosition.x - PlayerCtrl.PlayerPosition.x > 0 != dirold)
        {
            aistate._spriteRenderer.flipX = !aistate._spriteRenderer.flipX;
            dirold = aistate.AiPosition.x - PlayerCtrl.PlayerPosition.x > 0;
        }
        aistate._animator.SetBool("IsMove",true);
        
        
        if (Vector3.Distance(aistate.AiPosition,PlayerCtrl.PlayerPosition)>MaskCtrl.currectScal)
        {
            aistate.ChangeState(aistate.idleState);
        }
        if (Vector3.Distance(aistate.AiPosition,PlayerCtrl.PlayerPosition)<aistate.Distance)
        {
        }
    }
    void Istate.OnstateExit()
    {
    }
}


public interface Istate
{
    void OnStateEnter();
    void OnStateExecution();
    void OnstateExit();
}
