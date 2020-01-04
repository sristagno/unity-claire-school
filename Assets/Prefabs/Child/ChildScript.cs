using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChildScript : MonoBehaviour
{
    private GameObject _explosion;
    private NavMeshAgent _agentChild;
    private Animator _animatorChild;
    private AudioSource _audioSource;
    [SerializeField] private Transform target;
    [SerializeField] private AudioClip explosionSound;
    private GameObject _player;
    private bool _isInCage = true;
    private GameManager _gameManager;

    private Transform _playerTransform;

    private bool _isCageExploded;

    private static readonly int RunningState = Animator.StringToHash("runningState");

    // Start is called before the first frame update
    void Start()
    {
        _explosion = transform.Find("Explode").gameObject;
        _animatorChild = GetComponentInChildren<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _agentChild = GetComponentInChildren<NavMeshAgent>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerTransform = _player.GetComponent<Transform>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (_isInCage)
        {
            
            _agentChild.SetDestination(_player.transform.position);
            _agentChild.speed = 0f;
        }
        else
        {
            _animatorChild.SetBool(RunningState, true);
            if (_agentChild.destination != target.position)
            {
                _agentChild.SetDestination(target.position);    
            }
            _agentChild.speed = 5f;
            float distance = _agentChild.remainingDistance;
            if (_agentChild.pathPending)
            {
                distance = Vector3.Distance(transform.position, target.position);
            }

            if (distance <= _agentChild.stoppingDistance)
            {
                _agentChild.isStopped = true;
                _animatorChild.SetBool(RunningState, false);
                _agentChild.transform.rotation = target.rotation;
            }
        }
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player") && !_isCageExploded)
        {
            _audioSource.PlayOneShot(explosionSound);
            _explosion.SetActive(true);
            GetComponent<BoxCollider>().enabled = false;
            Destroy(transform.Find("Cage").gameObject);
            _isCageExploded = true;
            _isInCage = false;
            _gameManager.FreeChild();
        }
    }
}
