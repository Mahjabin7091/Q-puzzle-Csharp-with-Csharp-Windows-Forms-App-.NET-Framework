/*Author: mahjabin Sajadi
 *Date: 16/10/2021
 *Purpose: QGAME
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // for refer to the bicturebox

namespace MSajadiQGame
{
    class Materials : PictureBox
    {
        public string materialColor { get; set; }
        public string materialType { get; set; }
        public bool materialselected { get; set; }
    }
}
