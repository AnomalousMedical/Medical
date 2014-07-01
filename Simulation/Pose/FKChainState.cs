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
            FKLinkState state = new FKLinkState(localTranslation, localRotation);
            if (links.ContainsKey(name))
            {
                links[name] = state;
            }
            else
            {
                links.Add(name, state);
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
        /// Get the state specified by name. If the state does not exist you will be given the identity state.
        /// </summary>
        /// <param name="name">The name of the state to find.</param>
        /// <returns>The state specified by name or an identity state if that does not exist.</returns>
        public FKLinkState this[String name]
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
