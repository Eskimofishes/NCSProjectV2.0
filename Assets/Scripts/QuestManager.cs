using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public List<Quest> allActiveQuests;
    public List<Quest> allCompletedQuests;

    [Header("QuestTracker")]
    public GameObject questTrackerContent;
    public List<Quest> allTrackedQuests;
    // Start is called before the first frame update
    public GameObject trackerRowPrefab;
    public void TrackQuest(Quest quest)
    {
        allTrackedQuests.Add(quest);
        RefreshTrackerList();
    }
    public void UntrackQuest(Quest quest)
    {
        allTrackedQuests.Remove(quest);
        RefreshTrackerList();
    }
   public void RefreshTrackerList()
    {
        //Destroying the previous list
        foreach (Transform child in questTrackerContent.transform)
        {
            Destroy(child.gameObject);
        }
 
        foreach (Quest trackedQuest in allTrackedQuests)
        {
            GameObject trackerPrefab = Instantiate(trackerRowPrefab, Vector3.zero, Quaternion.identity);
            trackerPrefab.transform.SetParent(questTrackerContent.transform, false);
 
            TrackerRow tRow = trackerPrefab.GetComponent<TrackerRow>();
 
            tRow.questName.text = trackedQuest.questName;
            tRow.description.text = trackedQuest.questDescription;

            var req1 = trackedQuest.info.firstRequirmentItem;
            var req1Amount = trackedQuest.info.firstRequirementAmount;
            var req2 = trackedQuest.info.secondRequirmentItem;
            var req2Amount = trackedQuest.info.secondRequirementAmount;
 
            if (trackedQuest.info.secondRequirmentItem != "") // if we have 2 requirements
            {
                tRow.requirements.text = $"{req1}" + " " +InventoryManager.Instance.GetItemQuantity(req1) + "/" + $"{req1Amount}\n" +
               $"{req2}" + " " +InventoryManager.Instance.GetItemQuantity(req2) + "/" + $"{req2Amount}\n";
            }
            else // if we have only one
            {
                tRow.requirements.text = $"{req1}" + " " +InventoryManager.Instance.GetItemQuantity(req1) + "/" + $"{req1Amount}\n";
            }
 
 
        }
 
    }

    public void AddActiveQuest(Quest quest)
    {
        allActiveQuests.Add(quest);
        TrackQuest(quest);
        RefreshTrackerList();
    }
    public void MarkQuestCompleted(Quest quest)
    {
        allActiveQuests.Remove(quest);
        allCompletedQuests.Add(quest);
        UntrackQuest(quest);
        RefreshTrackerList();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
