using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager2 : MonoBehaviour
{
    public static QuestManager2 Instance { get; private set; }
    
    public List<Quest> availableQuests = new List<Quest>();
    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();
    
    // Event yang dipanggil saat quest selesai
    public UnityEvent<Quest> OnQuestCompleted;
    // Event yang dipanggil saat quest diaktifkan
    public UnityEvent<Quest> OnQuestActivated;
    // Event yang dipanggil saat foto quest sudah lengkap tapi belum diserahkan
    public UnityEvent<Quest> OnQuestPhotosCompleted;
    
    // Dictionary untuk event quest custom
    private Dictionary<string, UnityEvent> questEvents = new Dictionary<string, UnityEvent>();
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Initialize events
        if (OnQuestCompleted == null)
            OnQuestCompleted = new UnityEvent<Quest>();
            
        if (OnQuestActivated == null)
            OnQuestActivated = new UnityEvent<Quest>();
            
        if (OnQuestPhotosCompleted == null)
            OnQuestPhotosCompleted = new UnityEvent<Quest>();
            
        // Setup contoh quest
        InitializeQuests();
    }
    
    private void InitializeQuests()
    {
        // Contoh quest fotografi
        availableQuests.Add(new Quest("photoQuest1", "Burung Langka", "Ambil foto 3 burung langka di hutan.", true, 3, "Bird"));
        availableQuests.Add(new Quest("photoQuest2", "Bunga Ajaib", "Temukan dan foto bunga ajaib di padang rumput.", true, 1, "MagicFlower"));
        availableQuests.Add(new Quest("photoQuest3", "Bangunan Tua", "Dokumentasikan 2 bangunan tua di kota.", true, 2, "OldBuilding"));
    }
    
    public void ActivateQuest(string questId)
    {
        Quest quest = availableQuests.Find(q => q.questId == questId);
        
        if (quest != null)
        {
            quest.isActive = true;
            activeQuests.Add(quest);
            availableQuests.Remove(quest);
            OnQuestActivated.Invoke(quest);
            
            Debug.Log($"Quest activated: {quest.questName}");
        }
        else
        {
            Debug.LogWarning($"Quest dengan ID {questId} tidak ditemukan.");
        }
    }
    
    public bool IsQuestActive(string questId)
    {
        return activeQuests.Exists(q => q.questId == questId && q.isActive);
    }
    
    public bool HasActivePhotoQuest()
    {
        return activeQuests.Exists(q => q.requiresPhotography && !q.isCompleted);
    }
    
    // Mendaftarkan listener untuk event
    public void RegisterEventListener(string eventName, UnityAction action)
    {
        if (!questEvents.ContainsKey(eventName))
        {
            questEvents[eventName] = new UnityEvent();
        }
        
        questEvents[eventName].AddListener(action);
    }
    
    // Mempublikasikan event
    public void PublishEvent(string eventName, string questId)
    {
        if (questEvents.ContainsKey(eventName))
        {
            Debug.Log($"Event {eventName} dipanggil untuk quest ID: {questId}");
            questEvents[eventName].Invoke();
        }
        
        // Jika ini event foto, cek juga apakah bisa diproses untuk quest foto
        if (eventName == "OnPhotoCaptured")
        {
            ProcessPhotoCapture(questId);
        }
    }
    
    // Memproses pengambilan foto untuk quest tertentu
    private void ProcessPhotoCapture(string questId)
    {
        Quest quest = activeQuests.Find(q => q.questId == questId);
        
        if (quest != null && quest.requiresPhotography)
        {
            bool allPhotosCompleted = quest.AddPhoto();
            
            if (allPhotosCompleted)
            {
                // Disini kita TIDAK otomatis menyelesaikan quest
                // Melainkan memberi tahu bahwa foto-foto sudah lengkap dan siap diserahkan
                OnQuestPhotosCompleted.Invoke(quest);
                Debug.Log($"Semua foto untuk quest '{quest.questName}' sudah lengkap! Kembali ke NPC untuk menyelesaikan quest.");
            }
        }
    }
    
    // Memproses foto dengan tag objek
    public void ProcessPhoto(string taggedObject)
    {
        foreach (Quest quest in activeQuests)
        {
            if (quest.requiresPhotography && !quest.isCompleted && quest.targetTag == taggedObject)
            {
                bool allPhotosCompleted = quest.AddPhoto();
                
                if (allPhotosCompleted)
                {
                    // Disini kita TIDAK otomatis menyelesaikan quest
                    // Melainkan memberi tahu bahwa foto-foto sudah lengkap dan siap diserahkan
                    OnQuestPhotosCompleted.Invoke(quest);
                    Debug.Log($"Semua foto untuk quest '{quest.questName}' sudah lengkap! Kembali ke NPC untuk menyelesaikan quest.");
                }
            }
        }
    }
    
    // Method publik untuk menyelesaikan quest (dipanggil oleh NPC saat foto diserahkan)
    public void CompleteQuest(Quest quest)
    {
        if (quest == null) return;
        
        // Pastikan quest ada dalam activeQuests
        if (activeQuests.Contains(quest))
        {
            activeQuests.Remove(quest);
            completedQuests.Add(quest);
            quest.isCompleted = true;
            OnQuestCompleted.Invoke(quest);
            
            Debug.Log($"Quest '{quest.questName}' telah diselesaikan!");
        }
    }
    
    // Method publik untuk menyelesaikan quest berdasarkan ID
    public void CompleteQuestById(string questId)
    {
        Quest quest = activeQuests.Find(q => q.questId == questId);
        if (quest != null)
        {
            CompleteQuest(quest);
        }
    }
    
    public List<string> GetActiveQuestDescriptions()
{
    List<string> desc = new List<string>();

    foreach (Quest q in activeQuests)
    {
        string progress = "";

        if (q.requiresPhotography)
            progress = $" ({q.photosTaken}/{q.photosRequired})";

        // Tambahkan kondisi lain jika ada tipe progres berbeda
        // if (q.requiresCollectItem) progress = $" ({q.itemsCollected}/{q.itemsRequired})";

        desc.Add($"{q.questName}{progress}\n{q.description}");
    }

    return desc;
}
}