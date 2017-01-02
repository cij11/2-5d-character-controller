using UnityEngine;
using System.Collections;

public class CharacterIntegrator : MonoBehaviour
{

    public CharacterMovementActuator characterMovementActuator;
    bool isTargetting;

    StateInfo stateInfo;
    // Use this for initialization
    void Start()
    {
        stateInfo = new StateInfo();
    }

    // Update is called once per frame
    void Update()
    {
        if (isTargetting)
        {
			characterMovementActuator.ParachuteCommand();
        }
    }

    public void MoveHorizontal(float direction)
    {
        if (!isTargetting)
			characterMovementActuator.MoveHorizontalCommand(direction);
        else
			characterMovementActuator.FaceDirectionCommand(direction);
    }
    public void MoveVertical(float direction)
    {

    }
    
    public void Grab()
    {

    }

    public void Jump(float hor, float vert)
    {
		characterMovementActuator.JumpCommand(hor, vert);
    }
    public void StartTargetting()
    {
        isTargetting = true;
    }
    public void StopTargetting()
    {
        isTargetting = false;
    }

        public struct StateInfo
    {
        public float leftWallContactTimer;
        public float rightWallContactTimer;
        public float groundContactTimer;
        public int remainingDoubleJumps;

        public void Update()
        {
            leftWallContactTimer += Time.deltaTime;
            if (leftWallContactTimer > 10f)
            {
                leftWallContactTimer = 10f;
            }
            rightWallContactTimer += Time.deltaTime;
            if (rightWallContactTimer > 10f)
            {
                rightWallContactTimer = 10f;
            }
            groundContactTimer += Time.deltaTime;
            if(groundContactTimer > 10f){
                groundContactTimer = 10f;
            }
        }
    }

    public void ZeroRightWallContactTimer(){
        stateInfo.rightWallContactTimer = 0f;
    }
    public void ZeroLeftWallContactTimer(){
        stateInfo.rightWallContactTimer = 0f;
    }
    public void ZeroGroundContactTimer(){
        stateInfo.groundContactTimer = 0f;
    }

}
