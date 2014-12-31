﻿using Anomalous.GuiFramework;
using Engine;
using Engine.Attributes;
using Engine.Editing;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Medical
{
    public class Slideshow : Saveable
    {
        public const String SlideThumbName = "Thumb.png";
        public static readonly int BaseSlideScale = ScaleHelper.Scaled(1017);
        public const int CurrentVersion = 3;

        private static TaskController additionalTasks = new TaskController();

        public static TaskController AdditionalTasks
        {
            get
            {
                return additionalTasks;
            }
        }

        [DoNotSave]
        private List<Slide> slides = new List<Slide>();

        private int version;
        private bool vectorMode = true;

        public Slideshow()
        {
            version = CurrentVersion;
        }

        public void addSlide(Slide slide)
        {
            slides.Add(slide);
        }

        public void removeSlide(Slide slide)
        {
            slides.Remove(slide);
        }

        public void removeAt(int index)
        {
            slides.RemoveAt(index);
        }

        public void insertSlide(Slide before, Slide insert)
        {
            insertSlide(slides.IndexOf(before), insert);
        }

        public void insertSlide(int index, Slide slide)
        {
            slides.Insert(index, slide);
        }

        public int indexOf(Slide slide)
        {
            return slides.IndexOf(slide);
        }

        public Slide get(int index)
        {
            return slides[index];
        }

        public AnomalousMvcContext createContext(ResourceProvider resourceProvider, GUIManager guiManager, int startIndex = 0)
        {
            SlideshowRuntime slideshowRuntime = new SlideshowRuntime(this, resourceProvider, guiManager, startIndex, additionalTasks);
            return slideshowRuntime.Context;
        }

        public void cleanup(CleanupInfo cleanupInfo, ResourceProvider resourceProvider)
        {
            cleanupInfo.defineObjectClass(Slide.SlideActionClass);
            cleanupInfo.defineObjectClass(ShowPropAction.PropClass);
            foreach (Slide slide in slides)
            {
                slide.cleanup(cleanupInfo, resourceProvider);
            }
        }

        public IEnumerable<Slide> Slides
        {
            get
            {
                return slides.AsReadOnly();
            }
        }

        public int Count
        {
            get
            {
                return slides.Count;
            }
        }

        public int Version
        {
            get
            {
                return version;
            }
        }

        public bool VectorMode
        {
            get
            {
                return vectorMode;
            }
            set
            {
                vectorMode = value;
            }
        }

        public void updateToVersion(int newVersion, ResourceProvider slideshowResources)
        {
            if (newVersion <= CurrentVersion && newVersion > version)
            {
                foreach (Slide slide in slides)
                {
                    slide.updateToVersion(version, newVersion, slideshowResources);
                }
                version = newVersion;
            }
        }

        protected Slideshow(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info, ReflectedSaver.DefaultScanner);
            info.RebuildList("Slides", slides);
        }

        public virtual void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info, ReflectedSaver.DefaultScanner);
            info.ExtractList("Slides", slides);
        }
    }
}
