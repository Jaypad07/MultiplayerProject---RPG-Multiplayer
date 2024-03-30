using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Enemy : MonoBehaviourPun
{
    [Header("Info")]
    public string enemyName;
    public float moveSpeed;

    public int curHp;
    public int maxHp;

    public float chaseRange;
    public float attackRange;

    private PlayerController _targetPlayer;

    public float playerDetectRate = 0.2f;
    private float lastPlayerDetectTime;

    public string objectToSpawnOnDeath;

    [Header("Attack")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;

    [Header("Components")]
    public HeaderInfo healthBar;
    public SpriteRenderer sr;
    public Rigidbody2D rig;

    private void Start()
    {
        healthBar.Initialize(enemyName, maxHp);
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (_targetPlayer != null)
        {
            // Calculate the distance
            float dist = Vector2.Distance(transform.position, _targetPlayer.transform.position);
            
            // If we're able to attack, do so
            if (dist < attackRange && Time.time - lastAttackTime >= attackRange)
            {
                Attack();
            }
            // Otherwise, do we move after the player?
            else if (dist > attackRange)
            {
                Vector3 dir = _targetPlayer.transform.position - transform.position;
                rig.velocity = dir.normalized * moveSpeed;
            }
            else
            {
                rig.velocity = Vector2.zero;
            }
        }

        DetectPlayer();
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        _targetPlayer.photonView.RPC("TakeDamage", _targetPlayer.photonPlayer, damage);
    }
    
    // Updates the targeted player
    void DetectPlayer()
    {
        if (Time.time - lastPlayerDetectTime > playerDetectRate)
        {
            lastPlayerDetectTime = Time.time;
            
            // Loop through all the players
            foreach (PlayerController player in GameManager.instance.players)
            {
                // Calculate distance between us and the player
                float dist = Vector2.Distance(transform.position, player.transform.position);

                if (player == _targetPlayer)
                {
                    if (dist > chaseRange)
                    {
                        _targetPlayer = null;
                    }
                    else if (dist < chaseRange)
                    {
                        if (_targetPlayer == null)
                        {
                            _targetPlayer = player;
                        }
                    }
                }
            }
        }
    }
}
