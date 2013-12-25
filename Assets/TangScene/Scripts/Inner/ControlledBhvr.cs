using UnityEngine;
using PureMVC.Patterns;
//using PureMVC.Interfaces;

namespace TangScene
{

  [RequireComponent(typeof(Navigable))]
  public class ControlledBhvr : MonoBehaviour
  {
    private Navigable navigable;
    private ActorBhvr actorBhvr;

#region mono
    void Start()
    {
      

      // Cache.controlledActorId
      ActorBhvr actorBhvr = GetComponent<ActorBhvr>();
      if( actorBhvr != null )
	Cache.controlledActorId = actorBhvr.id;
      
    
      // set main camera scrolling target
      if( Camera.main != null )
	{
	  CameraScrolling scrolling = Camera.main.GetComponent<CameraScrolling>();
	  if( scrolling != null )
	    {
	      scrolling.target = transform;
	    }
	}
    }

    void OnEnable()
    {
      // ensure navigable
      navigable = GetComponent<Navigable>();
      
      if( navigable != null )
	{
	  //CommonDelegates.locationHandler = navigable.NavTo;
	  navigable.nextPositionChangeHandle = OnNextPositionChange;
	}
      
    }

    void OnDisable()
    {
      if( navigable != null )
	{
	  
	  //CommonDelegates.locationHandler -= navigable.NavTo;
	  if( navigable.nextPositionChangeHandle == OnNextPositionChange )
	    navigable.nextPositionChangeHandle = null;
	}

    }

    void OnNextPositionChange(Vector3 nextPosition)
    {
      Facade.Instance.SendNotification(NtftNames.LEADING_ACTOR_MOVE, nextPosition);
    }

#endregion
  }
}