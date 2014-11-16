using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace IrcDotRT
{
    using Collections;

    /// <summary>
    /// Represents an IRC user that exists on a specific channel on a specific <see cref="IrcClient"/>.
    /// </summary>
    /// <threadsafety static="true" instance="false"/>
    /// <seealso cref="IrcUser"/>
    /// <seealso cref="IrcChannel"/>
    [DebuggerDisplay("{ToString(), nq}")]
    public class IrcChannelUser : INotifyPropertyChanged
    {
        // Collection of channel modes currently active on user.
        private HashSet<char> modes;
        private ReadOnlySet<char> modesReadOnly;

        private IrcChannel channel;
        private IrcUser user;

        internal IrcChannelUser(IrcUser user, IEnumerable<char> modes = null)
        {
            this.user = user;

            this.modes = new HashSet<char>();
            modesReadOnly = new ReadOnlySet<char>(this.modes);
            if (modes != null)
                this.modes.AddRange(modes);
        }

        /// <summary>
        /// A read-only collection of the channel modes the user currently has.
        /// </summary>
        /// <value>The current channel modes of the user.</value>
        public ReadOnlySet<char> Modes
        {
            get { return modesReadOnly; }
        }

        /// <summary>
        /// Gets or sets the channel.
        /// </summary>
        /// <value>The channel.</value>
        public IrcChannel Channel
        {
            get { return channel; }
            internal set
            {
                channel = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Channel"));
            }
        }

        /// <summary>
        /// Gets the <see cref="IrcUser"/> that is represented by the <see cref="IrcChannelUser"/>.
        /// </summary>
        /// <value>The <see cref="IrcUser"/> that is represented by the <see cref="IrcChannelUser"/>.</value>
        public IrcUser User
        {
            get { return user; }
        }

        /// <summary>
        /// Occurs when the channel modes of the user have changed.
        /// </summary>
        public event EventHandler<EventArgs> ModesChanged;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Kicks the user from the channel, giving the specified comment.
        /// </summary>
        /// <param name="comment">The comment to give for the kick, or <see langword="null"/> for none.</param>
        public void Kick(string comment = null)
        {
            channel.Kick(user.NickName, comment);
        }

        /// <summary>
        /// Gives the user operator privileges in the channel.
        /// </summary>
        public void Op()
        {
            channel.SetModes("+o", user.NickName);
        }

        /// <summary>
        /// Removes operator privileges from the user in the channel.
        /// </summary>
        public void DeOp()
        {
            channel.SetModes("-o", user.NickName);
        }

        /// <summary>
        /// Voices the user in the channel.
        /// </summary>
        public void Voice()
        {
            channel.SetModes("+v", user.NickName);
        }

        /// <summary>
        /// Devoices the user in the channel
        /// </summary>
        public void DeVoice()
        {
            channel.SetModes("-v", user.NickName);
        }

        internal void HandleModeChanged(bool add, char mode)
        {
            lock (((ICollection)modesReadOnly).SyncRoot)
            {
                if (add)
                    modes.Add(mode);
                else
                    modes.Remove(mode);
            }

            OnModesChanged(new EventArgs());
        }

        /// <summary>
        /// Raises the <see cref="ModesChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        protected virtual void OnModesChanged(EventArgs e)
        {
            var handler = ModesChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format("{0}/{1}", channel.Name, user.NickName);
        }
    }
}
