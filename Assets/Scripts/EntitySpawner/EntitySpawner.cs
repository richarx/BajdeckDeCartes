using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField] private AudioClip entitySpawnSound;
    [SerializeField] private AudioClip entitySpawnedSound;

    [Serializable]
    public class Entity
    {
        public AchievementReward reward;
        public GameObject prefab;
        public Transform transform;
    }

    [SerializeField] private List<Entity> entityList = new();

    private GameObject entityToSpawn;
    private Vector2 entityFinalPosition;
    private AudioSource spawning;

    private Vector2 smoothPosition;

    private bool isAdjusting;

    void Update()
    {
        if (entityToSpawn == null)
            return;

        if (isAdjusting == true && (Vector2)entityToSpawn.transform.position != entityFinalPosition)
            AdjustEntityToPosition(entityToSpawn, entityFinalPosition);

        if (Vector2.Distance((Vector2)entityToSpawn.transform.position, entityFinalPosition) < 0.1f)
            StopAdjusting();
    }

    private void SpawnEntity(GameObject entity, Vector2 finalPosition)
    {
        Vector2 spawnPosition = finalPosition + new Vector2(0, 15f);

        entityToSpawn = Instantiate(entity, spawnPosition, Quaternion.identity);
        entityFinalPosition = finalPosition;

        Rigidbody2D[] rigidbodies = entityToSpawn.GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D rb in rigidbodies)
            rb.simulated = false;

        spawning = SFXManager.Instance.PlaySFX(entitySpawnSound, loop: true);

        isAdjusting = true;
    }

    private void AdjustEntityToPosition(GameObject entity, Vector2 objectPosition)
    {
        entityToSpawn.transform.position = Vector2.SmoothDamp(entityToSpawn.transform.position, objectPosition, ref smoothPosition, 0.4f);
    }

    private void StopAdjusting()
    {
        isAdjusting = false;

        SqueezeAndStretch squeeze = entityToSpawn.GetComponent<SqueezeAndStretch>();

        if (squeeze != null)
            squeeze.Trigger();

        if (spawning != null)
            Destroy(spawning.gameObject);

        SFXManager.Instance.PlaySFX(entitySpawnedSound);

        Rigidbody2D[] rigidbodies = entityToSpawn.GetComponentsInChildren<Rigidbody2D>();

        foreach (Rigidbody2D rb in rigidbodies)
            rb.simulated = true;

        entityToSpawn = null;
        entityFinalPosition = Vector2.zero;
    }

    public void GiveReward(AchievementReward reward)
    {
        if (reward == AchievementReward.Binder)
        {
            //Unlock binder

            return;
        }

        foreach (Entity entity in entityList)
        {
            if (entity.reward == reward)
                SpawnEntity(entity.prefab, entity.transform.position);
        }
    }
}
