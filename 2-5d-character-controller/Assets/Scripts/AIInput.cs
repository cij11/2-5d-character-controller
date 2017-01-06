using UnityEngine;
using System.Collections;

public class AIInput : MonoBehaviour
{

    CharacterIntegrator characterIntegrator;
    AimingController aimingController;
    FiringController firingController;
    CharacterContactSensor contactSensor;
    AIRaycastSensors raycastSensors;

    float epsilon = 0.001f;
    float desiredMovementDirection = -1f;


    // Use this for initialization
    void Start()
    {
        RegisterControllers();
    }

    void RegisterControllers()
    {
        GameObject actionControllers = this.transform.parent.FindChild("ActionControllers").gameObject;
        characterIntegrator = this.transform.parent.GetComponent<CharacterIntegrator>();
        aimingController = actionControllers.GetComponent<AimingController>();
        firingController = actionControllers.GetComponent<FiringController>();
        contactSensor = this.transform.parent.GetComponent<CharacterContactSensor>() as CharacterContactSensor;
        raycastSensors = this.transform.parent.GetComponent<AIRaycastSensors>() as AIRaycastSensors;
    }

    // Update is called once per frame
    void Update()
    {
        characterIntegrator.SetWalkingSpeed(4f); //This might be better as a public, or a loaded by the character integrator, or saved in a 'Specs' file that movement has access to.
        ChangeDirectionIfFindCliff();
        Movement();
        Aiming();
        Firing();
    }

    void ChangeDirectionIfFindCliff(){
        if (contactSensor.GetContactState() == ContactState.FLATGROUND){
            if(raycastSensors.GetLeftCliff()){
                desiredMovementDirection = 1f;
            }
            else if (raycastSensors.GetRightCliff()){
                desiredMovementDirection = -1f;
            }
        }
    }

    void Movement()
    {
        characterIntegrator.MoveHorizontal(desiredMovementDirection);
    }

    void Aiming()
    {
        aimingController.SetHorizontalInput(desiredMovementDirection);
    }

    void Firing()
    {

    }
}