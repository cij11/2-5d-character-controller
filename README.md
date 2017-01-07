# 2-5d-character-controller

2.5d rigid body based character controller for use in unity 3d.  
Features:  
Rigidbody based, for easy interaction with physics components. Does not use 2d elements, or character controller.  
Modular, hierarchical design:  
    Getting terrain info with CharacterContactSensor: Attaching CharacterContactSensor to a RigidBody lets that rigidbody use raycasts to determine if its standing on flat or steep terrain, adjacent to a wall, or airbourne. Requires included UpdateTimer class.  
    Arcade physics with CharacterMovementActuator: Once a rigidbody has a CharacterContactSensor, attach CharacterMovementActuator to apply arcade style control to the rigidbody. Requires included Physic Materials to be set in the unity inspector.  
    A scheme to convert player input into character controls via CharacterIntegrator.  
    Controllers for movement, aiming, firing, and weapon selection.  
    Virtual Joystick takes input from the player or AI.  
Arbitrary down/gravity axis to allow circular/planet type levels, wall/ceiling walking. Set gravityFocus to (0, -1000000, 0) in the inspector to have a conventional 'down' direction.  
Supports climb and descend slopes, rest on shallow slopes, wall grab/wall slide, wall jump, jetpack, parachute.  

Using CharacterContactSensor:  
Attach CharacterContactSensor to a rigidbody. To make the script more efficient, give a layer to the terrain gameobjects (Eg, 'Obstacle'). In the inspector, set the Collision Mask of CharacterContactSensor to 'Obstacle.
Make sure that UpdateTimer.cs is in the project folder.
    
Using CharacterMovementActuator:  
After attaching CharacterContactSensor to a rigidbody, attach CharacterMovementActuator to that same rigidbody. In the inspector, set the size of the PhysicMaterial array to 4. Set the four slots to CharGroundIdle, CharZeroFriction, CharWallGrab, and CharWallSlide, in that order.
