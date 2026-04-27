using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip coin;
    [SerializeField] private AudioClip water;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void CoinSound()
    {
        audioSource.PlayOneShot(coin);
    }

    public void WaterSound()
    {
        audioSource.PlayOneShot(water);
    }
}
