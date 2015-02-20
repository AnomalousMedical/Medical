﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public class MovementSequenceGroup
    {
        private LinkedList<MovementSequenceInfo> sequences = new LinkedList<MovementSequenceInfo>();

        public MovementSequenceGroup(String name)
        {
            this.Name = name;
        }

        public void addSequence(MovementSequenceInfo info)
        {
            sequences.AddLast(info);
        }

        public void removeSequence(MovementSequenceInfo info)
        {
            sequences.Remove(info);
        }

        public String Name { get; private set; }

        public IEnumerable<MovementSequenceInfo> Sequences
        {
            get
            {
                return sequences;
            }
        }

        public int Count
        {
            get
            {
                return sequences.Count;
            }
        }
    }
}
