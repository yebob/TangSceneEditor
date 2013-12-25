/*
 * Created by emacs
 * Date: 2013/10/8
 * Author: zzc
 */

using UnityEngine;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace TangScene
{
  public class ActorExitCmd : SimpleCommand
  {

    public const string NAME = "SCENE_ACTOR_EXIT";

    public override void Execute( INotification notification )
    {

      long actorId = (long) notification.Body;
      if( actorId != 0 )
	if( Cache.actors.ContainsKey( actorId ) )
	  {
	    GameObject.Destroy( Cache.actors[actorId] );
	    Cache.actors.Remove( actorId );
	    PureMVC.Patterns.Facade.Instance.SendNotification( NtftNames.ACTOR_EXITED, actorId );
	  }
    }
  }
}