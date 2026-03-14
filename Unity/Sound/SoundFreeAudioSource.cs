using UnityEngine;

namespace GameCore.Unity.Sound
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
            SoundManager.Inst.RegisterFreeAudioSource(this);
        }

        private void OnDisable()
        {
            SoundManager.Inst.UnregisterFreeAudioSource(this);
        }
    }
}
