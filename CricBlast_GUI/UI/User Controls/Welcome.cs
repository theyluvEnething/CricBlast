﻿using CricBlast_GUI.Database;
using CricBlast_GUI.UI.Admin_Controls;
using System;
using System.Threading;
using System.Windows.Forms;
using KeyAuth;
using System.Diagnostics;
using System.Net;
using System.Collections.Generic;
using System.Linq;



namespace CricBlast_GUI.UI.User_Controls
{
    public partial class Welcome : UserControl
    {
        public static api KeyAuthApp = new api(
            name: "Tiktok.Games",
            ownerid: "qdPVeuvG2s",
            secret: "44d7d1eff612aa588fedfcff6768e97a8fa1ee541a9486a00e1ed7f418fbc244",
            version: "1.0"
        );

        private void login_Click(object sender, System.EventArgs e)
        {


            KeyAuthApp.init();
            autoUpdate();

            Console.WriteLine(usernameTextBox.Text);
            string key = usernameTextBox.Text;
            Controls.Clear();

            KeyAuthApp.license(key);


            if (!KeyAuthApp.response.success)
            {
                new MessageBoxOk(Selected.ErrorMark, KeyAuthApp.response.message).ShowDialog();
                Console.WriteLine("\nStatus: " + KeyAuthApp.response.message);
                return;
            }


            Console.WriteLine("\nLogged In!"); // at this point, the client has been authenticated. Put the code you want to run after here
            var threadParameters1 = new ThreadStart(() =>
            {
                Invoke((Action)(() => {
                    new MessageBoxOk(Selected.CheckMark, "You have successfully logged in.").ShowDialog();

                    string[] optionFields = KeyAuthApp.getvar("AvailableGames").Split(',');
                    ChooseTeam chooseGameBox = new ChooseTeam(optionFields.ToList());
                    chooseGameBox.ShowDialog();
                }));
            });

            var thread1 = new Thread(threadParameters1);
            thread1.Start();

        }

        private void forgotPassword_Click(object sender, System.EventArgs e)
        {
            switch (_admin)
            {
                case true:
                    usernameTextBox.Text = "admin";
                    passwordTextBox.Text = "admin";
                    return;
                default:
                    new Recover().ShowDialog();
                    usernameTextBox.Text = Selected.UserDetails[2];
                    passwordTextBox.Text = Selected.UserDetails[3];
                    break;
            }
        }

        static void autoUpdate()
        {
            if (KeyAuthApp.response.message == "invalidver")
            {
                if (!string.IsNullOrEmpty(KeyAuthApp.app_data.downloadLink))
                {
                    Console.WriteLine("\n Auto update avaliable!");
                    Console.WriteLine(" Choose how you'd like to auto update:");
                    Console.WriteLine(" [1] Open file in browser");
                    Console.WriteLine(" [2] Download file directly");
                    int choice = int.Parse(Console.ReadLine());
                    switch (choice)
                    {
                        case 1:
                            Process.Start(KeyAuthApp.app_data.downloadLink);
                            Environment.Exit(0);
                            break;
                        case 2:
                            Console.WriteLine(" Downloading file directly..");
                            Console.WriteLine(" New file will be opened shortly..");

                            WebClient webClient = new WebClient();
                            string destFile = Application.ExecutablePath;

                            string rand = random_string();

                            destFile = destFile.Replace(".exe", $"-{rand}.exe");
                            webClient.DownloadFile(KeyAuthApp.app_data.downloadLink, destFile);

                            Process.Start(destFile);
                            Process.Start(new ProcessStartInfo()
                            {
                                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true,
                                FileName = "cmd.exe"
                            });
                            Environment.Exit(0);

                            break;
                        default:
                            Console.WriteLine(" Invalid selection, terminating program..");
                            Thread.Sleep(1500);
                            Environment.Exit(0);
                            break;
                    }
                }
                Console.WriteLine("\n Status: Version of this program does not match the one online. Furthermore, the download link online isn't set. You will need to manually obtain the download link from the developer.");
                Thread.Sleep(2500);
                Environment.Exit(0);
            }
        }

        static string random_string()
        {
            string str = null;

            Random random = new Random();
            for (int i = 0; i < 5; i++)
            {
                str += Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))).ToString();
            }
            return str;
        }






        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        private bool _admin;

        public Welcome()
        {
            InitializeComponent();
        }

        private void Welcome_Load(object sender, System.EventArgs e)
        {
            usernameTextBox.Text = Selected.UserDetails[2];
            passwordTextBox.Text = Selected.UserDetails[3];
        }

        private void refreshPicture_Click(object sender, System.EventArgs e)
        {
            switch (_admin)
            {
                case true:
                    loginAsPicture.Image = Properties.Resources.Unknown_User;
                    _admin = false;
                    logo.Image = Properties.Resources.Logo;
                    usernameLabel.Text = "USERNAME OR EMAIL";
                    usernameTextBox.Text = "";
                    passwordTextBox.Text = "";
                    createAccountLabel.Visible = true;
                    label5.Visible = true;
                    usernameTextBox.Focus();
                    return;
                case false:
                    loginAsPicture.Image = Properties.Resources.Admin_Colored;
                    _admin = true;
                    logo.Image = Properties.Resources.Admin_Logo;
                    usernameLabel.Text = "ADMIN NAME OR EMAIL";
                    usernameTextBox.Text = "";
                    passwordTextBox.Text = "";
                    createAccountLabel.Visible = false;
                    label5.Visible = false;
                    usernameTextBox.Focus();
                    break;
            }
        }

        private void createAccountLabel_Click(object sender, System.EventArgs e)
        {
            Controls.Clear();
            Controls.Add(value: new CreateAccount());
        }

        private void usernameTextBox_TextChanged(object sender, System.EventArgs e)
        {
            usernameRequired.Visible = false;
            if (usernameTextBox.Text.Contains("-"))
                passwordTextBox.Text = usernameTextBox.Text.Split('-')[1];
        }

        private void passwordTextBox_TextChanged(object sender, System.EventArgs e)
        {
            passwordRequired.Visible = false;
        }

        private void usernameTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) login.PerformClick();
        }

        private void usernameTextBox_Load(object sender, EventArgs e)
        {
            usernameTextBox.Focus();
        }

        private void loginAsPicture_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/RpHv4RNmd5");
        }
    }
}