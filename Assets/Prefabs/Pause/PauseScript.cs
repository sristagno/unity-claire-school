using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class PauseScript : MonoBehaviour
{
    private Image _pauseImage;
    private bool _isOnPause = false;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip pauseSound, unpauseSound;

    public bool IsOnPause
    {
        get { return _isOnPause; }
        private set
        {
            if (_isOnPause != value)
            {
                _isOnPause = value;
                if (_isOnPause)
                {
                    if(pauseSound != null)
                        _audioSource.PlayOneShot(pauseSound);
                    _pauseImage.enabled = true;
                    Time.timeScale = 0;
                }
                else
                {
                    if(unpauseSound != null)
                        _audioSource.PlayOneShot(unpauseSound);
                    _pauseImage.enabled = false;
                    Time.timeScale = 1;
                }
            }
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _pauseImage = transform.Find("PauseImg").GetComponent<Image>();
        _pauseImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsOnPause = !IsOnPause;
        }
    }
}
