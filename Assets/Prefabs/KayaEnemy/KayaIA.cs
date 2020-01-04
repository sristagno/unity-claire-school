using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class KayaIA : MonoBehaviour
{
    public float KayaDamage = 10f;
    
    private NavMeshAgent _kayaAgent;
    private Animator _kayaAnimator;
    private Transform _target;
    private GameObject _claire;
    private GameObject _particleExplode;
    private ClaireController _claireController;
    private AudioSource _audioSource;
    
    [SerializeField] private float idleDistance = 10f, walkDistance = 7f, attackDistance = 1f;
    [SerializeField] private AudioClip popSound;
    
    private static readonly int WalkState = Animator.StringToHash("walkState");
    private static readonly int AttackState = Animator.StringToHash("attackState");
    private static readonly int IdleState = Animator.StringToHash("idleState");

    // Start is called before the first frame update
    void Start()
    {
        _kayaAgent = GetComponent<NavMeshAgent>();
        _audioSource = GetComponent<AudioSource>();
        _claire = GameObject.FindGameObjectWithTag("Player");
        _target = _claire.GetComponent<Transform>();
        _claireController = _claire.GetComponent<ClaireController>();
        
        _kayaAnimator = GetComponent<Animator>();
        _particleExplode = transform.Find("Explode").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (_claireController.IsDead)
        {
            _kayaAnimator.SetBool(AttackState, false);
            _kayaAnimator.SetBool(WalkState, false);
            enabled = false;
        }
        else if (_kayaAgent.remainingDistance > walkDistance)
        {
            _kayaAgent.speed = 0;
            _kayaAnimator.SetBool(WalkState, false);
            _kayaAnimator.SetBool(AttackState, false);
            if (_kayaAgent.remainingDistance > idleDistance)
            {
                _kayaAnimator.SetBool(IdleState, false);
            }
            else
            {
                _kayaAnimator.SetBool(IdleState, true);
            }
        }
        else
        {
            if (_kayaAgent.remainingDistance < attackDistance)
            {
                _kayaAgent.speed = 0f;
                _kayaAnimator.SetBool(WalkState, false);
                _kayaAnimator.SetBool(AttackState, true); 
            }
            else
            {
                _kayaAgent.speed = 1f;
                _kayaAnimator.SetBool(WalkState, true);
                _kayaAnimator.SetBool(AttackState, false);
            }
        }
        
        _kayaAgent.SetDestination(_target.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            _particleExplode.SetActive(true);
            _audioSource.PlayOneShot(popSound);
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            Destroy(gameObject, popSound.length);
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _kayaAnimator.SetBool(AttackState, true);
            _kayaAnimator.SetBool(WalkState, false);
            _kayaAgent.speed = 0f;
        }
    }


    public void HitPlayer()
    {
        _claireController.MakeDamage(KayaDamage);
    }
}
