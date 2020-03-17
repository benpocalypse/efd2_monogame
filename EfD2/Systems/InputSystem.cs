
using ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EfD2.Components;

namespace EfD2.Systems
{
    public class InputSystem
    {
        public Filter filterMatch
        {
            get { return new Filter().AllOf(typeof(Input)); }
        }

        public Filter filterMovableMatch
        {
            get { return new Filter().AllOf(typeof(Input), typeof(Positionable), typeof(Movable)); }
        }

        public InputSystem()
        {
        }

        public void Update(GameTime gameTime)
        {
            GetInputs(gameTime);
            ProcessMovement(gameTime);
        }

        public void GetInputs(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMatch))
            {
                var input = e.GetComponent<Input>();

                // FIXME - Might not want to clear this every time
                input.CurrentInput.Clear();

                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    input.CurrentInput.Add(InputValue.Right);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    input.CurrentInput.Add(InputValue.Left);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.W))
                {
                    input.CurrentInput.Add(InputValue.Up);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    input.CurrentInput.Add(InputValue.Down);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.J))
                {
                    input.CurrentInput.Add(InputValue.A);

                    // FIXME - this isn't the right place to do this.
                    var sword = EntityMatcher.GetEntity("thesword");
                    sword.GetComponent<Drawable>().Visible = true;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.K))
                {
                    input.CurrentInput.Add(InputValue.B);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    input.CurrentInput.Add(InputValue.Start);
                }

                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    input.CurrentInput.Add(InputValue.Select);
                }
            }
        }

        public void ProcessMovement(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Globals g = Globals.Instance;

            foreach (Entity e in EntityMatcher.GetMatchedEntities(filterMovableMatch))
            {
                var position = e.GetComponent<Positionable>();
                var move = e.GetComponent<Movable>();
                var draw = e.GetComponent<Drawable>();

                bool accelerating = false;

                position.PreviousPosition = position.CurrentPosition;
                move.PreviousDirection = move.CurrentDirection;

                // If the component reacts to input, process it here
                if (e.HasComponent<Input>() == true)
                {
                    var input = e.GetComponent<Input>();

                    if (input.CurrentInput.Contains(InputValue.Right))
                    {
                        move.CurrentDirection = Direction.Right;
                        draw.FlipOnXAxis = false;
                        accelerating = true;
                    }

                    if (input.CurrentInput.Contains(InputValue.Left))
                    {
                        move.CurrentDirection = Direction.Left;
                        draw.FlipOnXAxis = true;
                        accelerating = true;
                    }

                    if (input.CurrentInput.Contains(InputValue.Up))
                    {
                        move.CurrentDirection = Direction.Up;
                        accelerating = true;
                    }

                    if (input.CurrentInput.Contains(InputValue.Down))
                    {
                        move.CurrentDirection = Direction.Down;
                        accelerating = true;
                    }
                }

                if (accelerating == true)
                {
                    // Update our acceleration based on direction
                    if (move.CurrentDirection == move.PreviousDirection)
                    {
                        if (move.Acceleration < move.MaxAcceleration)
                            move.Acceleration += g.Acceleration;
                    }
                    else
                    {
                        move.Acceleration = 0.0f;
                    }

                    switch (move.CurrentDirection)
                    {
                        case Direction.Right:
                            position.SetX(position.CurrentPosition.X + ((move.MoveSpeed * move.Acceleration) * delta));
                            break;

                        case Direction.Left:
                            position.SetX(position.CurrentPosition.X - ((move.MoveSpeed * move.Acceleration) * delta));
                            break;

                        case Direction.Up:
                            position.SetY(position.CurrentPosition.Y - ((move.MoveSpeed * move.Acceleration) * delta));
                            break;

                        case Direction.Down:
                            position.SetY(position.CurrentPosition.Y + ((move.MoveSpeed * move.Acceleration) * delta));
                            break;
                    }
                }
                else
                {
                    if (move.Acceleration > 0.0f)
                        move.Acceleration -= g.Decceleration;

                    if (move.Acceleration <= 0.0f)
                    {
                        move.Acceleration = 0.0f;
                        move.CurrentDirection = Direction.None;
                    }

                    switch (move.PreviousDirection)
                    {
                        case Direction.Right:
                            position.SetX(position.CurrentPosition.X + ((move.MoveSpeed * move.Acceleration) * delta));
                            break;

                        case Direction.Left:
                            position.SetX(position.CurrentPosition.X - ((move.MoveSpeed * move.Acceleration) * delta));
                            break;

                        case Direction.Up:
                            position.SetY(position.CurrentPosition.Y - ((move.MoveSpeed * move.Acceleration) * delta));
                            break;

                        case Direction.Down:
                            position.SetY(position.CurrentPosition.Y + ((move.MoveSpeed * move.Acceleration) * delta));
                            break;
                    }
                }

                // FIXME - this will need to be handled properly elsewhere...for now, it stays here.
                var anim = e.GetComponent<Drawable>();
                if (anim != null)
                {
                    if (move.CurrentDirection != Direction.None)
                    {
                        anim.Type = AnimationType.Moving;
                    }
                    else
                    {
                        anim.Type = AnimationType.Idle;
                    }
                }
            }
        }
    }
}
