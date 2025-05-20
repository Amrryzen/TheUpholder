using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string questId;
    public string questName;
    [TextArea(3, 5)]
    public string description;
    
    // Status quest
    public bool isActive;
    public bool isCompleted;
    
    // Quest yang memerlukan fotografi
    public bool requiresPhotography;
    public int photosRequired;
    public int photosTaken;
    public string targetTag; // Tag objek yang perlu difoto
    
    // Constructor untuk quest fotografi
    public Quest(string id, string name, string desc, bool requiresPhoto, int photoCount, string tag)
    {
        questId = id;
        questName = name;
        description = desc;
        isActive = false;
        isCompleted = false;
        requiresPhotography = requiresPhoto;
        photosRequired = photoCount;
        photosTaken = 0;
        targetTag = tag;
    }
    
    // Menambahkan foto dan mengembalikan true jika semua foto telah diambil
    public bool AddPhoto()
    {
        if (!requiresPhotography) return false;
        
        photosTaken++;
        Debug.Log($"Foto {photosTaken}/{photosRequired} diambil untuk quest '{questName}'");
        
        return photosTaken >= photosRequired;
    }
    
    // Reset progress foto
    public void ResetPhotos()
    {
        photosTaken = 0;
    }
    
    // Mendapatkan progres dalam bentuk persentase
    public float GetProgress()
    {
        if (isCompleted) return 1f;
        
        if (requiresPhotography)
        {
            return (float)photosTaken / photosRequired;
        }
        
        return isActive ? 0.5f : 0f;
    }
}