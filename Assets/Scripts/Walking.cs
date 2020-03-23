using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class Walking : MonoBehaviour
{
    public Transform Body;
    public Transform Head;
    public Animator Animator;
    public CharacterController Cont;
    public PlayerInput Input;
    public AudioClip[] StepAudio;
    public AudioSource StepSource;
    public System.Random Randomizer;
    public HealthBar Bar;
    public CapsuleCollider Collider;

    public bool IsRunning;
    public bool IsMoving;
    public bool Step = false;

    private bool _step;
    private bool _firstStepFrame = false;

    public float GeneralSpeed = 0.0f;
    public float Speed = 3.0f;
    public float RunSpeed = 5.0f;
    public float Sensitivity = 5.0f;
    public float AnimationTime = 0.0f;
    public float Stamina;
    public float Health;

    public float CurrentMaxStamina => Health / MaxHealth * MaxStamina;

    public float MaxHealth = 10;
    /// <summary>
    /// Seconds
    /// </summary>
    public float MaxStamina = 10;
    /// <summary>
    /// Seconds
    /// </summary>
    public float StaminaCooldown = 3;
    public float StaminaUpSpeed = 1;

    float _time_since_last_run = 0;
    void RegainStamina()
    {
        if (!IsRunning)
        {
            _time_since_last_run += Time.deltaTime;

            if (_time_since_last_run >= StaminaCooldown)
            {
                Stamina = Mathf.MoveTowards(Stamina, CurrentMaxStamina, Time.deltaTime * StaminaUpSpeed);
            }
        }
        else
        {
            _time_since_last_run = 0;
            Stamina = Mathf.MoveTowards(Stamina, 0, Time.deltaTime);
            if (Stamina <= 0) IsRunning = false;
        }

        Bar.Stamina = Stamina / CurrentMaxStamina;
        Bar.Health = Health / MaxHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Head == null) Head = transform.parent;
        if (Body == null) Body = Head.parent;
        if (Cont == null) Cont = Body.GetComponent<CharacterController>();
        if (Animator == null) Animator = GetComponent<Animator>();
        if (Input == null) Input = GetComponent<PlayerInput>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Randomizer = new System.Random();
        Health = MaxHealth;
        Stamina = MaxStamina;
    }

    float XBoundaries(float y)
    {
        if (Head.eulerAngles.x <= 90)
        {
            return Mathf.Min(90f - Head.localEulerAngles.x, y);
        }
        else
        {
            return Mathf.Max(270f - Head.localEulerAngles.x, y);
        }
    }

    void HurtHeal()
    {
        if (Input.actions["Hurt"].triggered) Health = Mathf.MoveTowards(Health, 0, 0.5f);
        if (Input.actions["Heal"].triggered) Health = Mathf.MoveTowards(Health, MaxHealth, 0.5f);
    }

    void PlayStepSound()
    {
        int index = Randomizer.Next(0, StepAudio.Length);
        Debug.Log($"Step: {index}");
        StepSource.PlayOneShot(StepAudio[index]);
    }

    // Update is called once per frame
    void Update()
    {
        HurtHeal();

        if (_step) _firstStepFrame = false;
        if (Step)
        {
            if (!_firstStepFrame && !_step)
            {
                _step = true;
                _firstStepFrame = true;
                PlayStepSound();
            }
        }
        else _step = false;


        var look = Input.actions["Look"].ReadValue<Vector2>();
        var move = Input.actions["Move"].ReadValue<Vector2>();
        var v = move.y;
        var h = move.x;

        var x = look.x;
        var y = look.y;

        var rotMatrix = Matrix4x4.Rotate(Body.localRotation);
        var moveVect = Vector3.ClampMagnitude(new Vector3(h, 0, v), 1);

        IsRunning = Input.actions["Run"].ReadValue<float>() == 1 && v > 0;
        IsMoving = moveVect.magnitude >= 0.5;

        Body.Rotate(0, x * Sensitivity, 0);
        Head.Rotate(XBoundaries(-y * Sensitivity), 0, 0);

        RegainStamina();

        Animator.SetFloat("InputSpeed", moveVect.magnitude == 0 ? 1 : moveVect.magnitude);

        Animator.SetBool("IsWalking", IsMoving && !IsRunning);
        Animator.SetBool("IsRunning", IsMoving && IsRunning);
        Cont.SimpleMove(rotMatrix.MultiplyPoint(moveVect) * GeneralSpeed);
    }
}
