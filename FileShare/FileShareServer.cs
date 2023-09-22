/************************************************************

 FileShareServer.cs  
		CopyRight 2000-2001
		This is a sample program made by Saurabh Nandu.
		E-mail: saurabhn@webveda.com
    	Website: learncsharp.cjb.net
		This program uses the 'System.Net.Sockets' class to act as a FileServer which waits for
		multiple clients to connect to it. Then the clients have the chance to Upload a file to the server
		or the client can download any of the file the server has for download.
		This file contains the source code for the multi-threaded FileServer
		31/01/2001
		compile: 
csc /r:System.dll;System.Drawing.dll;System.Winforms.dll;System.IO.dll;System.Net.dll;Microsoft.win32.Interop.dll FileShareServer.cs

************************************************************/

namespace SaurabhFileShare
{

    using System;
    using System.ComponentModel;
    using System.Net.Sockets;
    using System.Net;
    using System.Threading;
    using System.IO;


    /// <summary>
    ///		The Server class from the FileShare Client/Server Collection   
    /// </summary>
    public class FileShareServer 
    {

        /// <summary> 
        ///    Required by the Win Forms designer 
        /// </summary>
        private System.ComponentModel.Container components;
        //private System.WinForms.Label serverl;
        //private System.WinForms.RichTextBox serverlogt;
        //private System.WinForms.ListBox filelistBox;
        //private System.WinForms.Label filesl;
        //private System.WinForms.Label userl;
        //private System.WinForms.ListBox userlistBox;
        //a instance of your custom dialog

        //private System.WinForms.MainMenu mainMenu1;

        private ServerDialog fsd;
        //Default values of some variables
        private int _port = 4455;
        private int _Maxcon = 5;
        private int _clientcount;
        private Socket _serversocket = null;
        private Socket _clientsock = null;
        private string _filedown;
        private string _fileup;
        private bool _allowup;
        private bool _allowdown;
        private string _password;
        private Thread _serverthread = null;
        private FileInfo[] _dlfiles;
        
        ///<summary>
        ///		The constructor 
        ///</summary>
        public FileShareServer()
        {

            // Required for Win Form Designer support
            InitializeComponent();
        }

        /// <summary>
        ///    Clean up any resources being used
        /// </summary>
        public override void Dispose()
        {
            if (_serversocket != null)
            {
                //close the server socket
                _serversocket.Close();
            }
            if (_clientsock != null)
            {
                //close the Client socket
                _clientsock.Close();
            }
            if (_serverthread != null && _serverthread.IsAlive)
            {
                //close the server Thread
                _serverthread.Abort();
            }
            base.Dispose();
            components.Dispose();
        }


        /// <summary>
        ///    The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            Application.Run(new FileShareServer());
        }


        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with an editor
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.userlistBox = new System.WinForms.ListBox();
            this.serverlogt = new System.WinForms.RichTextBox();
            this.filelistBox = new System.WinForms.ListBox();
            this.mainMenu1 = new System.WinForms.MainMenu();
            this.userl = new System.WinForms.Label();
            this.serverl = new System.WinForms.Label();
            this.filesl = new System.WinForms.Label();

