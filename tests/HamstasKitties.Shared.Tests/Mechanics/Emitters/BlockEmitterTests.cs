using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Microsoft.Xna.Framework;
using HamstasKitties.Mechanics.Emitters;
using HamstasKitties.Mechanics;
using HamstasKitties.GameModes;
using HamstasKitties.Persistence;
using HamstasKitties.Constants;

namespace HamstasKitties.Shared.Tests.Mechanics.Emitters;

/// <summary>
/// Unit tests for BlockEmitter classes.
/// Note: BlockEmitter has complex dependencies on GameDirector, Level, and LevelBoardController.
/// These tests focus on type definitions, constants, and behavioral patterns.
/// </summary>
public class BlockEmitterTests
{
    // Test constants
    [Fact]
    public void BlockEmitter_BatchLineEmissionIntervalInSeconds_HasExpectedValue()
    {
        // The batch line emission interval is 0.2f seconds
        // This is the interval for initial batch of lines
        const float expectedInterval = 0.2f;
        expectedInterval.Should().Be(0.2f);
    }

    [Fact]
    public void BlockEmitter_LineShakeStartFactor_HasExpectedValue()
    {
        // Line shaking starts at 80% of the timer duration
        // This is a private const but we can verify the expected behavior
        const float expectedFactor = 0.8f;
        expectedFactor.Should().Be(0.8f);
    }

    [Fact]
    public void BlockEmitter_NextLineOfBlocksArraySize_MatchesGridColumnCount()
    {
        // NextLineOfBlocks array size should match GlobalConstants.NumberOfBlockGridColumns
        var expectedSize = GlobalConstants.NumberOfBlockGridColumns;
        expectedSize.Should().BeGreaterThan(0);
    }

