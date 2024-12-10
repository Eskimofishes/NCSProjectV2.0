using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPC : MonoBehaviour
{
    public bool playerInRange;

    public bool isTalkingWithPlayer;

    public List<Quest> quests;
    public Quest currentActiveQuest = null;
    public int activeQuestIndex = 0;
    public bool firstTimeInteraction = true;
    public int currentDialog;



    TextMeshProUGUI npcDialogText;
    Button optionButton1;
    TextMeshProUGUI optionButton1Text;
    Button optionButton2;
    TextMeshProUGUI optionButton2Text;
    Button optionButton3;
    TextMeshProUGUI optionButton3Text;


    // Start is called before the first frame update
    void Start()
    {
        npcDialogText = DialogSystem.Instance.dialogText;
 
        optionButton1 = DialogSystem.Instance.option1BTN;
        optionButton1Text = DialogSystem.Instance.option1BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
 
        optionButton2 = DialogSystem.Instance.option2BTN;
        optionButton2Text = DialogSystem.Instance.option2BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        optionButton3 = DialogSystem.Instance.option3BTN;
        optionButton3Text = DialogSystem.Instance.option3BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void StartConversation()
    {
        isTalkingWithPlayer = true;
 
        LookAtPlayer();
 
        // Interacting with the NPC for the first time
        if (firstTimeInteraction)
        {
            firstTimeInteraction = false;
            currentActiveQuest = quests[activeQuestIndex]; // 0 at start
            StartQuestInitialDialog();
            currentDialog = 0;
        }
        else // Interacting with the NPC after the first time
        {
 
            // If we return after declining the quest
            if (currentActiveQuest.declined)
            {
 
                DialogSystem.Instance.OpenDialogUI();
 
                npcDialogText.text = currentActiveQuest.info.comebackAfterDecline;
 
                SetAcceptAndDeclineOptions();
            }
 
 
            // If we return while the quest is still in progress
            if (currentActiveQuest.accepted && currentActiveQuest.isCompleted == false)
            {
                if (AreQuestRequirmentsCompleted())
                {
 
                    SubmitRequiredItems();
 
                    DialogSystem.Instance.OpenDialogUI();
 
                    npcDialogText.text = currentActiveQuest.info.comebackCompleted;
 
                    optionButton1Text.text = "[Take Reward]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() => {
                        ReceiveRewardAndCompleteQuest();
                    });
                }
                else
                {
                    DialogSystem.Instance.OpenDialogUI();
 
                    npcDialogText.text = currentActiveQuest.info.comebackInProgress;
 
                    optionButton1Text.text = "[Close]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() => {
                        DialogSystem.Instance.CloseDialogUI();
                        isTalkingWithPlayer = false;
                    });
                }
            }
 
            if (currentActiveQuest.isCompleted == true)
            {
                DialogSystem.Instance.OpenDialogUI();
 
                npcDialogText.text = currentActiveQuest.info.finalWords;
 
                optionButton1Text.text = "[Close]";
                optionButton1.onClick.RemoveAllListeners();
                optionButton1.onClick.AddListener(() => {
                    DialogSystem.Instance.CloseDialogUI();
                    isTalkingWithPlayer = false;
                });
            }
 
            // If there is another quest available
            if (currentActiveQuest.initialDialogCompleted == false)
            {
                StartQuestInitialDialog();
            }
 
        }
 
    }
 
    private void SetAcceptAndDeclineOptions()
    {
        optionButton1Text.text = currentActiveQuest.info.acceptOption;
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            AcceptedQuest();
        });
 
        optionButton2.gameObject.SetActive(true);
        optionButton2Text.text = currentActiveQuest.info.declineOption;
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() => {
            DeclinedQuest();
        });
        }
    private void SubmitRequiredItems()
    {
        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;

        if (!string.IsNullOrEmpty(firstRequiredItem))
        {
            if (!InventoryManager.Instance.RemoveFromInventory(firstRequiredItem, firstRequiredAmount))
            {
                Debug.LogWarning($"Failed to remove {firstRequiredAmount} {firstRequiredItem}(s).");
            }
        }

        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        if (!string.IsNullOrEmpty(secondRequiredItem))
        {
            if (!InventoryManager.Instance.RemoveFromInventory(secondRequiredItem, secondRequiredAmount))
            {
                Debug.LogWarning($"Failed to remove {secondRequiredAmount} {secondRequiredItem}(s).");
            }
        }
    }
 
    private bool AreQuestRequirmentsCompleted()
    {
        string firstRequiredItem = currentActiveQuest.info.firstRequirmentItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;
        string secondRequiredItem = currentActiveQuest.info.secondRequirmentItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        int firstItemQuantity = InventoryManager.Instance.GetItemQuantity(firstRequiredItem);
        int secondItemQuantity = InventoryManager.Instance.GetItemQuantity(secondRequiredItem);

        if (firstItemQuantity >= firstRequiredAmount && secondItemQuantity >= secondRequiredAmount)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
 
    private void StartQuestInitialDialog()
    {
        DialogSystem.Instance.OpenDialogUI();
        SoundManager.Instance.PlaySound(SoundManager.Instance.enterNPCSound);

 
        npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
        optionButton1Text.text = "Next";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(()=> {
            currentDialog++;
            CheckIfDialogDone();
        });
 
        optionButton2.gameObject.SetActive(false);
        optionButton3.gameObject.SetActive(false);
    }
 
    private void CheckIfDialogDone()
    {
        if (currentDialog == currentActiveQuest.info.initialDialog.Count - 1) // If its the last dialog 
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
 
            currentActiveQuest.initialDialogCompleted = true;
 
            SetAcceptAndDeclineOptions();
        }
        else  // If there are more dialogs
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
 
            optionButton1Text.text = "Next";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() => {
                currentDialog++;
                CheckIfDialogDone();
            });
        }
    }
    private void AcceptedQuest()
    {
        QuestManager.Instance.AddActiveQuest(currentActiveQuest);

        currentActiveQuest.accepted = true;
        currentActiveQuest.declined = false;
        SoundManager.Instance.PlaySound(SoundManager.Instance.acceptQuestSound);
 
        if (currentActiveQuest.hasNoRequirements)
        {
            npcDialogText.text = currentActiveQuest.info.comebackCompleted;
            optionButton1Text.text = "[Take Reward]";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() => {
                ReceiveRewardAndCompleteQuest();
            });
            optionButton2.gameObject.SetActive(false);
            optionButton3.gameObject.SetActive(false);
        }
        else
        {
            npcDialogText.text = currentActiveQuest.info.acceptAnswer;
            CloseDialogUI();
        }
 
 
 
    }
 
    private void CloseDialogUI()
    {
        optionButton1Text.text = "[Close]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        });
        optionButton2.gameObject.SetActive(false);
        optionButton3.gameObject.SetActive(false);
    }
 
