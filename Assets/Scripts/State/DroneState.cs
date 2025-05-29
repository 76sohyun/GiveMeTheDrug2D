using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneState : BaseState
{
      protected Drone drone;

      public DroneState(Drone _drone)
      {
          drone = _drone;
      }
      public override void Enter(){}

      public override void Update()
      {

      }

      public override void Exit(){}
}

public class Drone_Idle : DroneState
{
    public Drone_Idle(Drone _drone) : base(_drone) {HasPhysics = true; }
    
    public override void Enter()
    {
        drone.animator.Play(drone.DronIdle_Hash);
    }

    public override void Update()
    {
        Vector2 rayOrigin = drone.transform.position + new Vector3(drone.patrolVec.x, 0);
        Debug.DrawRay(rayOrigin, Vector2.up * 3f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, 3f, drone.DroneLayer);

        if (hit.collider == null)
        {
            drone.Back();
        }
    }

    public override void FixedUpdate()
    {
        if (!drone.isWaited)
        {
            drone.rigid.velocity = drone.patrolVec * drone.MoveSpeed;   
        }
    }
    public override void Exit(){}
}

