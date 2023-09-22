/* FileShareClient.cs
 
		CopyRight 2000-2001
		This is a sample program made by Saurabh Nandu.
		E-mail: saurabhn@webveda.com
    	Website: learncsharp.cjb.net
		This program uses the 'System.Net.Sockets' class to act as a FileClient 
		which connects to the specified Server. 
		On connecting it can Upload or Dwonload Files To/From the server.
		31/01/2001
		compile: 
csc /r:System.dll;System.Drawing.dll;System.Winforms.dll;System.IO.dll;System.Net.dll;Microsoft.win32.Interop.dll FileShareClient.cs
 */
namespace SaurabhFileShare {
    
    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.WinForms;
    using System.Net.Sockets;
    using System.Net ;
    using System.Threading ;
    using System.IO ;

    /// <summary>
    ///    This is the client Form 
    /// </summary>
    public class FileShareClient : System.WinForms.Form {

    /// <summary> 
    ///    Required by the Win Forms designer 
    /// </summary>
	private System.ComponentModel.Container components;
    private System.WinForms.StatusBar statusBar1;
    private System.WinForms.RichTextBox clientlogt;
    private System.WinForms.Label logl;
    private System.WinForms.ListBox downlistBox;
    private System.WinForms.Label downl;
    private	System.WinForms.ListBox uploadlistBox;
    private System.WinForms.Label uploadl;
    private System.WinForms.ToolBarButton toolBarButton4;
    private System.WinForms.ToolBarButton downloadb;
    private System.WinForms.ToolBarButton toolBarButton3;
    private System.WinForms.ToolBarButton uploadb;
    private System.WinForms.ToolBarButton toolBarButton2;
    private System.WinForms.ToolBarButton dicsob;
    private System.WinForms.ToolBarButton toolBarButton1;
    private System.WinForms.ToolBarButton connectb;
    private System.WinForms.ToolBar toolBar1;
    private System.WinForms.MainMenu mainMenu1;
    private int port=4455 ;
    private Socket clientsocket=null ;
	private string user ;
	private string address ;
	private string updir ;
	private string dldir ;
	private clientdialog ctdl ;
	private Thread clientthread ;
	private bool connected=false ;
	private bool locked=false ;
    private string readwrite ;  
    private File[] upfiles ;
    /// <summary>
    ///		This is the Constructor
    /// </summary>
	public FileShareClient() 
	{
            // Required for Win Form Designer support
            InitializeComponent();
    }

    /// <summary>
    ///    Clean up any resources being used
    /// </summary>
    public override void Dispose() {
		if(clientsocket!=null)
        {
        	SendMessage(clientsocket,"QUIT "+user) ;
			clientlogt.Text+="Disconnected !!" ;
        	clientsocket=null;
        }
        base.Dispose();
        components.Dispose();
   }

        /// <summary>
        ///    The main entry point for the application.
        /// </summary>
        public static void Main(string[] args) {
            Application.Run(new FileShareClient());
        }


        /// <summary>
        ///    Required method for Designer support 
        /// </summary>
    private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.logl = new System.WinForms.Label();
		this.uploadlistBox = new System.WinForms.ListBox();
		this.downloadb = new System.WinForms.ToolBarButton();
		this.downl = new System.WinForms.Label();
		this.uploadb = new System.WinForms.ToolBarButton();
		this.statusBar1 = new System.WinForms.StatusBar();
		this.downlistBox = new System.WinForms.ListBox();
		this.uploadl = new System.WinForms.Label();
		this.clientlogt = new System.WinForms.RichTextBox();
		this.dicsob = new System.WinForms.ToolBarButton();
		this.connectb = new System.WinForms.ToolBarButton();
		this.toolBar1 = new System.WinForms.ToolBar();
		this.toolBarButton4 = new System.WinForms.ToolBarButton();
		this.toolBarButton3 = new System.WinForms.ToolBarButton();
		this.toolBarButton2 = new System.WinForms.ToolBarButton();
		this.mainMenu1 = new System.WinForms.MainMenu();
		this.toolBarButton1 = new System.WinForms.ToolBarButton();
		
		
		MenuItem FileMenu = new MenuItem("File");
	    mainMenu1.MenuItems.Add(FileMenu);
    	FileMenu.MenuItems.Add(new MenuItem("Connect", new EventHandler(clientconnect)));
       	FileMenu.MenuItems.Add(new MenuItem("-"));
        FileMenu.MenuItems.Add(new MenuItem("Disconnect", new EventHandler(clientdisconnect)));
        FileMenu.MenuItems.Add(new MenuItem("-"));
        FileMenu.MenuItems.Add(new MenuItem("Exit", new EventHandler(clientexit)));
        MenuItem TransferMenu = new MenuItem("File Transfer");
        mainMenu1.MenuItems.Add(TransferMenu);
		TransferMenu.MenuItems.Add(new MenuItem("Donwload File", new EventHandler(downfile))) ;
		TransferMenu.MenuItems.Add(new MenuItem("-"));
		TransferMenu.MenuItems.Add(new MenuItem("Upload File", new EventHandler(uploadfile))) ;

		MenuItem ContactMenu = new MenuItem("Contact Me");
        mainMenu1.MenuItems.Add(ContactMenu);
		ContactMenu.MenuItems.Add(new MenuItem("Contact", new EventHandler(contactme))) ;
			

