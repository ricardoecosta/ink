using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using HamstasKitties.Management;
using HamstasKitties.Constants;
using HamstasKitties.UI;

namespace HamstasKitties.Mechanics
{
    public class BlocksMiniPhysicsEngine : IUpdateable
    {
        public BlocksMiniPhysicsEngine(List<LayerObject> blocksListRef)
        {
            ObjectsListRef = blocksListRef;
        }

        public void Update(TimeSpan time) { }

        private bool IsBlockStateValidForCollisionTesting(Block block)
        {
            return block.State != Block.States.IdleInNextLine && block.State != Block.States.Disposed;
        }

        public bool GetClosestLineIntersection(float currentPosX, float currentPosY, out Vector2 hCollisionPoint,
                                               out Vector2 vCollisionPoint, Block currentBlock, int precisionInPixels,
                                               out Block hCollisionBlock, out Block vCollisionBlock)
        {
            if (precisionInPixels <= 0)
            {
                precisionInPixels = 1;
            }

            Rectangle rect = currentBlock.GetCollisionArea(1);

            //     _______
            //  <--|---  | (vTopP1 to vTopP2)
            //     |     |
            //  <--|---  | (vBottomP1 to vBottomP2)
            Vector2 hTopP1 = new Vector2(rect.Center.X, rect.Top + precisionInPixels);
            Vector2 hTopP2 = new Vector2(currentPosX, rect.Top + precisionInPixels);
            Vector2 hBottomP1 = new Vector2(rect.Center.X, rect.Bottom - precisionInPixels);
            Vector2 hBottomP2 = new Vector2(currentPosX, rect.Bottom - precisionInPixels);

            //   (vLeftP1 to vLeftP2)
            //   ^   ^ (vRightP1 to vRightP2)
            //  _|___|_
            //  ||   ||
            //  ||   ||
            //  |_____|
            Vector2 vLeftP1 = new Vector2(rect.Left + precisionInPixels, rect.Center.Y);
            Vector2 vLeftP2 = new Vector2(rect.Left + precisionInPixels, currentPosY);
            Vector2 vRightP1 = new Vector2(rect.Right - precisionInPixels, rect.Center.Y);
            Vector2 vRightP2 = new Vector2(rect.Right - precisionInPixels, currentPosY);

            return GetClosestLineIntersection(ref hTopP1, ref hTopP2, ref hBottomP1, ref hBottomP2,
                                              ref vLeftP1, ref vLeftP2, ref vRightP1, ref vRightP2,
                                               out hCollisionPoint, out vCollisionPoint, currentBlock,
                                               out hCollisionBlock, out vCollisionBlock);
        }

