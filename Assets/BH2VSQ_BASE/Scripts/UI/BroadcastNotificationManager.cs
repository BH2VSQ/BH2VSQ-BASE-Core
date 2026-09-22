using UdonSharp;
using UnityEngine;
using TMPro;
using VRC.SDKBase;

namespace BH2VSQ.Base
{
    public class BroadcastNotificationManager : UdonSharpBehaviour
    {
        public GameObject root;
        public TMP_Text title;
        public TMP_Text messageText;
        public TMP_Text senderText;
        public AudioSource audioSource;
        public AudioClip normalSound;
        public AudioClip importantSound;
        public AudioClip emergencySound;
        public PlayerDataManager data;
        public LocalizationManager localization;
        public int normalSeconds = 8;
        public int importantSeconds = 20;
        private int[] queueIds = new int[16];
        private int[] queuePriorities = new int[16];
        private string[] queueMessages = new string[16];
        private string[] queueSenders = new string[16];
        private long[] queueExpires = new long[16];
        private int head, tail, currentId, currentPriority;
        private float closeTime;

        private void Start() { if (root != null) root.SetActive(false); SendCustomEventDelayedSeconds("Tick", 1f); }

        public void Enqueue(int id, int priority, string message, string sender, long expiresAt)
        {
            if (expiresAt <= Networking.GetNetworkDateTime().Ticks) return;
            if (priority != 2 && data != null && !data.notificationsEnabled) return;
            if (id == currentId) return;
            for (int i = head; i < tail; i++) if (queueIds[i % 16] == id) return;
            if (tail - head >= 16) head++;
            int slot = tail % 16;
            queueIds[slot] = id; queuePriorities[slot] = priority;
            queueMessages[slot] = message; queueSenders[slot] = sender;
            queueExpires[slot] = expiresAt;
            tail++;
            if (currentId == 0) ShowNext();
        }

        private void ShowNext()
        {
            long now = Networking.GetNetworkDateTime().Ticks;
            while (head < tail && queueExpires[head % 16] <= now) head++;
            if (head >= tail) { currentId = 0; if (root != null) root.SetActive(false); return; }
            int slot = head++ % 16;
            currentId = queueIds[slot]; currentPriority = queuePriorities[slot];
            if (root != null) root.SetActive(true);
            if (title != null && localization != null) title.text = localization.Get(currentPriority == 2 ? BaseText.Emergency : currentPriority == 1 ? BaseText.Important : BaseText.Notice);
            if (messageText != null) messageText.text = queueMessages[slot];
            if (senderText != null) senderText.text = queueSenders[slot];
            float remaining = (queueExpires[slot] - now) / 10000000f;
            int displaySeconds = currentPriority == 1 ? importantSeconds : normalSeconds;
            closeTime = currentPriority == 2 ? float.PositiveInfinity : Time.time + (remaining < displaySeconds ? remaining : displaySeconds);
            AudioClip clip = currentPriority == 2 ? emergencySound : currentPriority == 1 ? importantSound : normalSound;
            if (audioSource != null && data != null) audioSource.volume = data.notificationVolume;
            if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        }

        public void Close() { ShowNext(); }
        public void Tick()
        {
            if (currentId != 0 && Time.time >= closeTime) Close();
            SendCustomEventDelayedSeconds("Tick", 1f);
        }
    }
}
