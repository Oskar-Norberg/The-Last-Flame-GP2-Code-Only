using UnityEngine;

public class ToggleQuestIndicator : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;
    [SerializeField] private GameObject questIndicator;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float offset;
    
    private bool isIndicatorOn;
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
            questIndicator.SetActive(true);
        else
            questIndicator.SetActive(false);
        
        MoveIndicatorToTarget();
        IndicatorAlwaysFacesPlayer();
    }

    // private void HandleToggleIndicator()
    // {
    //     isIndicatorOn = !isIndicatorOn;
    //     questIndicator.SetActive(isIndicatorOn);
    // }

    private void IndicatorAlwaysFacesPlayer()
    {
        // Vector3 direction = playerTransform.position - transform.position;
        // transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        Vector3 direction = new Vector3(playerTransform.position.x, questIndicator.transform.position.y, playerTransform.position.z) - questIndicator.transform.position;
        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        questIndicator.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void MoveIndicatorToTarget()
    {
        Vector3 targetPosition = targetObject.transform.position + Vector3.up * offset;
        transform.position = targetPosition;
    }
}
