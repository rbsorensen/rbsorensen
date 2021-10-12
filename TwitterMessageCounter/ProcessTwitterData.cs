using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TwitterMessageCounter
{
    // Callback delegate
    public delegate void ProcessDataCallback(int messageCount);

    /// <summary>
    /// Process Twitter data thread class
    /// </summary>
    public class ProcessTwitterData
    {
        private static readonly object lockObject = new object();
        private string m_TwitterText;
        private ProcessDataCallback m_Callback;

        public ProcessTwitterData(string twitterText, ProcessDataCallback callback)
        {
            m_TwitterText = twitterText;
            m_Callback = callback;
        }

        /// <summary>
        /// Process data execution thread
        /// </summary>
        public void ProcessData()
        {
            try
            {
                // More processing of the twitter data could be done here - we only look for the number of twitter messages in the text 
                int messageCount = Regex.Matches(m_TwitterText, "\"id\"", RegexOptions.IgnoreCase).Count;

                // Only allow one thread at a time to perform the callback
                lock (lockObject)
                {
                    m_Callback(messageCount);
                }
            }
            catch
            {
            }
        }
    }
}
