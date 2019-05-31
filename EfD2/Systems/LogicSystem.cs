using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using ECS;
using EfD2.Components;
using EfD2.Interfaces;

namespace EfD2.Systems
{
    public class LogicSystem : IEcsSystem
    {
        public Filter filterMatch
        {
            get { return new Filter().AllOf(typeof(GameState)); }
        }

        public Filter filterActorMatch
        {
            get { return new Filter().AllOf(typeof(Actor)); }
        }

        public Filter filterEventMatch
        {
            get { return new Filter().AllOf(typeof(Events)); }
        }

        public void Update(GameTime gameTime)
        {
            ProcessEvents();
            ClearActiveEvents();
            ProcessGameState();
            ProcessActorActions();
        }

        public void ProcessEvents()
        {
            foreach (Entity e in EntityMatcher.GetMatchedEntities(filterEventMatch))
            {
                var ev = e.GetComponent<Events>();

                foreach (Event _event in ev.EventList.Where(_ => _.Triggered == true))
                {
                    switch (_event.Type)
                    {
                        case GameEventType.EnteredLevel:
                            break;

                        case GameEventType.ExitedLevel:
                            break;
                    }
                }
            }
        }

        public void ProcessGameState()
        {
            var gameState = EntityMatcher.GetMatchedEntities(filterMatch).First().GetComponent<GameState>();

            // Make sure if we're supposed to change state, we do.
            if (gameState.RequestedState != gameState.CurrentState)
            {
                ProcessExitState(gameState.CurrentState);
                ProcessEnterState(gameState.RequestedState);

                gameState.CurrentState = gameState.RequestedState;
            }

            //  Now, process our current state.
            switch (gameState.CurrentState)
            {
                case GameStateType.Intro:
                    gameState.CurrentState = GameStateType.TitleScreen;
                    break;

                case GameStateType.TitleScreen:
                    gameState.CurrentState = GameStateType.Playing;
                    break;

                case GameStateType.Playing:
                    break;

                case GameStateType.GameOver:
                    break;

            }
        }

        private void ProcessEnterState(GameStateType state)
        {
            switch (state)
            {
                case GameStateType.Intro:
                    break;

                case GameStateType.TitleScreen:
                    break;

                case GameStateType.Playing:
                    {
                        var ev = EntityMatcher.GetMatchedEntities(filterEventMatch).First().GetComponent<Events>();
                        ev.EventList.Add(new Event() { Triggered = true, Trigger = EventTrigger.GameState, Type = GameEventType.EnteredLevel });

                        Entity openSpaceNearExit = EntityMatcher.GetEntity("OpenSpaceNextToEntrance");
                        if (openSpaceNearExit != null)
                        {
                            var col = e.GetComponent<Collidable>();
                            e.GetComponent<Positionable>().CurrentPosition =
                                new Vector2(
                                            openSpaceNearExit.GetComponent<Positionable>().CurrentPosition.X + ((8 - col.BoundingBox.Width) / 2),
                                            openSpaceNearExit.GetComponent<Positionable>().CurrentPosition.Y + ((8 - col.BoundingBox.Height) / 2)
                                            );

                            // FIXME - This isn't the right place to handle this. When changing maps, all entities CollidingEntities 
                            //         should be cleared already. Not sure why this is necessary...
                            e.GetComponent<Positionable>().PreviousPosition = e.GetComponent<Positionable>().CurrentPosition;
                            col.CollidingEntities.Clear();
                        }
                    }
                    break;

                case GameStateType.GameOver:
                    break;
            }
        }

        private void ProcessExitState(GameStateType state)
        {
            switch (state)
            {
                case GameStateType.Intro:
                    break;

                case GameStateType.TitleScreen:
                    break;

                case GameStateType.Playing:
                    break;

                case GameStateType.GameOver:
                    break;
            }
        }

        public void ProcessActorActions()
        {
            foreach (Entity e in EntityMatcher.GetMatchedEntities(filterActorMatch))
            {
                var act = e.GetComponent<Actor>();

                // FIXME - Is this really where/how we should handle this?
                switch (act.Type)
                {
                    case ActorType.Player:
                        {
                            var gameState = EntityMatcher.GetEntity("The Game").GetComponent<GameState>();

                            if (gameState.CurrentState == GameStateType.EnterMap)
                            {
                                System.Console.WriteLine("LogicSystem().DoActorStuff();");
                                Entity openSpaceNearExit = EntityMatcher.GetEntity("OpenSpaceNextToEntrance");
                                if (openSpaceNearExit != null)
                                {
                                    var col = e.GetComponent<Collidable>();
                                    e.GetComponent<Positionable>().CurrentPosition =
                                        new Vector2(
                                                    openSpaceNearExit.GetComponent<Positionable>().CurrentPosition.X + ((8 - col.BoundingBox.Width) / 2),
                                                    openSpaceNearExit.GetComponent<Positionable>().CurrentPosition.Y + ((8 - col.BoundingBox.Height) / 2)
                                                    );

                                    // FIXME - This isn't the right place to handle this. When changing maps, all entities CollidingEntities 
                                    //         should be cleared already. Not sure why this is necessary...
                                    e.GetComponent<Positionable>().PreviousPosition = e.GetComponent<Positionable>().CurrentPosition;
                                    col.CollidingEntities.Clear();
                                }
                            }

                            // How do we handle attacking?
                            var attackState = e.GetComponent<Attacking>();

                        }
                        break;

                    default:
                        break;
                }
            }
        }

        public void ClearActiveEvents()
        {
            foreach (Entity e in EntityMatcher.GetMatchedEntities(filterEventMatch))
            {
                e.GetComponent<Events>().EventList.Clear(); 
            }
        }
    }
}
