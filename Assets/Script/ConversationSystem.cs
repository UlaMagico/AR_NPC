using UnityEngine;

public class ConversationSystem
{
    public GameObject conversationBlockPrefab;
    private GameObject currConversationBlock;
    static bool isTalking = false;

    //產生對話
    public void generateConversation()
    {
        if (isTalking) return;



    }
    
    public void closeConversation()
    {
        
        isTalking = false;
    }
}

/*
public class Conversation : MonoBehaviour
{
    string[] texts;


    void Update()
    {
        if()    
    }
}
*/