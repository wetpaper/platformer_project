using Godot;
using System;

public class Player : KinematicBody2D
{

    private Vector2 movement = Vector2.Zero;

    private float move_Speed = 180f;

    private float gravity = 20f;

    private float jump_Force = -700f;

    private Vector2 up_Dir = Vector2.Up;

    private float dashDistance = 40f;

    private bool isDashing;

    private float dashProgress;

    private float cooldown = 1f;

    private int wallJump = 3;

    private float runningFriction = 0.6f;

    private float stoppingFriction = 0.8f;

    private bool isWallJumping;

    private float airFriction = 0.78f;

    private AnimatedSprite animation;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        animation = GetNode<AnimatedSprite>("Animation");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {

        PlayerMovement(delta);
        Friction();

    }

    void Friction()
    {

        bool isRunning = Input.IsActionPressed("move_left") || Input.IsActionPressed("move_right");
        if (!isRunning && IsOnFloor())
        {

            movement.x *= stoppingFriction;

        }
        else if (isWallJumping && isDashing == false) 
        {

            movement.x *= airFriction;

        }
        else
        {

            movement.x *= runningFriction;

        } 

    }

    void PlayerMovement(float delta) //reminder to self after project try turning most if/else in here to a function
    {

        movement.y += gravity;

        float baseCharPosX = movement.x;

        if (Input.IsActionPressed("move_right"))
        {

            movement.x += move_Speed;

            AnimateMovement(true, true);

        }
        else if (Input.IsActionPressed("move_left"))
        {

            movement.x += -move_Speed;

            AnimateMovement(true, false);

        }
        else
        {

            AnimateMovement(false, false);

        }

        if (IsOnFloor()) // jump 
        {

            wallJump = 3; //reset wall jump
            isWallJumping = false;
            movement.y = Input.IsActionJustPressed("jump") ? jump_Force : movement.y;

        } // jump

        if (isWallJumping == true) // wall jump | wall jump
        {

            move_Speed = 110f;

        }
        else
        {

            move_Speed = 180f;

        }

        if (wallJump > 0)
        {

            if (IsOnWall() && Input.IsActionJustPressed("jump"))
            {

                wallJump--;
                isWallJumping = true;

                if (Input.IsActionPressed("move_left"))
                {
                    
                    movement.x = -jump_Force * 3;
                    movement.y = jump_Force;

                }
                else if (Input.IsActionPressed("move_right"))
                {

                    movement.x = jump_Force * 3;
                    movement.y = jump_Force;

                }

            }

        } // walljump | wall jump

        if (Input.IsActionJustPressed("dash"))  // dash -------------------------------------------------------
        {

            if (Input.IsActionPressed("move_right"))
            {

                if (isDashing == false)
                {

                    isDashing = true;

                }

            }
            else if (Input.IsActionPressed("move_left"))
            {

                if (isDashing == false)
                {

                    isDashing = true;

                }

            }

        }

        if (isDashing)
        {

            dashProgress += delta;

            if (dashProgress < 0.1f)
            {

                movement.x = baseCharPosX + (dashDistance * ((dashProgress * 500) / 1)) * Math.Sign(movement.x);
                movement = MoveAndSlide(movement, up_Dir); //linear
                movement.y = 0;
                return;

            }

            else if (isDashing && dashProgress > cooldown)
            {

                dashProgress = 0;
                isDashing = false;

            }

        } // dash ---------------------------------------------------------------------------------------------

        movement = MoveAndSlide(movement, up_Dir);

    } // player movement

    void AnimateMovement(bool moving, bool moveRight) // animation
    {

        if (moving)
        {

            animation.Play("walk");

            if (moveRight) 
            {

                animation.FlipH = true;

            }
            else
            {

                animation.FlipH = false;

            }

        }
        else
        {

            animation.Play("idle");
            
        }

    }// animation

    void _on_body_entered(PhysicsBody2D body)// respawn | reminder to self after project to try using loops instead of endless "else if"
    {

        if (body.IsInGroup("trap")) // level1
        {

            this.GlobalPosition = new Vector2(175, 675);

        }
        else if (body.IsInGroup("trap2")) // level2 
        {

            this.GlobalPosition = new Vector2(3468, 2245);

        }
        else if (body.IsInGroup("trap3")) // level3
        {

            this.GlobalPosition = new Vector2(132, 4588);

        }

    } // respawn

}