        /// <summary>
        /// Verifies if exists any block that intersects with given line p1->p2 and return the closest (from p1) intersection point.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="outValue"></param>
        /// <param name="blockToAvoid"></param>
        /// <returns></returns>
        public bool GetClosestLineIntersection(ref Vector2 hTopP1, ref Vector2 hTopP2,
                                               ref Vector2 hBottomP1, ref Vector2 hBottomP2,
                                               ref Vector2 vLeftP1, ref Vector2 vLeftP2,
                                               ref Vector2 vRightP1, ref Vector2 vRightP2,
                                               out Vector2 hCollisionPoint, out Vector2 vCollisionPoint,
                                               Block blockToAvoid,
                                               out Block hCollisionBlock, out Block vCollisionBlock)
        {
            float hDistance = Math.Min(PhysicsTools.DistanceBetweenPoints(ref hTopP1, ref hTopP2), PhysicsTools.DistanceBetweenPoints(ref hBottomP1, ref hBottomP2));
            float vDistance =  Math.Min(PhysicsTools.DistanceBetweenPoints(ref vLeftP1, ref vLeftP2), PhysicsTools.DistanceBetweenPoints(ref vRightP1, ref vRightP2));
            Vector2 hPointToReturn = Vector2.Zero;
            Vector2 vPointToReturn = Vector2.Zero;
            hCollisionPoint = Vector2.Zero;
            vCollisionPoint = Vector2.Zero;
            hCollisionBlock = null;
            vCollisionBlock = null;
            foreach (var objToTestAgainst in ObjectsListRef)
            {
                Block blockToTestAgainst = objToTestAgainst as Block;
                if (blockToTestAgainst != null &&
                    blockToTestAgainst != blockToAvoid &&
                    blockToTestAgainst.State != Block.States.IdleInNextLine &&
                    blockToTestAgainst.State != Block.States.Disposed)
                {
                    Rectangle rect = blockToTestAgainst.GetCollisionArea(1);
                    //horizontal Top.
                    if (PhysicsTools.LineIntersectsWithRectangle(ref rect, ref hTopP1, ref hTopP2, out hCollisionPoint))
                    {
                        float newDistance = PhysicsTools.DistanceBetweenPoints(ref hTopP1, ref hCollisionPoint);
                        if (hDistance > newDistance && hCollisionPoint != Vector2.Zero)
                        {
                            hCollisionBlock = blockToTestAgainst;
                            hPointToReturn = hCollisionPoint;
                            hDistance = newDistance;
                        }
                    }

                    //horizontal Bottom.
                    if (PhysicsTools.LineIntersectsWithRectangle(ref rect, ref hBottomP1, ref hBottomP2, out hCollisionPoint))
                    {
                        float newDistance = PhysicsTools.DistanceBetweenPoints(ref hBottomP1, ref hCollisionPoint);
                        if (hDistance > newDistance && hCollisionPoint != Vector2.Zero)
                        {
                            hCollisionBlock = blockToTestAgainst;
                            hPointToReturn = hCollisionPoint;
                            hDistance = newDistance;
                        }
                    }

                    //vertical left.
                    if (PhysicsTools.LineIntersectsWithRectangle(ref rect, ref vLeftP1, ref vLeftP2, out vCollisionPoint))
                    {
                        float newDistance = PhysicsTools.DistanceBetweenPoints(ref vLeftP1, ref vCollisionPoint);
                        if (vDistance > newDistance && vCollisionPoint != Vector2.Zero)
                        {
                            vCollisionBlock = blockToTestAgainst;
                            vPointToReturn = vCollisionPoint;
                            vDistance = newDistance;
                        }
                    }

                    //vertical right.
                    if (PhysicsTools.LineIntersectsWithRectangle(ref rect, ref vRightP1, ref vRightP2, out vCollisionPoint))
                    {
                        float newDistance = PhysicsTools.DistanceBetweenPoints(ref vRightP1, ref vCollisionPoint);
                        if (vDistance > newDistance && vCollisionPoint != Vector2.Zero)
                        {
                            vCollisionBlock = blockToTestAgainst;
                            vPointToReturn = vCollisionPoint;
                            vDistance = newDistance;
                        }
                    }
                }
            }
            hCollisionPoint = hPointToReturn;
            vCollisionPoint = vPointToReturn;
            return (Math.Abs(hDistance) > 0 && hCollisionPoint != Vector2.Zero) ||
                   (Math.Abs(vDistance) > 0 && vCollisionPoint != Vector2.Zero);
        }


        public void CheckBlockOverlapping()
        {
            Block[,] gridToCkeckFatalCollitions = new Block[GlobalConstants.NumberOfBlockGridRows, GlobalConstants.NumberOfBlockGridColumns];
            List<Block> blocksToRemove = null;
            foreach (var block in ObjectsListRef)
            {
                Block blockToTestAgainst = block as Block;
                if (blockToTestAgainst != null && (blockToTestAgainst.State == Block.States.Idle || blockToTestAgainst.State == Block.States.MatchingMode))
                {
                    //PART:  Check if exists any layer in the same house that another layer.
                    if (gridToCkeckFatalCollitions[blockToTestAgainst.RowIndex, blockToTestAgainst.ColumnIndex] == null)
                    {
                        gridToCkeckFatalCollitions[blockToTestAgainst.RowIndex, blockToTestAgainst.ColumnIndex] = blockToTestAgainst;
                    }
                    else //BOOOOMMMMM
                    {
                        if (blocksToRemove == null)
                        {
                            blocksToRemove = new List<Block>();
                        }
                        blocksToRemove.Add(blockToTestAgainst);
                        blocksToRemove.Add(gridToCkeckFatalCollitions[blockToTestAgainst.RowIndex, blockToTestAgainst.ColumnIndex]);
                    }
                }
            }

            if (blocksToRemove != null)
            {
                foreach (Block block in blocksToRemove)
                {
                    block.OrderRemoval(Block.RemovalEffectEnum.SimpleExplosion);
                }
            }
        }

        /// <summary>
        /// Verifies if exits any Block on fisrt row.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool HasAnyBlockAtPosition(int row, int column)
        {
            foreach (var block in ObjectsListRef)
            {
                Block blockToTestAgainst = block as Block;
                if (blockToTestAgainst != null &&
                    blockToTestAgainst.State != Block.States.IdleInNextLine &&
                    blockToTestAgainst.State != Block.States.Disposed &&
                    blockToTestAgainst.ColumnIndex == column &&
                    blockToTestAgainst.RowIndex == row)
                {
                    return true;
                }
            }
            return false;
        }
        private List<LayerObject> ObjectsListRef { get; set; }
    }
}
