using System;
using UnityEngine;


public class ZombieBoss : EnemyBase
{
[Header("Zombie Boss Settings")]
[SerializeField] private GameObject boulderPrefab;
[SerializeField] private GameObject fragmentPrefab;
[SerializeField] private Transform throwPoint;
[SerializeField] private float throwForce = 12f;
[SerializeField] private int fragmentCount = 5;
[SerializeField] private float fragmentSpreadForce = 5f;
[SerializeField] private float boulderArcHeight = 2f;
[SerializeField] private float boulderTravelTime = 1.5f;


protected override void HandleBehavior()
{
if (player == null) return;


float distance = DistanceToPlayer();
if (distance <= actionRadius && Time.time >= lastAttackTime + attackCooldown)
{
StopMoving();
Attack();
}
else
{
Move(player.position, speed);
}
}


protected override void Attack()
{
base.Attack();
ThrowBoulder();
}


private void ThrowBoulder()
{
if (boulderPrefab == null || throwPoint == null) return;


GameObject boulder = Instantiate(boulderPrefab, throwPoint.position, Quaternion.identity);
Boulder boulderScript = boulder.AddComponent<Boulder>();
boulderScript.Init(fragmentPrefab, fragmentCount, fragmentSpreadForce, damage, player.position, boulderArcHeight, boulderTravelTime);
}


public override EnemyType GetEnemyType()
{
return EnemyType.ZombieBoss;
}
}