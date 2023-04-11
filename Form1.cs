using System.Configuration;

namespace RSAChat
{
    public partial class Form1 : Form
    {
        string filePath = ConfigurationManager.AppSettings.Get("rsaDirectory");
        string myUsername = ConfigurationManager.AppSettings.Get("username");
        public Form1()
        {
            InitializeComponent();
            lstUsers.DataSource = GetUsers();
        }

        public void GenerateKeyPair()
        {
            //Use rsa to save the keys into public and private folders
            RSAHelper helper = new RSAHelper();
            byte[] privateKeyBytes = helper.GetKeyPair(true);
            byte[] publicKeyBytes = helper.GetKeyPair(false);
            File.WriteAllBytes(filePath + "/" + myUsername + "/private/privatekey.key", privateKeyBytes);
            File.WriteAllBytes(filePath + "/" + myUsername + "/public/publickey.key", publicKeyBytes);
        }
        public List<string> GetUsers()
        {
            List<string> users = new List<string>();

            foreach(var folder in 
                Directory.GetDirectories(ConfigurationManager.AppSettings.Get("rsaDirectory")))
            {
                users.Add(folder);
            }
            return users;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            GenerateKeyPair();
        }

        private void lstUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Add the try catch to prevent crash when public key doesn't exist
            try
            {
                string userPath = lstUsers.SelectedValue + "/public/publickey.key";
                RSAHelper publickey = new RSAHelper();
                publickey.LoadPublicKey(userPath);
                lblTargetPublicKey.Text = lstUsers.SelectedValue.ToString().Split("\\")[^1];
            } catch
            {
                MessageBox.Show("Key didn't load properly");
            }
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string userPath = lstUsers.SelectedValue + "/public/publickey.key";
            RSAHelper publickey = new RSAHelper();
            //first parameter where we want to save
            //second is the message
            //third is the public key we need to load (your classmates)
            string messagePath = lstUsers.SelectedValue 
                + "/messages/" + DateTime.Now.ToString("yyyyMMddhhmmss")+myUsername;
            publickey.Encrypt(messagePath, txtMessage.Text, userPath);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                List<string> messages;
                RSAHelper rsaHelper = new RSAHelper();
                string privateKeyPath = filePath + "/" + myUsername + "/private/privatekey.key";
                //rsaHelper.LoadPrivateKey(filePath + "/" + myUsername + "/private/privatekey.key");
                messages = rsaHelper.GetAllMessages(privateKeyPath, filePath + "/" + myUsername + "/messages");
                lstMessages.DataSource = null;
                lstMessages.DataSource = messages;
            } catch (Exception ex)
            {
                timer1.Stop();
                btnTimer.Text = "Start";
                MessageBox.Show(ex.Message);
            }

        }

        private void btnTimer_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled == false)
            {
                timer1.Start();
                ((Button)sender).Text = "Stop";
            } else
            {
                timer1.Stop();
                ((Button)sender).Text = "Start";
            }
        }
    }
}