		//@design this.TrayHeight = 90;
		//@design this.TrayLargeIcon = false;
		//@design this.TrayAutoArrange = true;
		logl.Location = new System.Drawing.Point(224, 40);
		logl.Text = "Client Connection Log";
		logl.Size = new System.Drawing.Size(151, 16);
		logl.AutoSize = true;
		logl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		logl.TabIndex = 0;
		logl.BackColor = System.Drawing.Color.DarkOrange;
		
		uploadlistBox.Location = new System.Drawing.Point(8, 64);
		uploadlistBox.Size = new System.Drawing.Size(200, 139);
		uploadlistBox.HorizontalScrollbar = true;
		uploadlistBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold);
		uploadlistBox.TabIndex = 5;
		
		downloadb.Text = "Download File";
		downloadb.ToolTipText = "Click to download selected file from server";
				
		downl.Location = new System.Drawing.Point(8, 216);
		downl.Text = "Files to Download";
		downl.Size = new System.Drawing.Size(122, 16);
		downl.AutoSize = true;
		downl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		downl.TabIndex = 3;
		downl.BackColor = System.Drawing.Color.DarkOrange;
		
		uploadb.Text = "Upload File";
		uploadb.ToolTipText = "Click here to uploadb the selected file to Server";
			
		statusBar1.BackColor = System.Drawing.SystemColors.Control;
		statusBar1.Location = new System.Drawing.Point(0, 393);
		statusBar1.Size = new System.Drawing.Size(492, 20);
		statusBar1.TabIndex = 0;
		statusBar1.Text = "Ready to Connect";
		
