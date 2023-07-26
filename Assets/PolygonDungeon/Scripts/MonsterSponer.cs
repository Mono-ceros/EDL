using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// 몬스터 스폰
/// </summary>
public class MonsterSponer : MonoBehaviour
{
    //실제로 할당될 몬스터를 담을 그릇
    Monster monster;
    MonsterData monsterData;
    
    [Header("Stage1 Monster")]
    public MonsterData[] s1MonsterDatas; // 몬스터 셋업 데이터
    public Monster[] s1MonsterPrefabs; // 몬스터 원본 프리팹
    [Header("Stage2 Monster")]
    public MonsterData[] s2MonsterDatas; 
    public Monster[] s2MonsterPrefabs; 
    [Header("Stage3 Monster")]
    public MonsterData[] s3MonsterDatas; 
    public Monster[] s3MonsterPrefabs; 

    public Transform[] spawnPoints; // 몬스터 스폰 위치

    int stage = 1; // 현재 스테이지
    [Header("몹 마릿수")]
    public int spawnAmount = 0;

    /// <summary>
    /// 지정한 스폰량만큼 몬스터 소환
    /// </summary>
    void Spawn()
    {
        //if(플래이어가 방에 입장하면)
        for (int i = 0; i < spawnAmount; i++)
        {
            monsterPool.Get();
        }
    }

    /// <summary>
    /// 몬스터를 생성하고 생성한 몬스터에게 추적할 대상을 할당
    /// </summary>
    void CreateMonster()
    {
        
        int num = 0;
        //랜덤 스폰 위치
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
        //스테이지별 랜덤 몹 생성
        switch (stage)
        {
            case 1:
                num = Random.Range(0, s1MonsterPrefabs.Length - 1);
                monsterData = s1MonsterDatas[num];
                monster = Instantiate(s1MonsterPrefabs[num], spawnPoint.position, spawnPoint.rotation);
                break;
            case 2:
                num = Random.Range(0, s2MonsterPrefabs.Length - 1);
                monsterData = s2MonsterDatas[num];
                monster = Instantiate(s2MonsterPrefabs[num], spawnPoint.position, spawnPoint.rotation);
                break;
            case 3:
                num = Random.Range(0, s3MonsterPrefabs.Length - 1);
                monsterData = s3MonsterDatas[num];
                monster = Instantiate(s3MonsterPrefabs[num], spawnPoint.position, spawnPoint.rotation);
                break;
            default:
                Debug.Log("스테이지 값이 이상함");
                break;
        }
        monster.Setup(monsterData);
        //monster.onDeath += () => 
    }
    /// <summary>
    /// 랜덤스폰이랑 소환이랑 따로 만들어야함
    /// </summary>
    void SpawnPoint()
    {

    }

    private ObjectPool<Monster> monsterPool;
    public Monster monsterPrefab;

    
    private void Create_Monster()
    {
        var monster = monsterPool.Get();
    }

}
