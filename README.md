# 2-5d-character-controller

2.5d rigid body based character controller for use in unity 3d.
Features:
Rigidbody based, for easy interaction with physics components. Does not use 2d elements, or character controller.
Arbitrary down/gravity axis to allow circular/planet type levels, wall/ceiling walking. Set gravityFocus to (0, -1000000, 0) in the inspector to have a conventional 'down' direction.

Supports climb and descend slopes, rest on shallow slopes, wall grab, wall slide, wall jump, jetpack, parachute.

WIP:
Ledge grab and pull up, background grab.

Not planned:
Moving platform support.

Planned:
Animation, Unity prefab, item usage and aiming, grab/throw/kick physics objects, ladders, crouch

To use:
Create a cube, and attach to it a rigidbody component. Also attach a fixed joint.

Import the 3 scripts RigidBodyController, PlayerInput, and CharacterControllerEnums.

Attach RigidBodyController and PlayerInput to the cube.

In the inspector, click and drag the Rigid Body Controller script component to the 'Character' variable of Player Input.

In the inspector, set the size of the physic Materials array to 4. Set the elements 0 through 3 to be the CharGroundIdle, CharZeroFriction, CharWallGrab, and CharWallSlide materials, respectively.

Create a collision layer called 'Obstacle'. Set any terrain to be in this collision layer.

It is recommended to increase the gravity of the horizontal and vertical axis in the input menu to 1000, as the controller is set up for digital rather than analogue control.

