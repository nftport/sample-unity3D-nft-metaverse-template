using UnityEngine.Events;

namespace NFTPort.Samples
{

    using UnityEngine;
    using UnityEngine.UI;
    using NFTPort;
    

    public class CustomMintSample : MonoBehaviour
    {
        [Header("Mints NFT when a 'Player' tag enters the collider trigger and presses SpaceBar")]
        [SerializeField] private Mint_Custom _mintCustom;
        [SerializeField] private Text output;
        [SerializeField] private GameObject Panel;
        
        [Header("Makes Animator speed 2x on success")]
        [SerializeField] private Animator animator;
        
        [Header("Move an Object after Success")]
        [SerializeField] private Transform objectToTransformAfterSuccess;
        [SerializeField] private Transform targetTransformForTheObject;
        [SerializeField] private Vector3 velocity;

        [SerializeField] private UnityEvent Aftermint;

        private bool inZone = false;
        private bool isMinted = false;
        
        private void Start()
        {
            Panel.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") == true)
            {
                Panel.SetActive(true);
                inZone = true;
            }
        }
 
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player") == true)
            {
                Panel.SetActive(false);
                inZone = false;
            }
        }

        private void Update()
        {
            if (inZone && !isMinted)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    MintEet();
                }
            }
            
            if(isMinted && targetTransformForTheObject!= null)
            {
                objectToTransformAfterSuccess.position = Vector3.SmoothDamp(objectToTransformAfterSuccess.position, targetTransformForTheObject.position, ref velocity, 0.5f, 10);
            }
        }

        void MintEet()
        {
            _mintCustom 
                    
                .OnError(error => OnMintError(error))
                .OnComplete(minted => OnMintComplete(minted))
                .SetParameters(
                    //We are referencing it and have already set other parameters on the component in editor.
                        mintToAddress: Port.ConnectedPlayerAddress //connected via NFTPort Player Connect WebGL build feature
                        )
                .Run();
        }

        void OnMintComplete(Minted_model minted)
        {
            output.text = "NFTPort | Mint Success (⌐■_■) : at: " + minted.transaction_external_url;
            AfterMint();
        }
        void OnMintError(string error)
        {
            output.text = error;
        }

        void AfterMint()
        {
            if(animator)
                animator.speed = 5f;
            
            isMinted = true;
            Aftermint.Invoke();
        }
        
        
    }
}