            //the Menu
            MenuItem ServerMenu = new MenuItem("Server");
            mainMenu1.MenuItems.Add(ServerMenu);
            ServerMenu.MenuItems.Add(new MenuItem("Start Server", new EventHandler(startserver)));
            ServerMenu.MenuItems.Add(new MenuItem("-"));
            ServerMenu.MenuItems.Add(new MenuItem("Stop Server", new EventHandler(stopserver)));
            ServerMenu.MenuItems.Add(new MenuItem("-"));
            ServerMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(serverexit)));
            MenuItem ContactMenu = new MenuItem("Contact Me");
            mainMenu1.MenuItems.Add(ContactMenu);
            ContactMenu.MenuItems.Add(new MenuItem("Contact", new EventHandler(contactme)));

            //@design this.TrayHeight = 90;
            //@design this.TrayLargeIcon = false;
            //@design this.TrayAutoArrange = true;
            userlistBox.Location = new System.Drawing.Point(8, 40);
            userlistBox.Size = new System.Drawing.Size(152, 169);
            userlistBox.Font = new System.Drawing.Font("Impact", 8f);
            userlistBox.TabIndex = 0;
            userlistBox.TabStop = false;

            serverlogt.ReadOnly = true;
            serverlogt.Size = new System.Drawing.Size(312, 368);
            serverlogt.ForeColor = System.Drawing.SystemColors.Window;
            serverlogt.TabIndex = 0;
            serverlogt.AutoSize = true;
            serverlogt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold);
            serverlogt.AcceptsTab = true;
            serverlogt.TabStop = false;
            serverlogt.Location = new System.Drawing.Point(176, 40);
            serverlogt.BackColor = System.Drawing.Color.Orange;

            filelistBox.Location = new System.Drawing.Point(8, 240);
            filelistBox.Size = new System.Drawing.Size(152, 160);
            filelistBox.HorizontalScrollbar = true;
            filelistBox.TabIndex = 0;
            filelistBox.TabStop = false;

            //@design mainMenu1.SetLocation(new System.Drawing.Point(7, 7));

            userl.Location = new System.Drawing.Point(10, 15);
            userl.Text = "Users Connected";
            userl.Size = new System.Drawing.Size(118, 16);
            userl.AutoSize = true;
            userl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            userl.TabIndex = 0;
            userl.BackColor = System.Drawing.Color.DarkOrange;

            serverl.Location = new System.Drawing.Point(176, 16);
            serverl.Text = "Server Activity Log";
            serverl.Size = new System.Drawing.Size(130, 16);
            serverl.AutoSize = true;
            serverl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            serverl.TabIndex = 0;
            serverl.BackColor = System.Drawing.Color.DarkOrange;

            filesl.Location = new System.Drawing.Point(8, 220);
            filesl.Text = "Files For Download";
            filesl.Size = new System.Drawing.Size(132, 16);
            filesl.AutoSize = true;
            filesl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            filesl.TabIndex = 0;
            filesl.BackColor = System.Drawing.Color.DarkOrange;
            this.Text = "FileShare Server , By Saurabh Nandu http://Learncsharp.cjb.n" +
                "et";
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.Menu = mainMenu1;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(492, 413);

            this.Controls.Add(serverl);
            this.Controls.Add(serverlogt);
            this.Controls.Add(filelistBox);
            this.Controls.Add(filesl);
            this.Controls.Add(userl);
            this.Controls.Add(userlistBox);
        }

        ///<summary>
        ///		This method is called when 'Start Server' is selected from the Server Menu
        ///		It First brings up the Dialog box to set the server properties
        ///		Then it starts the server based on the properties set by the user
        ///</summary>
        private void startserver(object sender, EventArgs e)
        {
            //Make a instance of our custom Dialog
            fsd = new ServerDialog();
            //Get the Result of the Dialog Box 
            DialogResult ret = fsd.ShowDialog(this);
            //If 'Ok' was pressed on the Dialog box then assign the dialog box values to the server variables   		
            if (ret == DialogResult.OK)
            {
                //get the properties set in the dialog box
                this._port = int.Parse(fsd.portt.Text);
                this._Maxcon = int.Parse(fsd.maxt.Text);
                this._fileup = fsd.fileupt.Text;
                this._filedown = fsd.filedlt.Text;
                this._allowup = fsd.allowup.Checked;
                this._allowdown = fsd.allowdl.Checked;
                this._password = fsd.passt.Text;
                if (!Directory.DirectoryExists(_filedown))
                {
                    MessageBox.Show(this, "The directory to Download Files Does not Exist \n Try Starting Again");
                }
                else if (!Directory.DirectoryExists(_fileup))
                {
                    MessageBox.Show(this, "The directory to Upload Files Does not Exist \n Try Starting Again");
                }
                else
                {
                    //Call a function which will get all the files in the "Download" directory
                    //of the server 
                    GetDirs();
                    //Start the Main Server Thread which will wait for users to connect
                    _serverthread = new Thread(new ThreadStart(InitilizeSocket));
                    _serverthread.Start();
                }

            }
        }

        ///<summary>
        ///		This method is called when 'Stop Server' is selected from the server Menu
        ///		It closes down all the threads and Sockets open
        ///</summary>
        private void stopserver(object sender, EventArgs e)
        {
            if (_serversocket != null)
            {
                //close the server socket
                _serversocket.Close();
            }
            if (_clientsock != null)
            {
                //close the Client socket
                _clientsock.Close();
            }
            if (_serverthread != null && _serverthread.IsAlive)
            {
                //close the server Thread
                _serverthread.Abort();
            }
            filelistBox.Items.Clear();
            userlistBox.Items.Clear();
            serverlogt.Text += "Server Shutdown !! \n";
        }

        ///<summary>
        ///		This method exits the Server Application
        ///</summary>
        private void serverexit(object sender, EventArgs e)
        {
            Dispose();
        }
        ///<summary>
        ///		Info About Me
        ///</summary>
        private void contactme(object sender, EventArgs e)
        {
            MessageBox.Show(this, "This is the FileShare Server made by Saurabh Nandu,\n E-mail: saurabhn@webveda.com \n Website:http//learncsharp.cjb.net");
        }

        ///<summary>
        ///		This method Gets all the fles in the Directory set by the user from where the user can
        ///		download files. It stores this in the ListBox on the Server
        ///</summary>
        private void GetDirs()
        {
            //check if the directory specified by the user exists
            if (Directory.DirectoryExists(_filedown) && _allowdown)
            {
                //store all the file names in a Array 
                _dlfiles = Directory.GetFilesInDirectory(_filedown);

                foreach (File f in _dlfiles)
                {
                    //Insert the Items into the ListBox
                    filelistBox.InsertItem(0, f);
                }
            }
        }

        ///<summary>
        ///	<para>
        ///		This is the main method which does all the Initial work to start-up the server
        ///	</para>
        ///	<para>
        ///		It runs in a while loop and accepts new Clients till the Max client capacity
        ///		specified by the user is reached.
        ///		Once Its there it stops acception new users
        ///	</para>
        ///</summary>
        public void InitilizeSocket()
        {
            //create a Socket of the Protocal TCP
            _serversocket = new Socket(AddressFamily.AfINet, SocketType.SockStream, ProtocolType.ProtTCP);
            _serversocket.Blocking = true;
            //Bind to the port specified by the user
            if (_serversocket.Bind(new IPEndPoint(IPAddress.InaddrAny, _port)) != 0)
            {
                serverlogt.Text += "Unable to bind to port:" + _port.ToString() + " \n";
            }
            //start listning for new Clients
            _serversocket.Listen(-1);
            serverlogt.Text += "Starting to listen on port :" + _port.ToString() + " \n";
            try
            {
                while (true)
                {
                    //Accept a new user 
                    Socket sock = _serversocket.Accept();
                    //Increase the clientcount (i.e. The number of clients connected to the server
                    _clientcount += 1;
                    //if client count is satisfied
                    if (sock != null && _clientcount <= _Maxcon)
                    {
                        serverlogt.Text += "Client Connected \n";
                        _clientsock = sock;
                        if (_clientsock != null)
                        {
                            //Accept the new Client and Start A seprate Thread for the New Client
                            Thread lis = new Thread(new ThreadStart(listensend));
                            lis.Start();
                        }
                    }
                    else
                    {
                        //If Max COnnections are reached then Disconnect the user
                        SendMessage(_clientsock, "NOPE Server Readched Max Limit");
                        _clientsock.Close();
                        _clientcount -= 1;
                    }
                }
            }
            catch (Exception ed)
            {
                MessageBox.Show(this, "Error Cannot Accept Clients !! " + ed.ToString());
            }

        }
        ///<summary>
        ///		This is the Method which get called for each connected Client
        ///		It Takes care of all the Receving and Sending 
        ///</summary>
        public void listensend()
        {
            //Get a copy of the Client Socket 
            Socket mySocket = _clientsock;
            //SendMessage is a Custom Method we have written below
            SendMessage(mySocket, "CONN Welcome to File Share Server");
            //Variable to check to connection
            bool DoNotExit = true;
            while (DoNotExit)
            {
                string ClientCommand;
                //A byte array to store the Received Command
                byte[] recs = new byte[2048];
                try
                {
                    //Receive a Message from Client
                    int rcount = mySocket.Receive(recs, recs.Length, 0);
                    //Decode the Message
                    string clientmessage = System.Text.Encoding.ASCII.GetString(recs);
                    if (rcount > 0)
                    {
                        //Call a Custom Method writen below to Parse the Message sent by the client
                        ClientCommand = ParseMessage(clientmessage);
                    }
                    else
                    {
                        //a command to do Nothing
                        ClientCommand = "NOOP";
                    }
                    //Call a Custom Method writen below to Take Action According to the Message Sent
                    DoNotExit = ParseCommand(ClientCommand, clientmessage, mySocket);

                }
                catch (Exception e)
                {
                    MessageBox.Show(this, "a Exception occured in clientthread :" + e.ToString());
                }
            }
        }

        ///<summary>
        ///	This method Sends the input message on the Given Socket
        ///</summary>
        private void SendMessage(Socket sendsock, string message)
        {
            byte[] sender = System.Text.Encoding.ASCII.GetBytes(message.ToCharArray());
            sendsock.Send(sender, sender.Length, 0);
        }

        ///<summary>
        ///		This Method returns the Command Sent by the client from the Message sent by the Client
        ///</summary>
        private string ParseMessage(string mess)
        {
            string ClientCommand;
            if (mess == "")
            {
                //return a standard command
                return "NOOP";
            }
            //All commands used in this example are of 4 Bytes hence we substring
            //the first 4 bytes of the total Message sent by the user
            ClientCommand = mess.Substring(0, 4);
            return ClientCommand;
        }
        ///<summary>
        ///		This Method inputs the Client Command , Message and Socket
        ///		Based on the client command it takes the necessary action
        ///</summary>
        private bool ParseCommand(string ClientCommand, string clientmessage, Socket mySocket)
        {
            //When the Client Connects It sends a USER command
            if (ClientCommand == "USER")
            {
                string temp = clientmessage.Substring(4);
                string mes = temp.Trim();
                serverlogt.Text += "User :" + mes;
                serverlogt.Text += "\t";
                serverlogt.Text += mySocket.RemoteEndpoint.ToString();
                serverlogt.Text += "\n";
                //Add the User to the ListBox
                userlistBox.InsertItem(0, mes);
                //In response to the USER command we send the user the FileNames
                //of All the files available for download
                SendMessage(mySocket, senddirectory());
                return true;
            }
            //NOOP is a command to do Nothing ..
            if (ClientCommand == "NOOP")
            {
                //Thread.Sleep(1000) ;
                return true;
            }
            //Client sends this Command in reply of a Error 
            if (ClientCommand == "OHHH")
            {
                return true;
            }
            //When the Client Quits a QUIT comand is Sent 
            if (ClientCommand == "QUIT")
            {
                string temp = clientmessage.Substring(4);
                string mes = temp.Trim();
                int i = userlistBox.FindStringExact(mes);
                serverlogt.Text += mes + " Has Quit \n ";
                //Decrease the total Count of users connected to the Server
                _clientcount -= 1;
            }
            //If a file is downloaded then the client Sends the Command
            if (ClientCommand == "RECD")
            {
                return true;
            }

            // When a Client wants to download a file, it sends the DOWN command
            //	along with the filename it wants to Download
            //In response the server sends the Size of the File requested
            if (ClientCommand == "DOWN")
            {
                bool check = false;
                string temp = clientmessage.Substring(4);
                string mes = temp.Trim();
                string fullpath = _filedown + mes;
                //Make a file out of the sent filename
                File ftemp = new File(_filedown + mes);
                check = true;
                if (check && _allowdown)
                {
                    //If the File is Found and Download is Allowed by the server 
                    //then sends the file size with the SIZE command
                    SendMessage(mySocket, "SIZE " + ftemp.Length.ToString());
                    ftemp = null;
                }
                else if (check && !_allowdown)
                {
                    SendMessage(mySocket, "NOPE Download not Allowed!");
                }
                else
                {
                    SendMessage(mySocket, "NOPE FileNotFound");
                }
                ftemp = null;
                return true;
            }

            //This is the Second command the in the Downloading of a File by the client
            //on receving the File Size the client sends the SEND command
            //In response the server sends the file to the client
            if (ClientCommand == "SEND")
            {
                try
                {
                    File ftemp;
                    string temp = clientmessage.Substring(4);
                    string mes = temp.Trim();
                    ftemp = new File(_filedown + mes);
                    //Get the Length of the file requested
                    //and set various variables
                    long total = ftemp.Length;
                    long rdby = 0;
                    int len = 0;
                    byte[] buffed = new byte[4096];
                    //Open the file requested for download 
                    FileStream fin = new FileStream(_filedown + mes, FileMode.Open, FileAccess.Read);
                    //One way of transfer over sockets is Using a NetworkStream 
                    //It provides some useful ways to transfer data 
                    NetworkStream nfs = new NetworkStream(mySocket);

                    //lock the Thread here
                    lock (this)
                    {
                        while (rdby < total && nfs.CanWrite)
                        {
                            //Read from the File (len contains the number of bytes read)
                            len = fin.Read(buffed, 0, buffed.Length);
                            //Write the Bytes on the Socket
                            nfs.Write(buffed, 0, len);
                            //Increase the bytes Read counter
                            rdby = rdby + len;
                        }
                    }
                    //Display a Message Showing Sucessful File Transfer
                    serverlogt.Text += "Sent file " + ftemp.FullName + " \n";
                    fin.Close();
                    return true;
                }
                catch (Exception ed)
                {
                    MessageBox.Show(this, "A Exception occured in transfer" + ed.ToString());
                    return true;
                }

            }
            //Client Send the Command UPFL when It wants to Upload a File to the Server
            //along with the UPFL command the client also sends the FileName and the File Size 
            //eg. UPFL c:\temp\readme.txt@1265
            //in response the Server sends a SEND command
            //upon receiving this Command the Client directly starts Uploading the File to the Server
            if (ClientCommand == "UPFL")
            {
                //Check is Server Allows Upload
                if (_allowup)
                {
                    string temp = clientmessage.Substring(4);
                    string mes = temp.Trim();
                    int var = mes.IndexOf("@");
                    //Get the File Size from the Client Message 
                    long size = Int64.Parse(mes.Substring(var + 1));
                    string filename = mes.Substring(0, var);
                    var = filename.LastIndexOf("\\");  //we use '\\' since a single slash is a escape sequence
                                                       //Substring the FileName to be uploaded by the client
                    string fileonly = filename.Substring(var + 1);
                    //Tell the Client to Send the File 
                    SendMessage(mySocket, "SEND File");
                    try
                    {
                        //Set the Variables
                        long rdby = 0;
                        int j = 0;
                        long v = 0;
                        byte[] writer = new byte[4096];
                        //Open a FileStrem to the New File 
                        FileStream fout = new FileStream(_fileup + fileonly, FileMode.OpenOrCreate, FileAccess.Write);
                        while (rdby < size)
                        {
                            //Read From the Socket
                            j = mySocket.Receive(writer, writer.Length, 0);
                            if (j > 0)
                            {
                                //Below we Calculate the Size of bytes to be written 
                                if (j >= 4096 && (size - rdby) >= 4096)
                                {
                                    v = 4096;
                                }
                                else if (j < 4096 && (size - rdby) >= 4096)
                                {
                                    v = j;
                                }
                                else
                                {
                                    v = (size - rdby);
                                }
                                //Write the Bytes to the New File
                                fout.Write(writer, 0, (int)v);
                                //Increase the Counter
                                rdby = rdby + v;
                            }
                        }
                        //Close the FileStream And Send a RECD command to the Client in 
                        //confirmation of File Recipt
                        fout.Close();
                        SendMessage(mySocket, "RECD File Received Properly");
                        serverlogt.Text += "Received File !!\n";
                        return true;

                    }
                    catch (Exception eg)
                    {
                        SendMessage(mySocket, "RECD Error in File Trensfer");
                        MessageBox.Show(this, "Exception occured in Upload:" + eg.ToString());
                    }
                    return true;
                }
                else
                {
                    SendMessage(mySocket, "NOPE Server upload Disabled");
                    return true;
                }

            }
            return false;
        }
        ///<summary>
        ///		This method Builds a custom string containing the file names of all
        ///		the Files the Server allow to download
        ///</summary>
        private string senddirectory()
        {
            string files = "LIST ";
            foreach (File f in _dlfiles)
            {
                files += "@" + f.ToString();
            }
            return files;
        }
    }//class












    ///<summary>
    ///		A Custom Dialog class. Used to set the Server Properites
    ///</summary>
    public class ServerDialog : Form
    {
        private System.ComponentModel.Container components;
        private System.WinForms.ToolTip toolTip1;
        private System.WinForms.Button defaultb;
        public System.WinForms.CheckBox allowup;
        public System.WinForms.CheckBox allowdl;
        public System.WinForms.TextBox filedlt;
        private System.WinForms.Label fdll;
        public System.WinForms.TextBox passt;
        private System.WinForms.Label passl;
        public System.WinForms.TextBox maxt;
        private System.WinForms.Label maxl;
        private System.WinForms.Button cancelb;
        private System.WinForms.Button okb;
        public System.WinForms.TextBox fileupt;
        private System.WinForms.Label fupl;
        public System.WinForms.TextBox portt;
        private System.WinForms.Label portl;
        private System.WinForms.Label lb1;

        ///<summary>
        ///	Constructor
        ///</summary>
        public ServerDialog()
        {
            InitializeComponent();
        }
        ///<summary>
        ///		Free resources 
        ///</summary>
        public override void Dispose()
        {
            base.Dispose();
            components.Dispose();
        }

        ///<summary>
        ///		Initilize WinForm Components
        ///</summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.passt = new System.WinForms.TextBox();
            this.filedlt = new System.WinForms.TextBox();
            this.maxl = new System.WinForms.Label();
            this.allowup = new System.WinForms.CheckBox();
            this.passl = new System.WinForms.Label();
            this.portl = new System.WinForms.Label();
            this.cancelb = new System.WinForms.Button();
            this.toolTip1 = new System.WinForms.ToolTip(components);
            this.portt = new System.WinForms.TextBox();
            this.fileupt = new System.WinForms.TextBox();
            this.maxt = new System.WinForms.TextBox();
            this.fdll = new System.WinForms.Label();
            this.lb1 = new System.WinForms.Label();
            this.defaultb = new System.WinForms.Button();
            this.fupl = new System.WinForms.Label();
            this.allowdl = new System.WinForms.CheckBox();
            this.okb = new System.WinForms.Button();

            //@design this.TrayHeight = 90;
            //@design this.TrayLargeIcon = false;
            //@design this.TrayAutoArrange = true;
            passt.Location = new System.Drawing.Point(192, 104);
            passt.PasswordChar = '*';
            toolTip1.SetToolTip(passt, "Enter the Password For the Clients to Connect to t" +
                "he Server. Not Implimented Yet");
            passt.TabIndex = 3;
            passt.Size = new System.Drawing.Size(128, 20);


            filedlt.Location = new System.Drawing.Point(192, 168);
            filedlt.Text = "c:\\FileShare\\Server\\Download\\";
            toolTip1.SetToolTip(filedlt, "Enter the Directory from which Clients can Download Files.");
            filedlt.TabIndex = 5;
            filedlt.Size = new System.Drawing.Size(176, 20);

            maxl.Location = new System.Drawing.Point(8, 72);
            maxl.Text = "Max No. of Clients";
            maxl.Size = new System.Drawing.Size(168, 16);
            maxl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            maxl.TabIndex = 0;
            maxl.BackColor = System.Drawing.Color.DarkOrange;

            allowup.Checked = true;
            allowup.Location = new System.Drawing.Point(8, 232);
            allowup.Text = "Clients Can Upload Files To Server ?";
            allowup.Size = new System.Drawing.Size(296, 16);
            allowup.CheckState = System.WinForms.CheckState.Checked;
            allowup.FlatStyle = System.WinForms.FlatStyle.Popup;
            allowup.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            toolTip1.SetToolTip(allowup, "When checked Clients will be able to Upload Files to Server.");
            allowup.TabIndex = 7;
            allowup.BackColor = System.Drawing.Color.DarkOrange;

            passl.Location = new System.Drawing.Point(8, 104);
            passl.Text = "Password";
            passl.Size = new System.Drawing.Size(168, 16);
            passl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            passl.TabIndex = 0;
            passl.BackColor = System.Drawing.Color.DarkOrange;

            portl.Location = new System.Drawing.Point(8, 40);
            portl.Text = "Port to Listen";
            portl.Size = new System.Drawing.Size(168, 16);
            portl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            portl.TabIndex = 0;
            portl.BackColor = System.Drawing.Color.DarkOrange;

            cancelb.Location = new System.Drawing.Point(192, 272);
            cancelb.DialogResult = System.WinForms.DialogResult.Cancel;
            cancelb.Size = new System.Drawing.Size(72, 24);
            cancelb.TabIndex = 9;
            cancelb.Text = "Cancel";


            //@design toolTip1.SetLocation(new System.Drawing.Point(7, 7));
            toolTip1.Active = true;

            portt.Location = new System.Drawing.Point(192, 40);
            portt.Text = "4455";
            toolTip1.SetToolTip(portt, "Enter the Port you want the Clients to Connect Default :" +
                " 4455");
            portt.TabIndex = 1;
            portt.Size = new System.Drawing.Size(128, 20);

            fileupt.Location = new System.Drawing.Point(192, 136);
            fileupt.Text = "c:\\FileShare\\Server\\Upload";
            toolTip1.SetToolTip(fileupt, "Enter the Directory where Client Uploaded Files will be Stor" +
                "ed.");
            fileupt.TabIndex = 4;
            fileupt.Size = new System.Drawing.Size(176, 20);


            maxt.Location = new System.Drawing.Point(192, 72);
            maxt.Text = "5";
            toolTip1.SetToolTip(maxt, "Enter the Maximum No. of Clients that can Connect to the " +
                "Server at a Given Time.");
            maxt.TabIndex = 2;
            maxt.Size = new System.Drawing.Size(128, 20);

            fdll.Location = new System.Drawing.Point(8, 168);
            fdll.Text = "File Download Directory";
            fdll.Size = new System.Drawing.Size(168, 16);
            fdll.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            fdll.TabIndex = 0;
            fdll.BackColor = System.Drawing.Color.DarkOrange;


            lb1.Location = new System.Drawing.Point(26, 8);
            lb1.Text = "Set The Server Properties";
            lb1.Size = new System.Drawing.Size(340, 16);
            lb1.ForeColor = System.Drawing.SystemColors.Window;
            lb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Bold);
            lb1.TabIndex = 0;
            lb1.BackColor = System.Drawing.Color.DarkOrange;
            lb1.TextAlign = System.WinForms.HorizontalAlignment.Center;

            defaultb.Location = new System.Drawing.Point(296, 272);
            toolTip1.SetToolTip(defaultb, "Reset the form to default values");
            defaultb.Size = new System.Drawing.Size(72, 24);
            defaultb.TabIndex = 10;
            defaultb.Text = "Defaults";
            defaultb.Click += new System.EventHandler(defaultb_click);

            fupl.Location = new System.Drawing.Point(8, 136);
            fupl.Text = "File Upload Directory";
            fupl.Size = new System.Drawing.Size(168, 16);
            fupl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            fupl.TabIndex = 0;
            fupl.BackColor = System.Drawing.Color.DarkOrange;

            allowdl.Checked = true;
            allowdl.Location = new System.Drawing.Point(8, 200);
            allowdl.Text = "Clients Can Download Files From Server?";
            allowdl.Size = new System.Drawing.Size(296, 16);
            allowdl.CheckState = System.WinForms.CheckState.Checked;
            allowdl.FlatStyle = System.WinForms.FlatStyle.Popup;
            allowdl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
            toolTip1.SetToolTip(allowdl, "When Checked Clients can Download Files from the" +
                " Server.");
            allowdl.TabIndex = 6;
            allowdl.BackColor = System.Drawing.Color.DarkOrange;

            okb.Location = new System.Drawing.Point(88, 272);
            okb.Size = new System.Drawing.Size(72, 24);
            okb.TabIndex = 8;
            okb.Text = "OK";
            okb.DialogResult = DialogResult.OK;
            okb.Click += new System.EventHandler(okb_click);

            this.Text = "Set Server Properties";
            this.MaximizeBox = false;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = cancelb;
            this.BorderStyle = System.WinForms.FormBorderStyle.Fixed3D;
            this.ShowInTaskbar = false;
            this.AcceptButton = okb;
            this.MinimizeBox = false;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(398, 307);
            //this.Closing += new CancelEventHandler(this.ServerDialog_closing);
            this.Controls.Add(defaultb);
            this.Controls.Add(allowup);
            this.Controls.Add(allowdl);
            this.Controls.Add(filedlt);
            this.Controls.Add(fdll);
            this.Controls.Add(passt);
            this.Controls.Add(passl);
            this.Controls.Add(maxt);
            this.Controls.Add(maxl);
            this.Controls.Add(cancelb);
            this.Controls.Add(okb);
            this.Controls.Add(fileupt);
            this.Controls.Add(fupl);
            this.Controls.Add(portt);
            this.Controls.Add(portl);
            this.Controls.Add(lb1);
        }

        ///<summary>
        ///		Method called when the 'Cancel' button is clicked
        ///</summary>
        private void ServerDialog_closing(Object source, CancelEventArgs e)
        {
            e.Cancel = true;
        }
        ///<summary>
        ///		Method called when the 'Ok' button is clicked
        ///</summary>
        private void okb_click(object sender, System.EventArgs e)
        {
            //Validate the Form
            if (portt.Text == "")
            {
                MessageBox.Show(this, "Please Enter a Port to Listen");
                return;
            }
            if (maxt.Text == "")
            {
                maxt.Text = "1";
                return;
            }
            if (allowdl.Checked && filedlt.Text == "")
            {
                MessageBox.Show(this, "Please Enter the Directory from where Clients can Download Files");
                return;
            }
            else
            {
                char last = filedlt.Text[filedlt.Text.Length - 1];
                if ('\\' != last)
                {
                    filedlt.Text += "\\";
                }

            }

            if (allowup.Checked && fileupt.Text == "")
            {
                MessageBox.Show(this, "Please Enter the Directory where Clients can Upload Files");
                return;
            }
            else
            {
                char last = fileupt.Text[fileupt.Text.Length - 1];
                if ('\\' != last)
                {
                    fileupt.Text += "\\";
                }

            }

        }
        ///<summary>
        ///		Method called when Default button is clicked
        ///</summary>
        protected void defaultb_click(object sender, System.EventArgs e)
        {
            portt.Text = "4455";
            maxt.Text = "5";
            passt.Text = "";
            fileupt.Text = "c:\\FileShare\\Server\\Upload\\";
            filedlt.Text = "c:\\FileShare\\Server\\Download\\";
            allowup.Checked = true;
            allowdl.Checked = true;
        }

    }

}
