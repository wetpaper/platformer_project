using Godot;
using System;

public class Player : KinematicBody2D
{

    private Vector2 movement = Vector2.Zero;

    private float move_Speed = 400f;

    private float gravity = 20f;

    private float jump_Force = -700f;

    private Vector2 up_Dir = Vector2.Up;

    private float dashDistance = 40f;

    private bool isDashing;

    private float dashProgress;

    private int cooldown = 1;

    private int wallJump = 3;

    // animation
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

    }

    void PlayerMovement(float delta)
    {

        movement.y += gravity;

        float baseCharPosX = movement.x;

        if (Input.IsActionPressed("move_right"))
        {

            movement.x = move_Speed;

            AnimateMovement(true, true);

        }
        else if (Input.IsActionPressed("move_left"))
        {

            movement.x = -move_Speed;

            AnimateMovement(true, false);

        }

        else
        {

            movement.x = 0f;

            AnimateMovement(false, false);

        }

        if (wallJump > 0)
        {
            
            if (IsOnWall() && !IsOnFloor() && Input.IsActionJustPressed("jump"))
            {

                wallJump--;

                if (Input.IsActionPressed("move_left"))
                {

                    movement.x = move_Speed * 15;
                    movement.y = jump_Force;
                    

                }
                else if (Input.IsActionPressed("move_right"))
                {

                    movement.x = -move_Speed * 15;
                    movement.y = jump_Force;
                    

                }

            }

        }

        if (IsOnFloor()) // only able to jump once if on the floor
        {

            movement.y = Input.IsActionJustPressed("jump") ? jump_Force : movement.y;
            wallJump = 3;

        } // jump

        if (Input.IsActionJustPressed("dash") && Input.IsActionPressed("move_right")) // dash ----------------
        {

            if (isDashing == false)
            {

                isDashing = true;

            }

        }
        else if (Input.IsActionJustPressed("dash") && Input.IsActionPressed("move_left"))
        {

            if (isDashing == false)
            {

                isDashing = true;

            }

        }

        if (isDashing)
        {

            dashProgress += delta;

            if (dashProgress < 0.1f)
            {

                movement.x = baseCharPosX + (dashDistance * ((dashProgress * 500) / 1)) * Math.Sign(movement.x);
                movement = MoveAndSlide(movement, up_Dir); //make movement linear instead of si·nus·oi·dal
                movement.y = 0;
                return;

            }

            else if (isDashing && dashProgress > cooldown)
            {

                dashProgress = 0;
                isDashing = false;

            }

        } // dash --------------------------------------------------------------------------------------------

        movement = MoveAndSlide(movement, up_Dir);

    } // player movement

    void AnimateMovement(bool moving, bool moveRight)
    {

        if (moving)
        {

            animation.Play("walk");

            if (moveRight) // flip sprite horizontally if facing (moving) the corresponding direction
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

    void _on_body_entered(PhysicsBody2D body)// teleport player to spawn when colliding with spikes
    {

        if (body.IsInGroup("trap")) // level1
        {

            this.GlobalPosition = new Vector2(175, 675);

        }
        else if (body.IsInGroup("trap2")) // level2 
        {

            this.GlobalPosition = new Vector2(831, 2689);

        }
        else if (body.IsInGroup("trap3")) // level3
        {

        }

    }

}












