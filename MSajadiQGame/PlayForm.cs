/*Author: mahjabin Sajadi
 *Date: 23/11/2021
 *Purpose: QGAME
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using MSajadiQGame.Properties;
using System.Threading;


namespace MSajadiQGame
{
    public partial class PlayForm : Form
    {
        /// <summary>
        /// defenition global variables
        /// </summary>

        private const int START_TOP = 150;
        private const int START_LEFT = 250;
        private const int WIDTH = 50;
        private const int HEIGHT = 50;
        private const int GAP = 2;

    
        Materials currentMaterial = new Materials();
        int rows;
        int columns;
        int moves = 0;
        bool correctDoor = false;
        bool isBox = false;
        int rCounter = 0;  // count remnent box

        public enum materialColor
        {
            blank,     //emplty material
            black,      //wall color
            red,         // door or box color
            green         // door or box color 
        }
        public enum materialType
        {
            blank,
            wall,
            door,
            box
        }
        /***************************************/
        public PlayForm()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
        /// <summary>
        /// Form Load
        /// </summary>

        private void PlayForm_Load(object sender, EventArgs e)
        {
            txtMove.Enabled = false;
            txtRemainingBox.Enabled = false;
            btnDown.Enabled = false;
            btnLeft.Enabled = false;
            btnRight.Enabled = false;
            btnUp.Enabled = false;

            this.KeyPreview = true;
        }
        /**********************************************/
        /// <summary>
        /// checks for direction Materials
        /// </summary>
  
        public bool Intersection(string direction)
        {
            foreach (Materials tile in this.Controls.OfType<Materials>().ToArray())
            {
                switch (direction)
                {
                    case "up":
                        if (currentMaterial.Left == tile.Left && currentMaterial.Top == (tile.Bottom + GAP) && !tile.materialselected)
                        {
                            if (tile.materialType == materialType.door.ToString() && currentMaterial.materialColor == tile.materialColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                    case "down":
                        if (currentMaterial.Left == tile.Left && currentMaterial.Bottom == (tile.Top - GAP) && !tile.materialselected)
                        {
                            if (tile.materialType == materialType.door.ToString() && currentMaterial.materialColor == tile.materialColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                    case "left":
                        if (currentMaterial.Left == (tile.Right + GAP) && currentMaterial.Top == tile.Top && !tile.materialselected)
                        {
                            if (tile.materialType == materialType.door.ToString() && currentMaterial.materialColor == tile.materialColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                    case "right":
                        if (currentMaterial.Right == (tile.Left - GAP) && currentMaterial.Top == tile.Top && !tile.materialselected)
                        {
                            if (tile.materialType == materialType.door.ToString() && currentMaterial.materialColor == tile.materialColor)
                            {
                                correctDoor = true;
                            }
                            return true;
                        }
                        break;
                }
            }
            correctDoor = false;
            return false;
        }
        /// <summary>
        /// check the correct box go to the correct door and check the win player
        /// </summary>
    
        public bool CheckWin()
        {
            foreach (Materials tile in this.Controls.OfType<Materials>().ToArray())
            {
                if (tile.materialType == materialType.box.ToString())
                {
                    return false;
                }
            }
            MessageBox.Show("You win.");
            return true;
        }
        /// <summary>
        /// selects a tile and returns if it is a box or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tile_Click(object sender, EventArgs e)
        {
            foreach (Materials tile in this.Controls.OfType<Materials>().ToArray())
            {
                tile.materialselected = false;
            }

            currentMaterial.Padding = new System.Windows.Forms.Padding(all: 0);
            currentMaterial.BackColor = Color.Transparent;


            if (((Materials)sender).materialType == materialType.box.ToString())
            {
                isBox = true;

                currentMaterial.Padding = new System.Windows.Forms.Padding(all: 0);
                currentMaterial.BackColor = Color.Transparent;

                currentMaterial = (Materials)sender;
                currentMaterial.materialselected = true;

                currentMaterial.Padding = new System.Windows.Forms.Padding(all: 2);
                currentMaterial.BackColor = Color.Blue;
            }
            else
            {
                isBox = false;
            }
        }
        /******************************************/
        /// <summary>
        /// Load level File thtat user Save in the game strip menue
        /// </summary>
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openfile.ShowDialog() == DialogResult.OK)
            {
                var levelList = new List<string>();
                string record;
                try
                {
                    using (StreamReader reader = new StreamReader(openfile.FileName))
                    {
                        while ((record = reader.ReadLine()) != null)
                        {
                            levelList.Add(record);
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error! Wrong loading.");
                }
                string[] loadArray = levelList.ToArray();

                int x = START_LEFT;
                int y = START_TOP;
                rows = int.Parse(Regex.Match(loadArray[0], @"\d+").Value);
                columns = int.Parse(Regex.Match(loadArray[1], @"\d+").Value);

                int loadCounter = 1;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        loadCounter++;

                        Materials tile = new Materials();

                        tile.Left = x;
                        tile.Top = y;

                        tile.Height = HEIGHT;
                        tile.Width = WIDTH;

                        tile.SizeMode = PictureBoxSizeMode.StretchImage;

                        tile.Click += new EventHandler(Tile_Click);

                        switch (loadArray[loadCounter])
                        {
                            case "0":
                                tile.BackColor = Color.LightGray;
                                tile.materialColor = materialColor.blank.ToString();
                                tile.materialType = materialType.blank.ToString();
                                break;
                            case "1":
                                tile.Image = Resources.black_wall;
                                tile.materialColor = materialColor.black.ToString();
                                tile.materialType = materialType.wall.ToString();
                                break;
                            case "3":
                                tile.Image = Resources.red_box;
                                tile.materialColor = materialColor.red.ToString();
                                tile.materialType = materialType.box.ToString();
                                rCounter++;
                                break;
                            case "2":
                                tile.Image = Resources.red_door;
                                tile.materialColor = materialColor.red.ToString();
                                tile.materialType = materialType.door.ToString();
                                break;
                            case "4":
                                tile.Image = Resources.green_box;
                                tile.materialColor = materialColor.green.ToString();
                                tile.materialType = materialType.box.ToString();
                                rCounter++;
                                break;
                            case "5":
                                tile.Image = Resources.green_door;
                                tile.materialColor = materialColor.green.ToString();
                                tile.materialType = materialType.door.ToString();
                                break;
                            default:
                                tile.BackColor = Color.LightGray;
                                tile.materialColor = materialColor.blank.ToString();
                                tile.materialType = materialType.blank.ToString();
                                break;
                        }

                        this.Controls.Add(tile);

                        x += WIDTH + GAP;

                        Application.DoEvents();
                        Thread.Sleep(25);
                    }
                    y += HEIGHT + GAP;
                    x = START_LEFT;
                }
                txtRemainingBox.Text = rCounter.ToString();

                foreach (Materials tile in this.Controls.OfType<Materials>().ToArray())
                {
                    if (tile.materialType == materialType.blank.ToString())
                    {
                        this.Controls.Remove(tile);
                        Application.DoEvents();
                        Thread.Sleep(25);
                    }
                }
                txtMove.Enabled = true;
                txtRemainingBox.Enabled = true;
                btnDown.Enabled = true;
                btnLeft.Enabled = true;
                btnRight.Enabled = true;
                btnUp.Enabled = true;
            }
        }

        /*************************Play form key pad***************************/
        private void PlayForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    e.Handled = true;
                    btnUp.PerformClick();
                    break;
                case Keys.S:
                    e.Handled = true;
                    btnDown.PerformClick();
                    break;
                case Keys.A:
                    e.Handled = true;
                    btnLeft.PerformClick();
                    break;
                case Keys.D:
                    e.Handled = true;
                    btnRight.PerformClick();
                    break;
                default:
                    break;
            }
        }
        /************************Buttons Definitions**************************/
        private void btnUp_Click(object sender, EventArgs e)
        {

            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMove.Text = moves.ToString();

                txtMove.Enabled = false;
                txtRemainingBox.Enabled = false;
                btnDown.Enabled = false;
                btnLeft.Enabled = false ;
                btnRight.Enabled = false;
                btnUp.Enabled = false;
                while (!Intersection("up"))
                {
                    if (txtRemainingBox.Enabled==true)
                    {
                        currentMaterial.Top -= GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentMaterial.Top -= HEIGHT + GAP;
                    }
                }
                txtMove.Enabled = true;
                txtRemainingBox.Enabled = true;
                btnDown.Enabled = true;
                btnLeft.Enabled = true;
                btnRight.Enabled = true;
                btnUp.Enabled = true;
                if (correctDoor)
                {
                    rCounter--;
                    txtRemainingBox.Text = rCounter.ToString();

                    this.Controls.Remove(currentMaterial);
                }
            }
            else
            {
                MessageBox.Show("Select a box!");
            }
            CheckWin();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {

            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMove.Text = moves.ToString();

                txtMove.Enabled = false;
                txtRemainingBox.Enabled = false;
                btnDown.Enabled = false;
                btnLeft.Enabled = false;
                btnRight.Enabled = false;
                btnUp.Enabled = false;

                while (!Intersection("down"))
                {
                    if (txtRemainingBox.Enabled == true)
                    {
                        currentMaterial.Top += GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentMaterial.Top += HEIGHT + GAP;
                    }
                }
                txtMove.Enabled = true;
                txtRemainingBox.Enabled = true;
                btnDown.Enabled = true;
                btnLeft.Enabled = true;
                btnRight.Enabled = true;
                btnUp.Enabled = true;

                if (correctDoor)
                {
                    rCounter--;
                    txtRemainingBox.Text = rCounter.ToString();

                    this.Controls.Remove(currentMaterial);
                }
            }
            else
            {
                MessageBox.Show("Select a box!");
            }
            CheckWin();

        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMove.Text = moves.ToString();

                txtMove.Enabled = false;
                txtRemainingBox.Enabled = false;
                btnDown.Enabled = false;
                btnLeft.Enabled = false;
                btnRight.Enabled = false;
                btnUp.Enabled = false;

                while (!Intersection("left"))
                {
                    if (txtRemainingBox.Enabled == true)
                    {
                        currentMaterial.Left -= GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentMaterial.Left -= WIDTH + GAP;
                    }
                }
                txtMove.Enabled = true;
                txtRemainingBox.Enabled = true;
                btnDown.Enabled = true;
                btnLeft.Enabled = true;
                btnRight.Enabled = true;
                btnUp.Enabled = true;

                if (correctDoor)
                {
                    rCounter--;
                    txtRemainingBox.Text = rCounter.ToString();

                    this.Controls.Remove(currentMaterial);
                }
            }
            else
            {
                MessageBox.Show("Select a box!");
            }
            CheckWin();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {

            if (isBox)
            {
                lblMoves.Select();
                moves++;
                txtMove.Text = moves.ToString();

                txtMove.Enabled = false;
                txtRemainingBox.Enabled = false;
                btnDown.Enabled = false;
                btnLeft.Enabled = false;
                btnRight.Enabled = false;
                btnUp.Enabled = false;

                while (!Intersection("right"))
                {
                    if (txtRemainingBox.Enabled == true)
                    {
                        currentMaterial.Left += GAP;
                        Application.DoEvents();
                        Thread.Sleep(5);
                    }
                    else
                    {
                        currentMaterial.Left += WIDTH + GAP;
                    }
                }
                txtMove.Enabled = true;
                txtRemainingBox.Enabled = true;
                btnDown.Enabled = true;
                btnLeft.Enabled = true;
                btnRight.Enabled = true;
                btnUp.Enabled = true;

                if (correctDoor)
                {
                    rCounter--;
                    txtRemainingBox.Text = rCounter.ToString();

                    this.Controls.Remove(currentMaterial);
                }
            }
            else
            {
                MessageBox.Show("Select a box!");
            }
            CheckWin();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
   
    }
}