		downlistBox.Location = new System.Drawing.Point(8, 240);
		downlistBox.Size = new System.Drawing.Size(200, 139);
		downlistBox.HorizontalScrollbar = true;
		downlistBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold);
		downlistBox.TabIndex = 6;
		
		uploadl.Location = new System.Drawing.Point(8, 40);
		uploadl.Text = "Files to Upload";
		uploadl.Size = new System.Drawing.Size(103, 16);
		uploadl.AutoSize = true;
		uploadl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		uploadl.TabIndex = 0;
		uploadl.BackColor = System.Drawing.Color.DarkOrange;
		
		clientlogt.ReadOnly = true;
		clientlogt.Size = new System.Drawing.Size(256, 320);
		clientlogt.ForeColor = System.Drawing.SystemColors.Window;
		clientlogt.TabIndex = 0;
		clientlogt.AutoSize = true;
		clientlogt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Bold);
		clientlogt.AcceptsTab = true;
		clientlogt.TabStop = false;
		clientlogt.Location = new System.Drawing.Point(224, 64);
		clientlogt.BackColor = System.Drawing.Color.Orange;
		
		dicsob.Text = "Disconnect";
		dicsob.Pushed = true;
		dicsob.ToolTipText = "Click Here to Disconnect from Server";
		
		connectb.Text = "Connect";
		connectb.ToolTipText = "Click here to connect to Server";
		
		toolBar1.Size = new System.Drawing.Size(492, 30);
		toolBar1.BorderStyle = System.WinForms.BorderStyle.Fixed3D;
		toolBar1.Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold);
		toolBar1.DropDownArrows = true;
		toolBar1.TabIndex = 1;
		toolBar1.TabStop = true;
		toolBar1.ShowToolTips = true;
		toolBar1.TextAlign = System.WinForms.ToolBarTextAlign.Right;
		toolBar1.Buttons.All = new System.WinForms.ToolBarButton[] {connectb,
			toolBarButton1,
			dicsob,
			toolBarButton2,
				uploadb,
			toolBarButton3,
			downloadb,
			toolBarButton4};
		toolBar1.ButtonClick+=new ToolBarButtonClickEventHandler(tools) ;
		toolBarButton4.Style = System.WinForms.ToolBarButtonStyle.Separator;
		toolBarButton3.Style = System.WinForms.ToolBarButtonStyle.Separator;
		toolBarButton2.Style = System.WinForms.ToolBarButtonStyle.Separator;
		//@design mainMenu1.SetLocation(new System.Drawing.Point(7, 7));
		toolBarButton1.Style = System.WinForms.ToolBarButtonStyle.Separator;
			
		this.Text = "FileShare Client , by Saurabh Nandu http://learncsharp.cjb.n" + 
			"et";
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		this.Menu = mainMenu1;
		this.BackColor = System.Drawing.SystemColors.Desktop;
		this.ClientSize = new System.Drawing.Size(492, 413);
		
		this.Controls.Add(statusBar1);
		this.Controls.Add(clientlogt);
		this.Controls.Add(logl);
		this.Controls.Add(downlistBox);
		this.Controls.Add(downl);
		this.Controls.Add(uploadlistBox);
		this.Controls.Add(uploadl);
		this.Controls.Add(toolBar1);
	}
		/// <summary>
		///		This method gets called when the user clicks on the connect Menu
		///		This method shows a Custom Dialog which contains all the 
		///		client settings. 
		///		It then gets the value from the dialog and starts connecting to the 
		///		Speficied Server .
		/// </summary>
		
		private void clientconnect(object sender, EventArgs e)
		{
			//Make a instance of our custom Dialog
			ctdl = new clientdialog() ;
			//Get the result pressed by the user on the Dialog
			DialogResult ret = ctdl.ShowDialog(this) ;
			//If the user pressed 'Ok' then assign the values to
			//our variables.
			if(ret == DialogResult.OK) {
   				this.port=int.Parse(ctdl.portt.Text) ;
   				this.address=ctdl.servert.Text ;
   				this.updir = ctdl.fileupt.Text ;
   				this.dldir=ctdl.filedlt.Text ;
   				this.user=ctdl.usert.Text ;
				if(!Directory.DirectoryExists(dldir))
				{
					MessageBox.Show(this,"The directory to Download Files Does not Exist \n Try Connecting Again");
				}
				else if(!Directory.DirectoryExists(updir))
				{
					MessageBox.Show(this,"The directory to Upload Files Does not Exist \n Try Connecting Again");
				}
				else
				{
					//clear the log screen
					clientlogt.Text="";
					//set the buttons
   					dicsob.Pushed=false ;
					connectb.Pushed=true ;
					statusBar1.Text="Connecting to Server... ";
					//to get the Upload and Download directories
					//call the Method which gets the files and updates
					//them in our ListBoxes
	   				GetDirs();
					//Call the method which will connect to the 
					//server.
   					InitilizeConn();
				}
   			}
		}
		/// <summary>
		///		This method is called when the client clicks the 
		///		Disconnect menuitem. It closes the connection to the server
		///		and resets the buttons and listboxes. 
		/// </summary>
		private void clientdisconnect(object sender, EventArgs e)
		{
			//If a socket connection Exisis then close it
			if(clientsocket!=null)
			{
				SendMessage(clientsocket, "QUIT "+user) ;
				clientsocket.Close();
			}
			//Abort the client thread
			if(clientthread!=null&&clientthread.IsAlive)
			{
				clientthread.Abort();
			
			}
			//reset the Buttons
			statusBar1.Text="Disconnected!!!";
			clientlogt.Text+="Disconnected !!" ;
			downlistBox.Items.Clear();
			dicsob.Pushed=true;
			downloadb.Pushed=false ;
			uploadb.Pushed=false ;
			connectb.Pushed=false ;
		}
		/// <summary>
		///		This method is called when the user clicks the Exit Menuitem.
		///		It calls the Dispose() method to exit the program
		/// </summary>
		private void clientexit(object sender, EventArgs e)
		{
			Dispose(); 
		}

		/// <summary>
		///		This is the method called when the user clicks the 
		///		Download Menuitem.
		///		It has the code to Download the selected file from the server 
		/// </summary>
		
		private void downfile(object sender, EventArgs e)
		{
			//check for connection and if some other download or
			//upload process is already going on.
			if(connected && !locked)
			{
				//Get the File selected for download
				if(downlistBox.SelectedItem==null)
				{
					MessageBox.Show(this,"Please Select a file to download first and then click download") ;
				}
				else
				{
					//Get the File Selected for doenload from the ListBox
					string selefile =(string)downlistBox.SelectedItem ;
					//Send the Server "DOWN" command with the FileName to download
					//from the server
					SendMessage(clientsocket ,"DOWN "+selefile) ;
					//set a global variable with the Filename
					readwrite=selefile ;
					clientlogt.Text+="Sending Download Request to server for file "+selefile+"\n" ;
					clientthread=null ;
					downloadb.Pushed=true ;
					uploadb.Pushed=true ;
					//Set the buttons and start the Downloading method "DownloadFile" in a 
					//Thread
					Thread downthread = new Thread(new ThreadStart(DownloadFile)) ;
					downthread.Start() ;
				}
			}
		}

		/// <summary>
		///		This method gets called when the user clicks on the Upload MenuItem
		///		It takes care of all the Upload procedure
		/// </summary>
		
		private void uploadfile(object sender, EventArgs e)
		{
			//Check if connection is present and if any other upload
			//or download process is going on.
			if(connected && !locked)
			{
				//Get the File Selected for upload from the ListBox
				if(uploadlistBox.SelectedItem==null)
				{
					MessageBox.Show(this,"Please Select a file to download first and then click Upload") ;
				}
				else
				{
					try{

						string selefile =uploadlistBox.SelectedItem.ToString() ;
						File ftemp = new File(selefile) ;
						//Construct a string to send to the server with the Command "UPFL"
						//With the Command the client also sends the FileName and the 
						//File Length
						string ttt = "UPFL "+ftemp.FullName+"@"+ftemp.Length.ToString() ;
						//Send the message
						SendMessage(clientsocket ,ttt) ;
						//Set a global variable for the Selected file
					  	readwrite=selefile ;
						//Do some buttons and Log settings
						clientlogt.Text+="Sending Upload Request to server for file "+selefile+"\n" ;
						downloadb.Pushed=true ;
						uploadb.Pushed=true ;
						//Start a thread on the Method "UploadFile" 
						Thread upthread = new Thread(new ThreadStart(UploadFile)) ;
						upthread.Start() ;
					}
					catch(Exception eg)
					{
						MessageBox.Show(this,"Exception occured in upload click "+eg.ToString());
						downloadb.Pushed=false ;
						uploadb.Pushed=false ;
					}
				}
			}
		}
		/// <summary>
		///		Method called when the Help MenuItem is clicked
		/// </summary>
		private void contactme(object sender, EventArgs e)
		{
			MessageBox.Show(this,"This is the FileShare Client made by Saurabh Nandu,\n E-mail: saurabhn@webveda.com \n Website:http//learncsharp.cjb.net") ; 
		}

		/// <summary>
		///		This method is the event handler for the the ToolBar buttons.
		///		The code here is the same repeat of the Above "Connect", "Disconnect"
		///		,"Download" and "Upload" MenuItem Methods 
		/// </summary>
		
		private void tools(object sender,  ToolBarButtonClickEventArgs e)
		{
			if(e.button==connectb)
			{
				
				ctdl = new clientdialog() ;
				DialogResult ret = ctdl.ShowDialog(this) ;
				if(ret == DialogResult.OK) {
   					this.port=int.Parse(ctdl.portt.Text) ;
   					this.address=ctdl.servert.Text ;
   					this.updir = ctdl.fileupt.Text ;
   					this.dldir=ctdl.filedlt.Text ;
   					this.user=ctdl.usert.Text ;
					if(!Directory.DirectoryExists(dldir))
					{
						MessageBox.Show(this,"The directory to Download Files Does not Exist \n Try Connecting Again");
					}
					else if(!Directory.DirectoryExists(updir))
					{
						MessageBox.Show(this,"The directory to Upload Files Does not Exist \n Try Connecting Again");
					}
					else
					{
						clientlogt.Text="";
   						dicsob.Pushed=false ;
						connectb.Pushed=true ;
						statusBar1.Text="Connecting To Server ...";
   						GetDirs();
   						InitilizeConn();
					}
   				}
			}
			else if(e.button==dicsob)
			{
				if(clientsocket!=null)
				{
					SendMessage(clientsocket, "QUIT "+user) ;
					clientsocket.Close();
				}
				if(clientthread!=null&&clientthread.IsAlive)
				{
					clientthread.Abort();
			
				}
				statusBar1.Text="Disconnected!!";
				clientlogt.Text+="Disconnected !!";
				downlistBox.Items.Clear();
				connectb.Pushed=false ;
				downloadb.Pushed=false ;
				uploadb.Pushed=false ;
				dicsob.Pushed=true;
			}
			else if(e.button==uploadb)
			{
				if(connected && !locked)
				{
					if(uploadlistBox.SelectedItem==null)
					{
						MessageBox.Show(this,"Please Select a file to download first and then click Upload") ;
					}
					else
					{
						string selefile =uploadlistBox.SelectedItem.ToString() ;
						File ftemp = new File(selefile) ;
						SendMessage(clientsocket ,"UPFL "+ftemp.FullName+"@"+ftemp.Length) ;
					  	readwrite=selefile ;
						clientlogt.Text+="Sending Upload Request to server for file "+selefile+"\n" ;
						downloadb.Pushed=true ;
						uploadb.Pushed=true ;
						Thread upthread = new Thread(new ThreadStart(UploadFile)) ;
						upthread.Start() ;
					}
				}
			}
			else if(e.button==downloadb)
			{
				if(connected && !locked)
				{
					if(downlistBox.SelectedItem==null)
					{
						MessageBox.Show(this,"Please Select a file to download first and then click download") ;
					}
					else
					{
						string selefile =(string)downlistBox.SelectedItem ;
						SendMessage(clientsocket ,"DOWN "+selefile) ;
					  	readwrite=selefile ;
						clientlogt.Text+="Sending Download Request to server for file "+selefile+"\n" ;
						downloadb.Pushed=true ;
						uploadb.Pushed=true ;
						Thread downthread = new Thread(new ThreadStart(DownloadFile)) ;
						downthread.Start() ;
					}
				}
			}
		}
		
		/// <summary>
		///		This method Will connect to the Server using the settings
		///		specified by the client properties dialog.
		/// </summary>
		public void InitilizeConn()
		{
			try
			{
				//Initilize a socket of the Typr TCP
				clientsocket = new Socket(AddressFamily.AfINet,SocketType.SockStream,ProtocolType.ProtTCP);
				clientlogt.Text+="Trying to connect to Server ......\r" ;
				//Resolve the DNS of the server
				IPAddress host_addr = DNS.Resolve(address);
				//Create a IPEndPoint to the Server
				IPEndPoint ep = new IPEndPoint(host_addr, port);
				//Check if the client could connect to the Server
				//If the client is connected Zero is returned
				if(clientsocket.Connect(ep)==0)
				{
					//Start a Thread on the Method "doTalk" whic will handle
					//futher talking with the server
					statusBar1.Text="Connected !!";
					clientlogt.Text+="Connected to Server: "+ep.ToString()+" \r" ;
					clientthread =  new Thread(new ThreadStart(doTalk)) ;
					clientthread.Start() ;
				}
				else
				{
					//Else there was a error Connecting
					dicsob.Pushed=true;
					downloadb.Pushed=false ;
					uploadb.Pushed=false ;
					connectb.Pushed=false ;
					statusBar1.Text="Error" ;
					clientlogt.Text="Error: Cannot Connect to Server !!";
				}
			}
			catch(Exception ed)
			{
				dicsob.Pushed=true;
				downloadb.Pushed=false ;
				uploadb.Pushed=false ;
				connectb.Pushed=false ;
				statusBar1.Text="Error" ;
				Console.WriteLine("Exception occured in Connecting to Server :"+ed.ToString()) ;
			}
		}
			/// <summary>
			///		A Method which will resolve the Files in the Upload and 
			///		download directory and add it to the ListBox
			/// </summary>
			private void GetDirs()
			{
				//check the dirctory
				if(Directory.DirectoryExists(updir)) {
					//Make a global variable containing all the files in the Upload Directory
				 	upfiles = Directory.GetFilesInDirectory(updir) ;
			 		//add the files to the ListBox
					foreach (File f in upfiles)
					{
						uploadlistBox.InsertItem(0,f);
					}
				}	
			}

		/// <summary>
		///		This Method runs in a Thread called from the InitilizeConn method.
		///		This method does the Sending and receiving of normal commands with the server
		/// </summary>
		private void doTalk()
		{
			//Bool variable to indicate if initial talk between the Client
			//and server is going on
			bool StayConnected=true ;
			while(StayConnected)
			{
				string ServerCommand ;
				//Declare a Byte receive buffer
				byte[] recs = new byte[2048];
				try{
					//Receive Bytes from the Server				
					int scount = clientsocket.Receive(recs , recs.Length,0) ;
					//Decode the Server Message to String
					string servermessage = System.Text.Encoding.ASCII.GetString(recs) ;
					//check if the server sent more than one Byte
					if(scount >0)
					{
						//Call a method ParseMessage with the Server Message as a Parameter
						// This Method will extract the Server Comand from the Server Message
						ServerCommand = ParseMessage(servermessage) ;
					}
					else
					{
						//If no message is received then assume a custom Command
						ServerCommand="NOOP" ;
					}
					//Call a method ParseComand with parameters as the Server Command and the Server Message
					//This method does action depending on the server command
					//It returns a Bool value depending on the server Command
					StayConnected = ParseCommand(ServerCommand , servermessage) ;
				}
				catch(Exception ec)
				{
					statusBar1.Text="Error!!" ;
					StayConnected = false ;	
					MessageBox.Show(this,"Exception Occured while Talking to Server :"+ec.ToString()) ;
				}
			}
		}
		/// <summary>
		///		This Method is Used to send Messages to the Server using the 
		///		Socket connection
		/// </summary>
		private void SendMessage(Socket sendsock , string message)
		{
			//Fill up a Byte array with the Command to send to the server encoded
			//in ASCII format
			byte[] sender = System.Text.Encoding.ASCII.GetBytes(message.ToCharArray()) ;
			//Send the Message
			sendsock.Send(sender, sender.Length,0) ;
		}
		/// <summary>
		///		This method takes Server Message as a Parameter and 
		///		send back the server command from the message
		/// </summary>
		private string ParseMessage(string clientlogt)
		{
			string ServerCommand ;
			if(clientlogt=="")
			{
				return "NOOP" ;
			}
			//The Command Length is 4 so we substring the first 4 
			//Bytes from the Message
			ServerCommand = clientlogt.Substring(0,4) ;
			return ServerCommand ;
		}
		/// <summary>
		///		This message Get the Server Command and Server Message as Parameters
		///		It Then Takes Care of all Executing to be done depending on the Server Command 
		/// </summary>
		private bool ParseCommand(string ServerCommand , string servermessage ) 
		{
			if(ServerCommand=="CONN")
			{
				//This is the First Message Sent by the server upon
				//receiving the Client Connection request
				string temp = servermessage.Substring(4) ;
				string mes= temp.Trim() ;
				clientlogt.Text+="Server :"+mes+" \r";
				//In response to this Command we Reply with the "USER" Command
				//along with it We send our Username to the Server
				SendMessage(clientsocket,"USER "+user) ;
				connected=true ;
				return true ;
			}
			if(ServerCommand=="NOOP") 
			{
				//A command to do Nothing
				Thread.Sleep(100);
				return true ;
			}
			if(ServerCommand=="LIST")
			{
				//This Command is sent by the server along wiht a list of all the files it is
				//offering for download
				//Individual FileNames are Seperated by a "@" sign
				string temp = servermessage.Substring(6) ;
				string mes= temp.Trim() ;
				string[] temp2= mes.Split(new char[]{'@'}) ;
				//Breakup the Server string and Fillup the ListBox with the
				//FileName only of the Files available for download
				foreach(string s in temp2)
				{
					int i = s.LastIndexOf("\\") ;
					downlistBox.InsertItem(0,s.Substring(i+1)) ;
				}				
				return false ;
			}
			if(ServerCommand=="RECD") 
			{
				//This command is sent by the Server to Indicate that it has
				//Received the Uploaded file
				return true ;
			}
			//If some other command is sent the return False
			return false ;
		}
		/// <summary>
		///		This method takes care of download the file from the Server
		/// </summary>
		private void DownloadFile()
		{
			//Declare some variable that will be used later
			bool done =false ;
			bool check= false ;
			locked=true ;
			long size=0 ;
			long rby=0 ;
			while(!done)
			{
				//declare a buffer
				byte[] rce = new byte[2048] ;
				//Recive a Sever Message
				int i = clientsocket.Receive(rce,rce.Length,0) ;
				//Convert it to string
				string servermessage = System.Text.Encoding.ASCII.GetString(rce);
				if(i>0)
				{
					//Parse the Message to get the Server Command
					string command = ParseMessage(servermessage);
					//Check if the command is SIZE 
					//this command is sent along with the Size of the File 
					//Which the client Requested for download
					if(command=="SIZE")
					{
						string temp = servermessage.Substring(4) ;
						string mes= temp.Trim() ;
						//store the File Size 
						size=Int64.Parse(mes) ;
						clientlogt.Text+="Receiving File of "+size+" bytes \n";
						//Send a "SEND" command to the server which will
						//make the server send the File
						SendMessage(clientsocket , "SEND "+readwrite );
						done=true ;
						check=true ;
					}
					if(command=="NOPE")
					{
						//a "NOPE" command is returned when the File asked if not availabe or
						//if server has restricted downloads
						string temp = servermessage.Substring(4) ;
						string mes= temp.Trim() ;
						clientlogt.Text+="File Receive Error "+mes+" \n" ;
						downloadb.Pushed=false ;
						uploadb.Pushed=false ;
						done=true ;
						//We send a "OHHH" command in response
						SendMessage(clientsocket ,"OHHH No Problem") ;
						check=false ;
					}					
				}
			}
			//The File Size has been received the continue 
			done=false ;
			//Make a File with the same name as the File that is being downloaded
			//Also open a Network Stream to the Server
			FileStream fout = new FileStream(dldir+readwrite, FileMode.OpenOrCreate , FileAccess.Write) ;			
			NetworkStream nfs = new NetworkStream(clientsocket) ;
			byte[] buffer = new byte[4096] ;
			statusBar1.Text="Downloading File from Server..." ;
			while(!done&&check)
			{
				try{
					long v=0 ;
					//loop till the Full bytes have been read
					while(rby<size)
					{
						//Read from the Network Stream
						int i = nfs.Read(buffer,0,buffer.Length) ;
						if(i>0)
						{
							//Some checking done to detremine the number of Bytes to be written
							if(i>=4096&&(size-rby)>=4096)
							{
								v=4096 ;
							}
							else if(i<4096 &&(size-rby)>=4096)
							{
								v= i;
							}
							else
							{
								v=(size-rby) ;
							}
							//Write the Bytes received to the File
							fout.Write(buffer,0,(int)v) ;
							rby=rby+v ;
							
						}
					}
						
					clientlogt.Text+="File Received sucessfully "+rby+"bytes \n";
					//Send a "RECD" Command to the Server in response
					statusBar1.Text="File Downloaded" ;
					SendMessage(clientsocket,"RECD File!!") ;
					downloadb.Pushed=false ;
					uploadb.Pushed=false ;
					fout.Close() ;
					done=true ;
					locked=false ;
				}
				catch(Exception ed)
				{
					statusBar1.Text="Error!!" ;
					MessageBox.Show(this,"A Exception occured in file transfer"+ed.ToString());
				}
			}
		}
			/// <summary>
			///		This method taks care of Uploading the Selected File to the Server
			/// </summary>
			private void UploadFile()
			{	
				//Set some Variables
				bool check =true ;
				locked=true ;
				bool done =false ;
				int  i=0 ;
				long rdby=0 ;
				File ftemp = new File(readwrite) ;
				long total = ftemp.Length ;
				ftemp=null ;
				byte[] rce = new byte[2048] ;
				string servermessage ;
				while(!done)
				{
					//Receive Bytes from the Server
					i = clientsocket.Receive(rce,rce.Length,0) ;
					servermessage = System.Text.Encoding.ASCII.GetString(rce);
					if(i>0)
					{
						//Parse the Server Message to get the Command
						string command = ParseMessage(servermessage);
						if(command=="SEND")
						{
							//Server Sends a "SEND" command if it allows the client to upload files
							string temp = servermessage.Substring(4) ;
							string mes= temp.Trim() ;
							clientlogt.Text+="Server Ready to Accept File \n";
							done=true ;
							check=true ;
						}
						if(command=="NOPE")
						{
							//A "NOPE" Command is sent if the Server does not allow Uploading 
							string temp = servermessage.Substring(4) ;
							string mes= temp.Trim() ;
							clientlogt.Text+="File Send Error "+mes+" \n" ;
							downloadb.Pushed=false ;
							uploadb.Pushed=false ;
							done=true ;
							//Send a Response to the Server
							SendMessage(clientsocket ,"OHHH No Problem") ;
							check=false ;
						}
					}								
				}
				//If the Server has Requested to Send the file then open up the 
				//FileStreams to the file to be Uploaded
				statusBar1.Text="Uploading File to Server" ;
				i=0;
				FileStream fin = new FileStream(readwrite , FileMode.Open , FileAccess.Read) ;
				byte[] reader = new byte[4096] ;
				//Loop till the File is totaly read
				while(rdby<total&&check)
				{
					//Read from the File
					i = fin.Read(reader,0,reader.Length) ;
					//Send the Bytes to the Server
					clientsocket.Send(reader,i,0) ;
					rdby=rdby+i ;
				}
				fin.Close();
				i=0;
				done=false ;
				while(!done){
					//After finishing Sending the File Wait for a Server Command
					i = clientsocket.Receive(rce,rce.Length,0) ;
					servermessage = System.Text.Encoding.ASCII.GetString(rce);
					if(i>0)
					{
						string command = ParseMessage(servermessage);
						if(command=="RECD")
						{
							//Server Sends a "RECD" Command if the file was receive sucessfully
							statusBar1.Text="File Uploaded" ;
							string temp = servermessage.Substring(4) ;
							string mes= temp.Trim() ;
							Console.WriteLine("Server Received File :"+mes) ;
							downloadb.Pushed=false ;
							uploadb.Pushed=false ;
							clientlogt.Text+="Server Received File \n";
							locked=false ;
							done=true ;
						}
						
					}
					
				}
				
			}

    }//client class
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
   /// <summary>
   ///		This is a Class which will be our Client Dialog Class
   /// </summary>
    public class clientdialog : System.WinForms.Form {

        /// <summary> 
        ///    Required by the Win Forms designer 
        /// </summary>
	private System.ComponentModel.Container components;
    private System.WinForms.ToolTip toolTip1;
    private System.WinForms.Button defaultb;
	public System.WinForms.TextBox filedlt;
    private System.WinForms.Label fdll;
    public System.WinForms.TextBox usert;
    private System.WinForms.Label userl;
    public System.WinForms.TextBox servert;
    private System.WinForms.Label serverl;
    private System.WinForms.Button cancelb;
    private System.WinForms.Button okb;
    public System.WinForms.TextBox fileupt;
    private System.WinForms.Label fupl;
    public System.WinForms.TextBox portt;
    private System.WinForms.Label portl;
    private System.WinForms.Label lb1;
    
    /// <summary>
    ///		Constructor of the Class
    /// </summary>
    
		public clientdialog() {
			// Required for Win Form Designer support
			InitializeComponent();
        }

        /// <summary>
        ///    Clean up any resources being used
        /// </summary>
        public override void Dispose() {
            base.Dispose();
            components.Dispose();
        }

        /// <summary>
        ///    Required method for Designer support - do not modify
        ///    the contents of this method with an editor
        /// </summary>
        private void InitializeComponent()
		{
		this.components = new System.ComponentModel.Container();
		this.servert = new System.WinForms.TextBox();
		this.fupl = new System.WinForms.Label();
		this.filedlt = new System.WinForms.TextBox();
		this.toolTip1 = new System.WinForms.ToolTip(components);
		this.portl = new System.WinForms.Label();
		this.serverl = new System.WinForms.Label();
		this.cancelb = new System.WinForms.Button();
		this.fileupt = new System.WinForms.TextBox();
		this.userl = new System.WinForms.Label();
		this.lb1 = new System.WinForms.Label();
		this.fdll = new System.WinForms.Label();
		this.okb = new System.WinForms.Button();
		this.portt = new System.WinForms.TextBox();
		this.usert = new System.WinForms.TextBox();
		this.defaultb = new System.WinForms.Button();
		
		//@design this.TrayHeight = 90;
		//@design this.TrayLargeIcon = false;
		//@design this.TrayAutoArrange = true;
		servert.Location = new System.Drawing.Point(192, 72);
		servert.Text = "localhost";
		toolTip1.SetToolTip(servert, "Enter the addess of the Server");
		servert.TabIndex = 2;
		servert.Size = new System.Drawing.Size(128, 20);
		
		fupl.Location = new System.Drawing.Point(8, 136);
		fupl.Text = "File Upload Directory";
		fupl.Size = new System.Drawing.Size(168, 16);
		fupl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		fupl.TabIndex = 0;
		fupl.BackColor = System.Drawing.Color.DarkOrange;
		
		filedlt.Location = new System.Drawing.Point(192, 168);
		filedlt.Text = "c:\\FileShare\\Client\\Download\\";
		toolTip1.SetToolTip(filedlt, "Enter the directory where the downloaded files will be store" + 
			"d");
		filedlt.TabIndex = 5;
		filedlt.Size = new System.Drawing.Size(176, 20);
		
		//@design toolTip1.SetLocation(new System.Drawing.Point(7, 7));
		toolTip1.Active = true;
		
		portl.Location = new System.Drawing.Point(8, 40);
		portl.Text = "Port to Connect ";
		portl.Size = new System.Drawing.Size(168, 16);
		portl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		portl.TabIndex = 0;
		portl.BackColor = System.Drawing.Color.DarkOrange;
		
		serverl.Location = new System.Drawing.Point(8, 72);
		serverl.Text = "Address of Server";
		serverl.Size = new System.Drawing.Size(168, 16);
		serverl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		serverl.TabIndex = 0;
		serverl.BackColor = System.Drawing.Color.DarkOrange;
		
		cancelb.Location = new System.Drawing.Point(192, 208);
		cancelb.DialogResult = System.WinForms.DialogResult.Cancel;
		cancelb.Size = new System.Drawing.Size(72, 24);
		cancelb.TabIndex = 7;
		cancelb.Text = "Cancel";
		
		fileupt.Location = new System.Drawing.Point(192, 136);
		fileupt.Text = "c:\\FileShare\\Client\\Upload";
		toolTip1.SetToolTip(fileupt, "Enter the Directory from where you can Upload Files.");
		fileupt.TabIndex = 4;
		fileupt.Size = new System.Drawing.Size(176, 20);
		
		userl.Location = new System.Drawing.Point(8, 104);
		userl.Text = "Username";
		userl.Size = new System.Drawing.Size(168, 16);
		userl.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		userl.TabIndex = 0;
		userl.BackColor = System.Drawing.Color.DarkOrange;
		
		lb1.Location = new System.Drawing.Point(26, 8);
		lb1.Text = "Set The Client Properties";
		lb1.Size = new System.Drawing.Size(340, 16);
		lb1.ForeColor = System.Drawing.SystemColors.Window;
		lb1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11f, System.Drawing.FontStyle.Bold);
		lb1.TabIndex = 0;
		lb1.BackColor = System.Drawing.Color.DarkOrange;
		lb1.TextAlign = System.WinForms.HorizontalAlignment.Center;
		
		fdll.Location = new System.Drawing.Point(8, 168);
		fdll.Text = "File Download Directory";
		fdll.Size = new System.Drawing.Size(168, 16);
		fdll.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
		fdll.TabIndex = 0;
		fdll.BackColor = System.Drawing.Color.DarkOrange;
		
		okb.Location = new System.Drawing.Point(88, 208);
		okb.Size = new System.Drawing.Size(72, 24);
		okb.TabIndex = 6;
		okb.Text = "OK";
		okb.DialogResult = DialogResult.OK;
		okb.Click += new System.EventHandler(okb_click);
		
		portt.Location = new System.Drawing.Point(192, 40);
		portt.Text = "4455";
		toolTip1.SetToolTip(portt, "Enter the Port of the server you want to Connect. Default : " + 
			"4455");
		portt.TabIndex = 1;
		portt.Size = new System.Drawing.Size(128, 20);
		
		usert.Location = new System.Drawing.Point(192, 104);
		toolTip1.SetToolTip(usert, "Enter you username");
		usert.TabIndex = 3;
		usert.Size = new System.Drawing.Size(128, 20);
		usert.Text="Guest" ;
		
		defaultb.Location = new System.Drawing.Point(296, 208);
		toolTip1.SetToolTip(defaultb, "Reset the form to default values");
		defaultb.Size = new System.Drawing.Size(72, 24);
		defaultb.TabIndex = 8;
		defaultb.Text = "Defaults";
		defaultb.Click += new System.EventHandler(defaultb_click);
		this.Text = "Set Client Properties";
		this.MaximizeBox = false;
		this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
		this.CancelButton = cancelb;
		this.BorderStyle = System.WinForms.FormBorderStyle.Fixed3D;
		this.ShowInTaskbar = false;
		this.AcceptButton = okb;
		this.MinimizeBox = false;
		this.BackColor = System.Drawing.SystemColors.Desktop;
		this.ClientSize = new System.Drawing.Size(398, 243);
		
		this.Controls.Add(defaultb);
		this.Controls.Add(filedlt);
		this.Controls.Add(fdll);
		this.Controls.Add(usert);
		this.Controls.Add(userl);
		this.Controls.Add(servert);
		this.Controls.Add(serverl);
		this.Controls.Add(cancelb);
		this.Controls.Add(okb);
		this.Controls.Add(fileupt);
		this.Controls.Add(fupl);
		this.Controls.Add(portt);
		this.Controls.Add(portl);
		this.Controls.Add(lb1);
	}
	/// <summary>
	///		Method Called when the Default Button is Clicked
	///		It sets the Client Properties to its Default's
	/// </summary>
	protected void defaultb_click(object sender, System.EventArgs e)
	{
		portt.Text="4455" ;
		servert.Text="localhost" ;
		usert.Text="Guest" ;
		fileupt.Text="c:\\FileShare\\Client\\Upload\\" ;
		filedlt.Text="c:\\FileShare\\Client\\Download\\" ;
	}
   /// <summary>
   ///	Method Called when the Cancel Button is clicked
   /// </summary>
   
   private void clientdialog_closing(Object source, CancelEventArgs e)
   {
		e.Cancel = true;
	}
	/// <summary>
	///		Method Called When OK button is Clicked
	/// </summary>
	protected void okb_click(object sender, System.EventArgs e)
	{
			//Do Some Checking and Validation
			if(portt.Text=="")
			{
				MessageBox.Show(this,"Please Enter a Port to Listen") ;
				return ;
			}
			if(servert.Text=="")
			{
				servert.Text="localhost" ;
				return ;
			}
			if(usert.Text=="")
			{
				servert.Text="guest" ;
				return ;
			}

			if(filedlt.Text=="")
			{
				MessageBox.Show(this,"Please Enter the directory from where clients can download files") ;
				return ;
			}
			else
			{
				char last = filedlt.Text[filedlt.Text.Length-1];
				if('\\'!=last)
				{
				filedlt.Text+="\\" ;
				}
			}
			if(!Directory.DirectoryExists(filedlt.Text))
			{
				MessageBox.Show(this,"The directory to Download Files Does not Exist");
				return;
			}
			if(fileupt.Text=="")
			{
				MessageBox.Show(this,"Please Enter the directory where clients can uploadb files") ;
				return ;
			}
			else
			{
				char last = fileupt.Text[fileupt.Text.Length-1];
				if('\\'!=last)
				{
					fileupt.Text+="\\" ;
				}
								
			}
		if(!Directory.DirectoryExists(fileupt.Text))
		{
			MessageBox.Show(this,"The directory to Upload Files Does not Exist");
			return;
		}
    }

}
}