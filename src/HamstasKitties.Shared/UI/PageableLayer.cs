using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using HamstasKitties.Animation.Tween;

namespace HamstasKitties.UI
{
    public abstract class PageableLayer : Layer
    {
        public PageableLayer(Scene scene, int zOrder, int pageWidth) :
            base(scene, LayerTypes.Interactive, Vector2.Zero, zOrder, true)
        {
            PageWidth = pageWidth;
            Pages = new List<LayerObject>();
        }

        protected abstract void OnPageMoved(Vector2 pos);

        public override void Initialize()
        {
            base.Initialize();

            // Register for swipe gestures.
            ParentScene.OnFlickLeft += OnFlickToLeftHandler;
            ParentScene.OnFlickRight += OnFlickToRightHandler;
        }

        public void AddPage(LayerObject page)
        {
            page.IsTouchEnabled = true;

            page.OnTouchDown += (sender, point) =>
            {
                if (!IsDragging && SlidePageTweener == null)
                {
                    IsDragging = true;
                    LastXPosition = LastXPositionOnTouchDown = point.X;
                }
            };

            page.OnDragging += (sender, point) =>
            {
                float fromOnTouchDownDragDelta = Math.Abs(point.X - LastXPositionOnTouchDown);
                if (IsDragging && fromOnTouchDownDragDelta > MinDraggingThreshold)
                {
                    UpdatePagesPosition(point.X - LastXPosition);
                }

                LastXPosition = point.X;
            };

            page.OnTouchReleased += (sender, point) =>
            {
                if (IsDragging)
                {
                    SnapToPage();
                    IsDragging = false;
                }
            };

            Pages.Add(page);
            page.AttachToParentLayer();

            MinXPos = -((Pages.Count - 1) * PageWidth + PageWidth / 2.1f);
        }

        public void ChangePage(int page)
        {
            if (SlidePageTweener == null && page > -1 && page < (Pages.Count + 1))
            {
                CurrentPage = page;
                CreateSlideTweener(Position.X, -(CurrentPage * PageWidth), TransitionDurationInSeconds);
                SlidePageTweener.Start();
            }
        }

        public void Dispose()
        {
            foreach (LayerObject page in Pages)
            {
                page.DetachFromParent();
                page.Dispose();
            }

            Pages.Clear();
            ParentScene.OnFlickLeft -= OnFlickToLeftHandler;
            ParentScene.OnFlickRight -= OnFlickToRightHandler;
        }

        private void UpdatePagesPosition(float increment)
        {
            float nextXPosition = (Position.X + increment);
            nextXPosition = MathHelper.Clamp(nextXPosition, MinXPos, PageWidth / 2.1f);
            Position = new Vector2(nextXPosition, Position.Y);
            OnPageMoved(Position);
        }

        private void SnapToPage()
        {
            if (SlidePageTweener == null)
            {
                CurrentPage = (int)(Math.Abs(Math.Round((float)Position.X / PageWidth)));
                ChangePage(CurrentPage);
            }
        }

        protected bool IsFirstPageSelected()
        {
            return CurrentPage == 0;
        }

        protected bool IsLastPageSelected()
        {
            return CurrentPage == (Pages.Count - 1);
        }

        #region IUpdateable

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);

            if (SlidePageTweener != null)
            {
                SlidePageTweener.Update(elapsedTime);
            }
        }

        #endregion

        #region Event Handlers

        private void OnFlickToLeftHandler(object sender, EventArgs args)
        {
            if (SlidePageTweener == null && !IsLastPageSelected())
            {
                IsDragging = false;

                CurrentPage++;
                CreateSlideTweener(Position.X, -(CurrentPage * PageWidth), TransitionDurationInSeconds);
                SlidePageTweener.Start();
            }
        }

        private void OnFlickToRightHandler(object sender, EventArgs args)
        {
            if (SlidePageTweener == null && !IsFirstPageSelected())
            {
                IsDragging = false;

                CurrentPage--;
                CreateSlideTweener(Position.X, -(CurrentPage * PageWidth), TransitionDurationInSeconds);
                SlidePageTweener.Start();
            }
        }

        private void CreateSlideTweener(float from, float to, float time)
        {
            SlidePageTweener = new Tweener(from, to, time, (t, b, c, d) => Quadratic.EaseOut(t, b, c, d), false);

            SlidePageTweener.OnUpdate += (value) =>
            {
                Position = new Vector2(value, Position.Y);
                OnPageMoved(Position);
            };

            SlidePageTweener.OnFinished += (senderObj, argsObj) =>
            {
                SlidePageTweener = null;
            };
        }

        #endregion

        public int PageWidth { get; protected set; }
        private float MinXPos { get; set; }
        private float LastXPosition { get; set; }
        private float LastXPositionOnTouchDown { get; set; }
        private bool IsDragging { get; set; }
        protected List<LayerObject> Pages { get; private set; }
        public int CurrentPage { get; protected set; }
        protected Tweener SlidePageTweener { get; set; }

        private const int MinDraggingThreshold = 50;
        private const float TransitionDurationInSeconds = 0.3f;
    }
}
