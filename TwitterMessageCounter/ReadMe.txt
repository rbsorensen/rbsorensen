This app reads the Twitter sample stream tweet data and periodically displays the count of messages returned as well as the average count per minute.

It is a .Net Framework 4.8 WinForms app with a UI for launching, terminating, and displaying results from the processing.

The Twitter data stream is continuous, so after every x messages have been read, the UI for the app is refreshed in order to view the results and handle app termination events.

As per the specification for this app, threading has been implemented to process each message.  But given the fact that processing of the data is minimal threading here serves no real purpose, other than as a programming example.
