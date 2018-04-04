﻿using System;
using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using SharpECS;
using EfD2.Components;
using EfD2;
using Microsoft.Xna.Framework.Content;


namespace EfD2.Systems
{
	internal class MapSystem 
		: EntitySystem
	{
		private const int MAPWIDTH =		24;
		private const int MAPHEIGHT =		18;

		// Defined storage for our 4 tiles
		private const int MT_EMPTY = 0;
		private const int MT_FLOOR = 1;
		private const int MT_WALL_TOP = 2;
		private const int MT_WALL_MID = 3;
		private const int MT_WALL_TOP_LEFT_CORNER = 4;
		private const int MT_WALL_TOP_RIGHT_CORNER = 5;
		private const int MT_WALL_BOTTOM_LEFT_CORNER = 6;
		private const int MT_WALL_BOTTOM_RIGHT_CORNER = 7;

		private const int MT_EXIT = 98;
		private const int MT_ENTRANCE = 99;

		private const int MAP_X_OFFSET = 3;
		private const int MAP_Y_OFFSET = 7;

		private int[,] MapArray = new int[24, 18];

		private ContentManager	Content;
		private EntityPool entityPool;

		public MapSystem(EntityPool pool, ref ContentManager _content)
			: base(pool, typeof(Positionable), typeof(Drawable))
        { 
			Content =  _content;
			entityPool = pool;

			for (int x = 0; x < MAPWIDTH; x++)
			{
				for (int y = 0; y < MAPHEIGHT; y++)
				{
					MapArray[x,y] = MT_EMPTY;
				}
			}
		}

		public void GenerateMap()
		{
			Random r = new Random();

			ClearMap();

			for (int x = 0; x < MAPWIDTH; x++)
			{
				for (int y = 0; y < MAPHEIGHT; y++)
				{
					MapArray[x, y] = MT_EMPTY;
				}
			}

			switch(r.Next(0, 3))
			{
				case 0:
					int ucWidth = 0;
			        int ucWidth2 = 0;
			        int ucHeight = 0;
			        int ucHeight2 = 0;
			        
			        int ucRoomOnePosition = r.Next(0,2);
			        int ucRoomTwoPosition = r.Next(0,2);

			        // First, decide the room width/height.
			        ucWidth = r.Next(9, 15);
			        ucHeight = r.Next(10, 16);
			        
			        // Draw room 1 first. Draw the left, right, top, and then bottom walls.
			        DrawLine(0, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)),
							 0, ucHeight + (ucRoomOnePosition * (MAPHEIGHT - ucHeight - 1)), MT_WALL_TOP);
					DrawLine(ucWidth, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)),
							 ucWidth, ucHeight + (ucRoomOnePosition * (MAPHEIGHT - ucHeight - 1)), MT_WALL_TOP);
					DrawLine(1, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)),
							 ucWidth - 1, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)), MT_WALL_MID);
					DrawLine(0, ucHeight + (ucRoomOnePosition * (MAPHEIGHT - ucHeight - 1)),
							 ucWidth, ucHeight + (ucRoomOnePosition * (MAPHEIGHT - ucHeight - 1)), MT_WALL_MID);

					// Finally, fill the floor of room 1 with 1 of our 3 floor tile types.
					FloodFill(ucWidth / 2, (ucRoomOnePosition * (MAPHEIGHT - (ucHeight / 2))) + 1, MT_FLOOR);


					// Now draw room 2. First, decide the room width/height.
					ucWidth2 = r.Next(6, 22 - ucWidth);
					ucHeight2 = r.Next(9, 17);

					// Draw the left, right, top, and then bottom walls.
					DrawLine(MAPWIDTH - ucWidth2, (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2)),
							 MAPWIDTH - ucWidth2, ucHeight2 + (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2 - 1)), MT_WALL_TOP);
					DrawLine(MAPWIDTH - 1, (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2)),
							 MAPWIDTH - 1, ucHeight2 + (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2 - 1)), MT_WALL_TOP);
					DrawLine(MAPWIDTH - ucWidth2 + 1, (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2)),
							 MAPWIDTH - 2, (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2)), MT_WALL_MID);
					DrawLine(MAPWIDTH - ucWidth2, ucHeight2 + (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2 - 1)),
							 MAPWIDTH - 1, ucHeight2 + (ucRoomTwoPosition * (MAPHEIGHT - ucHeight2 - 1)), MT_WALL_MID);

					// Finally, fill the floor of room 2 with 1 of our 3 floor tile types.
					FloodFill(MAPWIDTH - (ucWidth2 / 2), (ucRoomTwoPosition * (MAPHEIGHT - (ucHeight2 / 2))) + 1, MT_FLOOR);

					// Now lets connect the two rooms with a hallway...

					// If both rooms are on the same level..
					if (ucRoomOnePosition == ucRoomTwoPosition)
					{//...then lets just run a straight hallway between them.
					 // Draw the top.
						DrawLine(ucWidth, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)) + (ucHeight / 2) - 1,
								 MAPWIDTH - ucWidth2, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)) + (ucHeight / 2) - 1, MT_WALL_MID);

						// Draw the floor.
						DrawLine(ucWidth, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)) + (ucHeight / 2),
								 MAPWIDTH - ucWidth2, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)) + (ucHeight / 2), MT_FLOOR);

						// Now draw the bottom.
						DrawLine(ucWidth + 1, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)) + (ucHeight / 2) + 1,
								 MAPWIDTH - ucWidth2 - 1, (ucRoomOnePosition * (MAPHEIGHT - ucHeight)) + (ucHeight / 2) + 1, MT_WALL_MID);

						// FIXME - add these back in
						AddDoor(Direction.Up, true);
						AddDoor(Direction.Down, false);

						DebugThing();
					}
					else
					{//...we need to make a connecting hallway with a bend.
					 // If room 1 is on the top...
						if (ucRoomOnePosition == 0)
						{// Then draw a right elbow.
							DrawLine((ucWidth / 2) - 1, ucHeight, (ucWidth / 2) - 1, MAPHEIGHT - 2, MT_WALL_TOP);
							DrawLine((ucWidth / 2) - 1, MAPHEIGHT - 1, MAPWIDTH - ucWidth2, MAPHEIGHT - 1, MT_WALL_MID);

							DrawLine((ucWidth / 2), ucHeight, (ucWidth / 2) + 1, MAPHEIGHT - 2, MT_FLOOR);
							DrawLine((ucWidth / 2), MAPHEIGHT - 2, MAPWIDTH - ucWidth2, MAPHEIGHT - 2, MT_FLOOR);

							DrawLine((ucWidth / 2) + 1, ucHeight, (ucWidth / 2) + 1, MAPHEIGHT - 3, MT_WALL_TOP);
							DrawLine((ucWidth / 2) + 1, MAPHEIGHT - 3, MAPWIDTH - ucWidth2, MAPHEIGHT - 3, MT_WALL_MID);

							// FIXME - add these back in
							AddDoor(Direction.Up, true);
							AddDoor(Direction.Right, false);

							DebugThing();
						}
						else
						{//...finally, room 2 must be on top.
							DrawLine((ucWidth / 2) - 1, MAPHEIGHT - ucHeight - 1, (ucWidth / 2) - 1, 0, MT_WALL_TOP);
							DrawLine((ucWidth / 2), 0, MAPWIDTH - ucWidth2, 0, MT_WALL_MID);

							DrawLine((ucWidth / 2), MAPHEIGHT - ucHeight, (ucWidth / 2), 1, MT_FLOOR);
							DrawLine((ucWidth / 2), 1, MAPWIDTH - ucWidth2, 1, MT_FLOOR);

							DrawLine((ucWidth / 2) + 1, MAPHEIGHT - ucHeight - 1, (ucWidth / 2) + 1, 2, MT_WALL_TOP);
							DrawLine((ucWidth / 2) + 2, 2, MAPWIDTH - ucWidth2 - 1, 2, MT_WALL_MID);

							// FIXME - add these back in
							AddDoor(Direction.Right, true);
							AddDoor(Direction.Left, false);

							DebugThing();
						}
					}
					break;

				case 1:
					ucWidth = r.Next(18, MAPWIDTH);
					ucHeight = r.Next(14, MAPHEIGHT);
					int ucDoor = r.Next(0, 4);//(UP, RIGHT);

					// Draw the walls.
					DrawLine((MAPWIDTH / 2) - (ucWidth / 2), (MAPHEIGHT / 2) - (ucHeight / 2),
							 (MAPWIDTH / 2) - (ucWidth / 2), (MAPHEIGHT / 2) + (ucHeight / 2), MT_WALL_TOP);
					DrawLine((MAPWIDTH / 2) + (ucWidth / 2), (MAPHEIGHT / 2) - (ucHeight / 2),
							 (MAPWIDTH / 2) + (ucWidth / 2), (MAPHEIGHT / 2) + (ucHeight / 2), MT_WALL_TOP);
					DrawLine((MAPWIDTH / 2) - (ucWidth / 2), (MAPHEIGHT / 2) + (ucHeight / 2),
							 (MAPWIDTH / 2) + (ucWidth / 2), (MAPHEIGHT / 2) + (ucHeight / 2), MT_WALL_MID);
					DrawLine((MAPWIDTH / 2) - (ucWidth / 2) + 1, (MAPHEIGHT / 2) - (ucHeight / 2),
							 (MAPWIDTH / 2) + (ucWidth / 2) - 1, (MAPHEIGHT / 2) - (ucHeight / 2), MT_WALL_MID);

					// Fill the floor.
					FloodFill(MAPWIDTH / 2, MAPHEIGHT / 2, MT_FLOOR);

					// And add the doors.
					int ucDoor2 = r.Next(0, 4);

					while (ucDoor == ucDoor2)
					{
						ucDoor2 = r.Next(0, 4);
					}

					// FIXME - these need to be random
					AddDoor(Direction.Up, true);
					AddDoor(Direction.Down, false);

					DebugThing();
				break;

				case 2:
					if (r.Next(0, 2) == 0)
					{
						ucHeight = r.Next(8, 12);

						// Draw the walls.
						DrawLine(0, (MAPHEIGHT / 2) - (ucHeight / 2),
								 0, (MAPHEIGHT / 2) + (ucHeight / 2), MT_WALL_TOP);
						DrawLine(MAPWIDTH - 1, (MAPHEIGHT / 2) - (ucHeight / 2),
								 MAPWIDTH - 1, (MAPHEIGHT / 2) + (ucHeight / 2), MT_WALL_TOP);
						DrawLine(0, (MAPHEIGHT / 2) + (ucHeight / 2),
								 MAPWIDTH - 1, (MAPHEIGHT / 2) + (ucHeight / 2), MT_WALL_MID);
						DrawLine(1, (MAPHEIGHT / 2) - (ucHeight / 2),
								 MAPWIDTH - 2, (MAPHEIGHT / 2) - (ucHeight / 2), MT_WALL_MID);

						// Fill the floor.
						FloodFill(MAPWIDTH / 2, MAPHEIGHT / 2, MT_FLOOR);

						// And add the doors.
						AddDoor(Direction.Left, true);
						AddDoor(Direction.Right, false);

						DebugThing();
					}
					else
					{
						ucWidth = r.Next(8, 13);

						// Draw the walls.
						DrawLine((MAPWIDTH / 2) - (ucWidth / 2), 0,
								 (MAPWIDTH / 2) - (ucWidth / 2), MAPHEIGHT - 2, MT_WALL_TOP);
						DrawLine((MAPWIDTH / 2) + (ucWidth / 2), 0,
								 (MAPWIDTH / 2) + (ucWidth / 2), MAPHEIGHT - 2, MT_WALL_TOP);
						DrawLine((MAPWIDTH / 2) - (ucWidth / 2) + 1, 0,
								 (MAPWIDTH / 2) + (ucWidth / 2) - 1, 0, MT_WALL_MID);
						DrawLine((MAPWIDTH / 2) - (ucWidth / 2), MAPHEIGHT - 1,
								 (MAPWIDTH / 2) + (ucWidth / 2), MAPHEIGHT - 1, MT_WALL_MID);

						// Fill the floor.
						FloodFill(MAPWIDTH / 2, MAPHEIGHT / 2, MT_FLOOR);

						// And add the doors.
						AddDoor(Direction.Up, true);
						AddDoor(Direction.Down, false);

						DebugThing();
					}
				break;
				default:
					break;
			}

			DecorateMapTiles();
			DrawMap();
			 
			DebugThing();
		}

		public Vector2 GetOpenSpaceNearEntrance()
		{
			var pos = entityPool.GetEntity("Entrance").GetComponent<Positionable>();

			int x = Convert.ToInt32((pos.CurrentPosition.X / 8) - MAP_X_OFFSET);
			int y = Convert.ToInt32((pos.CurrentPosition.Y / 8) - MAP_Y_OFFSET);

			if (x < (MAPWIDTH-1) && MapArray[x + 1, y] == MT_FLOOR)
				return new Vector2(pos.CurrentPosition.X + 8, pos.CurrentPosition.Y);
			
			if (x > 0  && MapArray[x - 1, y] == MT_FLOOR)
				return new Vector2(pos.CurrentPosition.X - 8, pos.CurrentPosition.Y);

			if (y < (MAPHEIGHT- 1) && MapArray[x, y + 1] == MT_FLOOR)
				return new Vector2(pos.CurrentPosition.X, pos.CurrentPosition.Y + 8);

			if (y > 0 && MapArray[x, y - 1] == MT_FLOOR)
				return new Vector2(pos.CurrentPosition.X, pos.CurrentPosition.Y - 8);

			return new Vector2();
		}

		private void DebugThing()
		{
			if (entityPool.DoesEntityExist("Entrance"))
			{
				var comp = entityPool.GetEntity("Entrance");
				var pos = comp.GetComponent<Positionable>();

				Console.WriteLine("x = " + ((pos.CurrentPosition.X / 8) - MAP_X_OFFSET) + ", y = " + ((pos.CurrentPosition.Y / 8) - MAP_Y_OFFSET));
			}

			if (entityPool.DoesEntityExist("Exit"))
			{
				var comp = entityPool.GetEntity("Exit");
				var pos = comp.GetComponent<Positionable>();

				Console.WriteLine("x = " + ((pos.CurrentPosition.X / 8) - MAP_X_OFFSET) + ", y = " + ((pos.CurrentPosition.Y / 8) - MAP_Y_OFFSET));
			}
		}

		///****************************************************************************
		/// Fills the specified x,y empty space with the destination tile until a 
		/// non-empty tile is encountered.
		///****************************************************************************
		void FloodFill(int x, int y, int type)
		{
		   int ucLeft;
		   int ucRight;
		   int ucInLine = 1;
		 
		   /// Search to the left, filling along the way.
		   ucLeft = ucRight = x;

		   while (ucInLine == 1)
			{
				MapArray[ucLeft, y] = type;
				ucLeft--;
				ucInLine = (ucLeft < 0) ? 0 : Convert.ToInt32((MapArray[ucLeft, y] == MT_EMPTY));
			}

		   ucLeft++;

		   /// Search to the right, filling along the way.
		   ucInLine = 1;

		   while(ucInLine == 1)
		   {
				MapArray[ucLeft, y] = type;
		     	ucRight++;
			 	ucInLine = (ucRight > MAPWIDTH-1) ? 0 : Convert.ToInt32((MapArray[ucRight,y] == MT_EMPTY));
		   }

		   ucRight--;

		   /// Fill the top and bottom.
		   for(int i = ucLeft; i <= ucRight; i++)
		   {
		     if( y > 0 && (MapArray[i, y-1] == MT_EMPTY))
			 {
		         FloodFill(i, y - 1, type);
			 }

		     if( y < MAPHEIGHT-1 && (MapArray[i, y+1] == MT_EMPTY))
			 {
		         FloodFill(i, y + 1, type);
			 }
		    }
		}

		///****************************************************************************
		/// Implements Bresenham's line drawing algorithm to efficiently draw lines
		/// from one point to another in any direction.
		///****************************************************************************
		private void DrawLine(int startX, int startY, 
        				      int endX, int endY, int tile) 
		{
			int distance;
		    int xerr = 0;
			int yerr = 0;
			int delta_x;
			int delta_y;
		    int incx, incy;

		    /// Compute an x and y delta.
		    delta_x = endX - startX;
		    delta_y = endY - startY;

		    // Compute the direction of the incrementation in the x direction
		    // An increment of 0 means either a horizontal or vertical line.
		    if(delta_x > 0)
			{
				incx = 1;
			}
		    else 
			{
				if(delta_x==0) 
				{
					incx = 0;
				}
				else 
				{
					incx = -1;
				}
			}    

			// Compute the direction of the incrementation in the y direction
		    // An increment of 0 means either a horizontal or vertical line.
		    if(delta_y > 0)
			{
				incy = 1;
			}
		    else 
			{
				if(delta_y == 0)
				{
					incy = 0;
				} 
				else
				{
					incy = -1;
				}
			}

		    /// Determine which direction is the greater incrementation.
		    delta_x = Math.Abs(delta_x);
		    delta_y = Math.Abs(delta_y);

		    if(delta_x > delta_y) 
			{
				distance = delta_x;
			}
		    else
			{
				distance = delta_y;
			}

		    /// Now, finally draw our line.
		    for(uint t = 0; t <= (distance + 1); t++) 
			{
				MapArray[startX, startY] = tile;
		        
		        xerr += delta_x;
		        yerr += delta_y;

		        if(xerr > distance)
				{
		            xerr -= distance;
		            startX += incx;
		        }
		        if(yerr > distance)
				{
		            yerr -= distance;
		            startY += incy;
		        }
		    }
		}

		private void DecorateMapTiles()
		{
			/*
			 * Top Left Corner:
			 *  - Nothing to the left or we're at the leftmost tile
			 *  - Nothing above
			 * 
			 * Top Right Corner:
			 *  - Nothing to the right or we're at the rightmost tile
			 *  - Nothing above
			 * 
			 * Bottom Left Corner:
			 *  - Nothing to the left or we're at the leftmost tile
			 *  - Nothing below
			 * 
			 * Bottom Right Corner:
			 *  - Nothing to the right or we're at the rightmost tile
			 *  - Nothing below
			*/

			for (int i = 0; i < MAPWIDTH; i++)
			{
				for (int j = 0; j < MAPHEIGHT; j++)
				{
					if (MapArray[i, j] >= MT_WALL_TOP)
					{
						// Top Left Corner cases
						if ((i > 0) && (j > 0) &&
						   (MapArray[i - 1, j] < 2) &&
						   (MapArray[i, j - 1] < 2))
							MapArray[i, j] = MT_WALL_TOP_LEFT_CORNER;

						if((i == 0) && (j > 0) &&
						   (MapArray[i, j - 1] < 2))
							MapArray[i, j] = MT_WALL_TOP_LEFT_CORNER;

						if ((i > 0) && (j == 0) &&
						    (MapArray[i - 1, j] < 2))
							MapArray[i, j] = MT_WALL_TOP_LEFT_CORNER;

						if ((i == 0) && (j == 0) &&
						   (MapArray[i, j] != MT_EMPTY))
							MapArray[i, j] = MT_WALL_TOP_LEFT_CORNER;

						// Top Right Corner cases
						if ((i <= (MAPWIDTH-2)) && (j > 0) &&
						   (MapArray[i + 1, j] < 2) &&
						   (MapArray[i, j - 1] < 2))
							MapArray[i, j] = MT_WALL_TOP_RIGHT_CORNER;

						if ((i == (MAPWIDTH-1)) && (j > 0) &&
						   (MapArray[i, j - 1] < 2))
							MapArray[i, j] = MT_WALL_TOP_RIGHT_CORNER;

						if ((i <= (MAPWIDTH - 2)) && (j == 0) &&
							(MapArray[i + 1, j] < 2))
							MapArray[i, j] = MT_WALL_TOP_RIGHT_CORNER;

						if ((i == (MAPWIDTH-1)) && (j == 0) &&
						   (MapArray[i, j] != MT_EMPTY))
							MapArray[i, j] = MT_WALL_TOP_RIGHT_CORNER;

						// Bottom Left Corner cases
						if ((i > 0) && (j <= (MAPHEIGHT-2)) &&
						   (MapArray[i - 1, j] < 2) &&
						   (MapArray[i, j + 1] < 2))
							MapArray[i, j] = MT_WALL_BOTTOM_LEFT_CORNER;

						if ((i == 0) && (j <= (MAPHEIGHT-2)) &&
						   (MapArray[i, j + 1] < 2))
							MapArray[i, j] = MT_WALL_BOTTOM_LEFT_CORNER;

						if ((i > 0) && (j == (MAPHEIGHT-1)) &&
							(MapArray[i - 1, j] < 2))
							MapArray[i, j] = MT_WALL_BOTTOM_LEFT_CORNER;

						if ((i == 0) && (j == (MAPHEIGHT-1)) &&
						   (MapArray[i, j] != MT_EMPTY))
							MapArray[i, j] = MT_WALL_BOTTOM_LEFT_CORNER;

						// Bottom Right Corner cases
						if ((i <= (MAPWIDTH - 2)) && (j <= (MAPHEIGHT-2)) &&
						   (MapArray[i + 1, j] < 2) &&
						   (MapArray[i, j + 1] < 2))
							MapArray[i, j] = MT_WALL_BOTTOM_RIGHT_CORNER;

						if ((i == (MAPWIDTH - 1)) && (j <= (MAPHEIGHT-2)) &&
						   (MapArray[i, j + 1] < 2))
							MapArray[i, j] = MT_WALL_BOTTOM_RIGHT_CORNER;

						if ((i <= (MAPWIDTH - 2)) && (j == (MAPHEIGHT-1)) &&
							(MapArray[i + 1, j] < 2))
							MapArray[i, j] = MT_WALL_BOTTOM_RIGHT_CORNER;

						if ((i == (MAPWIDTH - 1)) && (j == (MAPHEIGHT-1)) &&
						   (MapArray[i, j] != MT_EMPTY))
							MapArray[i, j] = MT_WALL_BOTTOM_RIGHT_CORNER;
					}
				}
			}
		}


		///****************************************************************************
		/// This function adds doors to our map based on the type of map we've 
		/// generated. Essentially, we want the entrance and exit to a room to be
		/// fairly far apart to force the player to navigate our maze.
		///****************************************************************************
		void AddDoor(Direction direction, bool bEntrance)
		{
			Animation entrance = new Animation(AnimationType.None, Content.Load<Texture2D>("door1_1"));
			Animation exit = new Animation(AnimationType.None, Content.Load<Texture2D>("door1_2"));
			Random r = new Random();

		    // If our entry door is to be on the top, scan left to right, top to bottom for a spot
		    // to add it.
		    if(direction == Direction.Up)
		    {
		        for(int j = 0; j < MAPHEIGHT; j++)
		        {
		            for(int i = 0; i < MAPWIDTH; i++)
		            {
		                if((MapArray[i, j] == MT_WALL_TOP) || (MapArray[i, j] == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8) + (r.Next(3, 8)*8), (MAP_Y_OFFSET * 8) + (j * 8)), ZOrder = 1.0f },
									new Drawable(entrance),
									new Collidable() { Type = EntityType.Entrance }
								};

								var tempEntity = entityPool.CreateEntity("Entrance");
								tempEntity.AddComponents(componentCollection);
		                    }
		                    else
		                    {
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8) + (r.Next(3, 8)*8), (MAP_Y_OFFSET * 8) + (j * 8)) , ZOrder = 1.0f},
									new Drawable(exit),
									new Collidable() { Type = EntityType.Exit }
								};

								var tempEntity = entityPool.CreateEntity("Exit");
								tempEntity.AddComponents(componentCollection);
		                    }
		                    
		                    return;
		                }                
		            }            
		        }
		    }

		    // If our door is to be on the bottom, start in the lower right hand corner, and count
		    // right to left, bottom to top.
		    if(direction == Direction.Down)
		    {
		        for(int j = (MAPHEIGHT-1); j > 0; j--)
		        {
		            for(int i = (MAPWIDTH-1); i > 0; i--)
		            {
		                if((MapArray[i, j] == MT_WALL_TOP) || (MapArray[i, j] == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
		                        IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8) - (r.Next(1, 5)*8), (MAP_Y_OFFSET * 8) + (j * 8)), ZOrder = 1.0f },
									new Drawable(entrance),
									new Collidable() { Type = EntityType.Entrance }
								};

								var tempEntity = entityPool.CreateEntity("Entrance");
								tempEntity.AddComponents(componentCollection);
		                    }
		                    else
		                    {
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8) - (r.Next(1, 5)*8), (MAP_Y_OFFSET * 8) + (j * 8)), ZOrder = 1.0f },
									new Drawable(exit),
									new Collidable() { Type = EntityType.Exit }
								};

								var tempEntity = entityPool.CreateEntity("Exit");
								tempEntity.AddComponents(componentCollection);
		                    }

		                    return;
		                }
		            }
		        }
		    }
		    
		    // If our door is to be on the right, start in the lower right hand corner, and count
		    // bottom to top, right to left.
		    if(direction == Direction.Right)
		    {
		        for(int i = (MAPWIDTH-1); i > 0; i--)
		        {
		            for(int j = (MAPHEIGHT-1); j > 0; j--)
		            {
		                if((MapArray[i, j] == MT_WALL_TOP) || (MapArray[i, j] == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
		                        IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + ((j*8) - (r.Next(3, 7) * 8))), ZOrder = 1.0f },
									new Drawable(entrance),
									new Collidable() { Type = EntityType.Entrance }
								};

								var tempEntity = entityPool.CreateEntity("Entrance");
								tempEntity.AddComponents(componentCollection);
		                    }
		                    else
		                    {
		                        IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + ((j*8) - (r.Next(3, 7) * 8))), ZOrder = 1.0f },
									new Drawable(exit),
									new Collidable() { Type = EntityType.Exit }
								};

								var tempEntity = entityPool.CreateEntity("Exit");
								tempEntity.AddComponents(componentCollection);
		                    }
		                    
		                    return;
		                }
		            }
		        }
		    }

		    // If our door is to be on the left, start in the lower left hand corner, and count
		    // bottom to top, left to right.
		    if(direction == Direction.Left)
		    {
		        for(int i = 0; i < MAPWIDTH; i++)
		        {
		            for(int j = (MAPHEIGHT-1); j > 0; j--)
		            {
		                if ((MapArray[i, j] == MT_WALL_TOP) || (MapArray[i, j] == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
		                        IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + ((j*8) - (r.Next(2, 5) * 8))), ZOrder = 1.0f },
									new Drawable(entrance),
									new Collidable() { Type = EntityType.Entrance }
								};

								var tempEntity = entityPool.CreateEntity("Entrance");
								tempEntity.AddComponents(componentCollection);
		                    }
		                    else
		                    {
		                        IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + ((j*8) - (r.Next(2, 5) * 8))), ZOrder = 1.0f },
									new Drawable(exit),
									new Collidable() { Type = EntityType.Exit }
								};

								var tempEntity = entityPool.CreateEntity("Exit");
								tempEntity.AddComponents(componentCollection);
		                    }
		                
		                    return;
		                }
		            }
		        }
		    }
		}

		private void ClearMap()
		{
			Entity ent;

			if(entityPool.DoesEntityExist("Entrance"))
			{
				ent = entityPool.GetEntity("Entrance");
				entityPool.DestroyEntity(ref ent);
			}

			if (entityPool.DoesEntityExist("Exit"))
			{
				ent = entityPool.GetEntity("Exit");
				entityPool.DestroyEntity(ref ent);
			}

			for (int x = 0; x < MAPWIDTH; x++)
			{
				for (int y = 0; y < MAPHEIGHT; y++)
				{
					if (MapArray[x, y] != MT_EMPTY)
					{
						if (entityPool.DoesEntityExist("Map" + x + ", " + y))
						{
							var e = entityPool.GetEntity("Map" + x + ", " + y);
							//e.RemoveAllComponents();
							entityPool.DestroyEntity(ref e);
						}
					}
				}
			}
		}

		///****************************************************************************
		/// Draws the current map structure in the center of the screen to allow room
		/// for the HUD to be drawn as well.
		///****************************************************************************
		private void DrawMap()
		{
			// Each time we draw a map, randomly pick one of the 3 types of floors
			// to add a little variety to our maps.
			Random r = new Random();
			int ucFloor = r.Next(0, 3);

			Animation floorAnim;
			Animation wallTop = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_2"));
			Animation wallTopLeftCorner = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_5"));
			Animation wallTopRightCorner = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_6"));
			Animation wallBottomLeftCorner = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_4"));
			Animation wallBottomRightCorner = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_3"));
			Animation wallMiddle = new Animation(AnimationType.None, Content.Load<Texture2D>("wall1_1"));

			Console.WriteLine("Floor = " + ucFloor);

			if (ucFloor == 1)
				floorAnim = new Animation(AnimationType.None, Content.Load<Texture2D>("floor1_2"));
			else if (ucFloor == 2)
				floorAnim = new Animation(AnimationType.None, Content.Load<Texture2D>("floor1_3"));
			else
				floorAnim = new Animation(AnimationType.None, Content.Load<Texture2D>("floor1_1"));

			for (int i = 0; i < MAPWIDTH; i++)
			{
				for (int j = 0; j < MAPHEIGHT; j++)
				{
					int ucTile = MapArray[i, j];

					switch (ucTile)
					{
						case MT_FLOOR:
							{
								IComponent[] componentCollection = new IComponent[]
								{
								new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) },
								new Drawable(floorAnim)
								};

								var tempEntity = entityPool.CreateEntity("Map" + i + ", " + j);
								tempEntity.AddComponents(componentCollection);
							}
							break;
							
						case MT_WALL_TOP:
							{
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) },
									new Drawable(wallTop),
									new Collidable() { Type = EntityType.Wall }
								};

								var tempEntity = entityPool.CreateEntity("Map" + i + ", " + j);
								tempEntity.AddComponents(componentCollection);
							}
							break;
						case MT_WALL_MID:
							{
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) },
									new Drawable(wallMiddle),
									new Collidable() { Type = EntityType.Wall }
								};

								var tempEntity = entityPool.CreateEntity("Map" + i + ", " + j);
								tempEntity.AddComponents(componentCollection);
							}
							break;

						case MT_WALL_TOP_LEFT_CORNER:
							{
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) },
									new Drawable(wallTopLeftCorner),
									new Collidable() { Type = EntityType.Wall }
								};

								var tempEntity = entityPool.CreateEntity("Map" + i + ", " + j);
								tempEntity.AddComponents(componentCollection);
							}
							break;

						case MT_WALL_TOP_RIGHT_CORNER:
							{
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) },
									new Drawable(wallTopRightCorner),
									new Collidable() { Type = EntityType.Wall }
								};

								var tempEntity = entityPool.CreateEntity("Map" + i + ", " + j);
								tempEntity.AddComponents(componentCollection);
							}
							break;

						case MT_WALL_BOTTOM_LEFT_CORNER:
							{
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) },
									new Drawable(wallBottomLeftCorner),
									new Collidable() { Type = EntityType.Wall }
								};

								var tempEntity = entityPool.CreateEntity("Map" + i + ", " + j);
								tempEntity.AddComponents(componentCollection);
							}
							break;

						case MT_WALL_BOTTOM_RIGHT_CORNER:
							{
								IComponent[] componentCollection = new IComponent[]
								{
									new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) },
									new Drawable(wallBottomRightCorner),
									new Collidable() { Type = EntityType.Wall }
								};

								var tempEntity = entityPool.CreateEntity("Map" + i + ", " + j);
								tempEntity.AddComponents(componentCollection);
							}
							break;
							
						default:
							break;
					}
				}
			}
		}
	}
}