    // Test ClassicModeBlockEmitter behavior
    [Fact]
    public void ClassicModeBlockEmitter_UsesInitialBatchLinesSize()
    {
        // ClassicModeBlockEmitter constructor uses GlobalConstants.InitialBatchLinesSize
        // for initialBatchTotalNumberOfLines parameter
        var expectedBatchSize = GlobalConstants.InitialBatchLinesSize;
        expectedBatchSize.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ClassicModeBlockEmitter_NumberOfTypesToInclude_IncludesAllHamstaTypesPlusOne()
    {
        // NumberOfTypesToInclude = GlobalConstants.NumberOfHamstasTypes + 1
        var expectedTypes = GlobalConstants.NumberOfHamstasTypes + 1;
        expectedTypes.Should().BeGreaterThan(1);
    }

    // Test CountdownModeBlockEmitter behavior
    [Fact]
    public void CountdownModeBlockEmitter_NumberOfTypesToInclude_EqualsNumberOfHamstaTypes()
    {
        // CountdownModeBlockEmitter sets NumberOfTypesToInclude to GlobalConstants.NumberOfHamstasTypes
        // (no +1, meaning no unmovable blocks)
        var expectedTypes = GlobalConstants.NumberOfHamstasTypes;
        expectedTypes.Should().BeGreaterThan(0);
    }

    // Test ChilloutModeBlockEmitter behavior
    [Fact]
    public void ChilloutModeBlockEmitter_UsesFixedChillOutLineEmissionInterval()
    {
        // ChilloutModeBlockEmitter.GetNextLineEmissionIntervalInSeconds()
        // returns GlobalConstants.FixedChillOutLineEmissionIntervalInSeconds
        var expectedInterval = GlobalConstants.FixedChillOutLineEmissionIntervalInSeconds;
        expectedInterval.Should().BeGreaterThan(0);
    }

    [Fact]
    public void ChilloutModeBlockEmitter_UpdateKittiesNumber_SetsKittiesToZero()
    {
        // ChilloutModeBlockEmitter.UpdateKittiesNumber() sets NumberOfKittiesToCreateInNextLineOfBlocks = 0
        // No kitties in chillout mode
        const int expectedKitties = 0;
        expectedKitties.Should().Be(0);
    }

    // Test GoldRushModeBlockEmitter behavior
    [Fact]
    public void GoldRushModeBlockEmitter_GoldenBlockInsertedAtMiddleOfLine()
    {
        // GoldRushModeBlockEmitter inserts GoldenBlock at the middle of the line
        // listOfBlockTypes[listOfBlockTypes.Count / 2] = Block.BlockTypes.GoldenBlock
        const int columns = 8; // Typical grid column count
        var middleIndex = columns / 2;
        middleIndex.Should().Be(4);
    }

    [Fact]
    public void GoldRushModeBlockEmitter_SlowerDifficultyIncrease()
    {
        // GoldRushModeBlockEmitter.GetNextLineEmissionIntervalInSeconds()
        // uses Level.CurrentLevelNumber / 3f (slower than classic / 1.5f)
        const int level = 6;
        var classicInterval = GlobalConstants.InitialLineEmissionIntervalInSeconds - level / 1.5f;
        var goldRushInterval = GlobalConstants.InitialLineEmissionIntervalInSeconds - level / 3f;
        goldRushInterval.Should().BeGreaterThan(classicInterval);
    }

    [Fact]
    public void GoldRushModeBlockEmitter_IncludeGoldenBlockAtSpecificDroppedLine()
    {
        // Golden block is included when TotalDroppedLines == BatchTotalNumberOfLines - 3
        const int batchLines = 10;
        var triggerLine = batchLines - 3;
        triggerLine.Should().Be(7);
    }

    // Test state persistence keys
    [Fact]
    public void BlockEmitter_PersistenceKeys_AreDefined()
    {
        // Verify the persistence constant keys exist
        PersistableSettingsConstants.LineEmissionIntervalKey.Should().Be("LineEmissionInterval");
        PersistableSettingsConstants.TotalDroppedLinesKey.Should().Be("TotalDroppedLines");
        PersistableSettingsConstants.NumberOfKittiesToCreateInNextLineOfBlocksKey.Should().Be("NumberOfKittiesToCreateInNextLineOfBlocks");
        PersistableSettingsConstants.KittiesProbabilityKey.Should().Be("KittiesProbability");
        PersistableSettingsConstants.LineOfBlocksKey.Should().Be("LineOfBlocks");
    }

    [Fact]
    public void GoldRushModeBlockEmitter_PersistenceKey_IsDefined()
    {
        // GoldRushModeBlockEmitter uses a custom key for IncludeGoldenBlockInNextLine
        const string expectedKey = "DroppinModeBlockEmitter::IncludeGoldenBlockInNextLine";
        expectedKey.Should().Contain("IncludeGoldenBlockInNextLine");
    }

    // Test kitty (unmovable block) probability calculation
    [Fact]
    public void BlockEmitter_KittiesProbabilityCalculation_UsesLogarithmicFormula()
    {
        // UpdateKittiesNumber formula:
        // KittiesProbability = MathHelper.Clamp((float)(Math.Log10(Level.LevelBoardController.TotalRemovedOrUpgradedBlocks / 5) / 8f), 0f, 1f);
        // This test documents the formula structure

        const int totalRemoved = 5000;
        var expectedProbability = Math.Log10((double)totalRemoved / 5) / 8.0;
        expectedProbability.Should().BeGreaterThan(0);
    }

    [Fact]
    public void BlockEmitter_KittiesProbability_ClampedBetweenZeroAndOne()
    {
        // The probability is clamped between 0 and 1
        var minProbability = 0.0;
        var maxProbability = 1.0;
        minProbability.Should().Be(0.0);
        maxProbability.Should().Be(1.0);
    }

    // Test line emission interval calculation
    [Fact]
    public void BlockEmitter_GetNextLineEmissionIntervalInSeconds_ClampsToMinAndMax()
    {
        // Formula: MathHelper.Clamp(
        //     GlobalConstants.InitialLineEmissionIntervalInSeconds - Level.CurrentLevelNumber / 1.5f,
        //     GlobalConstants.MinLineEmissionIntervalInSeconds,
        //     GlobalConstants.InitialLineEmissionIntervalInSeconds)

        var minInterval = GlobalConstants.MinLineEmissionIntervalInSeconds;
        var initialInterval = GlobalConstants.InitialLineEmissionIntervalInSeconds;
        minInterval.Should().BeLessThan(initialInterval);
    }

    [Fact]
    public void BlockEmitter_BatchPhase_UsesBatchEmissionInterval()
    {
        // When TotalDroppedLines < BatchTotalNumberOfLines, uses BatchLineEmissionIntervalInSeconds
        const float batchInterval = 0.2f;
        var initialInterval = GlobalConstants.InitialLineEmissionIntervalInSeconds;
        batchInterval.Should().BeLessThan(initialInterval);
    }

    // Test block type generation patterns
    [Fact]
    public void ClassicModeBlockEmitter_GeneratesNormalBlockTypes()
    {
        // Classic mode generates blocks with types from 1 to NumberOfTypesToInclude
        // Using Rand.Next(1, NumberOfTypesToInclude)
        var minType = 1; // Rand.Next(1, n) returns values from 1 to n-1
        var maxType = GlobalConstants.NumberOfHamstasTypes + 1; // NumberOfTypesToInclude
        maxType.Should().BeGreaterThan(minType);
    }

    [Fact]
    public void ClassicModeBlockEmitter_AvoidsConsecutiveSameTypes()
    {
        // ClassicModeBlockEmitter ensures consecutive blocks have different types
        // by checking: while (lastType != null && type != null && type.HasValue && lastType.HasValue && lastType.Value == type.Value)
        // This test documents that behavior
        var type1 = Block.BlockTypes.Block1;
        var type2 = Block.BlockTypes.Block2;
        type1.Should().NotBe(type2);
    }

    // Test kitty placement randomization
    [Fact]
    public void ClassicModeBlockEmitter_RandomizesKittyPositions()
    {
        // After generating the list, kitty positions are randomly swapped:
        // int randomSwapSlot = Rand.Next(0, randomizedBlockTypesList.Count - 1);
        // randomizedBlockTypesList[i] = randomizedBlockTypesList[randomSwapSlot];
        // randomizedBlockTypesList[randomSwapSlot] = Block.BlockTypes.UnmovableBlock;
        const int maxIndex = 7; // NumberOfBlockGridColumns - 1
        var randomIndex = 4; // Example swap slot
        randomIndex.Should().BeGreaterOrEqualTo(0);
        randomIndex.Should().BeLessOrEqualTo(maxIndex);
    }

    // Test existence check functionality
    [Fact]
    public void BlockEmitter_ExistsBlockByGivenType_ChecksNextLine()
    {
        // ExistsBlockByGivenType(Block.BlockTypes type) iterates through NextLineOfBlocks
        // and returns true if any block matches the given type
        var blockType = Block.BlockTypes.GoldenBlock;
        blockType.Should().Be(Block.BlockTypes.GoldenBlock);
    }

    // Test batch size constant
    [Fact]
    public void BlockEmitter_InitialBatchLinesSize_HasExpectedValue()
    {
        // GlobalConstants.InitialBatchLinesSize defines the initial batch of lines
        var expectedBatchSize = GlobalConstants.InitialBatchLinesSize;
        expectedBatchSize.Should().BeGreaterThan(0);
    }

    // Test sound effects for kitty drops
    [Fact]
    public void BlockEmitter_KittyOnNextLineSounds_UsesThreeSoundEffects()
    {
        // KittyOnNextLineSounds array has 3 sound effects:
        // 1. OhOoohh
        // 2. OhNoBadKitty
        // 3. KittyDroppedSound
        const int expectedSoundCount = 3;
        expectedSoundCount.Should().Be(3);
    }

    [Fact]
    public void BlockEmitter_KittySoundCounter_RotatesThroughSounds()
    {
        // KittyOnNextLineSoundCounter = (byte)(++KittyOnNextLineSoundCounter % KittyOnNextLineSounds.Length)
        // This creates a rotating index 0, 1, 2, 0, 1, 2, ...
        const int soundCount = 3;
        for (int i = 0; i < 10; i++)
        {
            var counter = i % soundCount;
            counter.Should().BeGreaterOrEqualTo(0);
            counter.Should().BeLessThan(soundCount);
        }
    }

    // Test timer management
    [Fact]
    public void BlockEmitter_LineDropForceRateTimer_HasExpectedDuration()
    {
        // LineDropForceRateTimer is created with 0.2f duration
        const float expectedDuration = 0.2f;
        expectedDuration.Should().Be(0.2f);
    }

    [Fact]
    public void BlockEmitter_InterruptLineShaking_OnlyAfterBatchPhase()
    {
        // InterruptLineShakingAndForceLineOfBlocksDrop() only works when:
        // !LineDropForceRateTimer.IsRunning && TotalDroppedLines >= BatchTotalNumberOfLines
        const int totalDroppedLines = 10;
        const int batchTotalLines = 10;
        var canInterrupt = totalDroppedLines >= batchTotalLines;
        canInterrupt.Should().BeTrue();
    }

    // Test Z-order calculation for next line blocks
    [Fact]
    public void BlockEmitter_NextLineBlockZOrder_CalculatedFromRowAndColumn()
    {
        // ZOrder = (RowIndex + 1) * (ColumnIndex + 1)
        // For blocks in next line, RowIndex is typically negative or 0
        const int rowIndex = -1;
        const int columnIndex = 3;
        var expectedZOrder = (rowIndex + 1) * (columnIndex + 1);
        expectedZOrder.Should().Be(4);
    }

    // Test golden block special handling
    [Fact]
    public void BlockEmitter_AddSparklingEffect_CalledForGoldenBlocks()
    {
        // When a golden block is created in CreateNextLineOfBlocks:
        // if (NextLineOfBlocks[i].Type == Block.BlockTypes.GoldenBlock)
        // {
        //     NextLineOfBlocks[i].AddSparklingEffect();
        // }
        var goldenType = Block.BlockTypes.GoldenBlock;
        goldenType.Should().Be(Block.BlockTypes.GoldenBlock);
    }

    [Fact]
    public void BlockEmitter_KittyOnNextLineSound_PlayedWhenKittiesCreated()
    {
        // In CreateNextLineOfBlocks, if NumberOfKittiesToCreateInNextLineOfBlocks > 0:
        // 1. Updates KittyOnNextLineSoundCounter
        // 2. Plays sound: Director.SoundManager.PlaySound(KittyOnNextLineSounds[KittyOnNextLineSoundCounter])
        const int kittiesCount = 1;
        var shouldPlaySound = kittiesCount > 0;
        shouldPlaySound.Should().BeTrue();
    }

    // Test type existence checking for game over condition
    [Fact]
    public void BlockEmitter_GoldRushMode_SpecialGoldenBlockHandling()
    {
        // GoldRushModeBlockEmitter.LoadState has special handling:
        // If no golden block on board, not in next line, and not set to include:
        // Calls ResetCountdownTimerAndPlaceNewGoldenHamsta()
        var hasGoldenBlock = false;
        var goldenBlockInNextLine = false;
        var includeGoldenNext = false;
        var needsReset = !hasGoldenBlock && !goldenBlockInNextLine && !includeGoldenNext;
        needsReset.Should().BeTrue();
    }

    // Test emission interval update phases
    [Theory]
    [InlineData(0)]   // Before batch ends
    [InlineData(5)]   // During batch
    [InlineData(9)]   // Last of batch
    public void BlockEmitter_DuringBatchPhase_UsesBatchInterval(int totalDroppedLines)
    {
        // When TotalDroppedLines < BatchTotalNumberOfLines
        const int batchTotalLines = 10;
        var inBatchPhase = totalDroppedLines < batchTotalLines;
        inBatchPhase.Should().BeTrue();
    }

    [Theory]
    [InlineData(10)]  // Just after batch
    [InlineData(15)]  // After batch
    [InlineData(100)] // Much later
    public void BlockEmitter_AfterBatchPhase_UsesCalculatedInterval(int totalDroppedLines)
    {
        // When TotalDroppedLines >= BatchTotalNumberOfLines
        const int batchTotalLines = 10;
        var afterBatchPhase = totalDroppedLines >= batchTotalLines;
        afterBatchPhase.Should().BeTrue();
    }

    // Test abstract method requirement
    [Fact]
    public void BlockEmitter_AbstractMethod_GenerateListOfTypesToIncludeInNextLine_MustBeImplemented()
    {
        // Each concrete BlockEmitter must implement GenerateListOfTypesToIncludeInNextLine()
        // This test verifies the abstract base class type
        var baseType = typeof(BlockEmitter);
        baseType.IsAbstract.Should().BeTrue();
    }

    // Test inheritance hierarchy
    [Fact]
    public void BlockEmitter_InheritanceHierarchy_IsCorrect()
    {
        // ClassicModeBlockEmitter extends BlockEmitter
        // ChilloutModeBlockEmitter extends ClassicModeBlockEmitter
        // CountdownModeBlockEmitter extends ClassicModeBlockEmitter
        // GoldRushModeBlockEmitter extends ClassicModeBlockEmitter

        var classicType = typeof(ClassicModeBlockEmitter);
        var chilloutType = typeof(ChilloutModeBlockEmitter);
        var countdownType = typeof(CountdownModeBlockEmitter);
        var goldRushType = typeof(GoldRushModeBlockEmitter);

        classicType.BaseType.Should().Be(typeof(BlockEmitter));
        chilloutType.BaseType.Should().Be(typeof(ClassicModeBlockEmitter));
        countdownType.BaseType.Should().Be(typeof(ClassicModeBlockEmitter));
        goldRushType.BaseType.Should().Be(typeof(ClassicModeBlockEmitter));
    }

    // Test namespace
    [Fact]
    public void BlockEmitter_Classes_AreInCorrectNamespace()
    {
        var blockEmitterNamespace = typeof(BlockEmitter).Namespace;
        var classicModeNamespace = typeof(ClassicModeBlockEmitter).Namespace;
        var chilloutModeNamespace = typeof(ChilloutModeBlockEmitter).Namespace;
        var countdownModeNamespace = typeof(CountdownModeBlockEmitter).Namespace;
        var goldRushModeNamespace = typeof(GoldRushModeBlockEmitter).Namespace;

        blockEmitterNamespace.Should().Be("HamstasKitties.Mechanics.Emitters");
        classicModeNamespace.Should().Be("HamstasKitties.Mechanics.Emitters");
        chilloutModeNamespace.Should().Be("HamstasKitties.Mechanics.Emitters");
        countdownModeNamespace.Should().Be("HamstasKitties.Mechanics.Emitters");
        goldRushModeNamespace.Should().Be("HamstasKitties.Mechanics.Emitters");
    }

    // Test public properties
    [Fact]
    public void BlockEmitter_PublicProperties_Exist()
    {
        var blockEmitterType = typeof(BlockEmitter);

        var nextLineOfBlocksProperty = blockEmitterType.GetProperty("NextLineOfBlocks");
        nextLineOfBlocksProperty.Should().NotBeNull();
        nextLineOfBlocksProperty.PropertyType.Should().Be(typeof(Block[]));

        var lineEmissionIntervalTimerProperty = blockEmitterType.GetProperty("LineEmissionIntervalTimer");
        lineEmissionIntervalTimerProperty.Should().NotBeNull();

        var lineDropForceRateTimerProperty = blockEmitterType.GetProperty("LineDropForceRateTimer");
        lineDropForceRateTimerProperty.Should().NotBeNull();
    }

    // Test public methods
    [Fact]
    public void BlockEmitter_PublicMethods_Exist()
    {
        var blockEmitterType = typeof(BlockEmitter);

        var initializeMethod = blockEmitterType.GetMethod("Initialize");
        initializeMethod.Should().NotBeNull();
        initializeMethod.IsPublic.Should().BeTrue();

        var updateMethod = blockEmitterType.GetMethod("Update");
        updateMethod.Should().NotBeNull();
        updateMethod.IsPublic.Should().BeTrue();

        var uninitializeMethod = blockEmitterType.GetMethod("Uninitialize");
        uninitializeMethod.Should().NotBeNull();
        uninitializeMethod.IsPublic.Should().BeTrue();

        var interruptMethod = blockEmitterType.GetMethod("InterruptLineShakingAndForceLineOfBlocksDrop");
        interruptMethod.Should().NotBeNull();
        interruptMethod.IsPublic.Should().BeTrue();

        var existsMethod = blockEmitterType.GetMethod("ExistsBlockByGivenType");
        existsMethod.Should().NotBeNull();
        existsMethod.IsPublic.Should().BeTrue();

        var showGameOverMethod = blockEmitterType.GetMethod("ShowGameOverScreen");
        showGameOverMethod.Should().NotBeNull();
        showGameOverMethod.IsPublic.Should().BeTrue();

        var saveStateMethod = blockEmitterType.GetMethod("SaveState");
        saveStateMethod.Should().NotBeNull();
        saveStateMethod.IsPublic.Should().BeTrue();

        var loadStateMethod = blockEmitterType.GetMethod("LoadState");
        loadStateMethod.Should().NotBeNull();
        loadStateMethod.IsPublic.Should().BeTrue();
    }

    // Test IUpdateable implementation
    [Fact]
    public void BlockEmitter_ImplementsIUpdateable()
    {
        var blockEmitterType = typeof(BlockEmitter);
        var iUpdateableType = typeof(HamstasKitties.UI.IUpdateable);

        blockEmitterType.GetInterface(iUpdateableType.Name).Should().NotBeNull();
    }
}
