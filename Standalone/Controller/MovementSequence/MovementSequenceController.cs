﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Muscles;
using Engine.Resources;
using System.Xml;
using Logging;
using Engine.Saving.XMLSaver;
using Engine.Platform;
using Engine;
using Engine.ObjectManagement;

namespace Medical.Controller
{
    /// <summary>
    /// An event for the MuscleSequenceController.
    /// </summary>
    /// <param name="controller">The controller that fired the event.</param>
    public delegate void MovementSequenceEvent(MovementSequenceController controller);
    public delegate void MovementSequenceGroupEvent(MovementSequenceController controller, MovementSequenceGroup group);
    public delegate void MovementSequenceInfoEvent(MovementSequenceController controller, MovementSequenceGroup group, MovementSequenceInfo sequenceInfo);

    /// <summary>
    /// This class manages loading and playback of movement sequences.
    /// </summary>
    public class MovementSequenceController
    {
        public event MovementSequenceGroupEvent GroupAdded;
        public event MovementSequenceGroupEvent GroupRemoved;
        public event MovementSequenceInfoEvent SequenceAdded;
        public event MovementSequenceInfoEvent SequenceRemoved;
        public event MovementSequenceEvent CurrentSequenceChanged;
        public event MovementSequenceEvent PlaybackStarted;
        public event MovementSequenceEvent PlaybackStopped;
        public event MovementSequenceEvent PlaybackUpdate;

        private XmlSaver xmlSaver = new XmlSaver();
        private MovementSequence currentSequence;
        private MedicalController medicalController;
        private float currentTime = 0.0f;
        private bool playing = false;
        private MovementSequenceSet currentSequenceSet = new MovementSequenceSet();

        private MusclePosition neutralMovementState = new MusclePosition();

        public MovementSequenceController(MedicalController medicalController)
        {
            this.medicalController = medicalController;
        }

        internal void sceneLoaded(SimScene scene)
        {
            neutralMovementState.captureState();
        }

        /// <summary>
        /// Load the specified sequence and return it.
        /// </summary>
        /// <param name="sequenceInfo">The filename of the sequence to load.</param>
        /// <returns>The loaded sequence.</returns>
        public MovementSequence loadSequence(MovementSequenceInfo sequenceInfo)
        {
            return sequenceInfo.loadSequence(xmlSaver);
        }

        public void addMovementSequence(String groupName, MovementSequenceInfo info)
        {
            MovementSequenceGroup group = currentSequenceSet.getGroup(groupName);
            if (group == null)
            {
                group = new MovementSequenceGroup(groupName);
                currentSequenceSet.addGroup(group);
                if (GroupAdded != null)
                {
                    GroupAdded.Invoke(this, group);
                }
            }
            group.addSequence(info);
            if (SequenceAdded != null)
            {
                SequenceAdded.Invoke(this, group, info);
            }
        }

        public void removeMovementSequence(String groupName, MovementSequenceInfo info)
        {
            MovementSequenceGroup group = currentSequenceSet.getGroup(groupName);
            if (group != null)
            {
                if (info != null)
                {
                    if (SequenceRemoved != null)
                    {
                        SequenceRemoved.Invoke(this, group, info);
                    }
                    group.removeSequence(info);
                    if (group.Count == 0)
                    {
                        currentSequenceSet.removeGroup(group);
                        if (GroupRemoved != null)
                        {
                            GroupRemoved.Invoke(this, group);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Play the current sequence.
        /// </summary>
        public void playCurrentSequence()
        {
            if (currentSequence != null && !playing)
            {
                currentTime = 0.0f;
                playing = true;
                medicalController.OnLoopUpdate += medicalController_OnLoopUpdate;
                if (PlaybackStarted != null)
                {
                    PlaybackStarted.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Stop playing the current sequence.
        /// </summary>
        public void stopPlayback()
        {
            if (playing)
            {
                medicalController.OnLoopUpdate -= medicalController_OnLoopUpdate;
                playing = false;
                if (currentSequence != null)
                {
                    currentSequence.setPosition(0.0f);
                }
                if (PlaybackStopped != null)
                {
                    PlaybackStopped.Invoke(this);
                }
            }
        }

        /// <summary>
        /// Stops playing the current sequence, but does not reset the time back
        /// to 0. This will effectivly leave the scene as it was when the
        /// sequence was playing. Still fires the PlaybackStopped event.
        /// </summary>
        public void pausePlayback()
        {
            if (playing)
            {
                medicalController.OnLoopUpdate -= medicalController_OnLoopUpdate;
                playing = false;
                if (PlaybackStopped != null)
                {
                    PlaybackStopped.Invoke(this);
                }
            }
        }

        /// <summary>
        /// The sequence that is currently loaded for playback or manipulation.
        /// </summary>
        public MovementSequence CurrentSequence
        {
            get
            {
                return currentSequence;
            }
            set
            {
                if (currentSequence != value)
                {
                    currentSequence = value;
                    if (CurrentSequenceChanged != null)
                    {
                        CurrentSequenceChanged.Invoke(this);
                    }
                    currentTime = 0.0f;
                }
            }
        }

        /// <summary>
        /// The set of all sequences.
        /// </summary>
        public MovementSequenceSet SequenceSet
        {
            get
            {
                return currentSequenceSet;
            }
        }

        public bool Playing
        {
            get
            {
                return playing;
            }
        }

        public float CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
                if (currentSequence.Duration != 0.0f)
                {
                    currentTime %= currentSequence.Duration;
                }
                currentSequence.setPosition(currentTime);
            }
        }

        /// <summary>
        /// This is the muscle position that the scene has when it loads.
        /// </summary>
        public MusclePosition NeutralMovementState
        {
            get
            {
                return neutralMovementState;
            }
        }

        /// <summary>
        /// Update function during playback.
        /// </summary>
        /// <param name="time">The time delta.</param>
        void medicalController_OnLoopUpdate(Clock time)
        {
            CurrentTime += time.DeltaSeconds;
            if (PlaybackUpdate != null)
            {
                PlaybackUpdate.Invoke(this);
            }
        }
    }
}
