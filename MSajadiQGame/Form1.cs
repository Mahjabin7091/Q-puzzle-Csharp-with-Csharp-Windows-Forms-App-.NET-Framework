/*Author: mahjabin Sajadi
 *Date: 16/10/2021
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.IO;   // for calling stream reader and writer
using MSajadiQGame.Properties;  // for resource to the images

namespace MSajadiQGame
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Just for showing.
        /// </summary>
    
        public string Material2Number(string s,string c)
        {
            if (s == "blank")
                return "0";
            if (s == "wall" )
                return "1";
            if (s == "door" && c == "red")
                return "2";
            if (s == "box" && c == "red")
                return "3";
            if (s == "box" && c == "green")
                return "4";
            if (s == "door" && c == "green")
                return "5";
            else
                return "r";
        }

        /// <summary>
        /// definition the constant variable for creating the wall, door,box or nothing.
        /// </summary>

        private const int START_TOP = 150;
        private const int START_LEFT = 250;
        private const int WIDTH = 50;
        private const int HEIGHT = 50;
        private const int GAP = 2;

        /// <summary>
        /// defenition global variables
        /// </summary>
        currentMaterial currentMaterial = new currentMaterial();
        int rows;
        int columns;

        /// <summary>
        /// total variable
        /// </summary>
        int totalBlank=0;
        int totalWall = 0;
        int totalBox = 0;
        int totalDoor = 0;

        /// <summary>
        ///define material color with value type of enum
        /// </summary>
        public enum materialColor
        {
            blank,     //emplty material
            black,      //wall color
            red,         // door or box color
            green         // door or box color 
        }
        /// <summary>
        /// define the type of mateiral that user can select
        /// </summary>
        public enum materialType
        {
            blank,
            wall,
            door,
            box
        }
       
        /// <summary>
        /// 
        /// Initial Form
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }
        // Open Images in Generate box
        private void btnGenerate_Click(object sender, EventArgs e)
        {

            string errors = "";

            if (!int.TryParse(txtRow.Text, out rows))
            {
                errors += "Please enter number.\n";
            }
            if (!int.TryParse(txtColumn.Text, out columns))
            {
                errors += "Please enter number.\n";
            }

            if (errors == "")
            {
                int x = START_LEFT;
                int y = START_TOP;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        Materials tool = new Materials();
                        tool.Left = x;
                        tool.Top = y;

                        tool.Height = HEIGHT;
                        tool.Width = WIDTH;

                        tool.BackColor = Color.LightGray;

                        tool.materialColor = materialColor.blank.ToString();
                        tool.materialType = materialColor.blank.ToString();

                        tool.Click += new EventHandler(materials1_Click);

                        this.Controls.Add(tool);  // create the row and colomn

                        x += WIDTH + GAP;

                        Application.DoEvents();
                        Thread.Sleep(25);
                    }
                    y += HEIGHT + GAP;
                    x = START_LEFT;
                }
                
            }
            else
            {
                MessageBox.Show(errors, "Error");
            }

            /********************************************/
       
        }

        //for saving the game in the fileName and the second item theuser wants to save the numbers that the user enter
        private void save(string fileName, int numberOfItems)
        {
            //use stream writer and streamReader for saving 

            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine(numberOfItems);
            Random r = new Random();  //usefule for debug application create a random number

            for (int i = 0; i < numberOfItems; i++)
            {
                writer.WriteLine(r.Next(0, 100));  // limitation of random number that created

            }
            writer.Close();

        }
        // define a method for read the fileName that we saved it before
        private void load(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            int numberOfItems = int.Parse(reader.ReadLine());

            for (int i = 0; i < numberOfItems; i++)
            {
                Console.WriteLine(reader.ReadLine());
            }

            reader.Close();
        }

        // save as menue
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(savefile.FileName, false))
                {
                    writer.WriteLine($"rows:{rows}");
                    writer.WriteLine($"columns:{columns}");

                }
                foreach (Materials mt in this.Controls.OfType<Materials>().ToArray())
                {
                    using (StreamWriter writer = new StreamWriter(savefile.FileName, true))
                    {
                        writer.WriteLine($"{Material2Number(mt.materialType, mt.materialColor)}");
                    }
                }
                MessageBox.Show($"Saved successfully!" + "\n"+ "Total blank:" +
                    totalBlank + "\n" + "Total Wall:" + totalWall + "\n" + " Total Door:"  + totalDoor + "\n" + "Total Box:" + totalBox);
            }
        }
        /// <summary>
        ///    load from menu that we defined(test in class with proffessor)
        /// </summary>

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                var levelList = new List<string>();
                string record;
                using (StreamReader reader = new StreamReader(openfile.FileName))
                {
                    while ((record = reader.ReadLine()) != null)
                    {
                        levelList.Add(record);
                    }
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

                        tile.Click += new EventHandler(materials1_Click);

                        switch (loadArray[loadCounter])
                        {
                            case "blank::blank":
                                tile.BackColor = Color.LightGray;
                                tile.materialColor = materialColor.blank.ToString();
                                tile.materialType = materialType.blank.ToString();
                                break;
                            case "black::wall":
                                tile.Image = Resources.black_wall;
                                tile.materialColor = materialColor.black.ToString();
                                tile.materialType = materialType.wall.ToString();
                                break;
                            case "red::box":
                                tile.Image = Resources.red_box;
                                tile.materialColor = materialColor.red.ToString();
                                tile.materialType = materialType.box.ToString();
                                break;
                            case "red::door":
                                tile.Image = Resources.red_door;
                                tile.materialColor = materialColor.red.ToString();
                                tile.materialType = materialType.door.ToString();
                                break;
                            case "green::box":
                                tile.Image = Resources.green_box;
                                tile.materialColor = materialColor.green.ToString();
                                tile.materialType = materialType.box.ToString();
                                break;
                            case "green::door":
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
            }
           
        }

        /// <summary>
        /// the way to show the materials that user entered
        /// </summary>
        private void materials1_Click(object sender, EventArgs e)
        {
            Materials material = new Materials();
            material = (Materials)sender;
            material.materialColor = currentMaterial.materialColor;
            material.materialType = currentMaterial.materialType;


            string imageName = ($"{ material.materialColor} {material.materialType}");

            switch (imageName)
            {
                case "blank blank":
                    material.BackColor = Color.LightGray;
                    material.Image = null;
                    totalBlank++;
                    break;
                case "black wall":
                    material.Image = Resources.black_wall;
                    totalWall++;
                    //Console.WriteLine(totalWall);
                    break;
                case "red door":
                    material.Image = Resources.red_door;
                    totalDoor++;
                    break;
                case "red box":
                    material.Image = Resources.red_box;
                    totalBox++;
                    break;
                case "green door":
                    material.Image = Resources.green_door;
                    totalDoor++;
                    break;
                case "green box":
                    material.Image = Resources.green_box;
                    totalBox++;
                    break;     
                default:
                    material.BackColor = Color.LightGray;
                    material.Image = null;
                    totalBlank++;
                    break;
            }
        }
        /// <summary>
        /// Exit Game
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        /// <summary>
        /// The event to save the file that contains the number of wall, blank, door, and box.
        /// </summary>
        private void ToolboxButtons_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnNone":
                    currentMaterial.materialColor = materialColor.blank.ToString();
                    currentMaterial.materialType = materialType.blank.ToString();
                    break;
                case "btnWall":
                    currentMaterial.materialColor = materialColor.black.ToString();
                    currentMaterial.materialType = materialType.wall.ToString();
                    break;
                case "btnRedBox":
                    currentMaterial.materialColor = materialColor.red.ToString();
                    currentMaterial.materialType = materialType.box.ToString();
                    break;
                case "btnRedDoor":
                    currentMaterial.materialColor = materialColor.red.ToString();
                    currentMaterial.materialType = materialType.door.ToString();
                    break;
                case "btnGreenBox":
                    currentMaterial.materialColor = materialColor.green.ToString();
                    currentMaterial.materialType = materialType.box.ToString();
                    break;
                case "btnGreenDoor":
                    currentMaterial.materialColor = materialColor.green.ToString();
                    currentMaterial.materialType = materialType.door.ToString();
                    break;
                default:
                    MessageBox.Show(((Button)sender).Name);
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
