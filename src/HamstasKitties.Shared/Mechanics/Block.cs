using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using HnK.Management;
using HnK.Scenes;
using HamstasKitties.Animation;
using HamstasKitties.Utils;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Extensions;
using System.Runtime.Serialization;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace HamstasKitties.Mechanics
{
    public class Block : LayerObject
    {
        public Block(Layer layer, BlockTypes type, int columnIndex, LevelBoardController levelBoardController)
            : base(layer, null, Vector2.Zero)
        {
            CurrentLevel = (Level)GameDirector.Instance.CurrentScene;
            LevelBoardController = levelBoardController;
            ColumnIndex = columnIndex;
            State = States.IdleInNextLine;
            Position = new Vector2(ColumnIndex * GlobalConstants.BlockSize + BirthPositionX, BirthPositionY);
            BlockBottomLimitY = GlobalConstants.NumberOfBlockGridRows * GlobalConstants.BlockSize + BirthPositionY;
            IsTouchEnabled = true;
            Type = type;
            PositionBeforeStartedToShake = DragOffset = Vector2.Zero;
            AccessoriesList = new List<LayerObject>();
            CollidingBlocks = new List<Block>();
            CurrentSpecialType = NextSpecialType = SpecialTypes.None;
            DragDirection = DragDirections.None;
            RedefineTexture(false);
            SetupTouchHandlers();
            SetupTimers();
            SetupAnimationTweeners();
            LastDraggingPosition = Vector2.Zero;
            OriginBlocks = new Dictionary<ulong, Block>();
        }

        private void EnableColorSelectionMode()
        {
            Texture texture = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.RainbowRing);
            ColorSelectionModeAccessory = new LayerObject(ParentLayer, texture, Position);
            ColorSelectionModeAccessory.ZOrder = int.MaxValue;
            ColorSelectionModeAccessory.AttachToParentLayer();

            Tweener colorSelectionModeTweener = new Tweener(0, 360, 5, Linear.EaseNone, false);

            colorSelectionModeTweener.OnUpdate += (value) =>
            {
                ColorSelectionModeAccessory.Rotation = value;
            };

            colorSelectionModeTweener.OnFinished += (sender, args) =>
            {
                colorSelectionModeTweener.Start();
            };

            TweenerCollection.Add((int)Tweeners.ColorSelectionModeTweener, colorSelectionModeTweener);
            AccessoriesList.Add(ColorSelectionModeAccessory);

            CurrentLevel.SelectedMultiColorBlock = this;
            colorSelectionModeTweener.Start();
        }

        public void UpgradeToSpecialType()
        {
            DisposeAllAccessories();
            Texture specialTypeTexture = null;
            switch (CurrentSpecialType)
            {
                case SpecialTypes.None:
                    break;

                case SpecialTypes.Bomb:
                    specialTypeTexture = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAccessory);
                    break;

                case SpecialTypes.MagicBomb:
                    specialTypeTexture = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.MagicBombAccessory);
                    break;

                case SpecialTypes.Goku:
                    specialTypeTexture = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAccessory);
                    break;

                default:
                    break;
            }

            LayerObject newAccessory = null;
            if (specialTypeTexture != null)
            {
                newAccessory = new LayerObject(ParentLayer, specialTypeTexture, Position, new Vector2(specialTypeTexture.Width / 2.0f, specialTypeTexture.Height / 2.0f));
                newAccessory.AttachToParentLayer();
                AccessoriesList.Add(newAccessory);
            }

            // Special cases. Upgrades with animated layer objects.
            switch (CurrentSpecialType)
            {
                case SpecialTypes.None:
                    break;
                case SpecialTypes.Bomb:
                    Texture[] animationTextures = new Texture[]
                    {
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame01),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame02),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame03),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame04),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame05),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame06),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame07),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame08),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame09),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame10),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame11),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame12),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame13),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BombAnimationFrame14)
                    };
                    FrameBasedAnimatedLayerObject bombAnimationLayerObject = new FrameBasedAnimatedLayerObject(ParentLayer, Position, animationTextures, 77, 70, 15);
                    bombAnimationLayerObject.Origin = Origin;
                    bombAnimationLayerObject.AttachToParentLayer();
                    bombAnimationLayerObject.Play(true);
                    AccessoriesList.Add(bombAnimationLayerObject);
                    break;
                case SpecialTypes.MagicBomb:
                    break;
                case SpecialTypes.Goku:
                    animationTextures = new Texture[]
                    {
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame01),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame02),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame03),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame04),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame05),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame06),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame07),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame08),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame09),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame10),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame11),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame12),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame13),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame14),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame15),
                        GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.GokuAnimationFrame16)
                    };
                    int textureWidth = animationTextures[0].Width;
                    int textureHeight = animationTextures[0].Height;
                    FrameBasedAnimatedLayerObject gokuAnimationLayerObject = new FrameBasedAnimatedLayerObject(ParentLayer, Position, animationTextures, textureWidth, textureHeight, 15);
                    gokuAnimationLayerObject.Origin = new Vector2(textureWidth / 2.0f, textureHeight / 2.0f);
                    gokuAnimationLayerObject.AttachToParentLayer();
                    gokuAnimationLayerObject.Play(true);
                    AccessoriesList.Add(gokuAnimationLayerObject);
                    break;
                default:
                    break;
            }
        }

        #region Private Methods

        private void UpdateSpecialTypeFlag(SpecialTypes specialType)
        {
            NextSpecialType = specialType;
        }

        private void UpdateAccessoriesPositions()
        {
            foreach (var accessory in AccessoriesList)
            {
                accessory.Position = Position;
            }
        }

        private void RedefineTexture(bool isBlinkingTexture)
        {
            ResourcesManager resourcesManager = GameDirector.Instance.CurrentResourcesManager;
            switch (Type)
            {
                case BlockTypes.Block1:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.Block1Blink : GameDirector.TextureAssets.Block1)));
                    break;

                case BlockTypes.Block2:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.Block2Blink : GameDirector.TextureAssets.Block2)));
                    break;

                case BlockTypes.Block3:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.Block3Blink : GameDirector.TextureAssets.Block3)));
                    break;

                case BlockTypes.Block4:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.Block4Blink : GameDirector.TextureAssets.Block4)));
                    break;

                case BlockTypes.Block5:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.Block5Blink : GameDirector.TextureAssets.Block5)));
                    break;

                case BlockTypes.RainbowHamsta:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.RainbowHamstaBlink : GameDirector.TextureAssets.RainbowHamsta)));
                    break;

                case BlockTypes.UnmovableBlock:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.UnmovableBlockBlink : GameDirector.TextureAssets.UnmovableBlock)));
                    break;

                case BlockTypes.GoldenBlock:
                    DefineTexture(resourcesManager.GetCachedTexture((int)(isBlinkingTexture ? GameDirector.TextureAssets.GoldenBlock : GameDirector.TextureAssets.GoldenBlock)));
                    break;

                default:
                    break;
            }
            Size = new Point(GlobalConstants.BlockSize, GlobalConstants.BlockSize);
        }

        private void SetupTimers()
        {
            TimerCollection = new TimerCollection(5);

            // Timers initialization
            Timer blinkAnimationCountdownTimer = new Timer(Rand.Next(MinBlinkAnimationCountdownTime, MaxBlinkAnimationCountdownTime));
            Timer blinkingAnimationTimer = new Timer(BlinkAnimationDuration);
            Timer disposeTimer = new Timer(DisposeTimerDuration);
            Timer specialTypeHintCountdownTimer = new Timer(SpecialTypeHintCountdownTimerDuration);
            Timer specialTypeFormationAnimationTimer = new Timer(SpecialTypeFormationAnimationTimerDuration);
            Timer goldenHamstaAnimationTimer = new Timer(GoldenHamstaAnimationDurationInSeconds);

            // Event handlers
            blinkAnimationCountdownTimer.OnFinished += (sender, args) =>
            {
                RedefineTexture(true);
                blinkingAnimationTimer.Start();
            };
            blinkingAnimationTimer.OnFinished += (sender, args) =>
            {
                RedefineTexture(false);
                blinkAnimationCountdownTimer.RedefineTimerDuration(Rand.Next(MinBlinkAnimationCountdownTime, MaxBlinkAnimationCountdownTime));
                blinkAnimationCountdownTimer.Start();
            };
            disposeTimer.OnFinished += (sender, args) =>
            {
                Dispose();
            };
            specialTypeFormationAnimationTimer.OnUpdate += (sender, args) =>
            {
                if (TargetBlockWithSpecialType.GetCollisionArea(1).Contains(AbsolutePosition.ToPoint()))
                {
                    specialTypeFormationAnimationTimer.Stop();
                    Hide();
                    IsTouchEnabled = false;
                    CurrentSpecialType = NextSpecialType = SpecialTypes.None;
                    TimerCollection.GetTimer((int)Timers.DisposeTimer).Start();
                }
                else
                {
                    Vector2 lookAheadVector = TargetBlockWithSpecialType.Position - Position;
                    AngleToMoveTo = (float)Math.Atan2(lookAheadVector.Y, lookAheadVector.X);
                    TimeSpan elapsedTime = ((Timer.TimerOnUpdateEventArgs)args).ElapsedTime;
                    Position = new Vector2(Position.X + (float)Math.Cos(AngleToMoveTo) * (float)elapsedTime.TotalSeconds * PowerUpFormationSpeed, Position.Y + (float)Math.Sin(AngleToMoveTo) * (float)elapsedTime.TotalSeconds * PowerUpFormationSpeed);
                }
            };
            specialTypeHintCountdownTimer.OnFinished += (sender, args) =>
            {
                TweenerCollection.GetTweener((int)Tweeners.SpecialTypeHintAnimationTweener).Start();

                // Play new power up hamsta sound
                GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.NewPowerUpHamstaSound));

                switch (CurrentSpecialType)
                {
                    case SpecialTypes.Bomb:
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Bomber));
                        break;

                    case SpecialTypes.MagicBomb:
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.OhhhLennyWizard));
                        break;

                    case SpecialTypes.Goku:
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Lasah));
                        break;

                    default:
                        break;
                }
            };

            goldenHamstaAnimationTimer.OnUpdate += (sender, args) =>
            {
                if (AbsolutePosition.Y < CurrentLevel.HUDInfoLayer.CurrentLevelText.AbsolutePosition.Y + GlobalConstants.BlockSize)
                {
                    GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.MagicBomb));

                    Hide();
                    TimerCollection.GetTimer((int)Timers.DisposeTimer).Start();

                    goldenHamstaAnimationTimer.Stop();
                }
                else
                {
                    TimeSpan elapsedTime = ((Timer.TimerOnUpdateEventArgs)args).ElapsedTime;
                    Scale *= 0.98f;
                    Alpha *= 0.99f;

                    Vector2 lookAheadVector = CurrentLevel.HUDInfoLayer.CurrentLevelText.AbsolutePosition + new Vector2(0, GlobalConstants.BlockSize) - AbsolutePosition;
                    AngleToMoveTo = (float)Math.Atan2(lookAheadVector.Y, lookAheadVector.X);

                    Position = new Vector2(
                        Position.X + (float)Math.Cos(AngleToMoveTo) * (float)elapsedTime.TotalSeconds * GoldenHamstaAnimationSpeed,
                        Position.Y + (float)Math.Sin(AngleToMoveTo) * (float)elapsedTime.TotalSeconds * GoldenHamstaAnimationSpeed);
                }
            };

            // Adding timers to timer collection
            TimerCollection.Add((int)Timers.BlinkingAnimationTimer, blinkingAnimationTimer);
            TimerCollection.Add((int)Timers.BlinkAnimationCountdownTimer, blinkAnimationCountdownTimer);
            TimerCollection.Add((int)Timers.DisposeTimer, disposeTimer);
            TimerCollection.Add((int)Timers.SpecialTypeFormationAnimationTimer, specialTypeFormationAnimationTimer);
            TimerCollection.Add((int)Timers.SpecialTypeHintCountdownTimer, specialTypeHintCountdownTimer);
            TimerCollection.Add((int)Timers.GoldenHamstaAnimationTimer, goldenHamstaAnimationTimer);

            // Kick some timers
            blinkAnimationCountdownTimer.Start();
        }

        public void Shake(bool isShakeOnNextLine)
        {
            Scale = Vector2.One;
            if (isShakeOnNextLine)
            {
                Position = PositionBeforeStartedToShake + new Vector2(Rand.NextFloat(-ShakeOnNextLineEffectAmount, ShakeOnNextLineEffectAmount), 0);
            }
            else
            {
                Position = PositionBeforeStartedToShake + new Vector2(Rand.NextFloat(-ShakeEffectAmount, ShakeEffectAmount), Rand.NextFloat(-ShakeEffectAmount, ShakeEffectAmount));
            }
        }

        public void StopShaking()
        {
            Scale = Vector2.One;
            Position = PositionBeforeStartedToShake;
        }

        public void ConvertToGoldenBlock()
        {
            Type = BlockTypes.GoldenBlock;
            RedefineTexture(false);

            AddSparklingEffect();
        }

        public void ConvertToRainbowHamsta()
        {
            Type = BlockTypes.RainbowHamsta;
            RedefineTexture(false);

            GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Combo));
        }

        public void DisableColorSelectionMode()
        {
            TweenerCollection.Remove((int)Tweeners.ColorSelectionModeTweener);

            AccessoriesList.Remove(ColorSelectionModeAccessory);
            ColorSelectionModeAccessory.Dispose();

            CurrentLevel.SelectedMultiColorBlock = null;
        }

        public void AddSparklingEffect()
        {
            GoldenHamstaSparkle sparkle = new GoldenHamstaSparkle(ParentLayer, this);
            sparkle.IsTouchEnabled = false;
            sparkle.ZOrder = 1;
            sparkle.AttachToParentLayer();
        }

        public void RemoveOrUpgradeToSpecialType()
        {
            if (NextSpecialType != Block.SpecialTypes.None)
            {
                OriginBlocks = MatchingGroup.MatchedBlocks.ToDictionary(entry => entry.UniqueID, entry => entry);
                State = States.Idle;
                Position = PositionBeforeStartedToShake;
                CurrentSpecialType = NextSpecialType;
                UpgradeToSpecialType();

                if (OnUpgradedToSpecialType != null)
                {
                    OnUpgradedToSpecialType(this);
                }
                TimerCollection.GetTimer((int)Timers.SpecialTypeHintCountdownTimer).Start();
            }
            else
            {
                OrderRemoval(Block.RemovalEffectEnum.SimpleExplosion);
            }
        }

        private void SetupAnimationTweeners()
        {
            TweenerCollection = new TweenerCollection(3);
            Tweener bounceEffectTweener = new Tweener(1, 0.9f, 0.1f, (t, b, c, d) => Linear.EaseNone(t, b, c, d), true);
            Tweener specialTypeTransformAnimationTweener = new Tweener(0.9f, 1.3f, 0.2f, (t, b, c, d) => Quadratic.EaseInOut(t, b, c, d), true);
            bounceEffectTweener.OnUpdate += (tweeningValue) =>
            {
                if (State == States.Idle)
                {
                    Scale = new Vector2(1 + (1 - tweeningValue) * 0.85f, tweeningValue);
                    foreach (var accessory in AccessoriesList)
                    {
                        accessory.Scale = Scale;
                    }
                }
            };
            bounceEffectTweener.OnFinished += (sender, args) =>
            {
                ResetBounceEffectTweenerAndTransformation();
            };
            specialTypeTransformAnimationTweener.OnUpdate += (tweeningValue) =>
            {
                ZOrder = (RowIndex + 1) * (ColumnIndex + 1) * 100;
                Scale = new Vector2(tweeningValue, tweeningValue);
                foreach (var accessory in AccessoriesList)
                {
                    accessory.Scale = new Vector2(tweeningValue, tweeningValue);
                }
            };
            specialTypeTransformAnimationTweener.OnFinished += (sender, args) =>
            {
                ZOrder = (RowIndex + 1) * (ColumnIndex + 1);
                Scale = Vector2.One;

                foreach (var accessory in AccessoriesList)
                {
                    accessory.Scale = Vector2.One;
                }

                bounceEffectTweener.Stop();
                bounceEffectTweener.Reset();
            };
            // Adding tweeners to timer collection
            TweenerCollection.Add((int)Tweeners.BounceEffectTweener, bounceEffectTweener);
            TweenerCollection.Add((int)Tweeners.SpecialTypeHintAnimationTweener, specialTypeTransformAnimationTweener);
        }

        private void ResetBounceEffectTweenerAndTransformation()
        {
            Scale = Vector2.One;
            TweenerCollection.GetTweener((int)Tweeners.BounceEffectTweener).Stop();
            TweenerCollection.GetTweener((int)Tweeners.BounceEffectTweener).Reset();
        }

        private void SetupTouchHandlers()
        {
            OnTouchDown += (sender, pos) =>
            {
                if (State == States.Idle)
                {
                    OldColumnBeforeDraggingIndex = ColumnIndex;
                    OldRowBeforeDraggingIndex = RowIndex;

                    if (IsDraggable && LevelBoardController.CanMoveBlock(this))
                    {
                        State = States.Dragging;
                        LevelBoardController.TotalBlocksBeingDragged++;

                        PositionBeforeDragging = AbsolutePosition;
                        DragOffset = pos - AbsolutePosition;
                        LastDraggingPosition = pos;
                    }

                    // Each time user starts dragging a block, current combo is stopped
                    LevelBoardController.ComboManager.StopCurrentCombo();
                }
                else if (State == States.IdleInNextLine) // On tap force line drop if state equals IdleInNextLine!
                {
                    LevelBoardController.BlockEmitter.InterruptLineShakingAndForceLineOfBlocksDrop();
                }
            };

            OnDragging += (sender, touchPosition) =>
            {
                if (State == States.Dragging && touchPosition != LastDraggingPosition)
                {
                    Vector2 parentLayerOffset = Vector2.Zero;
                    if (ParentLayer != null)
                    {
                        parentLayerOffset = ParentLayer.Position;
                    }
                    touchPosition -= DragOffset;
                    //Lock block inside limits of board. (-16 offset of MaxX.)
                    touchPosition = new Vector2(MathHelper.Clamp(touchPosition.X, BirthPositionX, GlobalConstants.NumberOfBlockGridColumns * GlobalConstants.BlockSize + parentLayerOffset.X - 16),
                                                MathHelper.Clamp(touchPosition.Y, DropLineLimit, (GlobalConstants.NumberOfBlockGridRows - 1) * GlobalConstants.BlockSize + parentLayerOffset.Y));
                    //move block.
                    Vector2 lastPos = LastDraggingPosition;
                    Direction directions = GetDragDirection(lastPos, touchPosition);
                    Rectangle rect = GetCollisionArea(1);
                    float offset = 0;
                    float currentPosX = touchPosition.X;
                    float currentPosY = touchPosition.Y;


                    if (HasDirection(directions, Direction.Left))
                    {
                        offset = lastPos.X - touchPosition.X;
                        currentPosX = MathHelper.Clamp((rect.Left - ((offset == 0) ? 1 : offset)), 0, ParentLayer.ParentScene.Width);
                    }
                    else if (HasDirection(directions, Direction.Right))
                    {
                        offset = touchPosition.X - lastPos.X;
                        currentPosX = MathHelper.Clamp((rect.Right + ((offset == 0) ? 1 : offset)), 0, ParentLayer.ParentScene.Width);
                    }

                    if (HasDirection(directions, Direction.Bottom))
                    {
                        offset = touchPosition.Y - lastPos.Y;
                        currentPosY = MathHelper.Clamp((rect.Bottom + ((offset == 0) ? 1 : offset)), 0, ParentLayer.ParentScene.Height);
                    }
                    else if (HasDirection(directions, Direction.Top))
                    {
                        offset = lastPos.Y - touchPosition.Y;
                        currentPosY = MathHelper.Clamp((rect.Top - ((offset == 0) ? 1 : offset)), 0, ParentLayer.ParentScene.Height);
                    }

                    Vector2 hCollisionPoint = Vector2.Zero;
                    Vector2 vCollisionPoint = Vector2.Zero;
                    Vector2 newPosition = Vector2.Zero;
                    Block hCollisionBlock = null;
                    Block vCollisionBlock = null;
                    if (LevelBoardController.BlocksMiniPhysicsEngine.GetClosestLineIntersection(currentPosX, currentPosY, out hCollisionPoint, out vCollisionPoint, this, 5, out hCollisionBlock, out vCollisionBlock))
                    {
                        Vector2 oldAP = AbsolutePosition;
                        float halfBlock = GlobalConstants.BlockSize / 2;
                        //+6 FOR HORIZONTAL positions and + 3 for vertical positions. this is difference between center of CollisionArea and AbsolutePosition.
                        newPosition = new Vector2((hCollisionPoint != Vector2.Zero) ? (hCollisionPoint.X + (halfBlock * (HasDirection(directions, Direction.Left) ? 1 : -1)) + HorizontalOffset) : touchPosition.X,
                                                       (vCollisionPoint != Vector2.Zero) ? (vCollisionPoint.Y + (halfBlock * (HasDirection(directions, Direction.Top) ? 1 : -1)) + VerticalOffset) : touchPosition.Y);
                    }
                    else
                    {
                        newPosition = touchPosition;
                    }

                    AbsolutePosition = newPosition;
                    LastDraggingPosition = AbsolutePosition;
                    UpdateSlotOnDragging(ref rect, directions);
                }
            };

            OnTouchReleased += (sender, pos) =>
            {
                bool decrementTotalBlocksBeingDragged = false;

                if (State == States.Dragging)
                {
                    State = States.Falling;
                    decrementTotalBlocksBeingDragged = true;

                    UpdateDragDirections(PositionBeforeDragging, AbsolutePosition);
                    UpdateNewSlot(true);
                    SnapToColumn();
                }

                if (State == States.Idle || (State == States.Falling && PreviousState != States.IdleInNextLine))
                {
                    if (Type == BlockTypes.RainbowHamsta && OldColumnBeforeDraggingIndex == ColumnIndex && OldRowBeforeDraggingIndex == RowIndex)
                    {
                        if (CurrentLevel.SelectedMultiColorBlock == null)
                        {
                            EnableColorSelectionMode();
                        }
                        else
                        {
                            ulong currentSelectedMultiColorBlockId = CurrentLevel.SelectedMultiColorBlock.UniqueID;
                            CurrentLevel.SelectedMultiColorBlock.DisableColorSelectionMode();

                            if (UniqueID != currentSelectedMultiColorBlockId)
                            {
                                EnableColorSelectionMode();
                            }
                        }
                    }
                    else if (CurrentLevel.SelectedMultiColorBlock != null && IsValidToTriggerMultiColorAction())
                    {
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.MagicBomb));
                        LevelBoardController.RemoveAllBlocksFromColor(CurrentLevel.SelectedMultiColorBlock, Type);
                        CurrentLevel.SelectedMultiColorBlock.DisableColorSelectionMode();
                    }
                }

                if (decrementTotalBlocksBeingDragged)
                {
                    LevelBoardController.TotalBlocksBeingDragged--;
                }
            };
        }

        private bool IsValidToTriggerMultiColorAction()
        {
            return Type != BlockTypes.UnmovableBlock && Type != BlockTypes.GoldenBlock && Type != BlockTypes.RainbowHamsta;
        }

        /// <summary>
        /// Verify if column or row are changed and free/check board position.
        /// </summary>
        /// <returns>True if changed slot.</returns>
        private bool UpdateSlotOnDragging(ref Rectangle blockRect, Direction directions)
        {
            int precision = 1;
            int newRowIndex = (int)MathHelper.Clamp((float)Math.Round(Position.Y / GlobalConstants.BlockSize), 0, GlobalConstants.NumberOfBlockGridRows - 1);
            int newColumnIndexLeft = (int)MathHelper.Clamp((int)((blockRect.Left - (HorizontalMarginWidth - precision)) / GlobalConstants.BlockSize), 0, GlobalConstants.NumberOfBlockGridColumns - 1);
            int newColumnIndexRight = (int)MathHelper.Clamp((int)((blockRect.Right - (HorizontalMarginWidth + precision)) / GlobalConstants.BlockSize), 0, GlobalConstants.NumberOfBlockGridColumns - 1);
            int newColumnIndex = ColumnIndex;
            if (newColumnIndexLeft == newColumnIndexRight || (newColumnIndexLeft != ColumnIndex && newColumnIndexRight != ColumnIndex))
            {
                newColumnIndex = HasDirection(directions, Direction.Left) ? newColumnIndexLeft : newColumnIndexRight;
            }
            if (LevelBoardController.GridOfBlocks[newRowIndex][newColumnIndex] == null)
            {
                if ((RowIndex != newRowIndex || ColumnIndex != newColumnIndex) && OnRemoval != null)
                {
                    OnRemoval(this, false);
                }
                RowIndex = newRowIndex;
                ColumnIndex = newColumnIndex;
                ZOrder = (RowIndex + 1) * (ColumnIndex + 1);
                LevelBoardController.GridOfBlocks[RowIndex][ColumnIndex] = this;
                return true;
            }
            return false;
        }

        public void SnapToColumn()
        {
            float posX = ColumnIndex;

            // Snap column
            if (OldColumnBeforeDraggingIndex == ColumnIndex && DragDirection == DragDirections.Right &&
                LevelBoardController.IsAnyBlockAtGivenGridPosition(RowIndex, (ColumnIndex + 1)) == null)
            {
                posX = MathHelper.Clamp(ColumnIndex + 1, 0, GlobalConstants.NumberOfBlockGridColumns - 1) * GlobalConstants.BlockSize + BirthPositionX;
            }
            else if (OldColumnBeforeDraggingIndex == ColumnIndex && DragDirection == DragDirections.Left &&
                    LevelBoardController.IsAnyBlockAtGivenGridPosition(RowIndex, (ColumnIndex - 1)) == null)
            {
                posX = MathHelper.Clamp(ColumnIndex - 1, 0, GlobalConstants.NumberOfBlockGridColumns - 1) * GlobalConstants.BlockSize + BirthPositionX;
            }
            else
            {
                posX = ColumnIndex * GlobalConstants.BlockSize + BirthPositionX;
            }

            Vector2 newPos = new Vector2(posX, Position.Y);
            Block blockInNextPosition = LevelBoardController.GetBlockAtPosition(newPos);
            if (blockInNextPosition == null || blockInNextPosition == this)
            {
                Position = newPos;
            }
        }

        private void SnapToRow()
        {
            Position = new Vector2(Position.X, RowIndex * GlobalConstants.BlockSize);
        }

        private void UpdateDragDirections(Vector2 positionBeforeDragging, Vector2 currentPosition)
        {
            if (currentPosition.X - ColumnChangeDragThreshold > positionBeforeDragging.X)
            {
                DragDirection = DragDirections.Right;
            }
            else if (currentPosition.X + ColumnChangeDragThreshold < positionBeforeDragging.X)
            {
                DragDirection = DragDirections.Left;
            }
            else
            {
                DragDirection = DragDirections.None;
            }
        }

        private Direction GetDragDirection(Vector2 lastPos, Vector2 currentPosition)
        {
            Direction dir = Block.Direction.None;
            if (currentPosition.X > lastPos.X)
            {
                dir |= Direction.Right;
            }
            else if (currentPosition.X < lastPos.X)
            {
                dir |= Direction.Left;
            }

            if (currentPosition.Y > lastPos.Y)
            {
                dir |= Direction.Bottom;
            }
            else if (currentPosition.Y < lastPos.Y)
            {
                dir |= Direction.Top;
            }
            return dir;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns>True if changed slot.</returns>
        private void UpdateNewSlot(bool sendOnRemovalEventIfChangedSlot)
        {
            int newRowIndex = (int)MathHelper.Clamp((float)Math.Round(Position.Y / GlobalConstants.BlockSize), 0, GlobalConstants.NumberOfBlockGridRows - 1);
            int newColumnIndex = (int)MathHelper.Clamp((float)Math.Round(Position.X / GlobalConstants.BlockSize) - 1, 0, GlobalConstants.NumberOfBlockGridColumns - 1);
            if (LevelBoardController.GridOfBlocks[newRowIndex][newColumnIndex] == null)
            {
                if (sendOnRemovalEventIfChangedSlot && (RowIndex != newRowIndex || ColumnIndex != newColumnIndex) && OnRemoval != null)
                {
                    OnRemoval(this, false);
                }
                RowIndex = newRowIndex;
                ColumnIndex = newColumnIndex;
                ZOrder = (RowIndex + 1) * (ColumnIndex + 1);
            }
        }

        /// <summary>
        /// Do not change this method defines default collision used for example by touch panel manager.
        /// If you want to use collision area with a factor of 1 for example, just use method overloading!
        /// </summary>
        /// <returns></returns>
        public override Rectangle GetCollisionArea()
        {
            return base.GetCollisionArea(1);
        }

        #endregion

        #region Public Methods

        public void DropAndBecomeInteractive()
        {
            IsDraggable = (Type != BlockTypes.UnmovableBlock && Type != BlockTypes.GoldenBlock);
            State = States.Falling;
        }

        public enum RemovalEffectEnum { SimpleExplosion, RainbowExplosion, StarsExplosion }

        public void OrderRemoval(RemovalEffectEnum removalEffect)
        {
            PauseUpdateFromBlockAbove();
            DoOrderRemoval(removalEffect);

            if (State == States.Dragging)
            {
                LevelBoardController.TotalBlocksBeingDragged--;
            }
        }

        private void DoOrderRemoval(RemovalEffectEnum removalEffect)
        {
            if (State != States.Disposed)
            {
                State = States.Disposed;

                if (CurrentSpecialType == SpecialTypes.None)
                {
                    // If is there is any special type block on matched blocks group, then play special type formation animation.
                    Block possibleSpecialTypeBlock = (MatchingGroup != null) ? MatchingGroup.FindAnyMatchedSpecialTypeBlock() : null;

                    if (possibleSpecialTypeBlock != null)
                    {
                        Vector2 lookAheadVector = possibleSpecialTypeBlock.Position - Position;
                        AngleToMoveTo = (float)Math.Atan2(lookAheadVector.Y, lookAheadVector.X);
                        TargetBlockWithSpecialType = possibleSpecialTypeBlock;

                        TimerCollection.GetTimer((int)Timers.SpecialTypeFormationAnimationTimer).Start();
                    }
                    else if (Type == BlockTypes.GoldenBlock)
                    {
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Yeah));

                        TimerCollection.GetTimer((int)Timers.GoldenHamstaAnimationTimer).Start();
                    }
                    else
                    {
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaSimpleExplosionSound));
                        Hide();
                        TimerCollection.GetTimer((int)Timers.DisposeTimer).Start();
                    }

                    IsTouchEnabled = false;

                    if (Type == BlockTypes.UnmovableBlock)
                    {
                        KittyRemovedSoundCounter = (byte)(++KittyRemovedSoundCounter % KittyRemovedSounds.Length);
                        SoundEffect randomKittyRemovedSound = KittyRemovedSounds[Rand.Next(KittyRemovedSounds.Length)];
                        GameDirector.Instance.SoundManager.PlaySound(randomKittyRemovedSound);
                    }

                    if (OnRemoval != null)
                    {
                        OnRemoval(this, true);
                    }
                }
                else
                {
                    switch (CurrentSpecialType)
                    {
                        case SpecialTypes.Bomb:
                            GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.BombHamstaExplosionSound));
                            LevelBoardController.RequestBombPowerUpExecution(this);
                            GameDirector.Instance.CurrentScene.Camera.Shake(-4, 4, -4, 4, 1);
                            break;

                        case SpecialTypes.MagicBomb:
                            GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.MagicBomb));
                            LevelBoardController.RequestMagicBombPowerUpExecution(this);
                            break;

                        case SpecialTypes.Goku:
                            GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.GokuPower));
                            LevelBoardController.RequestGokuPowerUpExecution(this);
                            break;

                        default:
                            break;
                    }

                    Hide();
                    IsTouchEnabled = false;
                    TimerCollection.GetTimer((int)Timers.DisposeTimer).Start();
                }
            }
        }

        public void RequestBombUpgrade()
        {
            UpdateSpecialTypeFlag(SpecialTypes.Bomb);
        }

        public void RequestMagicBombUpgrade()
        {
            UpdateSpecialTypeFlag(SpecialTypes.MagicBomb);
        }

        public void RequestGokuUpgrade()
        {
            UpdateSpecialTypeFlag(SpecialTypes.Goku);
        }

        public void StartMatchingMode()
        {
            if (State != States.MatchingMode)
            {
                State = States.MatchingMode;
                UpdateNewSlot(true);
                SnapToColumn();
                PositionBeforeStartedToShake = Position;
            }
            Tweener bounceEffectTweener = TweenerCollection.GetTweener((int)Tweeners.BounceEffectTweener);
            if (bounceEffectTweener != null && bounceEffectTweener.IsRunning)
            {
                ResetBounceEffectTweenerAndTransformation();
            }
        }

        #endregion

        #region Overrided LayerObject Methods

        public override void Hide()
        {
            base.Hide();

            // Hide also all of its accessories.
            foreach (var accessory in AccessoriesList)
            {
                if (accessory is AnimatedLayerObject)
                {
                    ((AnimatedLayerObject)accessory).Stop();
                }
                accessory.Hide();
            }
        }

        public override void Dispose()
        {
            // Dispose also all of its accessories.
            DisposeAllAccessories();
            OnUpgradedToSpecialType = null;

            base.Dispose();
        }

        private void DisposeAllAccessories()
        {
            foreach (var accessory in AccessoriesList)
            {
                accessory.Dispose();
            }

            AccessoriesList.Clear();
        }

        public override void DrawJustLayerObject(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            base.DrawJustLayerObject(spriteBatch);

            //all of its acessories.
            foreach (var accessory in AccessoriesList)
            {
                accessory.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Test if given flags has given flag.
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private bool HasDirection(Direction flags, Direction dir)
        {
            return ((flags & dir) == dir);
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (!IsUpdatePaused)
            {
                switch (State)
                {
                    case States.Idle:
                        UpdateOnIdle();
                        break;

                    case States.Falling:
                        UpdateOnFalling(elapsedTime);
                        break;

                    case States.Dragging:
                        break;

                    case States.MatchingMode:
                        UpdateOnMatchingMode(elapsedTime);
                        break;

                    default:
                        break;
                }

                UpdateAccessoriesPositions();

                TimerCollection.Update(elapsedTime);
                TweenerCollection.Update(elapsedTime);

                base.Update(elapsedTime);
            }
            else
            {
                UpdatePausingTimer.Update(elapsedTime);
            }
        }

        private bool IsBlockAtBottom()
        {
            return LevelBoardController.GridOfBlocks[(int)MathHelper.Clamp(RowIndex + 1, 0, GlobalConstants.NumberOfBlockGridRows - 1)][ColumnIndex] != null;
        }

        private void UpdateOnIdle()
        {
            if (LevelBoardController.GridOfBlocks[RowIndex][ColumnIndex] == null)
            {
                if (this.OnIdle != null)
                {
                    OnIdle(this);
                }
            }

            if (RowIndex < (GlobalConstants.NumberOfBlockGridRows - 1))
            {
                if (!IsBlockAtBottom())
                {
                    Rectangle rect = this.GetCollisionArea(1);
                    Vector2 hCollisionPoint = Vector2.Zero;
                    Vector2 vCollisionPoint = Vector2.Zero;
                    Block hCollisionBlock = null;
                    Block vCollisionBlock = null;
                    LevelBoardController.BlocksMiniPhysicsEngine.GetClosestLineIntersection(rect.Left,
                        (rect.Bottom + (GlobalConstants.BlockSize / 2)), out hCollisionPoint, out vCollisionPoint, this, 5, out hCollisionBlock, out vCollisionBlock);

                    if (vCollisionPoint == Vector2.Zero && vCollisionBlock == null)
                    {
                        BecomeFallingWhenIdle();
                    }
                }
            }
        }

        public void BecomeFallingWhenIdle()
        {
            if (State == States.Idle)
            {
                State = States.Falling;
                if (OnRemoval != null)
                {
                    OnRemoval(this, false);
                }
            }
        }

        public void BecomeIdle()
        {
            if (State == States.Falling)
            {
                State = States.Idle;
                UpdateNewSlot(true);
                SnapToRow();
                Velocity = 0;

                if (OnIdle != null)
                {
                    OnIdle(this);
                }

                if (PreviousState == States.Falling)
                {
                    // Dropped sound effect and animation
                    if (Type == BlockTypes.GoldenBlock)
                    {
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.GoldHamstaDropSound));
                    }
                    else
                    {
                        GameDirector.Instance.SoundManager.PlaySound(GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaDroppedSound));
                    }

                    StartBounceEffect();
                }
            }
        }

        private void StartBounceEffect()
        {
            TweenerCollection.GetTweener((int)Tweeners.BounceEffectTweener).Start();
        }

        private void UpdateOnFalling(TimeSpan elapsedTime)
        {
            float deltaT = (float)elapsedTime.TotalSeconds;

            // Update velocity and position
            Velocity += Mass * GravityAcceleration * deltaT;
            PreviousPosition = Position;
            Vector2 newPos = new Vector2(AbsolutePosition.X, AbsolutePosition.Y + Velocity * deltaT);
            float offset = Math.Abs(newPos.Y - AbsolutePosition.Y);
            Rectangle rect = this.GetCollisionArea(1);
            Vector2 hCollisionPoint = Vector2.Zero;
            Vector2 vCollisionPoint = Vector2.Zero;
            float halfBlock = GlobalConstants.BlockSize / 2;
            Block hCollisionBlock = null;
            Block vCollisionBlock = null;
            if (LevelBoardController.BlocksMiniPhysicsEngine.GetClosestLineIntersection(newPos.X, (rect.Bottom + ((offset == 0) ? 1 : offset)), out hCollisionPoint, out vCollisionPoint, this, (int)halfBlock, out hCollisionBlock, out vCollisionBlock)
                && vCollisionPoint != Vector2.Zero && vCollisionBlock != null)
            {
                //+6 FOR HORIZONTAL positions and + 3 for vertical positions. this is difference between center of CollisionArea and AbsoulutePosition.
                AbsolutePosition = new Vector2(newPos.X,
                                          (vCollisionPoint != Vector2.Zero) ? (vCollisionPoint.Y + (-halfBlock) + VerticalOffset) : newPos.Y);
                if (vCollisionBlock != null && (vCollisionBlock.State == States.Idle || vCollisionBlock.State == States.MatchingMode))
                {
                    BecomeIdle();
                }
                return;
            }
            else
            {
                AbsolutePosition = newPos;
            }

            //If it is bottom line.
            Vector2 parentLayerOffset = Vector2.Zero;
            if (ParentLayer != null)
            {
                parentLayerOffset = ParentLayer.Position;
            }

            if (AbsolutePosition.Y >= ((GlobalConstants.NumberOfBlockGridRows - 1) * GlobalConstants.BlockSize + parentLayerOffset.Y))
            {
                Position = new Vector2(Position.X, BlockBottomLimitY);
                BecomeIdle();
            }
        }

        private void UpdateOnMatchingMode(TimeSpan elapsedTime)
        {
            Shake(false);
        }

        #endregion

        public void PauseUpdateFromBlockAbove()
        {
            try
            {
                Block blockAbove = LevelBoardController.GridOfBlocks[RowIndex - 1][ColumnIndex];
                if (blockAbove != null)
                {
                    blockAbove.PauseUpdate();
                }
            }
            catch { } // FIXME
        }

        public void PauseUpdate()
        {
            IsUpdatePaused = true;

            UpdatePausingTimer = new Timer(0.05f);
            UpdatePausingTimer.OnFinished += (sender, e) =>
            {
                IsUpdatePaused = false;
            };

            UpdatePausingTimer.Start();
        }

        #region Enums

        public enum States
        {
            IdleInNextLine,
            Idle,
            Falling,
            Dragging,
            MatchingMode,
            Disposed
        }

        [DataContract]
        public enum BlockTypes
        {
            [EnumMember(Value = "Block1")]
            Block1 = 1,
            [EnumMember(Value = "Block2")]
            Block2 = 2,
            [EnumMember(Value = "Block3")]
            Block3 = 3,
            [EnumMember(Value = "Block4")]
            Block4 = 4,
            [EnumMember(Value = "Block5")]
            Block5 = 5,
            [EnumMember(Value = "RainbowHamsta")]
            RainbowHamsta = 6,
            [EnumMember(Value = "UnmovableBlock")]
            UnmovableBlock = 9,
            [EnumMember(Value = "GoldenBlock")]
            GoldenBlock = 11,
        }

        [DataContract]
        public enum SpecialTypes
        {
            [EnumMember(Value = "None")]
            None,
            [EnumMember(Value = "Bomb")]
            Bomb,
            [EnumMember(Value = "MagicBomb")]
            MagicBomb,
            [EnumMember(Value = "Goku")]
            Goku
        }

        private enum Timers
        {
            BlinkAnimationCountdownTimer,
            BlinkingAnimationTimer,
            DisposeTimer,
            SpecialTypeFormationAnimationTimer,
            SpecialTypeHintCountdownTimer,
            SmoothColumnSnapTimer,
            GoldenHamstaAnimationTimer
        }

        private enum Tweeners
        {
            BounceEffectTweener,
            SpecialTypeHintAnimationTweener,
            ColorSelectionModeTweener
        }

        private enum DragDirections
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
        }

        [Flags]
        public enum Direction
        {
            None = 0x0,
            Top = 0x1,
            Bottom = 0x2,
            Left = 0x4,
            Right = 0x8
        }

        #endregion

        #region Events

        public delegate void OnIdleHandler(Block block);
        public event OnIdleHandler OnIdle;

        public delegate void OnRemovalHandler(Block block, bool isDefinitiveRemoval);
        public event OnRemovalHandler OnRemoval;

        public delegate void OnUpgradedToSpecialTypeHandler(Block block);
        public event OnUpgradedToSpecialTypeHandler OnUpgradedToSpecialType;

        #endregion

        #region Public Properties
        public MatchingGroup MatchingGroup { get; set; }
        public BlockTypes Type { get; set; }
        public SpecialTypes CurrentSpecialType { get; set; }
        public SpecialTypes NextSpecialType { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Used for unmovable blocks. Used internally.
        /// </summary>
        public bool IsDraggable { get; set; }
        public Dictionary<ulong, Block> OriginBlocks { get; set; }
        public BlockTypes? NewBlockTypeToCreateAfterRemoval { get; set; }
        public SpecialTypes NewBlockPowerUpToCreateAfterRemoval { get; set; }
        public Vector2 PositionBeforeStartedToShake { get; set; }
        public States State { get { return this.state; } set { PreviousState = this.state; this.state = value; } }
        public States PreviousState { get; set; }
        public List<Block> CollidingBlocks { get; set; }


        #endregion

        #region Private Properties
        private int OldColumnBeforeDraggingIndex { get; set; } // Used for dragging and snapping to column.
        private int OldRowBeforeDraggingIndex { get; set; }
        private Level CurrentLevel { get; set; }
        private LevelBoardController LevelBoardController;
        private float NextYPositionToFallTo { get; set; }
        private float Velocity { get; set; }
        private float AngleToMoveTo { get; set; }
        private Block TargetBlockWithSpecialType { get; set; }
        private DragDirections DragDirection { get; set; }
        List<LayerObject> AccessoriesList { get; set; }
        private Vector2 DragOffset { get; set; }
        private Vector2 PositionBeforeDragging { get; set; }
        public Vector2 PreviousPosition { get; set; }
        //public Vector2 PreviousDraggedPosition { get; set; }
        private TimerCollection TimerCollection;
        private TweenerCollection TweenerCollection;
        private float BlockBottomLimitY { get; set; }

        private States state;
        private Vector2 LastDraggingPosition { get; set; }
        private bool IsUpdatePaused { get; set; }
        private Timer UpdatePausingTimer { get; set; }

        private LayerObject ColorSelectionModeAccessory { get; set; }

        #endregion

        #region Constants

        private const float GravityAcceleration = 9.8f;
        private const float Mass = 600;

        private const float BlinkAnimationDuration = 0.1f;
        private const float DisposeTimerDuration = 3;
        private const float SpecialTypeFormationAnimationTimerDuration = 1f;
        private const float SpecialTypeHintCountdownTimerDuration = 1f;
        private const float GoldenHamstaAnimationDurationInSeconds = 3f;
        private const int MinBlinkAnimationCountdownTime = 5;
        private const int MaxBlinkAnimationCountdownTime = 15;
        private const float ShakeEffectAmount = 2.5f;
        private const float ShakeOnNextLineEffectAmount = 3f;
        private const int HorizontalMarginWidth = 10;

        public const int BirthPositionY = -GlobalConstants.BlockSize;
        private const int BirthPositionX = 48;
        private const int ColumnChangeDragThreshold = 5; // offset used when we move a block.
        private const float SmoothColumnSnapTimerDuration = 0.5f;

        private const int DropLineLimit = 235;

        //Offsets
        private const int VerticalOffset = 3; //offset for Absolute Positions.
        private const int HorizontalOffset = 6; //offset for Absolute Positions.

        private const int PowerUpFormationSpeed = 500;
        private const int GoldenHamstaAnimationSpeed = 1100;

        private static byte KittyRemovedSoundCounter = 1;
        private readonly SoundEffect[] KittyRemovedSounds =
        {
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.KittyClearedSound),
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.AhAhAh),
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.AhAhAhAhAh),
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.EvilLaugh)
        };

        #endregion
    }
}
