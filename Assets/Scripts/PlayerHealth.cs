using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    AttackbleEntity myEntity;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Awake()
    {
        myEntity = GetComponent<AttackbleEntity>();
    }

    private void FixedUpdate()
    {
        healthText.text = "" + myEntity.health;
    }

}
