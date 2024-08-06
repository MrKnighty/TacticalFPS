using UnityEngine;

public class AI_Base : MonoBehaviour
{
    [Header("Truths")]
    [SerializeField] bool canSeePlayerHead;
    [SerializeField] bool canSeePlayerBody;
    [SerializeField] bool somthingElse;
    [SerializeField] bool inCover;

    [Header("Drag-Ins")]
    [SerializeField] Transform t_playerBody;
    [SerializeField] Transform t_playerHead;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