private void ReceiveRewardAndCompleteQuest()
{
    SoundManager.Instance.PlaySound(SoundManager.Instance.acceptQuestSound);
    QuestManager.Instance.MarkQuestCompleted(currentActiveQuest);
    
    currentActiveQuest.isCompleted = true;

    int coinsReceived = currentActiveQuest.info.coinReward;
    InventoryManager.Instance.AddToInventory("Coin", currentActiveQuest.info.coinReward);
    Debug.Log($"You received {coinsReceived} gold coins!");

    if (!string.IsNullOrEmpty(currentActiveQuest.info.rewardItem1))
    {
        InventoryManager.Instance.AddToInventory(currentActiveQuest.info.rewardItem1);
    }

    if (!string.IsNullOrEmpty(currentActiveQuest.info.rewardItem2))
    {
        InventoryManager.Instance.AddToInventory(currentActiveQuest.info.rewardItem2);
    }

    activeQuestIndex++;

    // Start the next quest if available
    if (activeQuestIndex < quests.Count)
    {
        currentActiveQuest = quests[activeQuestIndex];
        currentDialog = 0;
        DialogSystem.Instance.CloseDialogUI();
        isTalkingWithPlayer = false;
    }
    else
    {
        DialogSystem.Instance.CloseDialogUI();
        isTalkingWithPlayer = false;
        Debug.Log("No more quests available.");
    }
}
 
    private void DeclinedQuest()
    {
        currentActiveQuest.declined = true;
 
        npcDialogText.text = currentActiveQuest.info.declineAnswer;
        SoundManager.Instance.PlaySound(SoundManager.Instance.declineQuestSound);
        CloseDialogUI();
    }
 
  
 
    public void LookAtPlayer()
    {
        var player = PlayerState.Instance.playerBody.transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);
 
        var yRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0,yRotation,0);
 
    }
 
 
 
}