using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using HamstasKitties.Animation;
using HamstasKitties.Extensions;
using HamstasKitties.Utils;

namespace HamstasKitties.UI
{
	/// <summary>
	/// Layer that have the behavior of a List View.
	/// Attenttion to add a new item you must use the method AddItem.
	///
	/// ListView Types:
	///
	/// - List: Only lists the added items.
	/// - Details: Uses the added columns and added Item subitems to fill the table.
	///            If you have 2 columns to fill the table you must have foreach row the ListItem with one SubItem.
	/// - Tree: Not Implemented
	///
	/// </summary>
	public class ListView : Layer
	{
		public class ListViewColumn
		{
			public ListViewColumn (int width)
			{
				Width = width;
				StartPositionX = 0;
			}

			public int Width { get; set; }

			public float StartPositionX { get; set; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="scene">Parent scene.</param>
		/// <param name="zOrder">Z-Order value.</param>
		public ListView (Scene scene, int zOrder, ViewType type) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
		{
			LastItemYPosition = 0;
			VecticalVelocity = ZERO;
			Friction = DefaultFriction;
			InternalFriction = Friction;
			VType = type;
			Columns = new List<ListViewColumn> ();
			HasDefaultColumn = false;
			SpaceBetweenItems = 0;
			GDevice = ParentScene.Director.GraphicsDeviceManager.GraphicsDevice;
			YOffset = ZERO;
		}

        #region Inherited from Layer

		public override void Initialize ()
		{
			base.Initialize ();

			//Initializes default columns.
			switch (VType) {
			case ViewType.Tree:
			case ViewType.List:
			case ViewType.Details:
				if (Columns.Count == 0) {
					Columns.Add (new ListViewColumn (Width)); // Default column.
					HasDefaultColumn = true;
				}
				break;
			default:
				break;
			}
			ClippingRectangle = new Rectangle ((int)Position.X, (int)Position.Y, Width, Height);
			DoubleBuffer = new RenderTarget2D (GDevice, ClippingRectangle.Width, ClippingRectangle.Height);
			LayerObject touchObj = new LayerObject (this, null, new Vector2 (0, 0));
			touchObj.Size = new Point (ClippingRectangle.Width, ClippingRectangle.Height);
			touchObj.IsTouchEnabled = true;
			touchObj.OnTouchDown += new LayerObject.TouchHandler (FingerTouchDownHandler);
			touchObj.OnDragging += new LayerObject.TouchHandler (FingerMovingHandler);
			touchObj.OnTouchReleased += new LayerObject.TouchHandler (FingerReleasedHandler);
			TouchableLayer = new Layer (ParentScene, LayerTypes.Interactive, Vector2.Zero, (ZOrder + 1), true);
			TouchableLayer.Width = ClippingRectangle.Width;
			TouchableLayer.Height = ClippingRectangle.Height;
			TouchableLayer.AttachObject (touchObj);
			ParentScene.AddLayer (TouchableLayer);
		}

		public override void Draw (SpriteBatch spriteBatch)
		{
			GDevice.RasterizerState = RState;
			GDevice.ScissorRectangle = ClippingRectangle;
			spriteBatch.Begin (SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, GDevice.RasterizerState);
			LayerObject obj = null;
			for (int idx = 0; idx < LayerObjects.Count; idx++) {
				obj = LayerObjects [idx];
				obj.Position = new Vector2 (obj.Position.X, obj.Position.Y + YOffset);
				if (obj.IsVisible && ClippingRectangle.Intersects (new Rectangle ((int)obj.AbsolutePosition.X, (int)obj.AbsolutePosition.Y,
				                                                                 (int)obj.Size.X, (int)obj.Size.Y))) {
					obj.Draw (spriteBatch);
					// Draw subitems if view type is ListView::Details
					/*if (VType == ViewType.Details) {
						if (obj is ListViewItem) {
							DrawSubItems ((ListViewItem)obj, spriteBatch);
						}
					}*/
				}
			}
			spriteBatch.End ();
		}

		public override void Update (TimeSpan elapsedTime)
		{
			if (!IsDragging) {
				if (VecticalVelocity > DefaultVelocityToStop) {
					VecticalVelocity *= Friction;
					YOffset = GetProcessedDelta (VecticalVelocity);
				} else {
					VecticalVelocity = ZERO;
					YOffset = ZERO;
				}
			}
		}
		#endregion

		/// <summary>
		/// Adds new ListViewColumn to the list.
		/// </summary>
		/// <param name="item"></param>
		public void AddColumn (ListViewColumn column)
		{
			if (column != null) {
				if (HasDefaultColumn) {
					Columns.Clear ();
					HasDefaultColumn = false;
				}

				if (Columns.Count > 0) {
					ListViewColumn lastColumn = Columns.Last ();
					column.StartPositionX = lastColumn.StartPositionX + lastColumn.Width;
				} else {
					column.StartPositionX = ZERO;
				}
				Columns.Add (column);
			}
		}

		/// <summary>
		/// Adds new ListViewItem to the list.
		/// </summary>
		/// <param name="item"></param>
		public void AddItem (ListViewItem item)
		{
			if (item != null) {
				if (!item.IsSubItem) {
					AttachObject (item);
					item.Position = new Vector2 (ZERO, LastItemYPosition);
					LastItemYPosition += item.Size.Y + SpaceBetweenItems;
				} else { // Subitems are only visible for Details List.
					if (VType != ViewType.Details) {
						item.IsVisible = false;
						item.IsTouchEnabled = false;
					}
				}
			}
		}

		/// <summary>
		/// Removes the givens ListViewItem to the list.
		/// </summary>
		/// <param name="item"></param>
		public void RemoveItem (ListViewItem item)
		{
			if (item != null) {
				DetachObject (item);
			}
		}

		/// <summary>
		/// Handles touch down event of ListViewItem.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		private void FingerTouchDownHandler (LayerObject obj, Vector2 position)
		{
			LastTouchDownPosition = position;
			IsDragging = true;
			YOffset = ZERO;
			VecticalVelocity = ZERO;
			DirectionOfMotion = MotionDirection.Stopped;
		}

		/// <summary>
		/// Handles dragging event of ListViewItem.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		private void FingerMovingHandler (LayerObject obj, Vector2 position)
		{
			if (IsDragging && (position.Y != LastTouchDownPosition.Y)) {
				DirectionOfMotion = ProcessMotionDirection (position);
				float delta = MathHelper.Clamp (Math.Abs ((LastTouchDownPosition.Y - position.Y)), MinVelocityDelta, MaxVelocityDelta);
				if (DirectionOfMotion != MotionDirection.Stopped) {
					YOffset = GetProcessedDelta (delta);
				}
				VecticalVelocity = delta;
				//For motion
				LastTouchDownPosition = position;
			}
		}

		/// <summary>
		/// Handles touch released event of ListViewItem.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		private void FingerReleasedHandler (LayerObject obj, Vector2 position)
		{
			IsDragging = false;
		}

		/// <summary>
		/// Processes current delta (Test list limits, bouncing values, etc..)
		/// </summary>
		/// <param name="currentDelta"></param>
		/// <returns></returns>
		private float GetProcessedDelta (float delta)
		{
			float processedDelta = delta;
			LayerObject firstItem = LayerObjects.First ();
			LayerObject lastItem = LayerObjects.Last ();

			//check if items are all inside of the rectangle.
			if (firstItem.AbsolutePosition.Y >= ClippingRectangle.Top &&
			    (lastItem.AbsolutePosition.Y + lastItem.Size.Y) <= ClippingRectangle.Bottom) {
				return ZERO;
			}

			// Verifies list limits. Without bouncing effect.
			switch (DirectionOfMotion) {
			case MotionDirection.Down:
				{
					if ((firstItem.AbsolutePosition.Y + processedDelta) >= ClippingRectangle.Top) {
						processedDelta = Math.Abs (firstItem.AbsolutePosition.Y - ClippingRectangle.Top);
					}
					break;
				}
			case MotionDirection.Up:
				{
					if (((lastItem.AbsolutePosition.Y + lastItem.Size.Y) - processedDelta) <= ClippingRectangle.Bottom) {
						processedDelta = Math.Abs ((lastItem.AbsolutePosition.Y + lastItem.Size.Y) - ClippingRectangle.Bottom);
					}
					break;
				}
			default:
				break;
			}
			return processedDelta * ((DirectionOfMotion == MotionDirection.Up) ? -1 : 1);
		}

		/// <summary>
		/// Processes current motion direction.
		/// </summary>
		/// <param name="position"></param>
		private MotionDirection ProcessMotionDirection (Vector2 position)
		{
			if (LastTouchDownPosition.Y > position.Y) {
				return ListView.MotionDirection.Up;
			} else if (LastTouchDownPosition.Y < position.Y) {
				return ListView.MotionDirection.Down;
			} else {
				return MotionDirection.Stopped;
			}
		}

		/// <summary>
		/// Draws all subitems of given item.
		/// </summary>
		/// <param name="item"></param>
		private void DrawSubItems (ListViewItem item, SpriteBatch spriteBatch)
		{
			int count = item.SubItems.Count;
			for (int i = 0; i < count; i++) {
				ListViewItem subItem = item.SubItems [i];
				// update position
				float posX = (Columns.Count > (i + 1)) ? Columns [i + 1].StartPositionX : 0;
				subItem.Position = new Vector2 (posX, item.Position.Y);
				subItem.IsVisible = true;
				subItem.IsTouchEnabled = true;
				subItem.Draw (spriteBatch);
			}
		}

		/// <summary>
		/// Hide all subitems of given item.
		/// </summary>
		/// <param name="item"></param>
		private void HideSubItems (ListViewItem item)
		{
			item.SubItems.ForEach ((subItem) =>
			{
				subItem.IsVisible = false;
				subItem.IsTouchEnabled = false;
			});
		}

        #region enums

		public enum MotionDirection
		{
			Stopped,
			Up,
			Down,
		}

		public enum ViewType
		{
			List,
			Details,
			Tree
		}
        #endregion

        #region properties
		public ViewType VType { get; set; }

		private Vector2 LastTouchDownPosition { get; set; }

		private bool IsDragging { get; set; }

		private int LastItemYPosition { get; set; }

		private bool HasDefaultColumn { get; set; }

		public int SpaceBetweenItems{ get; set; }

		protected List<ListViewColumn> Columns { get; set; }

		//Motion elements
		private MotionDirection DirectionOfMotion { get; set; }

		public float Friction { get; set; }

		private float InternalFriction { get; set; }

		public float VecticalVelocity { get; set; }

		private float YOffset { get; set; }

		private Layer TouchableLayer { get; set; }

		private RenderTarget2D DoubleBuffer { get; set; }

		private GraphicsDevice GDevice { get; set; }

		//consts
		private const float DefaultFriction = 0.95f;
		private const float DefaultVelocityToStop = 1.0f;

		//consts
		private const float MinVelocityDelta = 0f;
		private const float MaxVelocityDelta = 200f;

		private const float ZERO = .0f;

		// For clipping
		public static RasterizerState RState = new RasterizerState ()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };
		private Rectangle ClippingRectangle = Rectangle.Empty;
        #endregion
	}
}
