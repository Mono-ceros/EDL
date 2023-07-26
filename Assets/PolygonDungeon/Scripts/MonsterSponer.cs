using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ���� ����
/// </summary>
public class MonsterSponer : MonoBehaviour
{
    //������ �Ҵ�� ���͸� ���� �׸�
    Monster monster;
    MonsterData monsterData;
    
    [Header("Stage1 Monster")]
    public MonsterData[] s1MonsterDatas; // ���� �¾� ������
    public Monster[] s1MonsterPrefabs; // ���� ���� ������
    [Header("Stage2 Monster")]
    public MonsterData[] s2MonsterDatas; 
    public Monster[] s2MonsterPrefabs; 
    [Header("Stage3 Monster")]
    public MonsterData[] s3MonsterDatas; 
    public Monster[] s3MonsterPrefabs; 

    public Transform[] spawnPoints; // ���� ���� ��ġ

    int stage = 1; // ���� ��������
    [Header("�� ������")]
    public int spawnAmount = 0;

    /// <summary>
    /// ������ ��������ŭ ���� ��ȯ
    /// </summary>
    void Spawn()
    {
        //if(�÷��̾ �濡 �����ϸ�)
        for (int i = 0; i < spawnAmount; i++)
        {
            monsterPool.Get();
        }
    }

    /// <summary>
    /// ���͸� �����ϰ� ������ ���Ϳ��� ������ ����� �Ҵ�
    /// </summary>
    void CreateMonster()
    {
        
        int num = 0;
        //���� ���� ��ġ
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
        //���������� ���� �� ����
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
                Debug.Log("�������� ���� �̻���");
                break;
        }
        monster.Setup(monsterData);
        //monster.onDeath += () => 
    }
    /// <summary>
    /// ���������̶� ��ȯ�̶� ���� ��������
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
