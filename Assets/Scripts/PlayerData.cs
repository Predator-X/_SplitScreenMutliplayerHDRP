using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class PlayerData 
{
    public float health;
    public int score;
    public float currntTime;
    public float[] position;
    public float[] rotation;
    public int sceneIndexx;
    public PlayerData(PlayerController player, int sceneIndex )
    {
        sceneIndexx = sceneIndex;
        health = player.currentHealth;
        score = player.GetScore();
        currntTime = player.GetTime();

        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

        rotation = new float[3];
        rotation[0] = player.transform.rotation.x;
        rotation[1] = player.transform.rotation.y;
        rotation[2] = player.transform.rotation.z;


    }


}

[System.Serializable]
public class PlayerAchivments
{
   public string Name;
    public int Score;
    public float Time;
    public float TotalScore;

    public PlayerAchivments(string name , int score , float time , float totalscore)
    {
        Name = name;
        Score = score;
        Time = time;
        TotalScore = totalscore;
    }

}

[System.Serializable]
public class PlayersScoreListData
{
    
    public ArrayList ScoreArrayList;
    public PlayersScoreListData(ArrayList scoreListArrayList)
    {
        ScoreArrayList = new ArrayList();
        ScoreArrayList.AddRange(scoreListArrayList);
    }
   // PlayerAchivments[] playersScoreArrayList;// = new ArrayList();

}

[System.Serializable]
public class EnemyData
{
    //public string[] enemyNames;
    public float heath;
    public bool isItActive;
    public string enemyName;
    public float[] position;

    public EnemyData(Enemy enemy)
    {
        heath = enemy.currentHealth;
        enemyName = enemy.gameObject.name;
        isItActive = enemy.isActiveAndEnabled;
        position = new float[3];
        position[0] = enemy.transform.position.x;
        position[1] = enemy.transform.position.y;
        position[2] = enemy.transform.position.z;
    }
}

[System.Serializable]
public class SceneData
{
  //  public ArrayList sceneID = new ArrayList();

    public List<int> scenesID;// isSceneFinishtList;
    public List<bool> isSceneFinishtList;
    //  public ArrayList isSceneFinishtArray = new ArrayList();

    public bool isSceneFinisht;
    public int currentSceneID;
    public int lastCheckpoint;

    public SceneData(int currentSceneid,bool isScenefinisht,int lastchekpoint)//ArrayList sceneid
    {
        //sceneID = sceneid;
        scenesID.Add(currentSceneid);
        isSceneFinishtList.Add(isScenefinisht);
        isSceneFinisht = isScenefinisht;
        currentSceneID = currentSceneid;
        lastCheckpoint = lastchekpoint;
    }

}

[System.Serializable]
public class UserData
{
    public  string username, passport;


    public UserData(string userName, string passportt)
    {


        username = userName;
        passport = passportt;

    }
}