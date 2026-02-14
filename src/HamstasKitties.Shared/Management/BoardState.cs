using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HamstasKitties.Mechanics;
using HamstasKitties.Constants;

namespace HamstasKitties.Persistence
{
    /// <summary>
    /// Represents board data to persist on system.
    /// </summary>
    [DataContract()]
    public class BoardState
    {
        public BoardState()
        {
            BlocksOnBoard = new List<BlockState>();
        }

        /// <summary>
        /// Loads board data from model.
        /// </summary>
        /// <param name="board">Board Model</param>
        public void LoadFromModel(Block[][] board)
        {
            if (board == null)
            {
                return;
            }

            for (int i = 0; i < GlobalConstants.NumberOfBlockGridRows; i++)
            {
                for (int j = 0; j < GlobalConstants.NumberOfBlockGridColumns; j++)
                {
                    Block block = board[i][j];
                    if (block != null)
                    {
                        BlockState blockData = BlockState.CreateFromModel(block);
                        BlocksOnBoard.Add(blockData);
                    }
                }
            }
        }

        [DataMemberAttribute()]
        public List<BlockState> BlocksOnBoard { get; set; }
    }
}
