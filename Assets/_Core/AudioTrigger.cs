using System;
using UnityEngine;

namespace RPG.Core
{
    public class AudioTrigger : MonoBehaviour
    {

        [SerializeField] AudioClip clip;
        [SerializeField] int layerFilter = 0;
        [SerializeField] float playerDistanceThreshold = 5f;
        [SerializeField] bool isOneTimeOnly = true;
        [SerializeField] [Range(0f, 1f)] float volume;

        bool hasPlayed = false;
        AudioSource audioSource;
        GameObject player;

        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = clip;
            audioSource.volume = volume;
            player = GameObject.FindWithTag("Player");
        }

        void Update()
        {
            if(Vector3.Distance(transform.position , player.transform.position) <= playerDistanceThreshold)
            {
                RequestPlayAudioClip();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == layerFilter)
            {
                RequestPlayAudioClip();
            }
        }

        private void RequestPlayAudioClip()
        {
            if (isOneTimeOnly && hasPlayed)
            {
                return;
            }
            else if (audioSource.isPlaying == false)
            {
                audioSource.Play();
                hasPlayed = true;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 255f, 0f, .5f);
            Gizmos.DrawWireSphere(transform.position, playerDistanceThreshold);
        }
    }
}