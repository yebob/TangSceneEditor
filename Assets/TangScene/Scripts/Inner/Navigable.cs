/**
 * Created by Emacs
 * Date: 2013/9/2
 * Author: zzc
 */

using UnityEngine;

namespace TangScene
{

  [ExecuteInEditMode]
  public class Navigable : MonoBehaviour
  {

    public delegate void NextPositionChange(Vector3 nextPosition);
    public NextPositionChange nextPositionChangeHandle = null;

    public const float CACHE_DISTANCE = 10F; // 距离目标人物等于小于这个距离的时候人物动作由跑动改为站立(run=>idle)
    public const float NOTIFIED_DISTANCE = 32F; // 摇杆操作，角色移动超过这个距离发通知（如果需要 nextPositionChangeHandle != null）

    public float m_speed = 240F;

    private NavMeshAgent agent;
    private CharacterStatusBhvr statusBhvr;
    private Directional directional;

    private Vector3 lastPosition = Vector3.zero;
    private int cornerIndex = 0;
    private Vector3 m_nextPosition = Vector3.zero;
    private Vector3 lastRecordMovePosition = Vector3.zero;


    private Vector3 NextPosition
    {
      get
	{
	  return m_nextPosition;
	}
      set
	{
	  m_nextPosition = value;
	  if( nextPositionChangeHandle != null )
	    nextPositionChangeHandle(value);
	}
    }

    public float Speed
    {
      get
	{
	  return m_speed;
	}
      set
	{
	  m_speed = value;
	  if (agent != null){
	    agent.speed = value;
	  }
	}
    }
    

    /// <summary>
    ///   Navigate to a destination
    /// </summary>
    public void NavTo(Vector3 position)
    {
      NavTo(position, 0F);
    }

    /// <summary>
    ///   Navigate to a destination with stopping distance
    /// </summary>
    public void NavTo(Vector3 position, float stoppingDistance)
    {
      if( agent != null && agent.enabled)
	{
	  agent.ResetPath();
	  Vector3 fixedPosition = new Vector3(position.x, transform.localPosition.y, position.z);
	  agent.SetDestination(fixedPosition);
	  agent.stoppingDistance = stoppingDistance;
	  m_nextPosition = transform.localPosition;
	  cornerIndex = 0;
	}
      
    }

    /// <summary>
    ///   Move by joystick
    /// </summary>
    public void Move(Vector3 moveDirection)
    {
      if( agent != null && agent.enabled )
	{
	  agent.ResetPath();
	  Vector3 forward = transform.TransformDirection( moveDirection );
	  agent.Move( forward * agent.speed * Time.deltaTime );
	  if( statusBhvr.Status != CharacterStatus.run )
	    statusBhvr.Status = CharacterStatus.run;

	  if( Vector3.Distance( transform.localPosition, lastRecordMovePosition ) > NOTIFIED_DISTANCE )
	    {
	      lastRecordMovePosition = transform.localPosition;
	      NextPosition = transform.localPosition;
	    }
	  
	}
    }




#region mono

    void Start()
    {

      // agent
      agent = GetComponent<NavMeshAgent>();
      if( agent == null )
	{
	  agent = gameObject.AddComponent<NavMeshAgent>();
	  agent.radius = 0.5F;
	  agent.speed = m_speed;
	  agent.acceleration = 999999F;
	  agent.angularSpeed = 0F;
	  agent.stoppingDistance = 0F;
	  agent.autoTraverseOffMeshLink = true;
	  agent.autoBraking = true;
	  agent.autoRepath = true;
	  agent.height = 2;
	  agent.baseOffset = 0;
	  agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
	  agent.avoidancePriority = 50;
	}

      // character status bhvr
      statusBhvr = GetComponent<CharacterStatusBhvr>();

      // directional
      directional = GetComponent<Directional>();

      // last position
      lastPosition = transform.localPosition;	
    }

    void Update()
    {

      if( agent.hasPath )
	{

	  // status(run/idle) checking ------

	  if( Vector2.Distance(new Vector2(agent.destination.x,
					   agent.destination.z),
			       new Vector2(transform.localPosition.x,
					   transform.localPosition.z)) - agent.stoppingDistance
	      < CACHE_DISTANCE )
	    {

	      if( statusBhvr != null )
		{
		  if( statusBhvr.Status == CharacterStatus.run )
		    {
		      statusBhvr.Status = CharacterStatus.idle;
		    }
		}
	    }
	  else
	    {
	      if( statusBhvr != null )
		{
		  if( statusBhvr.Status != CharacterStatus.run )
		    {
		      statusBhvr.Status = CharacterStatus.run;
		    }
		}
	    }

	  // send next position notification ------
	      
	  if( Vector3.Distance(NextPosition, transform.localPosition) < 10F )
	    {
	      if( ++cornerIndex < agent.path.corners.Length )
		{
		  NextPosition = agent.path.corners[cornerIndex];
		  EightDirection currentDirection = VectorUtils.Direction(transform.localPosition, 
									  NextPosition);
		  if( directional.Direction != currentDirection )
		    directional.Direction = currentDirection;
		  
		}
	      
	    }
				
	} // if agent.hasPath
      
      else 
	{

	  if( lastPosition == transform.localPosition )
	    {
	      if( statusBhvr.Status == CharacterStatus.run )
		statusBhvr.Status = CharacterStatus.idle;
	    }
	  else if( statusBhvr.Status == CharacterStatus.run)
	    {
	      EightDirection currentDirection = VectorUtils.Direction(lastPosition,transform.localPosition );
	      if( directional.Direction != currentDirection )
		directional.Direction = currentDirection;
	    }
	    
	}
      
      // position at last update
      lastPosition = transform.localPosition;


    }
#endregion
  }
}