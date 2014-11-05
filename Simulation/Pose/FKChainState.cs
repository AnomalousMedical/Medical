using Engine;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    /// <summary>
    /// A collection of link states in an FK chain.
    /// </summary>
    public class FKChainState : Saveable
    {
        private Dictionary<String, FKLinkState> links = new Dictionary<string, FKLinkState>();

        public FKChainState()
        {

        }

        /// <summary>
        /// Set the state of a link in this chain, if it already exists it will be replaced.
        /// If it does not exist it will be added.
        /// </summary>
        /// <param name="state">The state to update.</param>
        public void setLinkState(String name, Vector3 localTranslation, Quaternion localRotation)
        {
            FKLinkState linkState;
            if (links.TryGetValue(name, out linkState))
            {
                linkState.LocalTranslation = localTranslation;
                linkState.LocalRotation = localRotation;
            }
            else
            {
                links.Add(name, new FKLinkState(localTranslation, localRotation));
            }
        }

        /// <summary>
        /// Remove a state from the chain.
        /// </summary>
        /// <param name="state">The state to remove.</param>
        public void removeLinkState(String name)
        {
            links.Remove(name);
        }

        /// <summary>
        /// Set this chain state to the blended version of the passed start and end states.
        /// </summary>
        /// <param name="start">The start state.</param>
        /// <param name="end">The end state.</param>
        /// <param name="blendAmount">The amount to blend.</param>
        public void setToBlendOf(FKChainState start, FKChainState end, float blendAmount)
        {
            if(blendAmount < 0.0f)
            {
                blendAmount = 0.0f;
            }
            else if(blendAmount > 1.0f)
            {
                blendAmount = 1.0f;
            }

            foreach(var stateName in start.ChainStateNames)
            {
                FKLinkState startState = start[stateName];
                FKLinkState endState = end[stateName];

                Vector3 trans = startState.getBlendedLocalTranslation(endState, blendAmount);
                Quaternion rot = startState.getBlendedLocalRotation(endState, blendAmount);

                setLinkState(stateName, trans, rot);
            }
        }

        /// <summary>
        /// Get an enumerator over all the names of the states.
        /// </summary>
        public IEnumerable<String> ChainStateNames
        {
            get
            {
                return links.Keys;
            }
        }

        /// <summary>
        /// Get the state specified by name. If the state does not exist you will be given the identity state.
        /// </summary>
        /// <param name="name">The name of the state to find.</param>
        /// <returns>The state specified by name or an identity state if that does not exist.</returns>
        internal FKLinkState this[String name]
        {
            get
            {
                FKLinkState state;
                if (links.TryGetValue(name, out state))
                {
                    return state;
                }
                return FKLinkState.IdentityState;
            }
        }

        protected FKChainState(LoadInfo info)
        {
            info.RebuildDictionary("Link", links);
        }

        public void getInfo(SaveInfo info)
        {
            info.ExtractDictionary("Link", links);
        }
    }
}
