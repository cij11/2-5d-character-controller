using UnityEngine;
using System.Collections;

public class MovementController : MonoBehaviour
{

    public RigidBodyController character;
    bool isTargetting;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isTargetting)
        {
            character.ParachuteCommand();
        }
    }

    public void MoveHorizontal(float direction)
    {
        if (!isTargetting)
            character.MoveHorizontalCommand(direction);
        else
            character.FaceDirectionCommand(direction);
    }
    public void MoveVertical(float direction)
    {
        if (!isTargetting)
        {
            if (direction < 0)
            {
                character.SlideCommand();
            }
        }
    }
    public void Grab()
    {

    }

    public void Jump(float hor, float vert)
    {
        character.JumpCommand(hor, vert);
    }
    public void StartTargetting()
    {
        isTargetting = true;
    }
    public void StopTargetting()
    {
        isTargetting = false;
    }

}
