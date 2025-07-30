using UnityEngine;

namespace Game.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("SFX")]
        public AudioClip spinClip;
        public AudioClip ballDropClip;
        public AudioClip winClip;
        public AudioClip loseClip;
        public AudioClip chipClip;

        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            audioSource = GetComponent<AudioSource>();
            if (!audioSource)
                audioSource = gameObject.AddComponent<AudioSource>();
        }

        public void PlaySpin() => audioSource.PlayOneShot(spinClip);
        public void PlayBallDrop() => audioSource.PlayOneShot(ballDropClip);
        public void PlayWin() => audioSource.PlayOneShot(winClip);
        public void PlayLose() => audioSource.PlayOneShot(loseClip);
        public void PlayChip() => audioSource.PlayOneShot(chipClip);
    }
}
