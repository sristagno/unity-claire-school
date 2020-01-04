using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private int itemValue = 10;
    [SerializeField] private MeshRenderer meshRenderer;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var audioSource = GetComponent<AudioSource>();
            var collider = GetComponent<Collider>();
            progressBar.Percentage += itemValue;
            if (collider != null)
            {
                collider.enabled = false;
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }

            meshRenderer.enabled = false;
            Destroy(gameObject, audioSource.clip.length);
        }
    }
}
