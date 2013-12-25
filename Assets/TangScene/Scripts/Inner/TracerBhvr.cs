/**
 * Tracer behaviour
 *
 * Date: 2013/10/24
 * Author: zzc
 */
using UnityEngine;
using PureMVC.Patterns;

namespace TangScene
{
  public enum TraceStatus
  {
    start,
    enter,
    lost
  }

  [RequireComponent(typeof(CharacterStatusBhvr))]
  [RequireComponent(typeof(Navigable))]
  public class TracerBhvr : MonoBehaviour
  {
    public const float DEFULT_CACHE_DISTANCE = 50F;
    public const float DEFAULT_START_DISTANCE = 200F;
    public const float MIN_CACHE_DISTANCE = 10F;
    public Transform target;
    [HideInInspector]
    public long tracerId;
    [HideInInspector]
    private long m_targetId;

    public long targetId {
      get {
	return m_targetId;
      }
      set {
	m_targetId = value;
	if (Cache.actors.ContainsKey (value))
	  target = Cache.actors [value].transform;
      }
    }

    public float cacheDistance;
    public float startDistance;
    private CharacterStatusBhvr characterStatusBhvr;
    private Navigable navigable;
    private TraceStatus traceStatus;


    public void TraceStart ()
    {

      Reset();

      traceStatus = TraceStatus.start;

      if (Cache.actors.ContainsKey (targetId)
	  && target != null
	  && target.localScale != Vector3.zero)
	{

	  float distance = Vector3.Distance (transform.localPosition, target.localPosition);
	  if (distance < cacheDistance)
	    {
	      Facade.Instance.SendNotification (NtftNames.TRACE_RANGE_ENTER, new TraceBean (tracerId, targetId));
	      traceStatus = TraceStatus.enter;
	    }
	  else if(distance < startDistance)
	    {
	      navigable.NavTo (target.localPosition, cacheDistance);	      
	    }

	} 
      else
	{
	  traceStatus = TraceStatus.lost;
	  Facade.Instance.SendNotification (NtftNames.TRACE_TARGET_LOST, new TraceBean (tracerId, targetId));
	  this.enabled = false;
	}
      
    }

    private void Reset ()
    {

      if (cacheDistance < MIN_CACHE_DISTANCE)
	cacheDistance = MIN_CACHE_DISTANCE;

      if (startDistance < cacheDistance)
	startDistance = cacheDistance * 2;
    }


    private void OnStatusChange (CharacterStatus status)
    {
      if (status == CharacterStatus.idle && traceStatus == TraceStatus.start) {
	Facade.Instance.SendNotification (NtftNames.TRACE_RANGE_ENTER, new TraceBean (tracerId, targetId));
	traceStatus = TraceStatus.enter;
      }
    }

    /// <summary>
    ///   Only be controlled actor used
    /// </summary>
    private void OnLocation (Vector3 position)
    {
      CommonDelegates.locationHandler -= OnLocation;
      this.enabled = false;
    }

    void Awake ()
    {

      characterStatusBhvr = GetComponent<CharacterStatusBhvr> ();
      navigable = GetComponent<Navigable> ();
      characterStatusBhvr.statusStartHandler += OnStatusChange;

      // ensure contains
      if (Cache.actors.ContainsKey (targetId))
	target = Cache.actors [targetId].transform;
      else
	this.enabled = false;

      // be controlled actor
      if (Cache.controlledActorId == tracerId)
	CommonDelegates.locationHandler += OnLocation;
      
    }

    void Update ()
    {

      if (Cache.actors.ContainsKey (targetId)
	  && target != null
	  && target.localScale != Vector3.zero) {

	if (characterStatusBhvr.Status == CharacterStatus.idle) {

	  float distance = Vector3.Distance (transform.localPosition, target.localPosition);
	  if (distance > startDistance)
	    navigable.NavTo (target.localPosition, cacheDistance);
	}

      } else {
	traceStatus = TraceStatus.lost;
	Facade.Instance.SendNotification (NtftNames.TRACE_TARGET_LOST, new TraceBean (tracerId, targetId));
	this.enabled = false;
      }
      
    }

    void OnEnable ()
    {

      if (characterStatusBhvr != null)
	characterStatusBhvr.statusStartHandler += OnStatusChange;

      if (Cache.controlledActorId == tracerId)
	CommonDelegates.locationHandler += OnLocation;

      TraceStart ();

    }

    void OnDisable ()
    {
      if (characterStatusBhvr != null)
	characterStatusBhvr.statusStartHandler -= OnStatusChange;
      
      Facade.Instance.SendNotification (NtftNames.TRACE_CANCEL, new TraceBean (tracerId, targetId)); 

    }


  }
}