using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerReceiveDamage : MonoBehaviour
{
    // public int HP = 100;

    public void TakeDamage(int damageAmount)
    {
        // HP -= damageAmount;

        // if(HP<= 0)
        // {
        //     Debug.Log("player dead");
        // }
        // else
        // {
        //     Debug.Log("Player hit");
        // }
        
        PlayerState.Instance.LoseHealth(damageAmount);
        if(PlayerState.Instance.currentHealth <= 0)
        {
            Debug.Log("Player Dead");
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.gettingHitSound);
            Debug.Log("Player Hit");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if(other.CompareTag("ZombieAttackingHand"))
        // {
        //     TakeDamage(25);
        // }

        Debug.Log($"Triggered by: {other.name} on {gameObject.name}");
        if (other.CompareTag("AttackingPoint"))
        {
            // Find the Enemy component on the parent object
            Enemy enemy = other.GetComponentInParent<Enemy>();

            if (enemy != null)
            {
                Debug.Log($"Player hit by {enemy.enemyName}, taking {enemy.damage} damage.");
                TakeDamage(enemy.damage);
            }
            else
            {
                Debug.LogWarning("Enemy component not found on ZombieAttackingHand's parent!");
            }
        }
    }
}
