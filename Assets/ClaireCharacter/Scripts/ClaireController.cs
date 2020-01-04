using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Serialization;

public class ClaireController : MonoBehaviour
{
    private static readonly int IsWalkingState = Animator.StringToHash("isWalking");
    private static readonly int IsWalkingBackState = Animator.StringToHash("isWalkingBack");
    private static readonly int RunState = Animator.StringToHash("run");
    private static readonly int JumpState = Animator.StringToHash("jump");
    private static readonly int IsDancingState = Animator.StringToHash("isDancing");
    private static readonly int DeadState = Animator.StringToHash("dead");
    private static readonly int Colliderheight = Animator.StringToHash("colliderheight");
    private const float Timeout = 20.0f;
    private AudioSource _audioDance;
    private AudioSource _claireAudioSource;
    private bool _isJumping = false;
    private bool _isWalking = false;
    private bool _isRunning = false;
    private bool _isDancing = false;
    private bool _isDead = false;

    [SerializeField] private float countDown = Timeout;
    [SerializeField] private float walkSpeed = 2f, runSpeed = 8f, rotateSpeed = 100f, jumpForce = 350f;
    [FormerlySerializedAs("JumpSound")] [SerializeField] private AudioClip jumpSound;
    [FormerlySerializedAs("EndJumpSound")] [SerializeField] private AudioClip endJumpSound;
    [FormerlySerializedAs("RightStepSound")] [SerializeField] private AudioClip rightStepSound;
    [FormerlySerializedAs("LeftStepSound")] [SerializeField] private AudioClip leftStepSound;
    [FormerlySerializedAs("DeadSound")] [SerializeField] private AudioClip deadSound;
    [FormerlySerializedAs("HurtSound")] [SerializeField] private AudioClip hurtSound;
    
    private Animator _claireAnimator;
    private CapsuleCollider _capsuleCollider;
    private Rigidbody _rigidBody;
    private float _axisH, _axisV;
    private bool _switchFoot = false;
    private static readonly int AxisHorizontal = Animator.StringToHash("axisHorizontal");


    public event EventHandler<bool> IsWalkingChanged;
    public event EventHandler<bool> IsRunningChanged;
    public event EventHandler<bool> IsJumpingChanged;
    public event EventHandler<bool> IsDancingChanged;
    public event EventHandler<float> IsHurted;
    public event EventHandler IsKilled;

    public bool IsWalking
    {
        get { return _isWalking; }
        private set
        {
            if (_isWalking != value)
            {
                _isWalking = value;
                if (_isWalking)
                {
                    IsRunning = false;
                }
                IsWalkingChanged?.Invoke(this, _isWalking);
            }
        }
    }
    
    public bool IsRunning
    {
        get { return _isRunning; }
        private set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                if (_isRunning)
                {
                    IsWalking = false;
                }
                IsRunningChanged?.Invoke(this, _isRunning);
            }
        }
    }
    
    public bool IsJumping
    {
        get { return _isJumping; }
        private set
        {
            if (_isJumping != value)
            {
                _isJumping = value;
                if (_isJumping)
                {
                    _claireAudioSource.PlayOneShot(jumpSound);
                    _rigidBody.AddForce(Vector3.up * jumpForce);
                    _claireAnimator.SetTrigger(JumpState);
                }
                else
                {
                    if (endJumpSound != null)
                    {
                        _claireAudioSource.PlayOneShot(endJumpSound);
                    }
                }
                IsJumpingChanged?.Invoke(this, _isJumping);
            }
        }
    }

    public bool IsDancing
    {
        get => _isDancing;
        private set
        {
            if (_isDancing != value)
            {
                _isDancing = value;
                IsDancingChanged?.Invoke(this, _isDancing);
                Dance(_isDancing);
            }
        }
    }
    
    public bool IsDead { get; private set; }

    private void Awake()
    {
        _claireAnimator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _audioDance = transform.Find("AudioDance").GetComponent<AudioSource>();
        _claireAudioSource = GetComponent<AudioSource>();
        _capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        _axisH = Input.GetAxis("Horizontal");
        _axisV = Input.GetAxis("Vertical");

        if (_axisV != 0)
        {
            float speed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runSpeed;
                _claireAnimator.SetFloat(RunState, _axisV);
                IsRunning = true;

            }
            else
            {
                IsWalking = true;
                _claireAnimator.SetFloat(RunState, 0);
            }
            transform.Translate(Vector3.forward * (speed * _axisV * Time.deltaTime));
            int walkingIdAnimator = _axisV > 0 ? IsWalkingState : IsWalkingBackState;
            _claireAnimator.SetBool(walkingIdAnimator, true);
            
            _claireAnimator.SetFloat(AxisHorizontal, 0);
        }
        else
        {
            _claireAnimator.SetBool(IsWalkingState, false);
            _claireAnimator.SetBool(IsWalkingBackState, false);
            _claireAnimator.SetFloat(RunState, 0);
            
            _claireAnimator.SetFloat(AxisHorizontal, _axisH);

            IsWalking = false;
            IsRunning = false;
        }

        if (_axisH != 0)
        {
            // Rotation
            transform.Rotate(Vector3.up * (rotateSpeed * Time.deltaTime * _axisH));
        }
        
        //Idle Dance
        if (_axisH == 0 && _axisV == 0 && !_isJumping)
        {
            countDown -= Time.deltaTime;
            if (countDown <= 0)
            {
                Dance(true);
            }
        }
        else
        {
            Dance(false);
        }

        if (!_claireAudioSource.isPlaying)
        {
            _claireAudioSource.pitch = 1f;
        }

        
        _capsuleCollider.height = _claireAnimator.GetFloat(Colliderheight);
    }


    private void Dance(bool isDancing)
    {
        _claireAnimator.SetBool(IsDancingState, isDancing);
        _audioDance.enabled = isDancing;
        IsDancing = isDancing;
        if (!IsDancing)
        {
            countDown = Timeout;
        }
    }

    public void MakeDamage(float damage)
    {
        if (hurtSound != null)
        {
            _claireAudioSource.PlayOneShot(hurtSound);
        }
        IsHurted?.Invoke(this, damage);
    }

    public void Kill()
    {
        Dance(false);
        _claireAnimator.SetTrigger(DeadState);
        if (deadSound != null)
        {
            _claireAudioSource.pitch = 1f;
            if(!_claireAudioSource.isPlaying)
                _claireAudioSource.PlayOneShot(deadSound);
        }

        IsDead = true;
        IsKilled?.Invoke(this, EventArgs.Empty);
        GetComponent<ClaireController>().enabled = false;
    }

    public void EndJumpSound()
    {
        IsJumping = false;
    }

    public void PlayFootStep(string isRunning = "false")
    {
        if (isRunning == "true" && !IsDead)
        {
            _claireAudioSource.pitch = 2f;
        }
        else
        {
            _claireAudioSource.pitch = 1f;
        }
        if (!_claireAudioSource.isPlaying)
        {
            _switchFoot = !_switchFoot;
            _claireAudioSource.PlayOneShot(_switchFoot ? leftStepSound: rightStepSound);
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isJumping)
        {
            Dance(false);
            IsJumping = true;
        }
    }
}
