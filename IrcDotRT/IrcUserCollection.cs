using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace IrcDotRT
{
    using Collections;

    /// <summary>
    /// Represents a collection of <see cref="IrcUser"/> objects.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <seealso cref="IrcUser"/>
    public class IrcUserCollection : ReadOnlyCollection<IrcUser>
    {
        private IrcClient client;
        
        internal IrcUserCollection(IrcClient client, IList<IrcUser> list)
            : base(list)
        {
            this.client = client;
        }

        /// <summary>
        /// Gets the IrcUser associated with the specified nickname.
        /// </summary>
        /// <param name="nickName">The nickname of the user to return</param>
        /// <returns>The user with the specified nickname</returns>
        public IrcUser this[string nickName]
        {
            get { return this.Where(c => c.NickName == nickName).First(); }
        }

        /// <summary>
        /// Gets the client to which the collection of users belongs.
        /// </summary>
        /// <value>The client to which the collection of users belongs.</value>
        public IrcClient Client
        {
            get { return client; }
        }
    }
}
