/*
 * Created by emacs
 * Date: 2013/10/9
 * Author: zzc
 */
using System;
using UnityEngine;
using PureMVC.Patterns;

namespace TangScene
{
  public class TS
  {

    public const string GOBJ_NAME = "TS";

#region Properties

    // controlled Actor ID
    public static long ControlledActorId {
      get {
	return Cache.controlledActorId;
      }
    }

    /// <summary>
    ///   确保场景中存在 TS GameObject
    /// </summary>
    public static GameObject EnsureTSGobj ()
    {

      if (Cache.tsGobj == null) {
	Cache.tsGobj = GameObject.Find (GOBJ_NAME);
	if (Cache.tsGobj == null)
	  Cache.tsGobj = NewTSGobj ();
      }

      return Cache.tsGobj;

    }

#endregion

#region SceneMethods

    /// <summary>
    ///   加载场景
    /// <param name="sceneId">需要加载的场景ID</param>
    /// </summary>
    public static void LoadScene (int sceneId)
    {
      LevelBhvr.SceneId = sceneId;
    }


    // load assetbundle -------

    /// <summary>
    ///   加载 AssetBundle
    /// </summary>
    public static void LoadAssetBundle (string name,
					Tang.ResourceLoader.LoadCompleted loadCompletedHandler
					)
    {
      LoadAssetBundle (name, loadCompletedHandler, null);
    }

    /// <summary>
    ///   加载 AssetBundle
    /// </summary>
    public static void LoadAssetBundle (string name,
					Tang.ResourceLoader.LoadCompleted loadCompletedHandler,
					Tang.ResourceLoader.LoadFailure loadFailureHandler
					)
    {
      LoadAssetBundle (name, loadCompletedHandler, loadFailureHandler, null);
    }

    /// <summary>
    ///   加载 AssetBundle
    /// </summary>
    public static void LoadAssetBundle (string name,
					Tang.ResourceLoader.LoadCompleted loadCompletedHandler,
					Tang.ResourceLoader.LoadFailure loadFailureHandler,
					Tang.ResourceLoader.LoadBegan loadBeganHandler)
    {
      Tang.ResourceLoader.Enqueue (Tang.ResourceUtils.GetAbFilePath (name), 
				   loadCompletedHandler, loadFailureHandler, 
				   loadBeganHandler);
    }

    // load XML file -------
	
    /// <summary>
    ///   加载 XML 文件
    /// </summary>	
    public static void LoadXml(string name , 
			       Tang.ResourceLoader.LoadCompleted loadCompletedHandler )
    {
      LoadXml(name, loadCompletedHandler, null);
    }

    /// <summary>
    ///   加载 XML 文件
    /// </summary>
    public static void LoadXml(string name ,
			       Tang.ResourceLoader.LoadCompleted loadCompletedHandler,
			       Tang.ResourceLoader.LoadFailure loadFailureHandler)
    {
      LoadXml(name, loadCompletedHandler, loadFailureHandler, null);
    }

    /// <summary>
    ///   加载 XML 文件
    /// </summary>
    public static void LoadXml (string name, 
				Tang.ResourceLoader.LoadCompleted loadCompletedHandler,
				Tang.ResourceLoader.LoadFailure loadFailureHandler,
				Tang.ResourceLoader.LoadBegan loadBeganHandler)
    {
      Tang.ResourceLoader.Enqueue( Tang.ResourceUtils.GetXmlFilePath (name), 
				   loadCompletedHandler, 
				   loadFailureHandler, 
				   loadBeganHandler);
    }

#endregion

#region ActorMethods

    /// <summary>
    ///   角色进入场景
    /// </summary>
    public static void ActorEnter (Actor actor)
    {
      Cache.notificationQueue.Enqueue (new Notification (ActorEnterCmd.NAME, actor));      
    }

    /// <summary>
    ///   角色离开场景
    /// </summary>
    public static void ActorExit (long actorId)
    {
      Cache.notificationQueue.Enqueue (new Notification (ActorExitCmd.NAME, actorId));
    }

    /// <summary>
    ///   角色走动
    /// </summary>
    public static void ActorNavigate (MoveBean bean)
    {
      Cache.notificationQueue.Enqueue (new Notification (ActorNavigateCmd.NAME, bean));
    }

    /// <summary>
    ///   角色瞬移
    /// </summary>
    public static void ActorShift (MoveBean bean)
    {
      Cache.notificationQueue.Enqueue( new Notification( ActorShiftCmd.NAME, bean) );
    }

    /// <summary>
    ///   攻击
    /// </summary>
    public static void Attack (AttackBean bean)
    {
      Cache.notificationQueue.Enqueue (new Notification (ActorAttackCmd.NAME, bean));
    }

    /// <summary>
    ///   攻击结果处理
    /// </summary>
    public static void AttackResult (AttackResultBean bean)
    {
      Cache.notificationQueue.Enqueue (new Notification (ActorAttackResultCmd.NAME, bean));
    }

