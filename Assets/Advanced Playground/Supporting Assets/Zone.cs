using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NFTPort.Samples
{
    //////â‰§â— â€¿â— â‰¦âœŒ _sz_ Î© //≧◠‿◠≦✌ _sz_ Ω
    public class Zone : MonoBehaviour
    {
        [SerializeField] private UnityEvent onZoneEnter;
        [SerializeField] private UnityEvent onZoneExit;
        [SerializeField] private GameObject ZoneCam;
        [SerializeField] private AudioSource audioPlayer;
        [SerializeField] private AudioClip ZoneEnterAudio;
        [SerializeField] private float waitAfterEnter = 1f;
        void Start()
        {
            if(ZoneCam)
                ZoneCam.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") == true)
            {
                if(ZoneCam)
                    ZoneCam.SetActive(true);
                
                StopCoroutine(Enter());
                StartCoroutine(Enter());
            }
        }

        IEnumerator Enter()
        {

            yield return new WaitForSeconds(waitAfterEnter);
            
            onZoneEnter.Invoke();

            if (audioPlayer)
            {
                audioPlayer.clip = ZoneEnterAudio;
                audioPlayer.Play();
            }
            
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player") == true)
            {
                StopAllCoroutines();
                onZoneExit.Invoke();
                if(ZoneCam)
                    ZoneCam.SetActive(false);
            }
        }
    }
}