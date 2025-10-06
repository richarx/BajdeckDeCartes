using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EntitySpawner : ReceivingAchievementMonoBehaviour
{
    [SerializeField] private AudioClip entitySpawnSound;
    [SerializeField] private AudioClip entitySpawnedSound;

    [Serializable] public class Entity
    {
        public AchievementReward reward;
        public GameObject prefab;
        public Transform transform;
    }

    [SerializeField] private List<Entity> entityList = new ();

    private GameObject entityToSpawn;
    private Vector2 entityFinalPosition;
    private AudioSource spawning;

    private Vector2 smoothPosition;

    private bool isAdjusting;

    void Start()
    {

    }

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
            Destroy(spawning);

        SFXManager.Instance.PlaySFX(entitySpawnedSound);

        entityToSpawn = null;
        entityFinalPosition = Vector2.zero;
    }

    public override void OnAchievementReceived(AchievementAsset achievement)
    {
        foreach(Entity entity in entityList)
        {
            if (entity.reward == achievement.Reward)
                SpawnEntity(entity.prefab, entity.transform.position);
        }
    }
}