    /// <summary>
    ///   切换控制的角色
    /// </summary>
    public static void SwitchControlledActor (long actorId)
    {
      if (Cache.controlledActorId != actorId)
	Cache.notificationQueue.Enqueue (new Notification (SwitchControlledActorCmd.NAME, actorId));
    }

    /// <summary>
    ///   切换选择的角色
    /// </summary>
    public static void SwitchSelectedActor (long actorId)
    {
      if (Cache.selectedActorId != actorId)
	Cache.notificationQueue.Enqueue (new Notification (SwitchSelectedActorCmd.NAME, actorId));
    }

    /// <summary>
    ///   取消角色选择
    /// </summary>
    public static void UnselectActor ()
    {
      if (Cache.selectedActorId != 0 && Cache.actors.ContainsKey (Cache.selectedActorId))
	Cache.notificationQueue.Enqueue (new Notification (UnselectActorCmd.NAME));
	
    }

    /// <summary>
    ///   修改角色速度
    /// </summary>
    public static void ChangeActorSpeed (long actorId, float speed)
    {
      if (Cache.actors.ContainsKey (actorId))
	{
          Cache.notificationQueue.Enqueue (new Notification (ChangeActorSpeedCmd.NAME, new ChangeSpeedBean (actorId, speed)));
        }
    }

    /// <summary>
    ///   移动角色（用于摇杆 bean.vector3 < Vector3.one）
    /// <param name="bean">传递需要移动的信息</param>
    /// </summary>
    public static void ActorMove (MoveBean bean)
    {
      Cache.notificationQueue.Enqueue (new Notification (ActorMoveCmd.NAME, bean));
    }

    /// <summary>
    ///   角色跟踪
    /// </summary>
    public static void ActorTrace(long tracerId, long targetId)
    {
      Cache.notificationQueue.Enqueue( new Notification( ActorTraceCmd.NAME, new TraceBean(tracerId, targetId) ));
    }

    /// <summary>
    ///   角色跟踪
    /// </summary>
    public static void ActorTrace(long tracerId, long targetId, float cacheDistance, float startDistance)
    {
      Cache.notificationQueue.Enqueue( new Notification( ActorTraceCmd.NAME, new TraceBean(tracerId, targetId, cacheDistance, startDistance) ));
    }

    /// <summary>
    ///   取消跟踪
    /// </summary>
    public static void CancelTrace( long tracerId )
    {
      Cache.notificationQueue.Enqueue( new Notification( CancelTraceCmd.NAME, tracerId ));
    }

    /// <summary>
    ///   取消控制角色
    /// </summary>
    public static void UncontrollActor()
    {
      Cache.notificationQueue.Enqueue( new Notification( UncontrollActorCmd.NAME) );
    }

    /// <summary>
    ///   冲刺
    /// </summary>
    public static void Sprint(long sourceId, long targetId)
    {
      Cache.notificationQueue.Enqueue( new Notification( SprintCmd.NAME, 
							 new SprintBean( sourceId, targetId) ) );
    }

    /// <summary>
    ///   冲刺
    /// </summary>
    public static void Sprint(long sourceId, long targetId, Vector3 destination)
    {
      Cache.notificationQueue.Enqueue( new Notification( SprintCmd.NAME,
							 new SprintBean( sourceId, targetId, destination) ) );
    }

    /// <summary>
    ///   僵直
    /// </summary>
    public static void Stagnant(long[] actorIds, float secondTime)
    {
      Cache.notificationQueue.Enqueue( new Notification( StagnantCmd.NAME, new StagnantBean( actorIds, secondTime  )
 ) );
    }

    /// <summary>
    ///   死亡
    /// </summary>
    public static void ActorDie(long actorId)
    {
      Cache.notificationQueue.Enqueue( new Notification( ActorDieCmd.NAME, actorId ) );
    }

    /// <summary>
    ///   重生
    /// </summary>
    public static void ActorRelive(long actorId)
    {
      Cache.notificationQueue.Enqueue( new Notification( ActorReliveCmd.NAME, actorId ) );
    }


#endregion
#region StagePropertyMethods
    

    public static void StagePropertyEnter (StageProperty stageProperty)
    {
      // TODO
      
    }

    public static void StagePropertyExit (long stagePropertyId)
    {
      // TODO
    }
#endregion
#region EffectMethods
    public static void PlayEnviromentEffect (int effectId, int seconds)
    {
      // TODO
    }

#endregion

#region GameObject

    public static GameObject GetActorGameObject(long actorId)
    {
      if( Cache.actors.ContainsKey(actorId) )
	return Cache.actors[ actorId ];
      else
	return null;
    }

#endregion


    


    private static GameObject NewTSGobj ()
    {
      GameObject gobj = new GameObject ();
      gobj.name = GOBJ_NAME;
      gobj.AddComponent<TsBhvr> ();
      gobj.AddComponent<LevelBhvr> ();
      gobj.AddComponent<Tang.ResourceLoader> ();
      gobj.AddComponent<ClearActorsOutOfEyeshotBhvr>();
      return gobj;
    }
  }
}