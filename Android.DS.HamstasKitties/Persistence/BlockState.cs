using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework;
using HnK.Mechanics;

namespace HnK.Persistence
{
    /// <summary>
    /// Represents the Block data to persiste on disk
    /// </summary>
    [DataContract()]
    public class BlockState
    {
        public BlockState() { }

        /// <summary>
        /// Creates a new BlockData instance from given Block instance.
        /// </summary>
        /// <param name="block">Instance to create the data to persist.</param>
        /// <returns></returns>
        public static BlockState CreateFromModel(Block block)
        {
            if (block == null)
            {
                return null;
            }

            BlockState data = new BlockState();
            data.Position = block.Position;
            data.Type = block.Type;
            data.SpecialType = block.CurrentSpecialType;
            data.Column = block.ColumnIndex;
            data.Row = block.RowIndex;

            return data;
        }

        [DataMemberAttribute()]
        public Vector2 Position { get; set; }

        [DataMemberAttribute()]
        public Block.BlockTypes Type { get; set; }

        [DataMemberAttribute()]
        public Block.SpecialTypes SpecialType { get; set; }

        [DataMemberAttribute()]
        public int Column { get; set; }

        [DataMemberAttribute()]
        public int Row { get; set; }
    }
}
