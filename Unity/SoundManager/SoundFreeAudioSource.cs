using UnityEngine;

namespace GameCore.Unity
{
    //满世界跑的挂上这个脚本，方便进行声音的禁用、调大小
    public class SoundFreeAudioSource : MonoBehaviour
    {
        public AudioSource Source { get; private set; }
        public int id;

        private void Start()
        {
            Source = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            SoundManager.Instance.RegisterFreeAudioSource(this);
        }

        private void OnDisable()
        {
            SoundManager.Instance.UnregisterFreeAudioSource(this);
        }
    }
}
