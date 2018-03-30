using System;
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
						//AddDoor(UP, true);
						//AddDoor(DOWN, false);
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
							//AddDoor(UP, true);
							//AddDoor(RIGHT, false);
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
							//AddDoor(RIGHT, true);
							//AddDoor(LEFT, false);
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

					//AddDoor(ucDoor, true);
					//AddDoor(ucDoor2, false);
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
						//AddDoor(LEFT, true);
						//AddDoor(RIGHT, false);
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
						//AddDoor(UP, true);
						//AddDoor(DOWN, false);
					}
				break;
				default:
					break;
			}

			DecorateMapTiles();
			DrawMap();
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
		/*
		void AddDoor(Direction direction, bool bEntrance)
		{
		    // If our entry door is to be on the top, scan left to right, top to bottom for a spot
		    // to add it.
		    if(direction == Direction.Up)
		    {
		        for(int j = 0; j < MAPHEIGHT; j++)
		        {
		            for(int i = 0; i < MAPWIDTH; i++)
		            {
		                if((MAP_TileIs(i, j) == MT_WALL_TOP) || (MAP_TileIs(i, j) == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
		                        objEntrance.ucX = i+GLB_RandomNum(3, 7);
		                        objEntrance.ucY = j;
		                        objEntrance.ucType = DOOR;
		                    }
		                    else
		                    {
		                        objExit.ucX = i+GLB_RandomNum(3,7);
		                        objExit.ucY = j;
		                        objExit.ucType = DOOR;
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
		                if((MAP_TileIs(i, j) == MT_WALL_TOP) || (MAP_TileIs(i, j) == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
		                        objEntrance.ucX = i-GLB_RandomNum(1,4);
		                        objEntrance.ucY = j;
		                        objEntrance.ucType = DOOR;
		                    }
		                    else
		                    {
		                        objExit.ucX = i-GLB_RandomNum(1,4);
		                        objExit.ucY = j;
		                        objExit.ucType = DOOR;
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
		                if((MAP_TileIs(i, j) == MT_WALL_TOP) || (MAP_TileIs(i, j) == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
		                        objEntrance.ucX = i;
		                        objEntrance.ucY = j-GLB_RandomNum(3,6);
		                        objEntrance.ucType = DOOR;
		                    }
		                    else
		                    {
		                        objExit.ucX = i;
		                        objExit.ucY = j-GLB_RandomNum(3,6);
		                        objExit.ucType = DOOR;
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
		                if((MAP_TileIs(i, j) == WALL_TOP) || (MAP_TileIs(i, j) == MT_WALL_MID))
		                {
		                    if(bEntrance == true)
		                    {
		                        objEntrance.ucX = i;
		                        objEntrance.ucY = j-GLB_RandomNum(2,4);
		                        objEntrance.ucType = DOOR;
		                    }
		                    else
		                    {
		                        objExit.ucX = i;
		                        objExit.ucY = j-GLB_RandomNum(2,4);
		                        objExit.ucType = DOOR;
		                    }
		                
		                    return;
		                }
		            }
		        }
		    }
		}
		*/

		private void ClearMap()
		{
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

			Entity[,] MapLayout = new Entity[MAPWIDTH, MAPHEIGHT];

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

			// Create all the entities we'll need
			for (int x = 0; x < MAPWIDTH; x++)
			{
				for (int y = 0; y < MAPHEIGHT; y++)
				{
					if (MapArray[x, y] != MT_EMPTY)
						MapLayout[x, y] = entityPool.CreateEntity("Map" + x + ", " + y);
				}
			}

			for (int i = 0; i < MAPWIDTH; i++)
			{
				for (int j = 0; j < MAPHEIGHT; j++)
				{
					int ucTile = MapArray[i, j];

					switch (ucTile)
					{
						case MT_FLOOR:
							MapLayout[i, j] += new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET*8) + (i*8), (MAP_Y_OFFSET * 8) + (j * 8)) };
							MapLayout[i, j] += new Drawable(floorAnim);
							break;
							
						case MT_WALL_TOP:
							MapLayout[i, j] += new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET * 8) + (i * 8), (MAP_Y_OFFSET * 8) + (j * 8)) };
							MapLayout[i, j] += new Drawable(wallTop);
							MapLayout[i, j] += new Collidable() { Type = EntityType.Wall };
							break;
						case MT_WALL_MID:
							MapLayout[i, j] += new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET * 8) + (i * 8), (MAP_Y_OFFSET * 8) + (j * 8)) };
							MapLayout[i, j] += new Drawable(wallMiddle);
							MapLayout[i, j] += new Collidable() { Type = EntityType.Wall };
							break;

						case MT_WALL_TOP_LEFT_CORNER:
							MapLayout[i, j] += new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET * 8) + (i * 8), (MAP_Y_OFFSET * 8) + (j * 8)) };
							MapLayout[i, j] += new Drawable(wallTopLeftCorner);
							MapLayout[i, j] += new Collidable() { Type = EntityType.Wall };
							break;

						case MT_WALL_TOP_RIGHT_CORNER:
							MapLayout[i, j] += new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET * 8) + (i * 8), (MAP_Y_OFFSET * 8) + (j * 8)) };
							MapLayout[i, j] += new Drawable(wallTopRightCorner);
							MapLayout[i, j] += new Collidable() { Type = EntityType.Wall };
							break;

						case MT_WALL_BOTTOM_LEFT_CORNER:
							MapLayout[i, j] += new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET * 8) + (i * 8), (MAP_Y_OFFSET * 8) + (j * 8)) };
							MapLayout[i, j] += new Drawable(wallBottomLeftCorner);
							MapLayout[i, j] += new Collidable() { Type = EntityType.Wall };
							break;

						case MT_WALL_BOTTOM_RIGHT_CORNER:
							MapLayout[i, j] += new Positionable() { CurrentPosition = new Vector2((MAP_X_OFFSET * 8) + (i * 8), (MAP_Y_OFFSET * 8) + (j * 8)) };
							MapLayout[i, j] += new Drawable(wallBottomRightCorner);
							MapLayout[i, j] += new Collidable() { Type = EntityType.Wall };
							break;
							
						default:
							break;

					}
				}
			}
		}
	}
}
