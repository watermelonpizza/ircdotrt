using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IrcDotRT
{
    internal class MessageProcessorAttribute : Attribute
    {
        public MessageProcessorAttribute(string commandName)
        {
            CommandName = commandName;
        }

        public string CommandName
        {
            get;
            private set;
        }
    }
}
