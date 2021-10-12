using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TwitterMessageCounter
{
    public partial class frmMain : Form
    {
        HttpWebRequest m_Request = null;
        HttpWebResponse m_Response = null;
        Stream m_Stream = null;
        StreamReader m_Reader = null;
        Stopwatch m_StopWatch = null;
        int m_CurrentMessages = 0;
        int m_TotalMessages = 0;

        public frmMain()
        {
            InitializeComponent();
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            CloseApp();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                // URL for Twitter access
                string url = "https://api.twitter.com/2/tweets/sample/stream";
                // Bearer token for authorization - the token string could be stored in an environmental variable or in a config file, and could also be encrypted, but is here for simplicity
                string bearerToken = "AAAAAAAAAAAAAAAAAAAAAESmUAEAAAAAsmaTmr9%2F25nSm7xRfes6w7SJwxg%3D0Ikv50srvMU8h0yiEYQs5P5v3FQFru8sFMRYVzXjN4k1zy3Q7G";

                Cursor.Current = Cursors.WaitCursor;
                btnStart.Enabled = false;
                // Create the request object
                m_Request = (HttpWebRequest)WebRequest.Create(url);
                // Set for no timeout
                m_Request.Timeout = -1;
                // Add the necessary auth header
                m_Request.Headers.Add("Authorization", string.Format("Bearer {0}", bearerToken));

                // Get the sample stream data from Twitter 
                m_Response = (HttpWebResponse)m_Request.GetResponse();
                m_Stream = m_Response.GetResponseStream();
                m_Reader = new StreamReader(m_Stream);

                // Start the stopwatch timer
                m_StopWatch = new Stopwatch();
                m_StopWatch.Start();
                m_CurrentMessages = 0;
                m_TotalMessages = 0;
                lblTotalMessages.Text = "";
                lblAverageMessages.Text = "";

                // Continue reading lines until the app is terminated
                while (true)
                {
                    // Periodically display the results
                    m_CurrentMessages++;
                    if (m_CurrentMessages > 10)
                    {
                        ProcessDataDisplay();
                        m_CurrentMessages = 0;
                    }

                    // Read a line of data
                    if (m_Reader != null)
                    {
                        string twitterText = m_Reader.ReadLine();
                        // Launch a thread to process the data - thread will automaticallly terminate after doing its thing
                        ProcessTwitterData processTwitterData = new ProcessTwitterData(twitterText, new ProcessDataCallback(ProcessDataResult));
                        Thread processTwitterDataThread = new Thread(new ThreadStart(processTwitterData.ProcessData));
                        processTwitterDataThread.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
                CloseConnections();
                btnStart.Enabled = true;
                btnStart.Focus();
                Cursor.Current = Cursors.Default;
            }
        }

        private void ctlTimer_Tick(object sender, EventArgs e)
        {
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                CloseApp();
        }

        /// <summary>
        /// Terminate the app
        /// </summary>
        private void CloseApp()
        {
            CloseConnections();
            Environment.Exit(0);
        }

        /// <summary>
        /// Close the connections
        /// </summary>
        private void CloseConnections()
        {
            try
            {
                if (m_StopWatch != null && m_StopWatch.IsRunning)
                    m_StopWatch.Stop();

                if (m_Reader != null)
                {
                    m_Reader.Close();
                    m_Reader.Dispose();
                    m_Reader = null;
                }
            }
            catch
            {
            }

            try
            {
                if (m_Stream != null)
                {
                    m_Stream.Close();
                    m_Stream.Dispose();
                    m_Stream = null;
                }
            }
            catch
            {
            }

            try
            {
                if (m_Response != null)
                {
                    m_Response.Close();
                    m_Response = null;
                }
            }
            catch
            {
            }

            try
            {
                if (m_Request != null)
                {
                    m_Request.Abort();
                    m_Request = null;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Display the message results
        /// </summary>
        private void ProcessDataDisplay()
        {
            // Set the label controls so that the results will show up
            lblTotalMessages.Text = m_TotalMessages.ToString("##,###,##0");
            int minutes = (int)(m_StopWatch.ElapsedMilliseconds + 60000) / 60000;
            if (minutes > 1)
                lblAverageMessages.Text = (m_TotalMessages / minutes).ToString("##,##0");

            // The refresh and doevents calls are necessary for the UI thread to be responsive - would be eliminated for a non-UI app 
            lblTotalMessages.Refresh();
            lblAverageMessages.Refresh();
            Application.DoEvents();
        }

        /// <summary>
        /// Update the message counter
        /// </summary>
        /// <param name="messageCount"></param>
        private void ProcessDataResult(int messageCount)
        {
            try
            {
                m_TotalMessages += messageCount;
            }
            catch
            {
            }
        }
    }
}
